namespace TestOrganizationDemo;

/// <summary>
/// Товар на складе. Количество на складе и зарезервированное количество
/// изменяются только через <see cref="InventoryService"/>.
/// </summary>
public sealed class Product
{
    public Product(string sku, string name, int quantityOnHand, decimal unitPrice, int reorderLevel)
    {
        Sku = sku;
        Name = name;
        QuantityOnHand = quantityOnHand;
        UnitPrice = unitPrice;
        ReorderLevel = reorderLevel;
    }

    public string Sku { get; }

    public string Name { get; }

    public decimal UnitPrice { get; }

    public int ReorderLevel { get; }

    public int QuantityOnHand { get; internal set; }

    public int ReservedQuantity { get; internal set; }

    public int AvailableQuantity => QuantityOnHand - ReservedQuantity;

    public decimal StockValue => QuantityOnHand * UnitPrice;
}
