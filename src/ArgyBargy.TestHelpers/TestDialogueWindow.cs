using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ArgyBargy.TestHelpers
{
    public class TestDialogueWindow : IDialogueWindow
    {
        public event KeyEventHandler PreviewKeyUp;
        public event TextCompositionEventHandler TextInput;
        public event CancelEventHandler Closing;

        public bool? ShowDialogResult { get; set; }

        public bool? ShowDialog()
        {
            return ShowDialogResult;
        }

        public void SetValue(DependencyProperty dependencyProperty, object value)
        {
        }

        public void Close()
        {
        }

        public void InvokeOnClosing(CancelEventArgs e)
        {
            CancelEventHandler handler = Closing;
            if (handler != null) handler(this, e);
        }

        public void InvokePreviewKeyDown(KeyEventArgs e)
        {
            var handler = PreviewKeyUp;
            if (handler != null) handler(this, e);
        }

        public void InvokePreviewTextInput(TextCompositionEventArgs e)
        {
            var handler = TextInput;
            if (handler != null) handler(this, e);
        }
    }
}