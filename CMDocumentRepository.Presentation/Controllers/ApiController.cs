using CMDocumentRepository.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMDocumentRepository.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetPagedDocumentsQuery
        {
            PageNumber = page,
            PageSize = pageSize
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var document = await _mediator.Send(new GetDocumentByIdQuery { Id = id });
        if (document == null) return NotFound();
        return Ok(document);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetPagedDocumentsQuery
        {
            PageNumber = page,
            PageSize = pageSize,
            Keyword = keyword
        });
        return Ok(result);
    }
}

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StatsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _mediator.Send(new GetDashboardStatsQuery());
        return Ok(stats);
    }
}
