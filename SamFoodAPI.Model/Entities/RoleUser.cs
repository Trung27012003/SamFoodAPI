using System;
using System.Collections.Generic;

namespace SamFoodAPI.Model.Entities;

public partial class RoleUser
{
    public int ID { get; set; }

    public int? RoleID { get; set; }

    public int? UserID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
