namespace TestOrganizationDemo;

public sealed record StockReportItem(
    string Sku,
    string Name,
    int QuantityOnHand,
    int ReservedQuantity,
    int AvailableQuantity,
    decimal UnitPrice,
    decimal StockValue);

public sealed record StockReport(
    IReadOnlyList<StockReportItem> Items,
    int TotalUnitsOnHand,
    int TotalReserved,
    int TotalAvailable,
    decimal TotalStockValue);
