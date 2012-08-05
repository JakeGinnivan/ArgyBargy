using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace ArgyBargy
{
    public class DetailsActionBase : INotifyPropertyChanged
    {
        private readonly ICommand originalCommand;

        public DetailsActionBase(string text, string tooltip, ICommand command, string automationId = null, bool isDefault = false)
        {
            Debug.Assert(!String.IsNullOrWhiteSpace(text), "The text property must be supplied for a details action");
            Debug.Assert(command != null, "A command must be provided for a details action");

            Text = text;
            Tooltip = tooltip;
            originalCommand = command;
            Command = command;
            IsDefault = isDefault;

            AutomationId = automationId ?? text.Replace(" ", "");
        }

        internal void UpdateTooltip(string newTooltip)
        {
            Tooltip = newTooltip;
        }

        public string AutomationId { get; private set; }
        public string Text { get; private set; }

        public string Tooltip { get; private set; }

        public ICommand Command { get; set; }

        public ICommand OriginalCommand
        {
            get { return originalCommand; }
        }

        public void RaiseCanExecuteChanged()
        {
            ((IRaiseCanExecuteChanged)Command).RaiseCanExecuteChanged();
        }

        public bool IsDefault { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}