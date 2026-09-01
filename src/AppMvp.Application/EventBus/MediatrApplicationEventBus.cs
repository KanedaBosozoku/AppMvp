using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.ApplicationCore.EventBus
{
    public class MediatrApplicationEventBus : IApplicationEventBus
    {
        private readonly IMediator _mediator;

        public MediatrApplicationEventBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task PublishAsync<T>(T evt) where T : INotification
            => _mediator.Publish(evt);
    }
}
