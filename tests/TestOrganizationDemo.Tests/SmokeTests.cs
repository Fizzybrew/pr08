using TestOrganizationDemo;

namespace TestOrganizationDemo.Tests;

/// <summary>
/// Быстрые проверки критических сценариев, которые должны проходить на каждом билде.
/// </summary>
[Trait(Traits.Category, Traits.Categories.Smoke)]
public class SmokeTests
{
    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Critical)]
    [Trait(Traits.Feature, Traits.Features.StockManagement)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void Smoke_AddProduct_ProductIsAvailable()
    {
        // Arrange
        var inventory = new InventoryService();

        // Act
        inventory.AddProduct("SMOKE-001", "Smoke product", 3, 10m);

        // Assert
        Assert.Equal(3, inventory.GetAvailableQuantity("SMOKE-001"));
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Critical)]
    [Trait(Traits.Feature, Traits.Features.Reservations)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void Smoke_ReserveAndRelease_StockIsRestored()
    {
        // Arrange
        var inventory = new InventoryService();
        inventory.AddProduct("SMOKE-002", "Smoke product", 10, 10m);

        // Act
        var reservation = inventory.Reserve("SMOKE-002", 3);
        inventory.ReleaseReservation(reservation.Id);

        // Assert
        Assert.Equal(10, inventory.GetAvailableQuantity("SMOKE-002"));
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Critical)]
    [Trait(Traits.Feature, Traits.Features.Reporting)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void Smoke_StockReport_IsGenerated()
    {
        // Arrange
        var inventory = new InventoryService();
        inventory.AddProduct("SMOKE-003", "Smoke product", 4, 10m);
        var reporting = new ReportingService(inventory);

        // Act
        var report = reporting.GenerateStockReport();

        // Assert
        var item = Assert.Single(report.Items);
        Assert.Equal("SMOKE-003", item.Sku);
        Assert.Equal(4, report.TotalUnitsOnHand);
    }
}
