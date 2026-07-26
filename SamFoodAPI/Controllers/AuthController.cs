using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SamFoodAPI.Middleware;
using SamFoodAPI.Model.Common;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        public AuthController(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return Unauthorized(ApiResponseFactory.Fail(null, "Vui lòng nhập Tên đăng nhập và Mật khẩu!"));
                }

                string userName = request.UserName ?? "";
                string password = EncryptionMD5.EncryptPassword(request.Password ?? "");

                var param = new
                {
                    UserName = userName,
                    Password = password
                };

                var users = await SqlDapper<CurrentUser>.ProcedureToListModelAsync("spLogin", param);
                var hasUser = users.FirstOrDefault() ?? new CurrentUser();

                if (hasUser.ID <= 0) return Unauthorized(ApiResponseFactory.Fail(null, "Sai tên đăng nhập hoặc mật khẩu!"));

                var claims = new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, hasUser.ID.ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName, hasUser.UserName ?? "")
                    };

                var dictionary = hasUser.GetType()
                                        .GetProperties()
                                        .ToDictionary(prop => prop.Name, prop => prop.GetValue(hasUser));
                foreach (var item in dictionary)
                {
                    if (item.Key.ToLower() == "passwordhash") continue;
                    var claim = new Claim(item.Key.ToLower(), item.Value?.ToString() ?? "");
                    claims.Add(claim);
                }

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
                    expires = token.ValidTo.AddHours(7)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Tên đăng nhập và mật khẩu không được để trống!"));
                }

                // Encrypt password using the same method as login
                string encryptedPassword = EncryptionMD5.EncryptPassword(request.Password);

                var param = new
                {
                    UserName = request.UserName,
                    Password = encryptedPassword,
                    FullName = request.FullName ?? "",
                    Email = request.Email ?? "",
                    PhoneNumber = request.PhoneNumber ?? ""
                };

                var result = await SqlDapper<RegisterResponse>.ProcedureToModelAsync("spRegister", param);

                if (result.Result == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, result.Message ?? "Đăng ký thất bại!"));
                }

                return Ok(ApiResponseFactory.Success(result, result.Message ?? "Đăng ký thành công!"));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Mật khẩu cũ và mật khẩu mới không được để trống!"));
                }

                if (request.NewPassword.Length < 6)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Mật khẩu mới phải có ít nhất 6 ký tự!"));
                }

                // Get current user ID from claims
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                                ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(ApiResponseFactory.Fail(null, "Không xác định được người dùng!"));
                }

                // Encrypt passwords
                string encryptedOldPassword = EncryptionMD5.EncryptPassword(request.OldPassword);
                string encryptedNewPassword = EncryptionMD5.EncryptPassword(request.NewPassword);

                var param = new
                {
                    UserID = userId,
                    OldPassword = encryptedOldPassword,
                    NewPassword = encryptedNewPassword
                };

                var result = await SqlDapper<ChangePasswordResponse>.ProcedureToModelAsync("spChangePassword", param);

                if (result.Result == 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, result.Message ?? "Đổi mật khẩu thất bại!"));
                }

                return Ok(ApiResponseFactory.Success(null, result.Message ?? "Đổi mật khẩu thành công!"));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }

    public class RegisterRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class RegisterResponse
    {
        public int Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? UserID { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordResponse
    {
        public int Result { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class CurrentUser
    {
        public int ID { get; set; }
        public string? UserName { get; set; }
        public string? PasswordHash { get; set; }
        public string? FullName { get; set; }
        public string? RoleCodes { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
