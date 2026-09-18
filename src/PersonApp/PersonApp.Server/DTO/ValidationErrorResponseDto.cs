using System.Collections.Generic;

namespace PersonApp.Server.DTO;

public sealed record ValidationErrorResponseDto(string Message, IReadOnlyDictionary<string, string> Errors);
