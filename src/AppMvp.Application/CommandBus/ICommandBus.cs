using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.ApplicationCore.CommandBus
{
    public interface ICommandBus
    {
        Task SendAsync<T>(T command) where T : IRequest<Unit>;
    }
}
