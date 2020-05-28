using System;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationSampleWebApp.DTOs;
using cw3.DTOs;
using cw3.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace cw3.Services
{
    public class LoginService : ILoginService
    {
        public IConfiguration Configuration { get; set; }

        public LoginService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public TokenResponse Login(LoginRequestDto requestDto)
        {
            using (var sqlConnection = new SqlConnection("Data Source=10.1.1.36; Initial Catalog=s8346;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                sqlConnection.Open();
                command.Connection = sqlConnection;
                command.CommandText = "select IndexNumber, Password, Salt from Student where IndexNumber = @index";
                command.Parameters.AddWithValue("index", requestDto.Login);
                command.Parameters.AddWithValue("password", requestDto.Password);
                var dr = command.ExecuteReader();
                if (!dr.Read())
                {
                    return null;
                }
                var indexNum = dr["IndexNumber"].ToString();
                var lastName = dr["LastName"].ToString();
                var password = dr["Password"].ToString();
                var salt = dr["Salt"].ToString();
                dr.Close();

                bool verify = PasswordHashing.Validate(requestDto.Password, salt, password);
                if (!verify)
                {
                    return null;
                }

                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, indexNum),
                new Claim(ClaimTypes.Name, lastName),
                new Claim(ClaimTypes.Role,  "Employee"),
                new Claim(ClaimTypes.Role,  "Student"),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );

                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                var refreshToken = Guid.NewGuid();
                command.CommandText = "UPDATE student SET RefreshToken = @rToken WHERE IndexNumber = @id";
                command.Parameters.AddWithValue("rToken", refreshToken);
                command.Parameters.AddWithValue("id", indexNum);
                var q = command.ExecuteNonQuery();
                TokenResponse tokenResponse = new TokenResponse();
                tokenResponse.refreshToken = refreshToken;
                tokenResponse.accessToken = accessToken;
                return tokenResponse;
            }
        }

            public TokenResponse RefreshToken(string rToken)
        {
            using (var sqlConnection = new SqlConnection("Data Source=10.1.1.36; Initial Catalog=s8346;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                sqlConnection.Open();
                command.Connection = sqlConnection;
                command.CommandText = "SELECT IndexNumber,LastName, RefreshToken FROM Student WHERE RefreshToken like @reftoken";
                command.Parameters.AddWithValue("reftoken", rToken);
                var dr = command.ExecuteReader();
                if (!dr.Read())
                {
                    return null;
                }
                var indexNum = dr["IndexNumber"].ToString();
                var lastName = dr["LastName"].ToString();
                dr.Close();

                var claims = new[]
                 {
                    new Claim(ClaimTypes.NameIdentifier, indexNum),
                    new Claim(ClaimTypes.Name, lastName),
                    new Claim(ClaimTypes.Role,  "Employee"),
                    new Claim(ClaimTypes.Role,  "Student"),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken
                (
                    issuer: "Artem",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );

                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                var refreshToken = Guid.NewGuid();
                command.CommandText = "UPDATE student SET RefreshToken = @rToken WHERE IndexNumber = @id";
                command.Parameters.AddWithValue("rToken", refreshToken);
                command.Parameters.AddWithValue("id", indexNum);
                var q = command.ExecuteNonQuery();

                TokenResponse tokenResponse = new TokenResponse();
                tokenResponse.refreshToken = refreshToken;
                tokenResponse.accessToken = accessToken;
                return tokenResponse;
            }
        }
    }
}
