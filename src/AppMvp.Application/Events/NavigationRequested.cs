using MediatR;

namespace AppMvp.Application.Events
{
    // Lightweight notification to request navigation to a given region/view
    public sealed record NavigationRequested(string RegionName, string ViewKey, object? Parameter = null) : INotification;
}
