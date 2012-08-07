using System.Windows;
using System.Windows.Controls;

namespace ArgyBargy
{
    public interface IShell
    {
        void DialogueIsVisible();
        void DialogueWasHidden();
        IDialogueWindow CreateDialogWindow(Control controlToShow, Window owner);
        double ActualHeight { get; }
        double ActualWidth { get; }
        void SetValue(DependencyProperty dependencyProperty, object value);
        void ClearValue(DependencyProperty dependencyProperty);
    }
}