using System;

namespace ArgyBargy.Demo
{
    public partial class Window1 : IDialogueViewWithoutResult
    {
        public Window1()
        {
            InitializeComponent();
        }

        public void DialogueDisplayed(IDialogueWindow window)
        {
            
        }

        public void DialogueClosed()
        {
        }

        public event EventHandler<DialogueResultEventArgs> Finished;
    }
}
