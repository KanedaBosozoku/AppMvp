using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.ApplicationCore.EventBus
{
    public interface IApplicationEventBus
    {
        Task PublishAsync<T>(T evt) where T : INotification;
    }
}
