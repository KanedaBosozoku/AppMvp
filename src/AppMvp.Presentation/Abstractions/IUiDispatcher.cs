using System;
using System.Threading.Tasks;

namespace AppMvp.Presentation.Abstractions
{
    public interface IUiDispatcher
    {
        void BeginInvoke(Action action);
        void BeginInvoke(Func<Task> action);
    }
}
