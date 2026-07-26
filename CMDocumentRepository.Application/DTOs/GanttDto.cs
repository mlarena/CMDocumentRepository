namespace CMDocumentRepository.Application.DTOs;

public record GanttDataDto
{
    public List<GanttItemDto> Items { get; init; } = new();
}

public record GanttItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty; // "document" or "task"
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public int Progress { get; init; } // 0-100
    public string? Url { get; init; }
    public string? Status { get; init; }
}