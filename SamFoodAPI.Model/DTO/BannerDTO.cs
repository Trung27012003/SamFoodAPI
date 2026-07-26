using SamFoodAPI.Model.Entities;
using System.Collections.Generic;

namespace SamFoodAPI.Model.DTO
{
    public class BannerWithDetailsDto : Banner
    {
        public List<BannerDetail> Details { get; set; } = new List<BannerDetail>();
    }

    public class BannerListDto : Banner
    {
        public int CountDetails { get; set; } = 0;
    }
}
