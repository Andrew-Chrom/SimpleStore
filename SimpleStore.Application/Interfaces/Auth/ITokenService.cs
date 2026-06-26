using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        Task<string> GenerateAsync(User user);
    }
    public interface IAccessTokenService : ITokenService
    {
    }
     public interface IRefreshTokenService : ITokenService
    {
    }

}
