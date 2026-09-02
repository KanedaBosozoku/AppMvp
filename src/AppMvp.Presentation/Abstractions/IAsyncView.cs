using System.Threading;
using System.Threading.Tasks;

namespace AppMvp.Presentation.Abstractions
{
    /// <summary>
    /// Optional interface for views that need asynchronous activation after being added to a region.
    /// </summary>
    public interface IAsyncView
    {
        Task ActivateAsync(CancellationToken cancellationToken = default);
    }
}
