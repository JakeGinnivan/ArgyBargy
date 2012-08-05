using System.Windows;
using System.Windows.Controls;

namespace ArgyBargy.Controls
{
    public class CommandLink : RadioButton
    {
        public CommandLink()
        {
            DefaultStyleKey = typeof(CommandLink);
        }

        public static DependencyProperty LinkProperty = DependencyProperty.Register(
            "Link", typeof(string), typeof(CommandLink));

        public static DependencyProperty NoteProperty = DependencyProperty.Register(
            "Note", typeof(string), typeof(CommandLink));

        public string Link
        {
            get { return (string)GetValue(LinkProperty); }
            set { SetValue(LinkProperty, value); }
        }

        public string Note
        {
            get { return (string)GetValue(NoteProperty); }
            set { SetValue(NoteProperty, value); }
        }
    }
}