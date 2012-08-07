using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ArgyBargy.Views;
using XText;

namespace ArgyBargy
{
    public class DialogService : IDialogService
    {
        private readonly IShell shell;
        private readonly ILogAdapter logAdapter;
        private readonly Func<IActionsDialogueView> actionsDialogueViewFactory;
        private readonly Func<ICommandLinksDialogueView> commandLinkDialogueViewFactory;
        private readonly Stack<DialogueController> activeDialogues = new Stack<DialogueController>();
        private readonly object busyLock = new object();
        private readonly IBusyView busyView;
        private Window currentBusyWindow;
        private int busyCount;
        private ApplicationState applicationState;

        public DialogService(
            IShell shell,
            IBusyView busyView,
            Func<IActionsDialogueView> actionsDialogueViewFactory = null,
            Func<ICommandLinksDialogueView> commandLinkDialogueViewFactory = null,
            ILogAdapter logAdapter = null)
        {
            this.busyView = busyView;
            this.busyView.Initialise(this);
            this.shell = shell;
            this.logAdapter = logAdapter ?? new TraceLogAdapter();
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
            var commandLinkDialogueView = commandLinkDialogueViewFactory();
            commandLinkDialogueView.Initialise(title, message, detailsActions.Cast<DetailsActionBase>().ToArray());
            var result = ShowDialogue(commandLinkDialogueView);
            if (commandLinkDialogueView.SelectedCommand != null)
                commandLinkDialogueView.SelectedCommand.Execute(null);
            return result;
        }

