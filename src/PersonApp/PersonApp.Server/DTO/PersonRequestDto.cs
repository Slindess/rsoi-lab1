namespace PersonApp.Server.DTO;

public sealed record PersonRequestDto(
    string? Name,
    int? Age,
    string? Address,
    string? Work);
