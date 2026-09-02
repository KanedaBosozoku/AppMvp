using AppMvp.ApplicationCore.DTOs;
using AppMvp.Domain.Repositories;
using AppMvp.Presentation.Abstractions;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppMvp.Presentation.ViewModels
{
    public sealed class PeopleViewModel : INotifyPropertyChanged
    {
        private readonly IPersonRepository _repo;
        private readonly IBusyIndicator _busy;

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

        public PeopleViewModel(IPersonRepository repo, IBusyIndicator busy)
        {
            _repo = repo;
            _busy = busy ?? throw new ArgumentNullException(nameof(busy));
        }

        public async Task LoadPeopleAsync(CancellationToken ct)
        {
            using var scope = _busy.Begin("Loading people…");
            try
            {
                IsBusy = true;
                BusyMessage = "Loading people…";

                People.RaiseListChangedEvents = false;
                People.Clear();

                var entities = await _repo.GetAllAsync(ct);

                foreach (var e in entities)
                {
                    var dto = new PersonDto(e.Id, e.Name, e.Email);
                    People.Add(new PersonViewModel(dto));
                }
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
            using var scope = _busy.Begin("Loading person…");
            try
            {
                IsBusy = true;
                BusyMessage = "Loading person…";

                var dto = await _repo.GetByIdAsync(id, ct);
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

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}