using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using XText;

namespace ArgyBargy.Views
{
    public class CommandLinksDialogueViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<DetailsActionBase> actions;

        public CommandLinksDialogueViewModel()
        {
            actions = new ObservableCollection<DetailsActionBase>();
            CancelCommand = new DelegateCommand<object>(o => OnFinished(new DialogueResultEventArgs(true)));
        }

        public void Initialise(string title, XSection message, params DetailsActionBase[] commandLinks)
        {
            Title = title;

            if (message != null)
                Message = message.BuildElement();

            BindActions(commandLinks);
        }

        private void BindActions(IEnumerable<DetailsActionBase> buttons)
        {
            Actions.Clear();
            foreach (var detailsAction in buttons)
            {
                var command = detailsAction.OriginalCommand;
                var asyncCommand = command as IAsyncCommand;

                detailsAction.Command = new DelegateCommand<object>(
                    o =>
                        {
                            OnFinished(new DialogueResultEventArgs(false));

                            if (asyncCommand != null)
                                SelectedCommand = asyncCommand;
                            else
                                command.Execute(o);
                        },
                    command.CanExecute);
                Actions.Add(detailsAction);
            }
        }

        public string Title { get; set; }
        public FrameworkElement Message { get; set; }
        public ICommand CancelCommand { get; private set; }

        public ObservableCollection<DetailsActionBase> Actions { get { return actions; } }

        public IAsyncCommand SelectedCommand { get; private set; }

        public event EventHandler<DialogueResultEventArgs> Finished;

        protected void OnFinished(DialogueResultEventArgs e)
        {
            var handler = Finished;
            if (handler != null) handler(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
