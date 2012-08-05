using XText;

namespace ArgyBargy
{
    public interface ICommandLinksDialogueView : IDialogueViewWithoutResult
    {
        void Initialise(string title, XSection message, params DetailsActionBase[] commandLinks);
        IAsyncCommand SelectedCommand { get; }
    }
}