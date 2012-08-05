using System.Windows;
using XText;

namespace ArgyBargy
{
    public interface IActionsDialogueView : IDialogueViewWithoutResult
    {
        void Initialise(
            string title,
            XSection message = null,
            FrameworkElement content = null,
            HorizontalAlignment titleAlignment = HorizontalAlignment.Left,
            HorizontalAlignment buttonsAlignment = HorizontalAlignment.Right,
            params DetailsAction[] buttons);
    }
}