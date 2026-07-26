using System;
using System.Collections.Generic;

namespace SamFoodAPI.Model.Entities;

public partial class ProductImage
{
    public int ID { get; set; }

    public int? ProductID { get; set; }

    public string? FileName { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsPrimary { get; set; }
}
