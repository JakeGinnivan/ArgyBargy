using System;
using System.Threading.Tasks;
using System.Windows;

namespace ArgyBargy.Demo
{
    public partial class Window1 : IDialogueViewWithoutResult
    {
        private readonly IDialogService dialogService;

        public Window1(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            InitializeComponent();
        }

        public void DialogueDisplayed(IDialogueWindow window)
        {
            
        }

        public void DialogueClosed()
        {
        }

        public event EventHandler<DialogueResultEventArgs> Finished;

        private async void ShowBusy(object sender, RoutedEventArgs e)
        {
            using (dialogService.ShowBusy())
                await TaskEx.Delay(5000);
        }
    }
}
