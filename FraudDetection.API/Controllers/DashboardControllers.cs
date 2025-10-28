using Microsoft.AspNetCore.Mvc;
using MediatR;
using FraudDetection.Application.Requests.Queries;

namespace FraudDetection.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //get dashboard statistics
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatistics(CancellationToken cancellationToken)
    {
        var query = new GetDashboardStatisticsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    //get recent transactions for dashboard
    [HttpGet("recent-transactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentTransactions(
        [FromQuery] int count = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRecentTransactionsQuery { Count = count };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    //get recent alerts for dashboard
    [HttpGet("recent-alerts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentAlerts(
        [FromQuery] int count = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRecentAlertsQuery { Count = count };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}