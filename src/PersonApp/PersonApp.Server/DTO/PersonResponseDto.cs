namespace PersonApp.Server.DTO;

public sealed record PersonResponseDto(
    int Id,
    string Name,
    int? Age,
    string? Address,
    string? Work);
