using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        string Generate(User user);
    }
    public interface IAccessTokenService : ITokenService
    {
    }
     public interface IRefreshTokenService : ITokenService
    {
    }

}
