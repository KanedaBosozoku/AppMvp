using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.ApplicationCore.DTOs
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public PersonDto(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
