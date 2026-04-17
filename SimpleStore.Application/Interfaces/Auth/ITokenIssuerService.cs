using SimpleStore.Application.Dto.Auth;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Interfaces.Auth
{
    public interface ITokenIssuerService
    {
        Task<AuthenticateResponse> IssueTokensAsync(User user, CancellationToken cancellationToken);
    }
}
