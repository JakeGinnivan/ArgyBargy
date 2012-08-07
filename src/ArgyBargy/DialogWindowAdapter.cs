using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ArgyBargy
{
    public class DialogWindowAdapter : IDialogueWindow
    {
        private readonly Window window;

        public DialogWindowAdapter(Window window)
        {
            this.window = window;
        }

        public event KeyEventHandler PreviewKeyUp
        {
            add { window.PreviewKeyUp += value; }
            remove { window.PreviewKeyUp -= value; }
        }

        public event TextCompositionEventHandler TextInput
        {
            add { window.TextInput += value; }
            remove { window.TextInput -= value; }
        }

        public event CancelEventHandler Closing
        {
            add { window.Closing += value; }
            remove { window.Closing -= value; }
        }

        public bool? ShowDialog()
        {
            return window.ShowDialog();
        }

        public void SetValue(DependencyProperty dependencyProperty, object value)
        {
            window.SetValue(dependencyProperty, value);
        }

        public void Close()
        {
            window.Close();
        }
    }
}