using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using XText;

namespace ArgyBargy.Views
{
    public class ActionDialogueViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<DetailsAction> actions;

        public ActionDialogueViewModel()
        {
            actions = new ObservableCollection<DetailsAction>();
        }

        public void Initialise(
            string title,
            XSection message,
            FrameworkElement frameworkElement,
            HorizontalAlignment titleAlignment,
            HorizontalAlignment buttonsAlignment,
            params DetailsAction[] buttons)
        {
            Title = title;
            Content = frameworkElement;
            TitleAlignment = titleAlignment;
            ButtonsAlignment = buttonsAlignment;
            Message = message != null ? message.BuildElement() : null;
            BindActions(buttons);
        }


        private void BindActions(IEnumerable<DetailsAction> buttons)
        {
            Actions.Clear();
            foreach (var detailsAction in buttons)
            {
                var isCancel = detailsAction is CancelDetailsAction;
                var command = detailsAction.Command;
                detailsAction.Command = new DelegateCommand<object>(
                    o =>
                    {
                        command.Execute(o);
                        OnFinished(new DialogueResultEventArgs(isCancel));
                    },
                    command.CanExecute);
                Actions.Add(detailsAction);
            }
        }

        public ObservableCollection<DetailsAction> Actions { get { return actions; } }

        public string Title { get; set; }
        public FrameworkElement Message { get; set; }
        public FrameworkElement Content { get; set; }
        public HorizontalAlignment TitleAlignment { get; set; }
        public HorizontalAlignment ButtonsAlignment { get; set; }
        
        public event EventHandler<DialogueResultEventArgs> Finished;

        public void OnFinished(DialogueResultEventArgs e)
        {
            var handler = Finished;
            if (handler != null) handler(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
