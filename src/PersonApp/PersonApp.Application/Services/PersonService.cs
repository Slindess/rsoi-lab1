using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PersonApp.Core.Interfaces.Repositories;
using PersonApp.Core.Interfaces.Services;
using PersonApp.Core.Models;

namespace PersonApp.Application.Services;

public sealed class PersonService(IPersonRepository personRepository) : IPersonService
{
    public Task<IReadOnlyList<Person>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return personRepository.GetAllAsync(cancellationToken);
    }

    public Task<Person?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return personRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<Person> CreateAsync(Person person, CancellationToken cancellationToken = default)
    {
        Normalize(person);
        return personRepository.AddAsync(person, cancellationToken);
    }

    public async Task<Person?> UpdateAsync(int id, Person person, CancellationToken cancellationToken = default)
    {
        Normalize(person);
        return await personRepository.UpdateAsync(id, person, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return personRepository.DeleteAsync(id, cancellationToken);
    }

    private static void Normalize(Person person)
    {
        person.Name = person.Name.Trim();
        person.Address = string.IsNullOrWhiteSpace(person.Address) ? null : person.Address.Trim();
        person.Work = string.IsNullOrWhiteSpace(person.Work) ? null : person.Work.Trim();
    }
}
