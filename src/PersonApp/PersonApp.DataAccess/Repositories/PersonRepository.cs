using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonApp.Core.Interfaces.Repositories;
using PersonApp.Core.Models;
using PersonApp.DataAccess.Database;

namespace PersonApp.DataAccess.Repositories;

public sealed class PersonRepository(PersonsDbContext dbContext) : IPersonRepository
{
    public async Task<IReadOnlyList<Person>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Persons
            .AsNoTracking()
            .OrderBy(person => person.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Person?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(person => person.Id == id, cancellationToken);
    }

    public async Task<Person> AddAsync(Person person, CancellationToken cancellationToken = default)
    {
        dbContext.Persons.Add(person);
        await dbContext.SaveChangesAsync(cancellationToken);
        return person;
    }

    public async Task<Person?> UpdateAsync(int id, Person person, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Persons.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(person.Name))
        {
            existing.Name = person.Name;
        }

        if (person.Age is not null)
        {
            existing.Age = person.Age;
        }

        if (person.Address is not null)
        {
            existing.Address = string.IsNullOrWhiteSpace(person.Address) ? null : person.Address;
        }

        if (person.Work is not null)
        {
            existing.Work = string.IsNullOrWhiteSpace(person.Work) ? null : person.Work;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var person = await dbContext.Persons.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (person is null)
        {
            return false;
        }

        dbContext.Persons.Remove(person);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
