using AppMvp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.Domain.Repositories
{
    public interface IPersonRepository
    {
        Task<Person?> GetByIdAsync(int id, CancellationToken token);
        Task<List<Person>> GetAllAsync(CancellationToken token);
        Task AddAsync(Person person, CancellationToken token);
        Task UpdateAsync(Person person, CancellationToken token);
        Task DeleteAsync(Person person);
    }
}
