using AppMvp.ApplicationCore.DTOs;
using AppMvp.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.ApplicationCore.Events
{
    public class PeopleLoadedEvent : INotification
    {
        public IReadOnlyList<PersonDto> People { get; }

        public PeopleLoadedEvent(IReadOnlyList<PersonDto> people)
        {
            People = people;
        }
    }
}
