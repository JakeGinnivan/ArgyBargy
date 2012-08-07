using System;

namespace ArgyBargy.Views
{
    public partial class BusyView : IBusyView
    {
        private IDialogService dialogService;
        private string information;

        public BusyView()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            dialogService.HideBusy();
        }

        public void Initialise(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }

        public string Information
        {
            get { return information; }
            set
            {
                if (Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                    Dispatcher.BeginInvoke((Action)(() => { Information = value; }));
                else
                {
                    information = value;
                    busyInformation.Text = information;
                }
            }
        }
    }
}
