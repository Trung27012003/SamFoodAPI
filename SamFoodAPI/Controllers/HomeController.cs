using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SamFoodAPI.Middleware;
using SamFoodAPI.Model.Common;
using SamFoodAPI.Model.DTO;
using SamFoodAPI.Model.Entities;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        public HomeController(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    return Unauthorized(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên đăng nhập và Mật khẩu!"));
                }

                //1. Check user
                string userName = user.UserName ?? "";
                string password = EncryptionMD5.EncryptPassword(user.PasswordHash ?? "");
                //password = user.PasswordHash;

                var param = new
                {
                    UserName = userName,
                    Password = password
                };

                var users = await SqlDapper<CurrentUser>.ProcedureToListModelAsync("spLogin", param);
                var hasUser = users.FirstOrDefault() ?? new CurrentUser();

                if (hasUser.ID <= 0) return Unauthorized(ApiResponseFactory.Fail(null, "Sai tên đăng nhập hoặc mật khẩu!"));


                //var hasUser = SQLHelper<object>.GetListData(login, 0)[0];

                //2. Tạo Claims
                var claims = new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.Sub,hasUser.ID.ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName,hasUser.UserName ?? "")
                    };

                //var dictionary = (IDictionary<string, object>)hasUser;
                var dictionary = hasUser.GetType()
                                        .GetProperties()
                                        .ToDictionary(prop => prop.Name, prop => prop.GetValue(hasUser));
                foreach (var item in dictionary)
                {
                    if (item.Key.ToLower() == "passwordhash") continue;
                    var claim = new Claim(item.Key.ToLower(), item.Value?.ToString() ?? "");
                    claims.Add(claim);
                }


                //3. Tạo token
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims.ToArray(),
                    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);


                return Ok(new
                {
                    access_token = tokenString,
                    expires = token.ValidTo.AddHours(+7)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message + "\n"));
            }
        }

    }
}
