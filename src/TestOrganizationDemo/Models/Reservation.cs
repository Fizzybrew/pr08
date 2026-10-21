namespace TestOrganizationDemo;

/// <summary>
/// Резерв товара. Пока резерв активен, товар нельзя удалить,
/// а зарезервированное количество недоступно для новых резервов.
/// </summary>
public sealed record Reservation(
    Guid Id,
    string Sku,
    int Quantity,
    DateTimeOffset CreatedAtUtc);
