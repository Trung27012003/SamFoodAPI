using System;
using System.Collections.Generic;

namespace SamFoodAPI.Model.Entities;

public partial class Promotion
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? PromotionCode { get; set; }

    public string? PromotionName { get; set; }

    public string? PromotionContent { get; set; }

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }

    public string? BannerImg { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public int? DiscountType { get; set; }

    public decimal? DiscountValue { get; set; }

    public bool? IsActive { get; set; }

    public decimal? MinOrderAmount { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    public int? UsageLimit { get; set; }

    public int? UsedCount { get; set; }
}
