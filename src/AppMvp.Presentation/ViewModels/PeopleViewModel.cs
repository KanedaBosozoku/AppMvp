using AppMvp.ApplicationCore.CommandBus;
using AppMvp.ApplicationCore.DTOs;
using AppMvp.Domain.Repositories;
using AppMvp.Presentation.Abstractions;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppMvp.Presentation.ViewModels
{
    public sealed class PeopleViewModel : INotifyPropertyChanged
    {
        private readonly IPersonRepository _repo;
        private readonly IBusyIndicator _busy;
        private readonly AppMvp.ApplicationCore.EventBus.IApplicationEventBus _eventBus;

        // Use BindingList so WinForms can observe collection changes and update UI automatically
        public BindingList<PersonViewModel> People { get; } = new();

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged(nameof(IsBusy));
                }
            }
        }

        private string? _busyMessage;
        public string? BusyMessage
        {
            get => _busyMessage;
            private set
            {
                if (_busyMessage != value)
                {
                    _busyMessage = value;
                    OnPropertyChanged(nameof(BusyMessage));
                }
            }
        }

        public PeopleViewModel(IPersonRepository repo, IBusyIndicator busy, AppMvp.ApplicationCore.EventBus.IApplicationEventBus eventBus)
        {
            _repo = repo;
            _busy = busy ?? throw new ArgumentNullException(nameof(busy));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task LoadPeopleAsync(CancellationToken ct)
        {
            using var scope = _busy.Begin("Loading people…", BusyScopes.PeopleRefresh);
            using var linked = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(ct, scope.Token);
            var token = linked.Token;
            try
            {
                IsBusy = true;
                BusyMessage = "Loading people…";

                People.RaiseListChangedEvents = false;

                var entities = await _repo.GetAllAsync(token);

                // In-place update strategy to preserve selection/index stability in bound DataGridView
                for (int i = 0; i < entities.Count; i++)
                {
                    var e = entities[i];
                    var dto = new PersonDto(e.Id, e.Name, e.Email);

                    if (i < People.Count)
                    {
                        var existing = People[i];
                        if (existing.Id == dto.Id)
                        {
                            // update existing item in-place
                            existing.DisplayName = dto.Name ?? string.Empty;
                            existing.Email = dto.Email ?? string.Empty;
                        }
                        else
                        {
                            // look for the item further down the list
                            var foundIndex = -1;
                            for (int j = i + 1; j < People.Count; j++)
                            {
                                if (People[j].Id == dto.Id)
                                {
                                    foundIndex = j;
                                    break;
                                }
                            }

                            if (foundIndex >= 0)
                            {
                                var item = People[foundIndex];
                                // move item up to current index
                                People.RemoveAt(foundIndex);
                                People.Insert(i, item);
                                item.DisplayName = dto.Name ?? string.Empty;
                                item.Email = dto.Email ?? string.Empty;
                            }
                            else
                            {
                                // insert new
                                People.Insert(i, new PersonViewModel(dto));
                            }
                        }
                    }
                    else
                    {
                        // append new item
                        People.Add(new PersonViewModel(dto));
                    }
                }

                // Remove any extra items at the end
                while (People.Count > entities.Count)
                    People.RemoveAt(People.Count - 1);
            }
            finally
            {
                People.RaiseListChangedEvents = true;
                People.ResetBindings();

                BusyMessage = null;
                IsBusy = false;
            }
        }

        public async Task LoadPersonAsync(int id, CancellationToken ct)
        {
            using var scope = _busy.Begin("Loading person…", BusyScopes.PeopleRefresh);
            using var linked = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(ct, scope.Token);
            var token = linked.Token;
            try
            {
                IsBusy = true;
                BusyMessage = "Loading person…";

                var dto = await _repo.GetByIdAsync(id, token);
                if (dto != null)
                {
                    // update selected person if needed
                }
            }
            finally
            {
                BusyMessage = null;
                IsBusy = false;
            }
        }

        public System.Threading.Tasks.Task EditPersonAsync(int personId)
        {
            return _eventBus.PublishAsync(new AppMvp.ApplicationCore.Commands.ShowEditPersonRequest(personId));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // Synchronous load used by event handlers that already have DTOs
        public void LoadPeople(IEnumerable<AppMvp.ApplicationCore.DTOs.PersonDto> dtos)
        {
            if (dtos == null) return;

            People.RaiseListChangedEvents = false;

            var list = dtos.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                var dto = list[i];

                if (i < People.Count)
                {
                    var existing = People[i];
                    if (existing.Id == dto.Id)
                    {
                        existing.DisplayName = dto.Name ?? string.Empty;
                        existing.Email = dto.Email ?? string.Empty;
                    }
                    else
                    {
                        var foundIndex = -1;
                        for (int j = i + 1; j < People.Count; j++)
                        {
                            if (People[j].Id == dto.Id)
                            {
                                foundIndex = j;
                                break;
                            }
                        }

                        if (foundIndex >= 0)
                        {
                            var item = People[foundIndex];
                            People.RemoveAt(foundIndex);
                            People.Insert(i, item);
                            item.DisplayName = dto.Name ?? string.Empty;
                            item.Email = dto.Email ?? string.Empty;
                        }
                        else
                        {
                            People.Insert(i, new PersonViewModel(dto));
                        }
                    }
                }
                else
                {
                    People.Add(new PersonViewModel(dto));
                }
            }

            while (People.Count > list.Count)
                People.RemoveAt(People.Count - 1);

            People.RaiseListChangedEvents = true;
            People.ResetBindings();
        }
    }
}