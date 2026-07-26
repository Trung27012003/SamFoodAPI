using System;
using System.Collections.Generic;

namespace SamFoodAPI.Model.Entities;

public partial class InvoiceDetail
{
    public int ID { get; set; }

    public int? InvoiceID { get; set; }

    public int? ProductID { get; set; }

    public int? Quantity { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public decimal? UnitPrice { get; set; }
}
