using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArgyBargy.Views
{
    public partial class DialogWindow : IDialogueWindow
    {
        public DialogWindow(Control content)
        {
            InitializeComponent();
            BorderSettings.Child = content;

            if (DialogueProperties.GetContainerStyle(content) != null)
            {
                BorderSettings.Style = DialogueProperties.GetContainerStyle(content);
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!e.Cancel)
                BorderSettings.Child = null;
        }

        private void DialogWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}