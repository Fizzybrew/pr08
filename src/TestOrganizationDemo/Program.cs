using System.Globalization;
using TestOrganizationDemo;

var inventory = new InventoryService();

inventory.AddProduct("SKU-001", "Keyboard", 10, 49.99m, 3);
inventory.AddProduct("SKU-002", "Mouse", 4, 19.99m, 5);
inventory.AddProduct("SKU-003", "Monitor", 2, 199.00m, 2);

Console.WriteLine("InventorySystem demo");
Console.WriteLine("---------------------");

var reservation = inventory.Reserve("SKU-001", 2);
Console.WriteLine($"Reserved {reservation.Quantity} unit(s) of {reservation.Sku}.");

inventory.IncreaseStock("SKU-002", 3);
Console.WriteLine("Increased SKU-002 stock by 3.");

var reporting = new ReportingService(inventory);
var report = reporting.GenerateStockReport();

Console.WriteLine();
Console.WriteLine($"Products: {report.Items.Count}");
Console.WriteLine($"On hand: {report.TotalUnitsOnHand}");
Console.WriteLine($"Reserved: {report.TotalReserved}");
Console.WriteLine($"Available: {report.TotalAvailable}");
Console.WriteLine(
    $"Stock value: {report.TotalStockValue.ToString("0.00", CultureInfo.InvariantCulture)}");

Console.WriteLine();
Console.WriteLine("Low stock report:");
foreach (var item in reporting.GenerateLowStockReport())
{
    var product = inventory.GetProduct(item.Sku);
    Console.WriteLine(
        $"  {item.Sku} ({item.Name}): available {item.AvailableQuantity}, " +
        $"reorder level {product.ReorderLevel}");
}

inventory.ReleaseReservation(reservation.Id);
Console.WriteLine();
Console.WriteLine(
    $"Released reservation. SKU-001 available: {inventory.GetAvailableQuantity("SKU-001")}");

Console.WriteLine();
Console.WriteLine("CSV report:");
Console.Write(reporting.ExportCsv());
