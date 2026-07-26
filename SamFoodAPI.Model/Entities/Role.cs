using System;
using System.Collections.Generic;

namespace SamFoodAPI.Model.Entities;

public partial class Role
{
    public int ID { get; set; }

    public string? RoleCode { get; set; }

    public string? RoleName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
