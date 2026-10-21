namespace TestOrganizationDemo.Tests;

/// <summary>
/// Централизованные константы для атрибутов Trait.
/// Использование констант исключает опечатки и упрощает переименование категорий.
/// </summary>
public static class Traits
{
    public const string Category = "Category";
    public const string Priority = "Priority";
    public const string Feature = "Feature";
    public const string Type = "Type";
    public const string Stability = "Stability";

    public static class Categories
    {
        public const string Unit = "Unit";
        public const string Integration = "Integration";
        public const string Smoke = "Smoke";
        public const string Regression = "Regression";
        public const string Performance = "Performance";
    }

    public static class Priorities
    {
        public const string Critical = "Critical";
        public const string High = "High";
        public const string Medium = "Medium";
    }

    public static class Features
    {
        public const string StockManagement = "StockManagement";
        public const string Reservations = "Reservations";
        public const string Reporting = "Reporting";
    }

    public static class Types
    {
        public const string Positive = "Positive";
        public const string Negative = "Negative";
        public const string Boundary = "Boundary";
    }

    public static class Stabilities
    {
        public const string Stable = "Stable";
        public const string Fast = "Fast";
        public const string Slow = "Slow";
    }
}
