# CMDocumentRepository — KODA.md

## Обзор проекта

**CMDocumentRepository** — корпоративная система управления документами (Document Management System, DMS). Приложение предоставляет функциональность для хранения, версионирования, согласования, управления правами доступа и аудита документов.

### Ключевые возможности

- CRUD-операции с документами (создание, редактирование, мягкое удаление, восстановление из корзины)
- Версионирование документов
- Система согласования (маршруты согласования, утверждение/отклонение/доработка)
- Управление задачами (Kanban-доска, приоритеты, статусы)
- Ролевая модель доступа (SuperAdmin, Admin, Manager, User, Auditor, Guest)
- Аудит действий пользователей (AuditLog)
- Поиск и фильтрация документов
- Экспорт в Excel (ClosedXML)
- Cookie-based аутентификация
- Тёмная/светлая тема
- REST API (эндпоинты Documents, Stats)
- Toast-уведомления
- Горячие клавиши (Ctrl+S, Ctrl+F)

---

## Архитектура

Проект построен по принципам **Clean Architecture** с пятью проектами:

```
CMDocumentRepository/
├── CMDocumentRepository.Domain/           # Слой доменной логики
├── CMDocumentRepository.Application/      # Слой бизнес-логики (CQRS)
├── CMDocumentRepository.Infrastructure/   # Слой инфраструктуры (БД, файлы, сервисы)
├── CMDocumentRepository.Presentation/     # Presentation-слой (MVC, контроллеры, Views)
└── CMDocumentRepository.Tests/            # Unit-тесты
```

### Зависимости между слоями

```
Presentation → Application → Domain
                |
                v
            Infrastructure → Application → Domain
```

- **Domain** не зависит ни от одного из проектов
- **Application** зависит только от Domain
- **Infrastructure** зависит от Application (и Domain через Application)
- **Presentation** зависит от Application и Infrastructure
- **Tests** зависит от всех проектов

---

## Технологический стек

| Компонент | Технология |
|-----------|-----------|
| Платформа | .NET 10 (net10.0) |
| Веб-фреймворк | ASP.NET Core MVC |
| ORM | Entity Framework Core 10 |
| База данных | PostgreSQL 15 |
| Паттерн | CQRS (MediatR 12) |
| Валидация | FluentValidation 11 |
| Маппинг | AutoMapper 14 |
| Хэширование | BCrypt.Net-Next 4 |
| Логирование | Serilog 8 (консоль + файлы) |
| Экспорт | ClosedXML 0.102 |
| Почта | MailKit 4 |
| Аутентификация | Cookie Authentication |
| Тестирование | xUnit 2.9, Moq, FluentAssertions |

---

## Доменная модель

### Сущности (11 штук)

| Сущность | Описание |
|----------|----------|
| **User** | Пользователь системы |
| **Document** | Документ (с мягким удалением) |
| **DocumentVersion** | Версия документа |
| **DocumentType** | Тип документа |
| **Category** | Категория документа |
| **DocumentPermission** | Права доступа к документу |
| **Approval** | Запись согласования |
| **AppTask** | Задача приложения |
| **AuditLog** | Журнал аудита |
| **SystemSetting** | Настройка системы |

### Enum-ы (5 штук)

| Enum | Значения |
|------|----------|
| **DocumentStatus** | Draft, PendingApproval, Approved, Rejected, Rework, Active, Archived |
| **UserRole** | SuperAdmin, Admin, Manager, User, Auditor, Guest |
| **ApprovalStatus** | Pending, Approved, Rejected, Rework |
| **TaskPriority** | Low, Medium, High, Critical |
| **AppTaskStatus** | Backlog, InProgress, Review, Done |

---

## Структура кода

### Domain Layer (`CMDocumentRepository.Domain/`)
```
Entities/          # 10 сущностей (BaseEntity отсутствует в репозитории)
Enums/             # 5 enum-ов
Interfaces/        # IRepository<T>, IUserRepository, IDocumentRepository и др. (11 репозиториев + 4 сервиса)
```

### Application Layer (`CMDocumentRepository.Application/`)
```
Commands/          # CQRS-команды (Auth, User, Document, Approval, Task, Reference)
Queries/           # CQRS-запросы (User, Document, Approval, Task, Reference)
DTOs/              # DTO для всех сущностей
Validators/        # 12 валидаторов FluentValidation
Behaviors/         # ValidationBehavior, AuditBehavior (pipeline для MediatR)
Extensions/        # DI-расширение AddApplication()
```

