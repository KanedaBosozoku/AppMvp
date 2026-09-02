using AppMvp.ApplicationCore.DTOs;
using AppMvp.Domain.Repositories;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AppMvp.Presentation.ViewModels
{
    //public sealed class PeopleViewModel
    //{
    //    public List<PersonDto> GetPeople()
    //    {
    //        return new List<PersonDto>
    //        {
    //            new PersonDto(1, "Alice Johnson"),
    //            new PersonDto(2, "Bob Smith"),
    //            new PersonDto(3, "Charlie Davis")
    //        };
    //    }

    //    public void LoadPerson(int id)
    //    {
    //        // Load person details if needed
    //    }

    //    public void LoadPeople(IReadOnlyList<PersonDto> people)
    //    {
    //        // Load people list if needed
    //    }
    //}



    //public sealed class PeopleViewModel
    //{
    //    private readonly IPersonRepository _repo;

    //    public List<PersonViewModel> People { get; } = new();

    //    public PeopleViewModel(IPersonRepository repo)
    //    {
    //        _repo = repo;
    //    }

    //    public async Task LoadPeopleAsync(CancellationToken ct)
    //    {
    //        People.Clear();

    //        var enties = await _repo.GetAllAsync(ct);
    //        var dtos = enties.Select(e => new PersonDto(e.Id, e.Name));
    //        foreach (var dto in dtos)
    //            People.Add(new PersonViewModel(dto));
    //    }

    //    public async Task LoadPersonAsync(int id, CancellationToken ct)
    //    {
    //        var dto = await _repo.GetByIdAsync(id, ct);
    //        if (dto != null)
    //        {
    //            // optional: update selected person
    //        }
    //    }

    //    // Optional: event-driven load
    //    public void LoadPeople(IEnumerable<PersonDto> dtos)
    //    {
    //        People.Clear();

    //        foreach (var dto in dtos)
    //            People.Add(new PersonViewModel(dto));
    //    }
    //}

    public sealed class PeopleViewModel : INotifyPropertyChanged
    {
        private readonly IPersonRepository _repo;

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

        public PeopleViewModel(IPersonRepository repo)
        {
            _repo = repo;
        }

        public async Task LoadPeopleAsync(CancellationToken ct)
        {
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