using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Domain.Entities
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Person(int id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }
}
