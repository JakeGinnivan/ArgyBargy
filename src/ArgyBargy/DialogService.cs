using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ArgyBargy.Views;
using XText;

namespace ArgyBargy
{
    public class DialogService : IDialogService
    {
        private readonly Window shell;
        private readonly Func<IActionsDialogueView> actionsDialogueViewFactory;
        private readonly Func<ICommandLinksDialogueView> commandLinkDialogueViewFactory;
        private readonly Stack<DialogueController> activeDialogues = new Stack<DialogueController>();


        public DialogService(
            Window shell,
            Func<IActionsDialogueView> actionsDialogueViewFactory = null,
            Func<ICommandLinksDialogueView> commandLinkDialogueViewFactory = null)
        {
            this.shell = shell;
            this.actionsDialogueViewFactory = actionsDialogueViewFactory ?? (()=> new ActionsDialogueView(new ActionDialogueViewModel()));
            this.commandLinkDialogueViewFactory = commandLinkDialogueViewFactory ?? (() => new CommandLinksDialogueView(new CommandLinksDialogueViewModel()));
        }

        public DialogueResult ShowActionsDialogue(string title, XSection message = null, FrameworkElement content = null, HorizontalAlignment titleAlignment = HorizontalAlignment.Left, HorizontalAlignment buttonsAlignment = HorizontalAlignment.Right, params DetailsAction[] buttons)
        {
            var actionsDialogueView = actionsDialogueViewFactory();
            actionsDialogueView.Initialise(title, message, content, titleAlignment, buttonsAlignment, buttons);
            return ShowDialogue(actionsDialogueView);
        }

        public DialogueResult ShowCommandLinkDialogueThenExecuteAction(string title, XSection message, params DetailsAction[] detailsActions)
        {
            ICommandLinksDialogueView commandLinkDialogueView = commandLinkDialogueViewFactory();
            commandLinkDialogueView.Initialise(title, message, detailsActions);
            var result = ShowDialogue(commandLinkDialogueView);
            if (commandLinkDialogueView.SelectedCommand != null)
                commandLinkDialogueView.SelectedCommand.Execute(null);
            return result;
        }

        public Window GetCurrentTopmostWindow()
        {
            return activeDialogues.Any() ? activeDialogues.Peek().DialogueWindow : shell;
        }

        public DialogueResult<TResult> ShowDialogue<TResult>(IDialogueView<TResult> dialogueView)
        {
            DialogueResult<TResult> result = null;

            EventHandler<DialogueResultEventArgs<TResult>> dialogueFinished =
                delegate(object sender, DialogueResultEventArgs<TResult> args)
                {
                    result = args.Result;

                    var parentWindow = (FrameworkElement)dialogueView;
                    while (!(parentWindow is Window))
                        parentWindow = (FrameworkElement)parentWindow.Parent;

                    ((Window)parentWindow).Close();
                };

            dialogueView.Finished += dialogueFinished;
            DisplayDialogue(dialogueView);
            dialogueView.Finished -= dialogueFinished;

            return result ?? DialogueResult<TResult>.CancelledResult();
        }

        public DialogueResult ShowDialogue(IDialogueViewWithoutResult dialogueView)
        {
            DialogueResult result = null;

            EventHandler<DialogueResultEventArgs> dialogueFinished =
                delegate(object sender, DialogueResultEventArgs args)
                {
                    result = args.Cancelled ? DialogueResult.CancelledResult() : new DialogueResult();

                    var parentWindow = (FrameworkElement)dialogueView;
                    while (!(parentWindow is Window))
                        parentWindow = (FrameworkElement)parentWindow.Parent;

                    ((Window)parentWindow).Close();
                };

            dialogueView.Finished += dialogueFinished;
            DisplayDialogue(dialogueView);
            dialogueView.Finished -= dialogueFinished;

            return result ?? DialogueResult.CancelledResult();
        }

