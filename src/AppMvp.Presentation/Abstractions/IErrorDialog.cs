using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppMvp.Presentation.Abstractions
{
    /// <summary>
    /// Abstraction for showing a modal error dialog with user-friendly message and technical details.
    /// Implementations should ensure UI-thread marshalling as needed.
    /// Returns true when the dialog closed normally.
    /// </summary>
    public interface IErrorDialog
    {
        Task<bool> ShowAsync(string userMessage, Exception? exception = null, IDictionary<string, string?>? properties = null, string? correlationId = null);
    }
}