        public Window GetCurrentTopmostWindow()
        {
            return activeDialogues.Any() ? activeDialogues.Peek().DialogueWindow : (Window)shell;
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

        [DllImport("user32")]
        internal static extern bool EnableWindow(IntPtr hwnd, bool bEnable);

        public IBusyView ShowBusy()
        {
            lock (busyLock)
            {
                if (busyCount == 0)
                {
                    var currentTopmostWindow = GetCurrentTopmostWindow();
                    var frameworkElement = ((FrameworkElement) currentTopmostWindow.Content);
                    var pointToScreen = frameworkElement.PointToScreen(new Point(0, 0));
                    currentBusyWindow = new Window
                                      {
                                          WindowStyle = WindowStyle.None,
                                          WindowStartupLocation = WindowStartupLocation.Manual,
                                          Left = pointToScreen.X,
                                          Top = pointToScreen.Y,
                                          ShowInTaskbar = false,
                                          ResizeMode = ResizeMode.NoResize,
                                          Height = frameworkElement.ActualHeight,
                                          Width = frameworkElement.ActualWidth,
                                          Content = busyView,
                                          Background = Brushes.Transparent,
                                          AllowsTransparency = true,
                                          Owner = currentTopmostWindow,
                                          HorizontalContentAlignment = HorizontalAlignment.Center,
                                          VerticalContentAlignment = VerticalAlignment.Center
                                      };
                    currentBusyWindow.SetValue(AutomationProperties.AutomationIdProperty, "busyWindow");
                    currentBusyWindow.Owner = currentTopmostWindow;

                    IntPtr handle = (new WindowInteropHelper(currentTopmostWindow)).Handle;
                    EnableWindow(handle, false);
                    currentBusyWindow.Show();
                }
                

                busyCount++;
                return busyView;
            }
        }

        public void HideBusy()
        {
            lock (busyLock)
            {
                if (busyCount == 0)
                {
                    Debug.Assert(busyCount != 0, "BusyCount should never drop below 0");
                }
                else
                {
                    busyCount--;

                    if (busyCount == 0)
                    {
                        applicationState = applicationState | ApplicationState.Busy;
                        SetShellAutomationHelpText();
                        var topmostWindow = GetCurrentTopmostWindow();
                        IntPtr handle = (new WindowInteropHelper(topmostWindow)).Handle;
                        EnableWindow(handle, true);
                        currentBusyWindow.Close();
                        currentBusyWindow.Content = busyView;
                        currentBusyWindow = null;
                        CommandManager.InvalidateRequerySuggested();
                    }
                }
            }
        }

        private void SetShellAutomationHelpText()
        {
            if (applicationState == ApplicationState.Available)
                shell.ClearValue(AutomationProperties.HelpTextProperty);
            else
                shell.SetValue(AutomationProperties.HelpTextProperty, applicationState.ToString());
        }

        protected virtual void DisplayDialogue(IDialogueView dialogueView)
        {
            lock (busyLock)
            {
                var currentTopmostWindow = GetCurrentTopmostWindow();
                if (busyCount > 0)
                {
                    var handle = (new WindowInteropHelper(currentTopmostWindow)).Handle;
                    EnableWindow(handle, true);
                }

                var window = dialogueView as Window;
                IDialogueWindow dialogueWindow;
                if (window != null)
                {
                    dialogueWindow = new DialogWindowAdapter(window);
                }
                else
                {
                    //Create a dialog window to wrap the user control
                    var dialogueViewAsControl = (Control) dialogueView;

                    if (dialogueViewAsControl != null)
                    {
                        dialogueViewAsControl.Opacity = 1;
                        dialogueViewAsControl.Background = Brushes.Transparent;
                        dialogueViewAsControl.HorizontalAlignment = HorizontalAlignment.Center;
                        dialogueViewAsControl.VerticalAlignment = VerticalAlignment.Center;
                    }

                    var createdDialogWindow = shell.CreateDialogWindow(dialogueViewAsControl, currentTopmostWindow);

                    var dialogueAsUserControl = dialogueView as UserControl;
                    if (dialogueAsUserControl != null)
                    {
                        var controlsAutomationId =
                            dialogueAsUserControl.GetValue(AutomationProperties.AutomationIdProperty);
                        if (controlsAutomationId != null && controlsAutomationId != DependencyProperty.UnsetValue)
                            createdDialogWindow.SetValue(AutomationProperties.AutomationIdProperty, controlsAutomationId);
                    }

                    window = (Window) createdDialogWindow;
                    dialogueWindow = createdDialogWindow;
                }

                try
                {
                    logAdapter.Info(string.Format("Displaying dialogue view - {0}", dialogueView.ToString()));

                    RoutedEventHandler dialogueLoaded = null;
                    dialogueLoaded = (o, e) =>
                                         {
                                             try
                                             {
                                                 dialogueView.DialogueDisplayed(dialogueWindow);
                                             }
                                             catch (Exception ex)
                                             {
                                                 logAdapter.Error(ex);
                                             }
                                             finally
                                             {
                                                 // ReSharper disable AccessToModifiedClosure
                                                 ((FrameworkElement) dialogueView).Loaded -= dialogueLoaded;
                                                 // ReSharper restore AccessToModifiedClosure
                                             }
                                         };
                    ((FrameworkElement) dialogueView).Loaded += dialogueLoaded;

                    shell.DialogueIsVisible();
                    
                    activeDialogues.Push(new DialogueController(window));

                    dialogueWindow.Closing += DialogClosing;

                    if (busyCount > 0)
                    {
                        currentBusyWindow.Owner = window;
                        var handle = (new WindowInteropHelper(currentTopmostWindow)).Handle;
                        EnableWindow(handle, false);
                    }

                    dialogueWindow.ShowDialog();

                    dialogueWindow.Closing -= DialogClosing;
                }
                finally
                {
                    dialogueView.DialogueClosed();
                    shell.DialogueWasHidden();
                    //  win.Content = null;
                    //_logger.InfoFormat("Hidden dialogue view - {0}", dialogueView.ToString());
                }
            }
        }

        private void DialogClosing(object sender, CancelEventArgs e)
        {
            var notifyMeOnCancel = sender as INotifyMeOnCancel;
            if (notifyMeOnCancel != null && !notifyMeOnCancel.AllowCancel())
            {
                e.Cancel = true;
                return;
            }

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