using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Models.Entities;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginRequest request)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            string userGuid = "";
            string roleName = "";

            using (SqlConnection con =
                new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"SELECT * FROM User_Master WHERE UserName=@UserName AND Password=@Password AND Status=1",con);

                cmd.Parameters.AddWithValue("@UserName", request.UserName);
                cmd.Parameters.AddWithValue("@Password", request.Password);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())
                {
                    return Unauthorized("Invalid Login");
                }

                userGuid = dr["UserGUID"].ToString();

                roleName = dr["RoleName"].ToString();
            }

            //------------------------------------------------
            // CREATE CLAIMS
            //------------------------------------------------

            var claims = new[]
            {
            new Claim("UserGUID", userGuid),

            new Claim("UserName", request.UserName),

            new Claim("RoleName", roleName)
        };

            //------------------------------------------------
            // CREATE TOKEN
            //------------------------------------------------

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpiryMinutes"]));

            var token = new JwtSecurityToken(issuer: _configuration["Jwt:Issuer"],

                    audience: _configuration["Jwt:Audience"],

                    claims: claims,

                    expires: expiry,

                    signingCredentials: creds
                );

            LoginResponse response = new LoginResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),

                    Expiration = expiry,

                    UserName = request.UserName,

                    RoleName = roleName
                };

            return Ok(response);
        }
    }
}
