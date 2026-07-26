using System;
using System.Collections.Generic;

namespace SamFoodAPI.Model.Entities;

public partial class Banner
{
    public int ID { get; set; }
    public string BannerCode { get; set; } = string.Empty;
    public string BannerName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SlideshowInterval { get; set; } = 5;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsDeleted { get; set; } = false;

    public virtual ICollection<BannerDetail> Details { get; set; } = new List<BannerDetail>();
}
