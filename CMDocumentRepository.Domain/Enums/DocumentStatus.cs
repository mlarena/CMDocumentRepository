namespace CMDocumentRepository.Domain.Enums;

public enum DocumentStatus
{
    Draft = 0,
    PendingApproval = 1,
    Approved = 2,
    Rejected = 3,
    Rework = 4,
    Active = 5,
    Archived = 6
}
