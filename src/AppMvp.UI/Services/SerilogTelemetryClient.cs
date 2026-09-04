using AppMvp.Presentation.Abstractions;
using Serilog;
using System;
using System.Collections.Generic;

namespace AppMvp.UI.Services
{
    public sealed class SerilogTelemetryClient : IErrorTelemetry
    {
        public void ReportException(Exception? exception, IDictionary<string, string?>? properties = null)
        {
            try
            {
                if (exception != null)
                {
                    if (properties != null)
                    {
                        var log = Log.ForContext("Properties", properties, destructureObjects: true);
                        log.Error(exception, "Unhandled exception reported");
                    }
                    else
                    {
                        Log.Error(exception, "Unhandled exception reported");
                    }
                }
                else
                {
                    Log.Error("Unhandled exception reported: <null>");
                }
            }
            catch { }
        }
    }
}
