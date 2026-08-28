using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using MediatR;

namespace CMDocumentRepository.Application.Commands;

public class SendForApprovalCommandHandler : IRequestHandler<SendForApprovalCommand, bool>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IApprovalRepository _approvalRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public SendForApprovalCommandHandler(
        IDocumentRepository documentRepository,
        IApprovalRepository approvalRepository,
        IUserRepository userRepository,
        IEmailService emailService)
    {
        _documentRepository = documentRepository;
        _approvalRepository = approvalRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<bool> Handle(SendForApprovalCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.DocumentId)
            ?? throw new KeyNotFoundException("Документ не найден");

        if (document.Status != DocumentStatus.Draft && document.Status != DocumentStatus.Rework)
            throw new InvalidOperationException("Документ можно отправить на согласование только из статуса Черновик или На доработке");

        var sender = await _userRepository.GetByIdAsync(request.SentBy)
            ?? throw new KeyNotFoundException("Отправитель не найден");

        var order = 1;
        foreach (var approverId in request.ApproverIds)
        {
            var approver = await _userRepository.GetByIdAsync(approverId);
            if (approver == null) continue;

            var approval = new Approval
            {
                Id = Guid.NewGuid(),
                DocumentId = request.DocumentId,
                ApproverId = approverId,
                Status = ApprovalStatus.Pending,
                OrderNumber = request.IsSequential ? order++ : 1,
                CreatedAt = DateTime.UtcNow
            };

            await _approvalRepository.AddAsync(approval);

            await _emailService.SendApprovalNotificationAsync(
                approver.Email,
                document.DocumentNumber,
                document.Title,
                $"/documents/{document.Id}");
        }

        document.Status = DocumentStatus.PendingApproval;
        await _documentRepository.UpdateAsync(document);

        return true;
    }
}

public class ApproveDocumentCommandHandler : IRequestHandler<ApproveDocumentCommand, bool>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IApprovalRepository _approvalRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public ApproveDocumentCommandHandler(
        IDocumentRepository documentRepository,
        IApprovalRepository approvalRepository,
        IUserRepository userRepository,
        IEmailService emailService)
    {
        _documentRepository = documentRepository;
        _approvalRepository = approvalRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<bool> Handle(ApproveDocumentCommand request, CancellationToken cancellationToken)
    {
        var approval = await _approvalRepository.GetByIdAsync(request.ApprovalId)
            ?? throw new KeyNotFoundException("Запись согласования не найдена");

        if (approval.ApproverId != request.ApproverId)
            throw new UnauthorizedAccessException("Вы не являетесь согласующим для этого документа");

        if (approval.Status != ApprovalStatus.Pending)
            throw new InvalidOperationException("Этот документ уже согласован или отклонён");

        approval.Status = ApprovalStatus.Approved;
        approval.Comment = request.Comment;
        approval.ApprovedAt = DateTime.UtcNow;
        await _approvalRepository.UpdateAsync(approval);

        if (await _approvalRepository.AllApprovedAsync(approval.DocumentId))
        {
            var document = await _documentRepository.GetByIdAsync(approval.DocumentId);
            if (document != null)
            {
                document.Status = DocumentStatus.Approved;
                document.ApprovedBy = request.ApproverId;
                document.ApprovedAt = DateTime.UtcNow;
                await _documentRepository.UpdateAsync(document);

                var creator = await _userRepository.GetByIdAsync(document.CreatedBy);
                if (creator != null)
                {
                    await _emailService.SendApprovalResultAsync(
                        creator.Email,
                        document.DocumentNumber,
                        document.Title,
                        "Согласован",
                        request.Comment);
                }
            }
        }

        return true;
    }
}

public class RejectDocumentCommandHandler : IRequestHandler<RejectDocumentCommand, bool>
{
    private readonly IApprovalRepository _approvalRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentVersionRepository _versionRepository;

    public RejectDocumentCommandHandler(
        IApprovalRepository approvalRepository,
        IDocumentRepository documentRepository,
        IDocumentVersionRepository versionRepository)
    {
        _approvalRepository = approvalRepository;
        _documentRepository = documentRepository;
        _versionRepository = versionRepository;
    }

    public async Task<bool> Handle(RejectDocumentCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Comment))
            throw new ArgumentException("Причина отклонения обязательна для заполнения");

        var approval = await _approvalRepository.GetByIdAsync(request.ApprovalId);
        if (approval == null)
            throw new KeyNotFoundException("Запись согласования не найдена");

        if (approval.Status != ApprovalStatus.Pending)
            throw new InvalidOperationException("Документ уже обработан");

        approval.Status = ApprovalStatus.Rejected;
        approval.Comment = request.Comment;
        approval.ApprovedAt = DateTime.UtcNow;
        await _approvalRepository.UpdateAsync(approval);

        var document = await _documentRepository.GetByIdAsync(approval.DocumentId);
        if (document != null)
        {
            document.Status = DocumentStatus.Rejected;
            await _documentRepository.UpdateAsync(document);
        }

        return true;
    }
}

public class RequestReworkCommandHandler : IRequestHandler<RequestReworkCommand, bool>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IApprovalRepository _approvalRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public RequestReworkCommandHandler(
        IDocumentRepository documentRepository,
        IApprovalRepository approvalRepository,
        IUserRepository userRepository,
        IEmailService emailService)
    {
        _documentRepository = documentRepository;
        _approvalRepository = approvalRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<bool> Handle(RequestReworkCommand request, CancellationToken cancellationToken)
    {
        var approval = await _approvalRepository.GetByIdAsync(request.ApprovalId)
            ?? throw new KeyNotFoundException("Запись согласования не найдена");

        if (approval.ApproverId != request.ApproverId)
            throw new UnauthorizedAccessException("Вы не являетесь согласующим для этого документа");

        if (approval.Status != ApprovalStatus.Pending)
            throw new InvalidOperationException("Этот документ уже согласован или отклонён");

        approval.Status = ApprovalStatus.Rework;
        approval.Comment = request.Comment;
        approval.ApprovedAt = DateTime.UtcNow;
        await _approvalRepository.UpdateAsync(approval);

        var document = await _documentRepository.GetByIdAsync(approval.DocumentId);
        if (document != null)
        {
            document.Status = DocumentStatus.Rework;
            await _documentRepository.UpdateAsync(document);

            var creator = await _userRepository.GetByIdAsync(document.CreatedBy);
            if (creator != null)
            {
                await _emailService.SendApprovalResultAsync(
                    creator.Email,
                    document.DocumentNumber,
                    document.Title,
                    "На доработке",
                    request.Comment);
            }
        }

        return true;
    }
}
