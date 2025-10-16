using InventorySys.Application.Common.Interfaces;
using InventorySys.Application.Common.Models;
using InventorySys.Domain.Constants;

namespace InventorySys.Application.Auth.Commands.Login;

public class LoginCommand : IRequest<AuthenticateResponse>
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(v => v.Username)
                .NotEmpty().WithMessage("Username is required.");

        RuleFor(v => v.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(IdentityConsts.PasswordRequiredLength).WithMessage($"Password must be at least {IdentityConsts.PasswordRequiredLength} characters long.");
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticateResponse>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<AuthenticateResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return _identityService.LoginAsync(request.Username, request.Password);
    }
}