### Infrastructure Layer (`CMDocumentRepository.Infrastructure/`)
```
Data/              # AppDbContext (10 DbSet), EntityTypeConfiguration (11 конфигураций)
Repositories/      # 11 репозиториев с LINQ-запросами
Services/          # FileService, JwtService, NumberingService, EmailService, ExportService, SearchService
```

### Presentation Layer (`CMDocumentRepository.Presentation/`)
```
Controllers/       # 9 контроллеров (Account, Home, Document, User, Approval, Task, Search, Admin, Api)
Views/             # 25+ представлений (Shared/Layout, Document, User, Approval, Task, Search, Admin, Account)
Middleware/        # ExceptionHandlingMiddleware, RequestLoggingMiddleware
wwwroot/           # CSS, JS
```

### Tests (`CMDocumentRepository.Tests/`)
```
Unit/              # 28 unit-тестов (User, Document, Task, Validation)
Helpers/           # TestData, MockRepositories
```

---

## Сборка и запуск

### Предварительные требования

- .NET 10 SDK
- PostgreSQL 15+

### Команды

```bash
# Восстановление пакетов
dotnet restore

# Сборка
dotnet build

# Запуск приложения
dotnet run --project CMDocumentRepository.Presentation

# Запуск с указанием URL
dotnet run --project CMDocumentRepository.Presentation --urls "http://localhost:5000"

# Запуск тестов
dotnet test

# Создание миграции
dotnet ef migrations add MigrationName --project CMDocumentRepository.Infrastructure --startup-project CMDocumentRepository.Presentation

# Применение миграций к БД
dotnet ef database update --project CMDocumentRepository.Infrastructure --startup-project CMDocumentRepository.Presentation

# Публикация (Release)
dotnet publish -c Release -o ./publish
```

### Данные для входа (после первого запуска)

| Поле | Значение |
|------|----------|
| URL | http://localhost:5000 |
| Логин | su |
| Пароль | 1234567890 |
| Роль | SuperAdmin |

> Суперпользователь создаётся автоматически при первом запуске (SeedData).

### Строка подключения

```
Host=localhost;Database=CMDocumentRepository;Username=postgres;Password=12345678
```

Настройка находится в `CMDocumentRepository.Infrastructure/appsettings.json`.

---

## Разработка

### Правила кодирования

- **Target Framework**: net10.0 для всех проектов
- **ImplicitUsings**: enabled
- **Nullable**: enabled
- Используется **CQRS** через MediatR для всех бизнес-операций
- Валидация входных данных через **FluentValidation** с `ValidationBehavior` для MediatR
- Контроллеры не содержат бизнес-логики — только вызовы `IMediator`

### Добавление новой сущности (чек-лист)

1. Создать сущность в `Domain/Entities/`
2. Создать enum в `Domain/Enums/` (если нужно)
3. Создать интерфейс репозитория в `Domain/Interfaces/`
4. Создать конфигурацию в `Infrastructure/Data/Configurations/`
5. Добавить `DbSet` в `AppDbContext`
6. Реализовать репозиторий в `Infrastructure/Repositories/`
7. Зарегистрировать DI в `Program.cs`
8. Создать DTO в `Application/DTOs/`
9. Создать команды/запросы в `Application/Commands/` и `Application/Queries/`
10. Создать обработчики
11. Создать валидатор в `Application/Validators/`
12. Создать контроллер в `Presentation/Controllers/`
13. Создать представления в `Presentation/Views/`
14. Создать миграцию: `dotnet ef migrations add Name --project CMDocumentRepository.Infrastructure`

### Добавление нового контроллера

```csharp
[Authorize]
public class MyController : Controller
{
    private readonly IMediator _mediator;

    public MyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _mediator.Send(new GetAllMyQuery());
        return View(result);
    }
}
```

### Тестирование

- Фреймворк: **xUnit 2.9**
- Моки: **Moq 4.20**
- Ассертации: **FluentAssertions 6**
- Покрытие: **coverlet.collector**
- Все тесты находятся в `CMDocumentRepository.Tests/`

```bash
# Запустить все тесты
dotnet test

# Запустить тесты с покрытием
dotnet test /p:CollectCoverage=true
```

---

## Нерешённые задачи

