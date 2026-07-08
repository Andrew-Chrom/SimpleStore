using SimpleStore.Application.Command.Auth;
using SimpleStore.Application.Common;
using SimpleStore.Application.Dto.Auth;
using SimpleStore.Application.Query.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<Result<Guid>> RegisterAsync(RegisterCommand command, CancellationToken ct);
        Task<Result<AuthenticateResponse>> LoginAsync(LoginQuery command, CancellationToken ct);
    }
}
