using System.Threading;

namespace AppMvp.Presentation.Abstractions
{
    public interface IBusyScope : System.IDisposable
    {
        CancellationToken Token { get; }
        string? Id { get; }
    }
}