        protected virtual void DisplayDialogue(IDialogueView dialogueView)
        {
            var window = dialogueView as Window;
            if (window != null)
            {
                window.Owner = GetCurrentTopmostWindow();
                window.ShowDialog();
                return;
            }

            var shellAsWindow = shell;
            var dialogueViewAsControl = (Control)dialogueView;

            if (dialogueViewAsControl != null)
            {
                dialogueViewAsControl.Opacity = 1;
                dialogueViewAsControl.Background = Brushes.Transparent;
                dialogueViewAsControl.HorizontalAlignment = HorizontalAlignment.Center;
                dialogueViewAsControl.VerticalAlignment = VerticalAlignment.Center;
            }

            var dialogueWindow = new DialogWindow(dialogueViewAsControl, dialogueView as INotifyMeOnCancel)
            {
                Owner = GetCurrentTopmostWindow(),
                Height = shellAsWindow.ActualHeight - 15,
                Width = shellAsWindow.ActualWidth - 15,
                AllowsTransparency = true,
                ShowInTaskbar = false
            };

            var dialogueAsUserControl = dialogueView as UserControl;
            if (dialogueAsUserControl != null)
            {
                object controlsAutomationId = dialogueAsUserControl.GetValue(AutomationProperties.AutomationIdProperty);
                if (controlsAutomationId != null && controlsAutomationId != DependencyProperty.UnsetValue)
                    dialogueWindow.SetValue(AutomationProperties.AutomationIdProperty, controlsAutomationId);
            }

            try
            {
                //_logger.InfoFormat("Displaying dialogue view - {0}", dialogueView.ToString());

                dialogueWindow.KeyUp += (o, e) =>
                {
                    if (e.Key == Key.Escape)
                    {
                        var cancelAuthority = dialogueView as INotifyMeOnCancel;
                        if (cancelAuthority == null || cancelAuthority.AllowCancel())
                            dialogueWindow.Close();
                    }
                };

                RoutedEventHandler dialogueLoaded = null;
                dialogueLoaded = (o, e) =>
                {
                    // Adding this because we had an exception thrown from DialogueDisplayed which
                    // gets swallowed on developers machines, but caused a fatal crash in production
                    // by logging at least if an exception doesnt bring down smoke tests and is swallowed
                    // the test will fail.
                    try
                    {
                        dialogueView.DialogueDisplayed(dialogueWindow);
                    }
                    catch (Exception ex)
                    {
                        //try
                        //{
                        //    TerminalErrorReporter.ReportErrorToCentral(ex, false);
                        //}
                        //catch (Exception)
                        //{
                        //    _logger.Error("An error happened when it tried to send the crash information.", ex);
                        //}
                    }
                    finally
                    {
                        // ReSharper disable AccessToModifiedClosure
                        ((FrameworkElement)dialogueView).Loaded -= dialogueLoaded;
                        // ReSharper restore AccessToModifiedClosure
                    }
                };
                ((FrameworkElement)dialogueView).Loaded += dialogueLoaded;

                //_shell.DialogueIsVisible();

                activeDialogues.Push(new DialogueController(dialogueWindow));

                dialogueWindow.Closing += PopActiveDialogue;

                dialogueWindow.ShowDialog();

                dialogueWindow.Closing -= PopActiveDialogue;
            }
            finally
            {
                dialogueView.DialogueClosed();
                //_shell.DialogueWasHidden();
                //  win.Content = null;
                //_logger.InfoFormat("Hidden dialogue view - {0}", dialogueView.ToString());
            }
        }

        private void PopActiveDialogue(object sender, CancelEventArgs e)
        {
            activeDialogues.Pop();
            //_shell.DialogueIsAboutToClose();
        }

        private sealed class DialogueController : INotifyPropertyChanged
        {
            public Window DialogueWindow { get; private set; }

            public DialogueController(Window dialogueWindow)
            {
                DialogueWindow = dialogueWindow;
            }

            public void Cancel()
            {
                DialogueWindow.Close();
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null) 
                    handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}