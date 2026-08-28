namespace CMDocumentRepository.Application.DTOs;

public record DocumentMetadataDto
{
    public Guid Id { get; init; }
    public Dictionary<string, string?>? Metadata { get; init; }
}