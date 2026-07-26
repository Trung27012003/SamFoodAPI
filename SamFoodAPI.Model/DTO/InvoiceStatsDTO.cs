namespace SamFoodAPI.Model.DTO;

public class InvoiceStatsDTO
{
    public int NewCount { get; set; }
    public int ShippingCount { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }
    public int ActiveCount { get; set; }
    public int TotalCount { get; set; }
    public decimal MonthRevenue { get; set; }

    // Dapper hứng thẳng cột Trend (nvarchar(max) JSON) vào string
    public string? Trend { get; set; }
}
