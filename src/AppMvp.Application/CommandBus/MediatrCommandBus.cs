using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.ApplicationCore.CommandBus
{
    public class MediatrCommandBus : ICommandBus
    {
        private readonly IMediator _mediator;

        public MediatrCommandBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task SendAsync<T>(T command) where T : IRequest<Unit>
            => _mediator.Send(command);
    }
}
