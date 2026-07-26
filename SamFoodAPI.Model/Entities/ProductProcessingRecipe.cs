using System;
using System.Collections.Generic;

namespace SamFoodAPI.Model.Entities;

public partial class ProductProcessingRecipe
{
    public int ID { get; set; }

    public int? ProductID { get; set; }

    public int? Step { get; set; }

    public string? StepName { get; set; }

    public string? Description { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
