using System;
using System.Collections.Generic;

namespace SamFoodAPI.Model.Entities;

public partial class ProductIngredient
{
    public int ID { get; set; }

    public int? ProductID { get; set; }

    public string? IngredientName { get; set; }

    public decimal? Quantity { get; set; }

    public int? UnitCountID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
