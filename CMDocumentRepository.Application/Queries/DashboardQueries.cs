using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using MediatR;

namespace CMDocumentRepository.Application.Queries;

public record GetDashboardStatsQuery : IRequest<DashboardStats> { }

public record DashboardStats
{
    public int TotalDocuments { get; init; }
    public int ActiveDocuments { get; init; }
    public int PendingApprovals { get; init; }
    public int TotalUsers { get; init; }
    public int TotalTasks { get; init; }
    public int CompletedTasks { get; init; }
    public int DocumentsThisMonth { get; init; }
    public int DocumentsByStatus_Draft { get; init; }
    public int DocumentsByStatus_Pending { get; init; }
    public int DocumentsByStatus_Approved { get; init; }
    public int DocumentsByStatus_Archived { get; init; }
}

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStats>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IApprovalRepository _approvalRepository;

    public GetDashboardStatsQueryHandler(
        IDocumentRepository documentRepository,
        IUserRepository userRepository,
        ITaskRepository taskRepository,
        IApprovalRepository approvalRepository)
    {
        _documentRepository = documentRepository;
        _userRepository = userRepository;
        _taskRepository = taskRepository;
        _approvalRepository = approvalRepository;
    }

    public async Task<DashboardStats> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var allDocs = await _documentRepository.GetAllAsync();
        var docs = allDocs.Where(d => !d.IsDeleted).ToList();
        var users = await _userRepository.GetAllAsync();
        var tasks = await _taskRepository.GetAllAsync();
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        return new DashboardStats
        {
            TotalDocuments = docs.Count,
            ActiveDocuments = docs.Count(d => d.Status == DocumentStatus.Active),
            PendingApprovals = docs.Count(d => d.Status == DocumentStatus.PendingApproval),
            TotalUsers = users.Count(),
            TotalTasks = tasks.Count(),
            CompletedTasks = tasks.Count(t => t.Status == AppTaskStatus.Done),
            DocumentsThisMonth = docs.Count(d => d.CreatedAt >= monthStart),
            DocumentsByStatus_Draft = docs.Count(d => d.Status == DocumentStatus.Draft),
            DocumentsByStatus_Pending = docs.Count(d => d.Status == DocumentStatus.PendingApproval),
            DocumentsByStatus_Approved = docs.Count(d => d.Status == DocumentStatus.Approved),
            DocumentsByStatus_Archived = docs.Count(d => d.Status == DocumentStatus.Archived)
        };
    }
}
