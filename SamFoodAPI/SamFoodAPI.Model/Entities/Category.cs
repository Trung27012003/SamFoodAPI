using System;
using System.Collections.Generic;

namespace SamFoodAPI.Model.Entities;

public partial class Category
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? CategoryCode { get; set; }

    public string? CategoryName { get; set; }

    public int? ParentID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
