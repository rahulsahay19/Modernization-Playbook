namespace HealthCare.Claims.DocumentsService.Application.DTOs;

public sealed record DocumentCommandResult(
    DocumentResponse? Document,
    string? Error,
    bool NotFound)
{
    public static DocumentCommandResult Success(DocumentResponse document) =>
        new(document, null, false);

    public static DocumentCommandResult BadRequest(string error) =>
        new(null, error, false);

    public static DocumentCommandResult Missing() =>
        new(null, null, true);
}
