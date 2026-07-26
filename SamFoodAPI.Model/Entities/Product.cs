using System;
using System.Collections.Generic;

namespace SamFoodAPI.Model.Entities;

public partial class Product
{
    public int ID { get; set; }

    public int? CategoryID { get; set; }

    public int? STT { get; set; }

    public string? ProductCode { get; set; }

    public string? ProductName { get; set; }

    /// <summary>
    /// 1:Còn hành; 2: hết hàng; 3: Hàng mới
    /// </summary>
    public int? Status { get; set; }

    public decimal? UnitPrice { get; set; }

    public string? Origin { get; set; }

    public string? Descriptions { get; set; }

    public decimal? Weight { get; set; }

    public int? UnitCountID { get; set; }

    public string? StorageInstructions { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
