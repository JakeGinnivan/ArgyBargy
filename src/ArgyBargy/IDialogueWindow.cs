using System.Windows.Input;

namespace ArgyBargy
{
    public interface IDialogueWindow
    {
        event KeyEventHandler PreviewKeyUp;
        event TextCompositionEventHandler TextInput;
    }
}