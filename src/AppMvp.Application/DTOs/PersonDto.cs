using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.ApplicationCore.DTOs
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public PersonDto(int id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }
}
