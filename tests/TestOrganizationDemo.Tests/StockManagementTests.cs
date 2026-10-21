using TestOrganizationDemo;

namespace TestOrganizationDemo.Tests;

[Trait(Traits.Category, Traits.Categories.Unit)]
[Trait(Traits.Feature, Traits.Features.StockManagement)]
public class StockManagementTests
{
    private readonly InventoryService _inventory = new();

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Critical)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void AddProduct_ValidData_AddsProductToStock()
    {
        // Arrange & Act
        _inventory.AddProduct("SKU-001", "Keyboard", 10, 49.99m, 3);

        // Assert
        var product = _inventory.GetProduct("SKU-001");

        Assert.Equal("Keyboard", product.Name);
        Assert.Equal(10, product.QuantityOnHand);
        Assert.Equal(0, product.ReservedQuantity);
        Assert.Equal(10, product.AvailableQuantity);
        Assert.Equal(49.99m, product.UnitPrice);
        Assert.Equal(3, product.ReorderLevel);
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.High)]
    [Trait(Traits.Type, Traits.Types.Negative)]
    [Trait(Traits.Stability, Traits.Stabilities.Stable)]
    public void AddProduct_DuplicateSku_ThrowsInvalidOperationException()
    {
        // Arrange
        _inventory.AddProduct("SKU-001", "Keyboard", 10, 49.99m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            _inventory.AddProduct("SKU-001", "Another keyboard", 5, 39.99m));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [Trait(Traits.Priority, Traits.Priorities.High)]
    [Trait(Traits.Type, Traits.Types.Negative)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void AddProduct_InvalidSku_ThrowsArgumentException(string sku)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _inventory.AddProduct(sku, "Keyboard", 10, 49.99m));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [Trait(Traits.Priority, Traits.Priorities.High)]
    [Trait(Traits.Type, Traits.Types.Boundary)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void AddProduct_NegativeQuantity_ThrowsArgumentException(int quantity)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _inventory.AddProduct("SKU-001", "Keyboard", quantity, 49.99m));
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Medium)]
    [Trait(Traits.Type, Traits.Types.Boundary)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void AddProduct_ZeroQuantity_IsAllowed()
    {
        // Act
        _inventory.AddProduct("SKU-001", "Keyboard", 0, 49.99m);

        // Assert
        var product = _inventory.GetProduct("SKU-001");
        Assert.Equal(0, product.QuantityOnHand);
        Assert.Equal(0, product.AvailableQuantity);
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.High)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void IncreaseStock_ValidQuantity_IncreasesOnHandQuantity()
    {
        // Arrange
        _inventory.AddProduct("SKU-001", "Keyboard", 5, 49.99m);

        // Act
        _inventory.IncreaseStock("SKU-001", 7);

        // Assert
        var product = _inventory.GetProduct("SKU-001");
        Assert.Equal(12, product.QuantityOnHand);
        Assert.Equal(12, product.AvailableQuantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [Trait(Traits.Priority, Traits.Priorities.Medium)]
    [Trait(Traits.Type, Traits.Types.Boundary)]
    [Trait(Traits.Stability, Traits.Stabilities.Fast)]
    public void IncreaseStock_NonPositiveQuantity_ThrowsArgumentException(int quantity)
    {
        // Arrange
        _inventory.AddProduct("SKU-001", "Keyboard", 5, 49.99m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _inventory.IncreaseStock("SKU-001", quantity));
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.High)]
    [Trait(Traits.Type, Traits.Types.Positive)]
    [Trait(Traits.Stability, Traits.Stabilities.Stable)]
    public void RemoveProduct_ExistingSku_RemovesProduct()
    {
        // Arrange
        _inventory.AddProduct("SKU-001", "Keyboard", 10, 49.99m);

        // Act
        _inventory.RemoveProduct("SKU-001");

        // Assert
        Assert.Throws<InvalidOperationException>(() => _inventory.GetProduct("SKU-001"));
    }

    [Fact]
    [Trait(Traits.Priority, Traits.Priorities.Medium)]
    [Trait(Traits.Type, Traits.Types.Negative)]
    [Trait(Traits.Stability, Traits.Stabilities.Stable)]
    public void RemoveProduct_UnknownSku_ThrowsInvalidOperationException()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _inventory.RemoveProduct("SKU-UNKNOWN"));
    }
}
