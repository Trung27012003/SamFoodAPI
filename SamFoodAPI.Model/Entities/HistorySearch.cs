using System;
using System.Collections.Generic;

namespace SamFoodAPI.Model.Entities;

public partial class HistorySearch
{
    public int ID { get; set; }

    public string? Keyword { get; set; }

    public string? Hastag { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
