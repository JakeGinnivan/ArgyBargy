using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ArgyBargy.Views;
using XText;

namespace ArgyBargy.Demo
{
    public partial class MainWindow : IShell
    {
        private readonly IDialogService dialogService;

        public MainWindow()
        {
            InitializeComponent();
            dialogService = new DialogService(this, new BusyView());
        }

        private void ShowCommandLink(object sender, RoutedEventArgs e)
        {
            dialogService.ShowCommandLinkDialogueThenExecuteAction(
                "Example command link dialog",
                new XSection(new XParagraph("With some message which can have", (XBold)"bold text", "and other formatting")),
                new DetailsAction("Option 1", "Subtext 1", () => MessageBox.Show("Option 1")));
        }

        private void ShowNormalWindow(object sender, RoutedEventArgs e)
        {
            dialogService.ShowDialogue(new Window1(dialogService));
        }

        private void ShowActionsDialog(object sender, RoutedEventArgs e)
        {
            dialogService.ShowActionsDialogue(
                "Example Actions Dialog",
                new XSection(new XParagraph("More example", (XItalic)"text")),
                new Border
                {
                    Child = new TextBlock { Text = "And you can have a Wpf control too" }
                },
                buttons: new[]
                            {
                                new AffirmativeDetailsAction("Ok", "Ok", () => MessageBox.Show("Ok")),
                                new DetailsAction("Maybe", "Just to show we can do anything", ()=>MessageBox.Show("Maybe")), 
                                new CancelDetailsAction(), 
                            });
        }

        public void DialogueIsVisible()
        {

        }

        public void DialogueWasHidden()
        {

        }

        public IDialogueWindow CreateDialogWindow(Control controlToShow, Window owner)
        {
            return new DialogWindow(controlToShow)
                       {
                           Owner = owner,
                           Height = ActualHeight - 15,
                           Width = ActualWidth - 15
                       };
        }

        private async void ShowBusy(object sender, RoutedEventArgs e)
        {
            using (dialogService.ShowBusy())
                await TaskEx.Delay(5000);
        }
    }
}
