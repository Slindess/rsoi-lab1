using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonApp.Application.Services;
using PersonApp.Core.Interfaces.Repositories;
using PersonApp.Core.Models;
using PersonApp.DataAccess.Database;
using PersonApp.DataAccess.Repositories;
using Xunit;

namespace PersonApp.Tests.Unit;

public sealed class PersonServiceTests
{
    [Fact]
    public async Task CreateAsync_AddsPerson()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var person = await service.CreateAsync(new Person
        {
            Name = " Alice ",
            Age = 31,
            Address = " Moscow ",
            Work = " Engineer "
        });

        Assert.True(person.Id > 0);
        Assert.Equal("Alice", person.Name);
        Assert.Equal("Moscow", person.Address);
        Assert.Equal("Engineer", person.Work);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPersonsOrderedById()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var first = await service.CreateAsync(new Person { Name = "First" });
        var second = await service.CreateAsync(new Person { Name = "Second" });

        var persons = await service.GetAllAsync();

        Assert.Collection(
            persons,
            item => Assert.Equal(first.Id, item.Id),
            item => Assert.Equal(second.Id, item.Id));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullWhenPersonDoesNotExist()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var person = await service.GetByIdAsync(404);

        Assert.Null(person);
    }

    [Fact]
    public async Task UpdateAsync_ChangesOnlyProvidedFields()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var created = await service.CreateAsync(new Person
        {
            Name = "Before",
            Age = 20,
            Work = "Developer"
        });

        var person = await service.UpdateAsync(created.Id, new Person
        {
            Name = "After",
            Address = "Kazan"
        });

        Assert.NotNull(person);
        Assert.Equal("After", person.Name);
        Assert.Equal(20, person.Age);
        Assert.Equal("Kazan", person.Address);
        Assert.Equal("Developer", person.Work);
    }

    [Fact]
    public async Task DeleteAsync_RemovesExistingPerson()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        var created = await service.CreateAsync(new Person { Name = "Alice" });

        var removed = await service.DeleteAsync(created.Id);
        var person = await service.GetByIdAsync(created.Id);

        Assert.True(removed);
        Assert.Null(person);
    }

    private static PersonService CreateService(PersonsDbContext dbContext)
    {
        IPersonRepository repository = new PersonRepository(dbContext);
        return new PersonService(repository);
    }

    private static PersonsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<PersonsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PersonsDbContext(options);
    }
}
