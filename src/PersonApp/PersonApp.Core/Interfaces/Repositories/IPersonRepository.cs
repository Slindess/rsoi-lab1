using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PersonApp.Core.Models;

namespace PersonApp.Core.Interfaces.Repositories;

public interface IPersonRepository
{
    Task<IReadOnlyList<Person>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Person?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Person> AddAsync(Person person, CancellationToken cancellationToken = default);

    Task<Person?> UpdateAsync(int id, Person person, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
