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
        // This event is published by the application layer after people have been loaded.
        // Pattern: publish DTOs from the application layer (no UI types here), and handle
        // the event in the UI layer where the handler can safely marshal to the UI thread
        // and apply the DTOs to viewmodels. Keeping the handler in the UI avoids threading
        // and layering issues while still preventing an extra repository call.

        public PeopleLoadedEvent(IReadOnlyList<PersonDto> people)
        {
            People = people;
        }
    }
}
