namespace TestOrganizationDemo;

/// <summary>
/// Простой складской сервис: товары, остатки и резервы.
/// Состояние хранится в памяти и не разделяется между экземплярами.
/// </summary>
public sealed class InventoryService
{
    private readonly Dictionary<string, Product> _products =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<Guid, Reservation> _reservations = new();

    public void AddProduct(
        string sku,
        string name,
        int quantity,
        decimal unitPrice,
        int reorderLevel = 5)
    {
        var normalizedSku = NormalizeRequired(sku, nameof(sku), "SKU");
        var normalizedName = NormalizeRequired(name, nameof(name), "Product name");

        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));

        if (reorderLevel < 0)
            throw new ArgumentException("Reorder level cannot be negative.", nameof(reorderLevel));

        if (_products.ContainsKey(normalizedSku))
            throw new InvalidOperationException(
                $"Product with SKU '{normalizedSku}' already exists.");

        _products.Add(
            normalizedSku,
            new Product(normalizedSku, normalizedName, quantity, unitPrice, reorderLevel));
    }

    public Product GetProduct(string sku)
    {
        var normalizedSku = NormalizeRequired(sku, nameof(sku), "SKU");

        if (!_products.TryGetValue(normalizedSku, out var product))
            throw new InvalidOperationException(
                $"Product with SKU '{normalizedSku}' was not found.");

        return product;
    }

    public IReadOnlyList<Product> GetAllProducts() =>
        _products.Values
            .OrderBy(product => product.Sku, StringComparer.OrdinalIgnoreCase)
            .ToList();

    public int GetAvailableQuantity(string sku) => GetProduct(sku).AvailableQuantity;

    public void IncreaseStock(string sku, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        GetProduct(sku).QuantityOnHand += quantity;
    }

    public void RemoveProduct(string sku)
    {
        var product = GetProduct(sku);

        if (product.ReservedQuantity > 0)
            throw new InvalidOperationException(
                $"Cannot remove product '{product.Sku}' while it has active reservations.");

        _products.Remove(product.Sku);
    }

    public Reservation Reserve(string sku, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Reservation quantity must be positive.", nameof(quantity));

        var product = GetProduct(sku);

        if (quantity > product.AvailableQuantity)
            throw new InvalidOperationException(
                $"Insufficient stock for product '{product.Sku}'. " +
                $"Available: {product.AvailableQuantity}, requested: {quantity}.");

        product.ReservedQuantity += quantity;

        var reservation = new Reservation(
            Guid.NewGuid(),
            product.Sku,
            quantity,
            DateTimeOffset.UtcNow);

        _reservations.Add(reservation.Id, reservation);
        return reservation;
    }

    public void ReleaseReservation(Guid reservationId)
    {
        if (!_reservations.TryGetValue(reservationId, out var reservation))
            throw new InvalidOperationException(
                $"Reservation '{reservationId}' was not found.");

        var product = GetProduct(reservation.Sku);
        product.ReservedQuantity -= reservation.Quantity;
        _reservations.Remove(reservationId);
    }

    public IReadOnlyList<Reservation> GetReservations() => _reservations.Values.ToList();

    private static string NormalizeRequired(string? value, string parameterName, string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{displayName} cannot be empty.", parameterName);

        return value.Trim();
    }
}
