using System;
using XText;

namespace ArgyBargy.Views
{
    public partial class CommandLinksDialogueView : ICommandLinksDialogueView
    {
        private readonly CommandLinksDialogueViewModel viewModel;

        public CommandLinksDialogueView(CommandLinksDialogueViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = this.viewModel;
            InitializeComponent();
        }

        public void Initialise(string title, XSection message, params DetailsActionBase[] commandLinks)
        {
            viewModel.Initialise(title, message, commandLinks);
        }

        public IAsyncCommand SelectedCommand
        {
            get { return viewModel.SelectedCommand; }
        }

        public void DialogueDisplayed(IDialogueWindow window)
        {
            
        }

        public void DialogueClosed()
        {
        }

        public event EventHandler<DialogueResultEventArgs> Finished
        {
            add { viewModel.Finished += value; }
            remove { viewModel.Finished -= value; }
        }
    }
}
