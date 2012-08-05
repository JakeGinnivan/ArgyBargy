using System.Windows;
using XText;

namespace ArgyBargy
{
    public interface IDialogService
    {
        Window GetCurrentTopmostWindow();
        DialogueResult ShowActionsDialogue(
            string title,
            XSection message = null,
            FrameworkElement content = null,
            HorizontalAlignment titleAlignment = HorizontalAlignment.Left,
            HorizontalAlignment buttonsAlignment = HorizontalAlignment.Right,
            params DetailsAction[] buttons);
        DialogueResult ShowCommandLinkDialogueThenExecuteAction(string title, XSection message, params DetailsAction[] detailsActions);
        DialogueResult<TResult> ShowDialogue<TResult>(IDialogueView<TResult> dialogueView);
        DialogueResult ShowDialogue(IDialogueViewWithoutResult dialogueView);
    }
}
