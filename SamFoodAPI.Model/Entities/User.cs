using System;
using System.Collections.Generic;

namespace SamFoodAPI.Model.Entities;

public partial class User
{
    public int ID { get; set; }

    public string? UserName { get; set; }

    public string? PasswordHash { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public bool? IsActive { get; set; }
}
