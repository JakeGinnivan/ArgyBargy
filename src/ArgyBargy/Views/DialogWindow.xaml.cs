using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArgyBargy.Views
{
    public partial class DialogWindow : IDialogueWindow
    {
        private readonly INotifyMeOnCancel cancelAuthority;

        public DialogWindow(Control content, INotifyMeOnCancel cancelAuthority)
        {
            this.cancelAuthority = cancelAuthority;
            InitializeComponent();
            BorderSettings.Child = content;

            if (DialogueProperties.GetContainerStyle(content) != null)
            {
                BorderSettings.Style = DialogueProperties.GetContainerStyle(content);
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            if (cancelAuthority == null || cancelAuthority.AllowCancel())
                Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            BorderSettings.Child = null;
            base.OnClosing(e);
        }

        private void DialogWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed && e.MiddleButton != MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}