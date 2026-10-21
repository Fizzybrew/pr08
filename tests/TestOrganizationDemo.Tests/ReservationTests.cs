using TestOrganizationDemo;

namespace TestOrganizationDemo.Tests;

[Trait(Traits.Category, Traits.Categories.Unit)]
[Trait(Traits.Feature, Traits.Features.Reservations)]
public class ReservationTests
{
    private readonly InventoryService _inventory = new();

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Critical)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void Reserve_AvailableStock_CreatesReservationAndUpdatesAvailability()
    {
        // Arrange
        _inventory.AddProduct("SKU-001", "Keyboard", 10, 49.99m);

        // Act
        var reservation = _inventory.Reserve("SKU-001", 4);

        // Assert
        var product = _inventory.GetProduct("SKU-001");
        Assert.Equal("SKU-001", reservation.Sku);
        Assert.Equal(4, reservation.Quantity);
        Assert.Equal(10, product.QuantityOnHand);
        Assert.Equal(4, product.ReservedQuantity);
        Assert.Equal(6, product.AvailableQuantity);
        Assert.Single(_inventory.GetReservations());
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Critical)]
    [Trait(Traits.Type, Traits.Types.Boundary)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void Reserve_ExactAvailableStock_IsAllowed()
    {
        // Arrange
        _inventory.AddProduct("SKU-001", "Keyboard", 5, 49.99m);

        // Act
        _inventory.Reserve("SKU-001", 5);

        // Assert
        var product = _inventory.GetProduct("SKU-001");
        Assert.Equal(5, product.ReservedQuantity);
        Assert.Equal(0, product.AvailableQuantity);
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.High)]
    [Trait(Traits.Type, Traits.Types.Boundary)]
    [Trait(Traits.Stability, Traits.Stabilities.Stable)]
    public void Reserve_MoreThanAvailable_ThrowsInvalidOperationException()
    {
        // Arrange
        _inventory.AddProduct("SKU-001", "Keyboard", 5, 49.99m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _inventory.Reserve("SKU-001", 6));

        var product = _inventory.GetProduct("SKU-001");
        Assert.Equal(0, product.ReservedQuantity);
        Assert.Equal(5, product.AvailableQuantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [Trait(Traits.Priority, Traits.Priorities.High)]
    [Trait(Traits.Type, Traits.Types.Boundary)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void Reserve_NonPositiveQuantity_ThrowsArgumentException(int quantity)
    {
        // Arrange
        _inventory.AddProduct("SKU-001", "Keyboard", 5, 49.99m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _inventory.Reserve("SKU-001", quantity));
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Medium)]
    [Trait(Traits.Type, Traits.Types.Negative)]
    [Trait(Traits.Stability, Traits.Stabilities.Stable)]
    public void Reserve_UnknownSku_ThrowsInvalidOperationException()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _inventory.Reserve("SKU-UNKNOWN", 1));
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Critical)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void ReleaseReservation_ValidReservation_RestoresAvailability()
    {
        // Arrange
        _inventory.AddProduct("SKU-001", "Keyboard", 10, 49.99m);
        var reservation = _inventory.Reserve("SKU-001", 4);

        // Act
        _inventory.ReleaseReservation(reservation.Id);

        // Assert
        var product = _inventory.GetProduct("SKU-001");
        Assert.Equal(0, product.ReservedQuantity);
        Assert.Equal(10, product.AvailableQuantity);
        Assert.Empty(_inventory.GetReservations());
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Medium)]
    [Trait(Traits.Type, Traits.Types.Negative)]
    [Trait(Traits.Stability, Traits.Stabilities.Stable)]
    public void ReleaseReservation_UnknownId_ThrowsInvalidOperationException()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            _inventory.ReleaseReservation(Guid.NewGuid()));
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.High)]
    [Trait(Traits.Type, Traits.Types.Negative)]
    [Trait(Traits.Stability, Traits.Stabilities.Stable)]
    public void RemoveProduct_WithActiveReservation_ThrowsInvalidOperationException()
    {
        // Arrange
        _inventory.AddProduct("SKU-001", "Keyboard", 10, 49.99m);
        _inventory.Reserve("SKU-001", 4);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _inventory.RemoveProduct("SKU-001"));

        var product = _inventory.GetProduct("SKU-001");
        Assert.Equal(4, product.ReservedQuantity);
        Assert.Equal(6, product.AvailableQuantity);
    }
}
