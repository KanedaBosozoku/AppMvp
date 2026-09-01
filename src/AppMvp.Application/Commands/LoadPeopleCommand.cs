using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.ApplicationCore.Commands
{
    public record LoadPeopleCommand() : IRequest<Unit>;

}
