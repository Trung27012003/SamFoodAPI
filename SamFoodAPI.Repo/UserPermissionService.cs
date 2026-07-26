using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SamFoodAPI.IRepo;
using SamFoodAPI.Model.Context;
using SamFoodAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Repo
{
    public class UserPermissionService: IUserPermissionService
    {
        protected SamFoodContext _dbContext { get; set; }
        public Dictionary<string, string> Claims { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserPermissionService(SamFoodContext db, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> HasPermissionAsync(string userId, string permission)
        {
            if (!int.TryParse(userId, out var id)) return false;

            var permissions = permission.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries); //NTA B update 051125

            var role = (from r in _dbContext.Roles
                        join ru in _dbContext.RoleUsers on r.ID equals ru.ID into ru2
                        from ru in ru2.DefaultIfEmpty()
                        select new
                        {
                            RoleUser = ru,
                            RoleCode = r.RoleCode
                        }).ToList();
                        
            foreach (var perm in permissions) //NTA B update 051125
            {
                var hasPerm = await _dbContext.RoleUsers.AnyAsync(p => p.UserID == id);
                if (hasPerm) return true;
            }
            return false;
        }


        public Dictionary<string, string> GetClaims()
        {
            var claims = _httpContextAccessor.HttpContext?.User?.Claims.ToDictionary(x => x.Type, x => x.Value);
            return claims;
        }
    }
}
