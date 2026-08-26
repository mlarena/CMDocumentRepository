using System.Text.Json;
using CMDocumentRepository.Application.Commands;
using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CMDocumentRepository.Application.Behaviors;

/// <summary>
/// Pipeline behavior для автоматической записи audit-логов при выполнении команд.
/// Перехватывает все команды (не запросы) и фиксирует действия пользователей.
/// </summary>
public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDocumentPermissionRepository _permissionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditBehavior<TRequest, TResponse>> _logger;

    public AuditBehavior(
        IAuditLogRepository auditLogRepository,
        IUserRepository userRepository,
        IDocumentRepository documentRepository,
        ITaskRepository taskRepository,
        IDocumentTypeRepository documentTypeRepository,
        ICategoryRepository categoryRepository,
        IDocumentPermissionRepository permissionRepository,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuditBehavior<TRequest, TResponse>> logger)
    {
        _auditLogRepository = auditLogRepository;
        _userRepository = userRepository;
        _documentRepository = documentRepository;
        _taskRepository = taskRepository;
        _documentTypeRepository = documentTypeRepository;
        _categoryRepository = categoryRepository;
        _permissionRepository = permissionRepository;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var userId = ExtractUserId();
        if (userId == null)
        {
            // Если пользователь не авторизован — пропускаем логирование
            return await next();
        }

        var action = MapAction(request);
        var entityType = MapEntityType(request);
        var entityId = ExtractEntityId(request);

        // Если это запрос, а не команда — пропускаем (только чтение)
        if (string.IsNullOrEmpty(action))
        {
            return await next();
        }

        // Извлекаем old values (данные ДО выполнения) — только для update/delete команд
        JsonDocument? oldValues = null;
        JsonDocument? newValues = null;

        if (request is UpdateDocumentCommand updateDoc)
        {
            var doc = await _documentRepository.GetByIdAsync(updateDoc.Id);
            if (doc != null)
            {
                oldValues = SerializeToJson(GetDocumentSnapshot(doc));
                newValues = SerializeToJson(new
                {
                    updateDoc.Title,
                    updateDoc.Description,
                    updateDoc.CategoryId,
                    updateDoc.DocumentTypeId,
                    updateDoc.ValidFrom,
                    updateDoc.ValidUntil,
                    updateDoc.ChangeComment
                });
            }
        }
        else if (request is UpdateUserCommand updateUser)
        {
            var user = await _userRepository.GetByIdAsync(updateUser.Id);
            if (user != null)
            {
                oldValues = SerializeToJson(GetUserSnapshot(user));
                newValues = SerializeToJson(new
                {
                    updateUser.Email,
                    updateUser.FirstName,
                    updateUser.LastName,
                    updateUser.MiddleName,
                    updateUser.Role,
                    updateUser.IsActive
                });
            }
        }
        else if (request is UpdateTaskCommand updateTask)
        {
            var task = await _taskRepository.GetByIdAsync(updateTask.Id);
            if (task != null)
            {
                oldValues = SerializeToJson(new
                {
                    task.Title,
                    task.Description,
                    task.Priority,
                    task.Status,
                    task.AssigneeId,
                    task.DueDate
                });
                newValues = SerializeToJson(new
                {
                    updateTask.Title,
                    updateTask.Description,
                    updateTask.Priority,
                    updateTask.Status,
                    updateTask.AssigneeId,
                    updateTask.DueDate
                });
            }
        }
        else if (request is UpdateDocumentTypeCommand updateDocType)
        {
            var docType = await _documentTypeRepository.GetByIdAsync(updateDocType.Id);
            if (docType != null)
            {
                oldValues = SerializeToJson(new { docType.Name, docType.Code, docType.Description });
                newValues = SerializeToJson(new { updateDocType.Name, updateDocType.Description });
            }
        }
        else if (request is UpdateCategoryCommand updateCategory)
        {
            var category = await _categoryRepository.GetByIdAsync(updateCategory.Id);
            if (category != null)
            {
                oldValues = SerializeToJson(new { category.Name, category.Code, category.Description });
                newValues = SerializeToJson(new { updateCategory.Name, updateCategory.Description });
            }
        }
        else if (request is DeleteDocumentCommand deleteDoc)
        {
            var doc = await _documentRepository.GetByIdAsync(deleteDoc.Id);
            if (doc != null)
            {
                oldValues = SerializeToJson(GetDocumentSnapshot(doc));
            }
        }
        else if (request is RestoreDocumentCommand restoreDoc)
        {
            var doc = await _documentRepository.GetByIdAsync(restoreDoc.Id);
            if (doc != null)
            {
                oldValues = SerializeToJson(GetDocumentSnapshot(doc));
            }
        }
        else if (request is PermanentDeleteDocumentCommand permDelete)
        {
            var doc = await _documentRepository.GetByIdAsync(permDelete.Id);
            if (doc != null)
            {
                oldValues = SerializeToJson(GetDocumentSnapshot(doc));
            }
        }
        else if (request is DeleteUserCommand deleteUser)
        {
            var user = await _userRepository.GetByIdAsync(deleteUser.Id);
            if (user != null)
            {
                oldValues = SerializeToJson(GetUserSnapshot(user));
            }
        }
        else if (request is DeleteTaskCommand deleteTask)
        {
            var task = await _taskRepository.GetByIdAsync(deleteTask.Id);
            if (task != null)
            {
                oldValues = SerializeToJson(new { task.Title, task.Status, task.Priority });
            }
        }
        else if (request is DeleteDocumentTypeCommand deleteDocType)
        {
            var docType = await _documentTypeRepository.GetByIdAsync(deleteDocType.Id);
            if (docType != null)
            {
                oldValues = SerializeToJson(new { docType.Name, docType.Code });
            }
        }
        else if (request is DeleteCategoryCommand deleteCategory)
        {
            var category = await _categoryRepository.GetByIdAsync(deleteCategory.Id);
            if (category != null)
            {
                oldValues = SerializeToJson(new { category.Name, category.Code });
            }
        }
        else if (request is ToggleUserLockCommand toggleLock)
        {
            var user = await _userRepository.GetByIdAsync(toggleLock.Id);
            if (user != null)
            {
                oldValues = SerializeToJson(new { user.IsLocked, user.LockedUntil });
                newValues = SerializeToJson(new { Locked = toggleLock.Lock, LockedUntil = toggleLock.LockMinutes.HasValue ? DateTime.UtcNow.AddMinutes(toggleLock.LockMinutes.Value) : (DateTime?)null });
            }
        }
        else if (request is SetDocumentStatusCommand setStatus)
        {
            var doc = await _documentRepository.GetByIdAsync(setStatus.Id);
            if (doc != null)
            {
                oldValues = SerializeToJson(new { OldStatus = doc.Status.ToString() });
                newValues = SerializeToJson(new { NewStatus = setStatus.Status.ToString() });
            }
        }
        else if (request is SendForApprovalCommand sendApproval)
        {
            var doc = await _documentRepository.GetByIdAsync(sendApproval.DocumentId);
            if (doc != null)
            {
                oldValues = SerializeToJson(new { DocumentNumber = doc.DocumentNumber, DocumentTitle = doc.Title });
                newValues = SerializeToJson(new { ApproverCount = sendApproval.ApproverIds.Count, IsSequential = sendApproval.IsSequential });
                entityId = sendApproval.DocumentId;
            }
        }
        else if (request is ApproveDocumentCommand approve)
        {
            entityId = approve.ApprovalId;
        }
        else if (request is RejectDocumentCommand reject)
        {
            entityId = reject.ApprovalId;
        }
        else if (request is RequestReworkCommand rework)
        {
            entityId = rework.ApprovalId;
        }
        else if (request is SetDocumentPermissionCommand setPerm)
        {
            oldValues = SerializeToJson(new { DocumentId = setPerm.DocumentId, UserId = setPerm.UserId });
            newValues = SerializeToJson(new { setPerm.CanRead, setPerm.CanEdit, setPerm.CanApprove, setPerm.CanDelete });
            entityId = setPerm.DocumentId;
        }
        else if (request is RemoveDocumentPermissionCommand removePerm)
        {
            oldValues = SerializeToJson(new { DocumentId = removePerm.DocumentId, UserId = removePerm.UserId });
            newValues = SerializeToJson(new { Removed = true });
            entityId = removePerm.DocumentId;
        }

        // Выполняем команду
        TResponse result;
        try
        {
            result = await next();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при выполнении команды {CommandType}", typeof(TRequest).Name);
            throw;
        }

        // Записываем audit-лог
        var auditEntry = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId.Value,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            CreatedAt = DateTime.UtcNow
        };

        await _auditLogRepository.AddAsync(auditEntry);

        return result;
    }

    #region Helpers

    /// <summary>
    /// Извлекает ID пользователя из HttpContext (claim NameIdentifier).
    /// </summary>
    private Guid? ExtractUserId()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) is var claim && claim != null)
            {
                if (Guid.TryParse(claim.Value, out var userId))
                    return userId;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось извлечь userId из HttpContext");
        }
        return null;
    }

    /// <summary>
    /// Маппит тип команды на строковое действие.
    /// </summary>
    private static string MapAction(TRequest request)
    {
        return request switch
        {
            // Документы
            CreateDocumentCommand => "Создан документ",
            UpdateDocumentCommand => "Обновлён документ",
            DeleteDocumentCommand => "Удалён документ",
            RestoreDocumentCommand => "Восстановлен документ",
            PermanentDeleteDocumentCommand => "Удалён навсегда документ",
            SetDocumentStatusCommand => "Изменён статус документа",

            // Пользователи
            CreateUserCommand => "Создан пользователь",
            UpdateUserCommand => "Обновлён пользователь",
            DeleteUserCommand => "Удалён пользователь",
            ToggleUserLockCommand => "Заблокирован/Разблокирован пользователь",

            // Задачи
            CreateTaskCommand => "Создана задача",
            UpdateTaskCommand => "Обновлена задача",
            DeleteTaskCommand => "Удалена задача",
            MoveTaskCommand => "Перемещена задача",

            // Согласование
            SendForApprovalCommand => "Отправлен на согласование",
            ApproveDocumentCommand => "Документ согласован",
            RejectDocumentCommand => "Документ отклонён",
            RequestReworkCommand => "Документ отправлен на доработку",

            // Справочники
            CreateDocumentTypeCommand => "Создан тип документа",
            UpdateDocumentTypeCommand => "Обновлён тип документа",
            DeleteDocumentTypeCommand => "Удалён тип документа",
            CreateCategoryCommand => "Создана категория",
            UpdateCategoryCommand => "Обновлена категория",
            DeleteCategoryCommand => "Удалена категория",

            // Права доступа
            SetDocumentPermissionCommand => "Назначены права доступа",
            RemoveDocumentPermissionCommand => "Удалены права доступа",

            // Аутентификация
            LoginCommand => "Выполнен вход в систему",
            LogoutCommand => "Выполнен выход из системы",
            RefreshTokenCommand => "Обновлён refresh токен",
            ChangePasswordCommand => "Изменён пароль",
            ResetPasswordCommand => "Сброшен пароль",

            // Все остальное — пропускаем
            _ => null
        };
    }

    /// <summary>
    /// Маппит тип команды на строку EntityType.
    /// </summary>
    private static string MapEntityType(TRequest request)
    {
        return request switch
        {
            CreateDocumentCommand or UpdateDocumentCommand or DeleteDocumentCommand
                or RestoreDocumentCommand or PermanentDeleteDocumentCommand or SetDocumentStatusCommand
                => "Document",

            CreateUserCommand or UpdateUserCommand or DeleteUserCommand or ToggleUserLockCommand
                => "User",

            CreateTaskCommand or UpdateTaskCommand or DeleteTaskCommand or MoveTaskCommand
                => "Task",

            SendForApprovalCommand or ApproveDocumentCommand or RejectDocumentCommand or RequestReworkCommand
                => "Approval",

            CreateDocumentTypeCommand or UpdateDocumentTypeCommand or DeleteDocumentTypeCommand
                => "DocumentType",

            CreateCategoryCommand or UpdateCategoryCommand or DeleteCategoryCommand
                => "Category",

            SetDocumentPermissionCommand or RemoveDocumentPermissionCommand
                => "DocumentPermission",

            LoginCommand or LogoutCommand or RefreshTokenCommand or ChangePasswordCommand or ResetPasswordCommand
                => "Auth",

            _ => "Unknown"
        };
    }

    /// <summary>
    /// Извлекает EntityId из команды, если возможно.
    /// </summary>
    private static Guid? ExtractEntityId(TRequest request)
    {
        return request switch
        {
            UpdateDocumentCommand cmd => cmd.Id,
            DeleteDocumentCommand cmd => cmd.Id,
            RestoreDocumentCommand cmd => cmd.Id,
            PermanentDeleteDocumentCommand cmd => cmd.Id,
            SetDocumentStatusCommand cmd => cmd.Id,
            UpdateUserCommand cmd => cmd.Id,
            DeleteUserCommand cmd => cmd.Id,
            ToggleUserLockCommand cmd => cmd.Id,
            UpdateTaskCommand cmd => cmd.Id,
            DeleteTaskCommand cmd => cmd.Id,
            MoveTaskCommand cmd => cmd.TaskId,
            UpdateDocumentTypeCommand cmd => cmd.Id,
            DeleteDocumentTypeCommand cmd => cmd.Id,
            UpdateCategoryCommand cmd => cmd.Id,
            DeleteCategoryCommand cmd => cmd.Id,
            _ => null
        };
    }

    /// <summary>
    /// Сериализует объект в JsonDocument для хранения в AuditLog.
    /// </summary>
    private static JsonDocument? SerializeToJson(object data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            return JsonDocument.Parse(json);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    /// <summary>
    /// Создаёт снимок документа для логирования.
    /// </summary>
    private static object GetDocumentSnapshot(Document doc)
    {
        return new
        {
            doc.DocumentNumber,
            doc.Title,
            doc.Description,
            doc.Status,
            doc.Version,
            doc.CategoryId,
            doc.DocumentTypeId,
            doc.CreatedBy,
            doc.CreatedAt
        };
    }

    /// <summary>
    /// Создаёт снимок пользователя для логирования.
    /// </summary>
    private static object GetUserSnapshot(User user)
    {
        return new
        {
            user.UserName,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role,
            user.IsActive,
            user.IsLocked
        };
    }

    #endregion
}
