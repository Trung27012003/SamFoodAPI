using System;
using System.Collections.Generic;

namespace SamFoodAPI.Model.Entities;

public partial class Invoice
{
    public int ID { get; set; }

    public string? BillCode { get; set; }

    public DateTime? BillDate { get; set; }

    public int? Status { get; set; }

    public string? CustomerName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public DateTime? DateDelivery { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? DiscountAmount { get; set; }

    public int? PromotionID { get; set; }

    public int? PaymentMethod { get; set; }
}
