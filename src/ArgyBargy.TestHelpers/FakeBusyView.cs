namespace ArgyBargy.TestHelpers
{
    public class FakeBusyView : IBusyView
    {
        public void Initialise(IDialogService dialogService)
        {
            
        }

        public string Information { get; set; }
        public void Dispose()
        {

        }
    }
}