using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ArgyBargy
{
    public interface IDialogueWindow
    {
        event KeyEventHandler PreviewKeyUp;
        event TextCompositionEventHandler TextInput;
        event CancelEventHandler Closing;
        bool? ShowDialog();
        void SetValue(DependencyProperty dependencyProperty, object value);
        void Close();
    }
}