using Devshift.ResponseMessage;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Api.Shared.Facades;

namespace QueueManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueueController : ControllerBase
{
    private readonly IQueueFacade _queue;

    public QueueController(IQueueFacade queue)
    {
        _queue = queue;
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentQueue()
    {
        var resp = await _queue.GetCurrentQueueAsync();
        if (resp.Message == Message.Success)
        {
            return Ok(resp);
        }
        return BadRequest();
    }

    [HttpPost("next")]
    public async Task<IActionResult> GetNextQueue()
    {
        var resp = await _queue.GetNextQueueAsync();
        if (resp.Message == Message.Success)
        {
            return Ok(resp);
        }
        return BadRequest();
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetQueue()
    {
        var resp = await _queue.ResetQueueAsync();
        if (resp.Message == Message.Success)
        {
            return Ok(resp);
        }
        return BadRequest();
    }
}
