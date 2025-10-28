using Microsoft.AspNetCore.Mvc;
using MediatR;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Application.Requests.Queries;

namespace FraudDetection.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FraudAlertsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FraudAlertsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //get all fraud alerts with optional filters
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAlerts(
        [FromQuery] string? status,
        [FromQuery] string? riskLevel,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllFraudAlertsQuery
        {
            Status = status,
            RiskLevel = riskLevel,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    //get fraud alert by id
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAlert(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetFraudAlertByIdQuery { AlertId = id };
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    //mark alert as under investigation
    [HttpPut("{id}/investigate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> InvestigateAlert(
        Guid id,
        [FromBody] InvestigateRequest request,
        CancellationToken cancellationToken)
    {
        var command = new InvestigateFraudAlertCommand
        {
            AlertId = id,
            InvestigatedBy = request.InvestigatedBy
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return NotFound();

        return Ok(new { message = "Alert marked as investigating" });
    }

    //resolve fraud alert
    [HttpPut("{id}/resolve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResolveAlert(
        Guid id,
        [FromBody] ResolveRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ResolveFraudAlertCommand
        {
            AlertId = id,
            ResolvedBy = request.ResolvedBy,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return NotFound();

        return Ok(new { message = "Alert resolved" });
    }

    //mark alert as false positive
    [HttpPut("{id}/false-positive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsFalsePositive(
        Guid id,
        [FromBody] ReviewRequest request,
        CancellationToken cancellationToken)
    {
        var command = new MarkAsFalsePositiveCommand
        {
            AlertId = id,
            ReviewedBy = request.ReviewedBy,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return NotFound();

        return Ok(new { message = "Alert marked as false positive" });
    }

    //confirm fraud
    [HttpPut("{id}/confirm-fraud")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmFraud(
        Guid id,
        [FromBody] ReviewRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmFraudCommand
        {
            AlertId = id,
            ConfirmedBy = request.ReviewedBy,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return NotFound();

        return Ok(new { message = "Fraud confirmed" });
    }

    //get recent alerts
    [HttpGet("recent")]
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

// Request DTOs for controller actions
public record InvestigateRequest(string InvestigatedBy);
public record ResolveRequest(string ResolvedBy, string Notes);
public record ReviewRequest(string ReviewedBy, string Notes);