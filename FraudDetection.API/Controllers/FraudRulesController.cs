using Microsoft.AspNetCore.Mvc;
using MediatR;
using FraudDetection.Application.Requests.Commands;
using FraudDetection.Application.Requests.Queries;

namespace FraudDetection.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FraudRulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FraudRulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    //get all fraud rules with optional filters
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRules(
        [FromQuery] bool? isActive,
        [FromQuery] string? ruleType,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllFraudRulesQuery
        {
            IsActive = isActive,
            RuleType = ruleType
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    //get fraud rule by id
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRule(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetFraudRuleByIdQuery { RuleId = id };
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    //create new fraud rule
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRule(
        [FromBody] CreateFraudRuleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetRule), new { id = result.Id }, result);
    }

    //update fraud rule
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRule(
        Guid id,
        [FromBody] UpdateFraudRuleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateFraudRuleCommand
        {
            RuleId = id,
            Name = request.Name,
            Description = request.Description,
            RiskLevel = request.RiskLevel
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    //activate fraud rule
    [HttpPut("{id}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateRule(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ActivateFraudRuleCommand { RuleId = id };
        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return NotFound();

        return Ok(new { message = "Rule activated" });
    }

    //deactivate fraud rule
    [HttpPut("{id}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateRule(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateFraudRuleCommand { RuleId = id };
        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return NotFound();

        return Ok(new { message = "Rule deactivated" });
    }

    //update fraud rule priority
    [HttpPut("{id}/priority")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePriority(
        Guid id,
        [FromBody] UpdatePriorityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateFraudRulePriorityCommand
        {
            RuleId = id,
            NewPriority = request.Priority
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return NotFound();

        return Ok(new { message = "Rule priority updated" });
    }
}

// Request DTOs for controller actions
public record UpdateFraudRuleRequest(string Name, string Description, string RiskLevel);
public record UpdatePriorityRequest(int Priority);