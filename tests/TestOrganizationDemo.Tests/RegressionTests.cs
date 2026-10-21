using TestOrganizationDemo;

namespace TestOrganizationDemo.Tests;

/// <summary>
/// Регрессионные тесты для сценариев, которые уже приводили к ошибкам
/// или критичны при изменении складской логики.
/// </summary>
[Trait(Traits.Category, Traits.Categories.Regression)]
public class RegressionTests
{
    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.High)]
    [Trait(Traits.Feature, Traits.Features.StockManagement)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Stable)]
    public void Regression_RemoveProductAfterReservationReleased_ProductIsRemoved()
    {
        // Arrange
        var inventory = new InventoryService();
        inventory.AddProduct("REG-001", "Regression product", 5, 10m);
        var reservation = inventory.Reserve("REG-001", 2);

        // Act
        inventory.ReleaseReservation(reservation.Id);
        inventory.RemoveProduct("REG-001");

        // Assert
        Assert.Throws<InvalidOperationException>(() => inventory.GetProduct("REG-001"));
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.High)]
    [Trait(Traits.Feature, Traits.Features.Reservations)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Stable)]
    public void Regression_Reservation_DoesNotChangeQuantityOnHand()
    {
        // Arrange
        var inventory = new InventoryService();
        inventory.AddProduct("REG-002", "Regression product", 10, 10m);

        // Act
        inventory.Reserve("REG-002", 4);

        // Assert
        var product = inventory.GetProduct("REG-002");
        Assert.Equal(10, product.QuantityOnHand);
        Assert.Equal(4, product.ReservedQuantity);
        Assert.Equal(6, product.AvailableQuantity);
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.High)]
    [Trait(Traits.Feature, Traits.Features.Reservations)]
    [Trait(Traits.Type, Traits.Types.Negative)]
    [Trait(Traits.Stability, Traits.Stabilities.Stable)]
    public void Regression_FailedReservation_DoesNotChangeStock()
    {
        // Arrange
        var inventory = new InventoryService();
        inventory.AddProduct("REG-003", "Regression product", 5, 10m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => inventory.Reserve("REG-003", 6));

        var product = inventory.GetProduct("REG-003");
        Assert.Equal(5, product.QuantityOnHand);
        Assert.Equal(0, product.ReservedQuantity);
        Assert.Equal(5, product.AvailableQuantity);
        Assert.Empty(inventory.GetReservations());
    }
}
