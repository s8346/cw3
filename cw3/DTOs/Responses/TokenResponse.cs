using System;
namespace cw3.DTOs.Responses
{
    public class TokenResponse
    {
        public string accessToken { get; set; }
        public Guid refreshToken { get; set; }
    }
}
