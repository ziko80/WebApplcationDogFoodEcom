namespace WebApplcationDogFoodEcom.Server.Models;

public enum ProductCategory
{
    Medicine,
    Vaccine,
    Accessory,
    Toys,
    Feeding,
    Grooming,
    Hygiene,
    Travel
}

public enum PetType
{
    Dog,
    Cat,
    Bird,
    Rabbit
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public PetType PetType { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public string DosageInfo { get; set; } = string.Empty;
    public string TargetCondition { get; set; } = string.Empty;
}
