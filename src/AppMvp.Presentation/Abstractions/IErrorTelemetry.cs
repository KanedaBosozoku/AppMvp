using System;
using System.Collections.Generic;

namespace AppMvp.Presentation.Abstractions
{
    public interface IErrorTelemetry
    {
        void ReportException(Exception? exception, IDictionary<string, string?>? properties = null);
    }
}
