using System;

namespace ArgyBargy
{
    public interface IBusyView : IView, IDisposable
    {
        void Initialise(IDialogService dialogService);
        string Information { get; set; }
    }
}