using System;
using System.Windows;
using System.Windows.Automation;
using XText;

namespace ArgyBargy.Views
{
    public partial class ActionsDialogueView : IActionsDialogueView 
    {
        private readonly ActionDialogueViewModel viewModel;

        public ActionsDialogueView(ActionDialogueViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = this.viewModel;
            InitializeComponent();
        }

        public void Initialise(
            string title,
            XSection message = null,
            FrameworkElement content = null,
            HorizontalAlignment titleAlignment = HorizontalAlignment.Left,
            HorizontalAlignment buttonsAlignment = HorizontalAlignment.Right,
            params DetailsAction[] buttons)
        {
            AutomationProperties.SetAutomationId(this, title);
            viewModel.Initialise(title, message, content, titleAlignment, buttonsAlignment, buttons);
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
