IF OBJECT_ID('spGetInvoiceStats', 'P') IS NOT NULL DROP PROC spGetInvoiceStats;
GO

CREATE PROC spGetInvoiceStats
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = CAST(GETDATE() AS DATE);
    DECLARE @MonthStart DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

    -- 14-day trend: orders per day, revenue per day
    SELECT
        CAST(BillDate AS DATE) AS Day,
        COUNT(*) AS OrderCount,
        SUM(CASE WHEN Status = 3 THEN TotalAmount ELSE 0 END) AS Revenue
    INTO #Trend
    FROM Invoice
    WHERE IsDeleted = 0 AND BillDate >= DATEADD(DAY, -13, @Today)
    GROUP BY CAST(BillDate AS DATE)
    ORDER BY Day ASC;

    -- Single row: KPI scalars + trend as JSON string
    SELECT
        SUM(CASE WHEN Status = 0  THEN 1 ELSE 0 END) AS NewCount,
        SUM(CASE WHEN Status = 2  THEN 1 ELSE 0 END) AS ShippingCount,
        SUM(CASE WHEN Status = 3  THEN 1 ELSE 0 END) AS CompletedCount,
        SUM(CASE WHEN Status = 4  THEN 1 ELSE 0 END) AS CancelledCount,
        SUM(CASE WHEN Status NOT IN (3,4) AND IsDeleted = 0 THEN 1 ELSE 0 END) AS ActiveCount,
        SUM(CASE WHEN IsDeleted = 0 THEN 1 ELSE 0 END) AS TotalCount,
        ISNULL(SUM(CASE WHEN Status = 3 AND BillDate >= @MonthStart THEN TotalAmount ELSE 0 END), 0) AS MonthRevenue,
        (SELECT Day, OrderCount, Revenue FROM #Trend FOR JSON PATH) AS Trend
    FROM Invoice
    WHERE IsDeleted = 0;

    DROP TABLE #Trend;
END
GO
