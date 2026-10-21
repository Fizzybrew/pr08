using TestOrganizationDemo;

namespace TestOrganizationDemo.Tests;

[Trait(Traits.Category, Traits.Categories.Integration)]
[Trait(Traits.Feature, Traits.Features.Reporting)]
public class ReportingTests
{
    private readonly InventoryService _inventory = new();
    private readonly ReportingService _reporting;

    public ReportingTests()
    {
        _reporting = new ReportingService(_inventory);
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.High)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void GenerateStockReport_MultipleProducts_ReturnsAggregatedTotals()
    {
        // Arrange
        _inventory.AddProduct("SKU-A", "Keyboard", 10, 10.50m, 2);
        _inventory.AddProduct("SKU-B", "Mouse", 3, 20.00m, 1);
        _inventory.Reserve("SKU-A", 2);

        // Act
        var report = _reporting.GenerateStockReport();

        // Assert
        Assert.Equal(2, report.Items.Count);
        Assert.Equal(13, report.TotalUnitsOnHand);
        Assert.Equal(2, report.TotalReserved);
        Assert.Equal(11, report.TotalAvailable);
        Assert.Equal(165.00m, report.TotalStockValue);

        var keyboard = report.Items.Single(item => item.Sku == "SKU-A");
        Assert.Equal(10, keyboard.QuantityOnHand);
        Assert.Equal(2, keyboard.ReservedQuantity);
        Assert.Equal(8, keyboard.AvailableQuantity);
        Assert.Equal(105.00m, keyboard.StockValue);
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Medium)]
    [Trait(Traits.Type, Traits.Types.Boundary)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void GenerateStockReport_EmptyInventory_ReturnsEmptyReport()
    {
        // Act
        var report = _reporting.GenerateStockReport();

        // Assert
        Assert.Empty(report.Items);
        Assert.Equal(0, report.TotalUnitsOnHand);
        Assert.Equal(0, report.TotalReserved);
        Assert.Equal(0, report.TotalAvailable);
        Assert.Equal(0m, report.TotalStockValue);
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.High)]
    [Trait(Traits.Type, Traits.Types.Boundary)]
    [Trait(Traits.Stability, Traits.Stabilities.Stable)]
    public void GenerateLowStockReport_IncludesProductAtReorderLevel()
    {
        // Arrange
        _inventory.AddProduct("SKU-LOW", "Low stock", 5, 1m, 5);
        _inventory.AddProduct("SKU-OK", "Enough stock", 6, 1m, 5);

        // Act
        var lowStock = _reporting.GenerateLowStockReport();

        // Assert
        var item = Assert.Single(lowStock);
        Assert.Equal("SKU-LOW", item.Sku);
        Assert.Equal(5, item.AvailableQuantity);
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.High)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Stable)]
    public void GenerateLowStockReport_ReservationMakesProductLowStock()
    {
        // Arrange
        _inventory.AddProduct("SKU-R", "Reserved product", 10, 2m, 5);
        _inventory.Reserve("SKU-R", 6);

        // Act
        var lowStock = _reporting.GenerateLowStockReport();

        // Assert
        Assert.Contains(lowStock, item => item.Sku == "SKU-R");
        Assert.Equal(4, lowStock.Single(item => item.Sku == "SKU-R").AvailableQuantity);
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Medium)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void ExportCsv_ReturnsHeaderAndRows()
    {
        // Arrange
        _inventory.AddProduct("SKU-A", "Keyboard", 2, 10.50m, 1);
        _inventory.AddProduct("SKU-B", "Mouse, Wireless", 1, 20.00m, 1);

        // Act
        var csv = _reporting.ExportCsv();
        var lines = csv.Split(
            new[] { "\r\n", "\n" },
            StringSplitOptions.RemoveEmptyEntries);

        // Assert
        Assert.Equal(3, lines.Length);
        Assert.Equal(
            "Sku,Name,QuantityOnHand,ReservedQuantity,AvailableQuantity,UnitPrice,StockValue",
            lines[0]);
        Assert.Contains("SKU-A,Keyboard,2,0,2,10.50,21.00", lines);
        Assert.Contains("SKU-B,\"Mouse, Wireless\",1,0,1,20.00,20.00", lines);
    }
}
