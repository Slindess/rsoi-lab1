using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonApp.Core.Interfaces.Services;
using PersonApp.Core.Models;
using PersonApp.Server.DTO;

namespace PersonApp.Server.Controllers.v1;

[ApiController]
[Route("api/v1/persons")]
public sealed class PersonsController(IPersonService personService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PersonResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PersonResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var persons = await personService.GetAllAsync(cancellationToken);
        return Ok(persons.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PersonResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonResponseDto>> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var person = await personService.GetByIdAsync(id, cancellationToken);
        if (person is null)
        {
            return NotFound(new ErrorResponseDto($"Person with id '{id}' was not found."));
        }

        return Ok(ToDto(person));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PersonRequestDto request, CancellationToken cancellationToken)
    {
        var validation = Validate(request);
        if (validation is not null)
        {
            return BadRequest(validation);
        }

        var person = await personService.CreateAsync(ToModel(request), cancellationToken);
        Response.Headers.Location = $"/api/v1/persons/{person.Id}";

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(PersonResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonResponseDto>> Update(
        [FromRoute] int id,
        [FromBody] PersonRequestDto request,
        CancellationToken cancellationToken)
    {
        var validation = Validate(request, requireName: false);
        if (validation is not null)
        {
            return BadRequest(validation);
        }

        var person = await personService.UpdateAsync(id, ToModel(request), cancellationToken);
        if (person is null)
        {
            return NotFound(new ErrorResponseDto($"Person with id '{id}' was not found."));
        }

        return Ok(ToDto(person));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var removed = await personService.DeleteAsync(id, cancellationToken);
        if (!removed)
        {
            return NotFound(new ErrorResponseDto($"Person with id '{id}' was not found."));
        }

        return NoContent();
    }

    private static Person ToModel(PersonRequestDto request)
    {
        return new Person
        {
            Name = request.Name ?? string.Empty,
            Age = request.Age,
            Address = request.Address,
            Work = request.Work
        };
    }

    private static PersonResponseDto ToDto(Person person)
    {
        return new PersonResponseDto(person.Id, person.Name, person.Age, person.Address, person.Work);
    }

    private static ValidationErrorResponseDto? Validate(PersonRequestDto request, bool requireName = true)
    {
        var errors = new Dictionary<string, string>();

        if (requireName && string.IsNullOrWhiteSpace(request.Name))
        {
            errors["name"] = "Name is required.";
        }

        if (request.Name is not null && string.IsNullOrWhiteSpace(request.Name))
        {
            errors["name"] = "Name cannot be empty.";
        }

        if (request.Age is < 0)
        {
            errors["age"] = "Age must be greater than or equal to zero.";
        }

        return errors.Count == 0
            ? null
            : new ValidationErrorResponseDto("Invalid data.", errors);
    }
}
