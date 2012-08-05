using System.Windows;
using System.Windows.Controls;
using XText;

namespace ArgyBargy.Demo
{
    public partial class MainWindow 
    {
        private readonly IDialogService dialogService;

        public MainWindow()
        {
            InitializeComponent();
            dialogService = new DialogService(this);
        }

        private void ShowCommandLink(object sender, RoutedEventArgs e)
        {
            dialogService.ShowCommandLinkDialogueThenExecuteAction(
                "Example command link dialog",
                new XSection(new XParagraph("With some message which can have",(XBold) "bold text","and other formatting")),
                new DetailsAction("Option 1", "Subtext 1",() => MessageBox.Show("Option 1")));
        }

        private void ShowNormalWindow(object sender, RoutedEventArgs e)
        {
            dialogService.ShowDialogue(new Window1());
        }

        private void ShowActionsDialog(object sender, RoutedEventArgs e)
        {
            dialogService.ShowActionsDialogue(
                "Example Actions Dialog",
                new XSection(new XParagraph("More example", (XItalic)"text")),
                new Border{
                    Child = new TextBlock { Text= "And you can have a Wpf control too"}
                },
                buttons:new[]
                            {
                                new AffirmativeDetailsAction("Ok", "Ok", () => MessageBox.Show("Ok")),
                                new DetailsAction("Maybe", "Just to show we can do anything", ()=>MessageBox.Show("Maybe")), 
                                new CancelDetailsAction(), 
                            });
        }
    }
}
