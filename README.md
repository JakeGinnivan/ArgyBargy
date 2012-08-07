ArgyBargy
=========

A WPF library which makes Dialog Management quite easy.

WPF makes dialogues hard, especially when trying to do it in a testable way. Then you throw a shell and busy windows as cross cutting concerns and things get complex.

Argy Bargy is the result of many arguments with WPF about how this should be easier, and this library helps you win!

## Command Link Dialog

    dialogService.ShowCommandLinkDialogueThenExecuteAction(
                "Example command link dialog",
                new XSection(new XParagraph("With some message which can have", (XBold)"bold text", "and other formatting")),
                new DetailsAction("Option 1", "Subtext 1", () => MessageBox.Show("Option 1")));
                
   ![Command Link Dialog](https://dl.dropbox.com/u/6428676/CommandLink.PNG)
   
## Actions Dialog

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
                    
   ![Actions Dialog](https://dl.dropbox.com/u/6428676/ActionsDialog.PNG)
   
## Other Dialogues
Argy Bargy supports showing user controls as windows (it will automatically create a window to host that control). This allows you to cross cut window creation across your application.

You **must** inherit from `IDialogueWindow` to be able to show the dialogue using Argy Bargy. The control can be a `UserControl` or a `Window`. For Example:

    public class MyWindow : Window, IMyWindow //Using IMyWindow allows for testing/mocking :)
    {
        // impl
    }
    
    public interface IMyWindow : IDialogueWindow {}

## Unit testing
This example uses Autofac, but plenty of ways to acheive the same thing

    containerBuilder
        .RegisterAssemblyTypes(assembly)
        .AssignableTo<IDialogueView>()
        .AsImplementedInterfaces();
    
    public class MyWindow : Window, IMyWindow
    {
        public MyWindow(MyViewModel myViewModel)
        {
            DataContext = myViewModel;
            InitializeComponent();
        }
    }
    
    public class MyViewModel : ViewModelBase
    {
        public MyViewModel(Func<IAnotherView> anotherViewFactory, IDialogService dialogService)
        {
            OpenWindowCommand = new DelegateCommand(()=>dialogService.ShowDialog(anotherViewFactory()));
        }
    }
    
    
    //Test UI Service has a heap of helpers to make testing dialogs easy
    var testDialogService = new TestDialogService();
    testDialogService.AddResult(DialogResult.Cancelled());
    
    Assert.True(testDialogService.ShowDialog(Substitute.For<IDialogueView>()).Cancelled);
   
## Busy Handling
Argy Bargy has the ability to cross cut a busy control which can be used throughout the application.

I'm not showing a screenshot because it looks pretty crappy at the moment, but the good news is that it is extensible, just provide your own IBusyView control and you are good to go!.

    using (dialogService.ShowBusy())
        await TaskEx.Delay(5000);
        
After 5 seconds the busy window will get disposed, and close. You can also use `dialogService.ShowBusy()` then `dialogService.HideBusy()` but using the IDisposable pattern is quite clean (especially when using the new async stuff)

## Coming Soon
Error dialog, possibly extensions to show any INotifyPropertyChanged POCO and have a form generated using Magellan.Forms.