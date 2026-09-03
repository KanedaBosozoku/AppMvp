using AppMvp.ApplicationCore.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Presentation.ViewModels
{
    public class PersonViewModel
    {
        public int Id { get; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public PersonViewModel(PersonDto dto)
        {
            Id = dto.Id;
            DisplayName = dto.Name ?? string.Empty;
            Email = dto.Email ?? string.Empty;
        }
    }

}