### Приоритет 0: Критические баги
- [ ] **Версионирование:** Ошибка уникальности при загрузке 3-й версии (IX_DocumentVersions_DocumentId_VersionNumber). Добавить IsMajorVersion, транзакции, проверку дубликатов
- [ ] **FTS Поиск:** SearchVector отсутствует в сущности Document. Добавить поле, GIN индекс, триггер PostgreSQL
- [ ] **JSONB Metadata:** Нет ValueConverter для JsonDocument. Добавить HasConversion() в DocumentConfiguration
- [ ] **Выбор согласующих:** Бэкенд поддерживает, но UI отсутствует. Добавить модальное окно на Details.cshtml

### Приоритет 1: Функциональные пробелы
- [ ] **Блокировка входов:** Нет полей LoginAttempts/LastLoginAttemptAt в User. Добавить логику блокировки после 5 неудачных попыток
- [ ] **Уведомления о сроках:** Нет фоновой задачи для проверки ValidUntil. Создать BackgroundService
- [ ] **Drag-and-drop загрузка:** Добавить DnD зону на Create/Edit.cshtml с прогресс-баром
- [ ] **Массовые операции:** Добавить чекбоксы и bulk-команды (BulkDelete, BulkSetStatus)
- [ ] **Смена статуса из UI:** Нет кнопки для изменения статуса. Добавить dropdown на Details.cshtml
- [ ] **Редактирование метаданных:** Нет UI для метаданных. Добавить секцию на Edit.cshtml

### Приоритет 2: Архитектурные улучшения
- [ ] **Разбить QueryHandlers.cs:** 1069 строк, 25 handlers → 8 отдельных файлов
- [ ] **Устранить N+1 запросы:** Заменить foreach с GetByIdAsync на ToDictionary
- [ ] **Добавить AutoMapper:** Заменить ручной маппинг в ~30 местах

### Приоритет 3: UI/UX улучшения
- [ ] **Модальные окна:** Заменить confirm() на Bootstrap-модалки
- [ ] **DataTables:** Подключить CDN для сортировки/фильтрации списков
- [ ] **Toast уведомления:** Перевести TempData на toast-компонент
- [ ] **Календарь:** Подключить flatpickr для полей дат
- [ ] **Последние документы:** Добавить блок на Home/Index.cshtml
- [ ] **Избранные:** Создать сущность DocumentBookmark

### Приоритет 4: Безопасность
- [ ] **Rate Limiting:** Настроить ASP.NET Rate Limiting middleware
- [ ] **Автоматический выход:** Настроить timeout для cookie auth

### Приоритет 5: Деплой и документация
- [ ] **Swagger/OpenAPI:** Добавить в Program.cs
- [ ] **Docker:** Создать Dockerfile + docker-compose.yml
- [ ] **Bash-скрипты:** install.sh, update.sh, backup.sh для Debian 12
- [ ] **Systemd + Nginx:** Настроить сервис и reverse proxy

### Приоритет 6: Тестирование
- [ ] **Unit тесты:** Расширить покрытие до >70% (сейчас ~10%)
- [ ] **Интеграционные тесты:** Добавить тесты для API, аутентификации, файлов

---

## Известные проблемы

### AutoMapper 14.0.0 — уязвимость
Известная уязвимость GHSA-rvv3-g6hj-g44x. Необходимо обновить при выходе патча.

### EF Core tools version
Версия tools 10.0.9 старше runtime 10.0.10. Для обновления:
```bash
dotnet tool update --global dotnet-ef
```

---

## Документация

Полная документация проекта находится в папке `docs/`:

| Файл | Описание |
|------|----------|
| `ЗАДАНИЕ_НА_РАЗРАБОТКУ.md` | Техническое задание на разработку |
| `ПЛАН_РАЗРАБОТКИ.md` | План разработки |
| `ПЛАН_ШАГОВ.md` | Детальный план шагов |
| `ПЛАН_ДОРАБОТОК.md` | План доработок |
| `СТАТУС_ПРОЕКТА.md` | Текущий статус проекта |
| `УПРАВЛЕНИЕ_ПРОЦЕССАМИ.md` | Процессы управления проектом |
| `РУКОВОДСТВО_ПРОГРАММИСТА.md` | Руководство для разработчиков |
| `РУКОВОДСТВО_ПОЛЬЗОВАТЕЛЯ.md` | Руководство для пользователей |
| `РУКОВОДСТВО_АДМИНИСТРАТОРА.md` | Руководство для администраторов |
| `что необходимо доработать.md` | Список необходимых доработок |
