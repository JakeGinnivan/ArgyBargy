using System.Windows;

namespace ArgyBargy
{
    public static class DialogueProperties
    {
        public static readonly DependencyProperty ContainerStyleProperty =
            DependencyProperty.Register("ContainerStyle",
                                        typeof(Style),
                                        typeof(DialogueProperties));

        public static Style GetContainerStyle(FrameworkElement control)
        {
            return (Style)control.GetValue(ContainerStyleProperty);
        }

        public static void SetContainerStyle(FrameworkElement control, Style value)
        {
            control.SetValue(ContainerStyleProperty, value);
        }
    }
}