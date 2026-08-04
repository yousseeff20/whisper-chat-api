using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using Whisper.Application.Common.Interfaces;
using Whisper.Domain.Common;
using Whisper.Domain.Entities;

namespace Whisper.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(UserManager<User> userManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null)
        {
            return Result.Failure<LoginResponse>(new Error("Auth.InvalidCredentials", "Invalid username or password."));
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isValidPassword)
        {
            return Result.Failure<LoginResponse>(new Error("Auth.InvalidCredentials", "Invalid username or password."));
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        var refreshToken = "mock-refresh-token"; // Should implement actual refresh token logic

        return Result.Success(new LoginResponse(
            token,
            refreshToken,
            user.Id.ToString(),
            user.UserName!,
            user.Avatar ?? ""
        ));
    }
}
