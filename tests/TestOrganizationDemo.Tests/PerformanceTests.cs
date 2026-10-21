using TestOrganizationDemo;

namespace TestOrganizationDemo.Tests;

/// <summary>
/// Нагрузочные проверки выполняются без измерения времени,
/// чтобы результат не зависел от скорости конкретной машины.
/// </summary>
[Trait(Traits.Category, Traits.Categories.Performance)]
public class PerformanceTests
{
    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Medium)]
    [Trait(Traits.Feature, Traits.Features.StockManagement)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Slow)]
    public void BulkAdd_FiveThousandProducts_AllAreAvailable()
    {
        // Arrange
        var inventory = new InventoryService();

        // Act
        for (var i = 0; i < 5_000; i++)
            inventory.AddProduct($"SKU-{i:D5}", $"Product {i}", 10, 1m, 1);

        // Assert
        var products = inventory.GetAllProducts();
        Assert.Equal(5_000, products.Count);
        Assert.All(products, product => Assert.Equal(10, product.AvailableQuantity));
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Medium)]
    [Trait(Traits.Feature, Traits.Features.Reservations)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Slow)]
    public void BulkReserveAndRelease_OneThousandOperations_KeepStockConsistent()
    {
        // Arrange
        var inventory = new InventoryService();
        inventory.AddProduct("SKU-BULK", "Bulk product", 1_000, 1m);

        // Act
        var reservations = Enumerable.Range(0, 1_000)
            .Select(_ => inventory.Reserve("SKU-BULK", 1))
            .ToList();

        foreach (var reservation in reservations)
            inventory.ReleaseReservation(reservation.Id);

        // Assert
        var product = inventory.GetProduct("SKU-BULK");
        Assert.Equal(1_000, product.QuantityOnHand);
        Assert.Equal(0, product.ReservedQuantity);
        Assert.Equal(1_000, product.AvailableQuantity);
        Assert.Empty(inventory.GetReservations());
    }
}
