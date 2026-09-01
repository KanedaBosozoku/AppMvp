using AppMvp.Domain.Entities;
using AppMvp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Infrastructure.Persistence
{
    public class PersonRepository : IPersonRepository
    {
        private readonly Dictionary<int, Person> _store = new()
        {
            [1] = new Person(1, "Alice"),
            [2] = new Person(2, "Bob"),
            [3] = new Person(3, "Charlie"),
            [4] = new Person(4, "Mikey"),
            [5] = new Person(5, "Tommy"),
            [6] = new Person(6, "Nala"),
            [7] = new Person(7, "Winnie"),
        };

        private int _nextId = 8;

        public Task<Person?> GetByIdAsync(int id, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            _store.TryGetValue(id, out var person);
            return Task.FromResult(person);
        }

        public async Task<List<Person>>GetAllAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(4000);
            //return Task.FromResult(_store.Values.ToList());
            return new List<Person>(_store.Values);
        }

        public Task AddAsync(Person person, CancellationToken token)
        {
            //person.AssignId(_nextId++);
            _store[person.Id] = person;
            token.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Person person, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (_store.ContainsKey(person.Id))
                _store[person.Id] = person;

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Person person)
        {
            _store.Remove(person.Id);
            return Task.CompletedTask;
        }
    }
}
