using MediatR;
using Whisper.Domain.Common;

namespace Whisper.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Username, string Password) : IRequest<Result<LoginResponse>>;

public record LoginResponse(string AccessToken, string RefreshToken, string UserId, string Username, string AvatarUrl);
