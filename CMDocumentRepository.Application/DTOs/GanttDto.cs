namespace CMDocumentRepository.Application.DTOs;

public record GanttDataDto
{
    public List<GanttItemDto> Items { get; init; } = new();
    public List<GanttDependencyDto> Dependencies { get; init; } = new();
    public DateTime TodayDate { get; init; }
    public string ZoomLevel { get; init; } = "month";
    public List<string> Groups { get; init; } = new();
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
    public string? Priority { get; init; } // Low, Medium, High, Critical
    public string? AssigneeName { get; init; }
    public string? CategoryName { get; init; }
    public string? DocumentTypeName { get; init; }
    public string? Group { get; init; } // для группировки
    public string Color { get; init; } = "#4a90d9";
    public Guid? ParentTaskId { get; init; }
    public List<Guid> ChildTaskIds { get; init; } = new();
    public string? Description { get; init; }
}

public record GanttDependencyDto
{
    public Guid FromId { get; init; }
    public Guid ToId { get; init; }
    public string Type { get; init; } = "FS"; // FS=Finish-to-Start
}