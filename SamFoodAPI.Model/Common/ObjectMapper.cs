using SamFoodAPI.Model.DTO;
using SamFoodAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Model.Common
{
    public class ObjectMapper
    {
        public static CurrentUser GetCurrentUser(Dictionary<string, string> claims)
        {
            CurrentUser currentUser = new CurrentUser();
            if (claims == null || (claims.TryGetValue("iscandidate", out var isCandidate) && isCandidate == "true"))
            {
                return currentUser; // Nếu là Token ứng viên thì không map vào CurrentUser nhân viên
            }

            var props = typeof(CurrentUser).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (!prop.CanWrite) continue;

                var value = claims.TryGetValue(prop.Name.ToLower(), out var rawValuea);

                if (claims.TryGetValue(prop.Name.ToLower(), out var rawValue))
                {
                    try
                    {
                        object? parsedValue = prop.PropertyType switch
                        {
                            Type t when t == typeof(string) => rawValue,
                            Type t when t == typeof(int) || t == typeof(int?) => int.TryParse(rawValue, out var i) ? i : 0,
                            Type t when t == typeof(bool) || t == typeof(bool?) => bool.TryParse(rawValue, out var b) ? b : false,
                            Type t when t == typeof(DateTime) || t == typeof(DateTime?) => DateTime.TryParse(rawValue, out var d) ? d : null,
                            _ => null
                        };

                        if (parsedValue != null) prop.SetValue(currentUser, parsedValue);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{prop.Name}\r\n{ex.Message}\r\n{ex.ToString()}");
                    }
                }
            }

            return currentUser;
        }

        public static BannerWithDetailsDto ToBannerWithDetailsDto(Banner banner, IEnumerable<BannerDetail> details)
        {
            if (banner == null) return null!;
            var dto = new BannerWithDetailsDto
            {
                ID = banner.ID,
                BannerCode = banner.BannerCode,
                BannerName = banner.BannerName,
                Description = banner.Description,
                SlideshowInterval = banner.SlideshowInterval,
                IsActive = banner.IsActive,
                CreatedDate = banner.CreatedDate,
                UpdatedDate = banner.UpdatedDate,
                IsDeleted = banner.IsDeleted,
                Details = details?.OrderBy(d => d.SortOrder).ThenBy(d => d.ID).ToList() ?? new List<BannerDetail>()
            };
            return dto;
        }

        public static BannerListDto ToBannerListDto(Banner banner, int countDetails)
        {
            if (banner == null) return null!;
            return new BannerListDto
            {
                ID = banner.ID,
                BannerCode = banner.BannerCode,
                BannerName = banner.BannerName,
                Description = banner.Description,
                SlideshowInterval = banner.SlideshowInterval,
                IsActive = banner.IsActive,
                CreatedDate = banner.CreatedDate,
                UpdatedDate = banner.UpdatedDate,
                IsDeleted = banner.IsDeleted,
                CountDetails = countDetails
            };
        }
    }
}
