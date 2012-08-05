using System.Threading.Tasks;
using System.Windows.Input;

namespace ArgyBargy
{
    public interface IAsyncCommand<in T> : ICommand, IRaiseCanExecuteChanged
    {
        Task ExecuteAsync(T obj);
    }
}