using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Whisper.Application.Features.Auth.Commands.Login;

namespace Whisper.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return Unauthorized(result);
    }
}
