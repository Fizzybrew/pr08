using System.Globalization;
using System.Text;

namespace TestOrganizationDemo;

/// <summary>
/// Формирует отчёты по текущему состоянию склада.
/// </summary>
public sealed class ReportingService
{
    private readonly InventoryService _inventory;

    public ReportingService(InventoryService inventory)
    {
        _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
    }

    public StockReport GenerateStockReport()
    {
        var items = _inventory.GetAllProducts()
            .Select(ToReportItem)
            .ToList();

        return new StockReport(
            items,
            items.Sum(item => item.QuantityOnHand),
            items.Sum(item => item.ReservedQuantity),
            items.Sum(item => item.AvailableQuantity),
            items.Sum(item => item.StockValue));
    }

    public IReadOnlyList<StockReportItem> GenerateLowStockReport() =>
        _inventory.GetAllProducts()
            .Where(product => product.AvailableQuantity <= product.ReorderLevel)
            .Select(ToReportItem)
            .ToList();

    public string ExportCsv()
    {
        var report = GenerateStockReport();
        var csv = new StringBuilder();

        csv.AppendLine("Sku,Name,QuantityOnHand,ReservedQuantity,AvailableQuantity,UnitPrice,StockValue");

        foreach (var item in report.Items)
        {
            csv.AppendLine(string.Join(
                ',',
                EscapeCsv(item.Sku),
                EscapeCsv(item.Name),
                item.QuantityOnHand.ToString(CultureInfo.InvariantCulture),
                item.ReservedQuantity.ToString(CultureInfo.InvariantCulture),
                item.AvailableQuantity.ToString(CultureInfo.InvariantCulture),
                item.UnitPrice.ToString("0.00", CultureInfo.InvariantCulture),
                item.StockValue.ToString("0.00", CultureInfo.InvariantCulture)));
        }

        return csv.ToString();
    }

    private static StockReportItem ToReportItem(Product product) =>
        new(
            product.Sku,
            product.Name,
            product.QuantityOnHand,
            product.ReservedQuantity,
            product.AvailableQuantity,
            product.UnitPrice,
            product.StockValue);

    private static string EscapeCsv(string value) =>
        value.Contains(',') || value.Contains('"') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
}
