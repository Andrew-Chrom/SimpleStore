using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Application.Dto.Auth
{
    public class AuthenticateResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
