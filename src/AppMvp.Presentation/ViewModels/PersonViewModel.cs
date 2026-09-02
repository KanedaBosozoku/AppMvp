using AppMvp.ApplicationCore.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.ViewModels
{
    public class PersonViewModel
    {
        public int Id { get; }
        public string DisplayName { get; }
        public string Email { get; }

        public PersonViewModel(PersonDto dto)
        {
            Id = dto.Id;
            DisplayName = $"{dto.Name}";
            Email = dto.Email;
        }
    }

}
