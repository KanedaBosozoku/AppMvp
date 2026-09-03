using MediatR;
using System;

namespace AppMvp.ApplicationCore.Commands
{
    public sealed class ShowEditPersonRequest : INotification
    {
        public int PersonId { get; }

        public ShowEditPersonRequest(int personId)
        {
            PersonId = personId;
        }
    }
}
