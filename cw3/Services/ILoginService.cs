using System;
using AuthenticationSampleWebApp.DTOs;
using cw3.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Services
{
    public interface ILoginService
    {
        TokenResponse Login(LoginRequestDto login);
        TokenResponse RefreshToken(string rToken);
    }
}
