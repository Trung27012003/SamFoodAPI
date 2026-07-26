using System;
using System.Collections.Generic;

namespace SamFoodAPI.Model.Entities;

public partial class UnitCount
{
    public int ID { get; set; }

    public string? UnitCode { get; set; }

    public string? UnitName { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Descriptions { get; set; }
}
