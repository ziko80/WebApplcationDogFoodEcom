using WebApplcationDogFoodEcom.Server.Models;

namespace WebApplcationDogFoodEcom.Server.Data;

public static class ProductStore
{
    public static List<Product> Products { get; } =
    [
        // Medicines
        new() { Id = 1, Name = "Heartgard Plus", Description = "Monthly chewable for heartworm prevention in pets", Price = 45.99m, Category = ProductCategory.Medicine, Brand = "Boehringer Ingelheim", StockQuantity = 120, DosageInfo = "1 chewable per month", TargetCondition = "Heartworm Prevention" },
        new() { Id = 2, Name = "NexGard", Description = "Flea and tick treatment chewable tablet for pets", Price = 38.50m, Category = ProductCategory.Medicine, Brand = "Boehringer Ingelheim", StockQuantity = 95, DosageInfo = "1 tablet monthly", TargetCondition = "Flea & Tick" },
        new() { Id = 3, Name = "Apoquel", Description = "Itch relief for allergic dermatitis in pets", Price = 72.00m, Category = ProductCategory.Medicine, Brand = "Zoetis", StockQuantity = 60, DosageInfo = "Twice daily for 14 days, then once daily", TargetCondition = "Allergies & Itching" },
        new() { Id = 4, Name = "Rimadyl", Description = "Pain relief and anti-inflammatory for pets", Price = 35.00m, Category = ProductCategory.Medicine, Brand = "Zoetis", StockQuantity = 80, DosageInfo = "2mg per pound body weight daily", TargetCondition = "Pain & Inflammation" },
        new() { Id = 5, Name = "Metronidazole", Description = "Antibiotic for gastrointestinal infections in pets", Price = 18.99m, Category = ProductCategory.Medicine, Brand = "Generic", StockQuantity = 150, DosageInfo = "As prescribed by vet", TargetCondition = "GI Infections" },
        new() { Id = 6, Name = "Simparica Trio", Description = "All-in-one parasite protection for pets", Price = 55.00m, Category = ProductCategory.Medicine, Brand = "Zoetis", StockQuantity = 70, DosageInfo = "1 chewable monthly", TargetCondition = "Heartworm, Flea, Tick & Worms" },

        // Vaccines
        new() { Id = 7, Name = "DHPP Vaccine", Description = "Core vaccine: Distemper, Hepatitis, Parainfluenza, Parvovirus", Price = 25.00m, Category = ProductCategory.Vaccine, Brand = "Nobivac", StockQuantity = 200, DosageInfo = "3 doses at 6-8, 10-12, 14-16 weeks", TargetCondition = "Distemper, Hepatitis, Parvo" },
        new() { Id = 8, Name = "Rabies Vaccine", Description = "Required core vaccine for rabies prevention in all pets", Price = 20.00m, Category = ProductCategory.Vaccine, Brand = "Imrab", StockQuantity = 250, DosageInfo = "Single dose at 12-16 weeks, booster at 1 year", TargetCondition = "Rabies" },
        new() { Id = 9, Name = "Bordetella Vaccine", Description = "Kennel cough prevention nasal spray or injection", Price = 22.00m, Category = ProductCategory.Vaccine, Brand = "Nobivac", StockQuantity = 180, DosageInfo = "Annual or semi-annual", TargetCondition = "Kennel Cough" },
        new() { Id = 10, Name = "Leptospirosis Vaccine", Description = "Protection against leptospira bacteria for pets", Price = 28.00m, Category = ProductCategory.Vaccine, Brand = "Nobivac", StockQuantity = 140, DosageInfo = "2 doses 2-4 weeks apart, annual booster", TargetCondition = "Leptospirosis" },
        new() { Id = 11, Name = "FVRCP Vaccine", Description = "Core feline vaccine for rhinotracheitis, calicivirus, and panleukopenia", Price = 30.00m, Category = ProductCategory.Vaccine, Brand = "Nobivac", StockQuantity = 100, DosageInfo = "3 doses at 6-8, 10-12, 14-16 weeks", TargetCondition = "Feline Respiratory & Panleukopenia" },
        new() { Id = 12, Name = "Lyme Disease Vaccine", Description = "Prevention of Lyme disease from tick bites in pets", Price = 32.00m, Category = ProductCategory.Vaccine, Brand = "Nobivac", StockQuantity = 90, DosageInfo = "2 doses 2-4 weeks apart, annual booster", TargetCondition = "Lyme Disease" },

        // Accessories
        new() { Id = 13, Name = "Adjustable Pet Collar", Description = "Durable nylon collar with quick-release buckle for pets of all sizes", Price = 12.99m, Category = ProductCategory.Accessory, Brand = "PetSafe", StockQuantity = 300, DosageInfo = "N/A", TargetCondition = "Everyday Wear" },
        new() { Id = 14, Name = "Retractable Leash", Description = "16 ft retractable leash with ergonomic grip for comfortable walks", Price = 24.99m, Category = ProductCategory.Accessory, Brand = "Flexi", StockQuantity = 200, DosageInfo = "N/A", TargetCondition = "Walking & Exercise" },
        new() { Id = 15, Name = "Stainless Steel Pet Bowl", Description = "Non-slip stainless steel food and water bowl for any pet", Price = 9.99m, Category = ProductCategory.Accessory, Brand = "PetSafe", StockQuantity = 250, DosageInfo = "N/A", TargetCondition = "Feeding" },
        new() { Id = 16, Name = "Pet Grooming Brush", Description = "Self-cleaning slicker brush for removing loose fur and tangles", Price = 14.50m, Category = ProductCategory.Accessory, Brand = "FURminator", StockQuantity = 180, DosageInfo = "N/A", TargetCondition = "Grooming" },
        new() { Id = 17, Name = "Pet Carrier Bag", Description = "Airline-approved soft-sided carrier for small pets", Price = 39.99m, Category = ProductCategory.Accessory, Brand = "Sherpa", StockQuantity = 75, DosageInfo = "N/A", TargetCondition = "Travel" },
        new() { Id = 18, Name = "Interactive Toy Ball", Description = "Treat-dispensing puzzle ball to keep pets mentally stimulated", Price = 11.99m, Category = ProductCategory.Accessory, Brand = "KONG", StockQuantity = 220, DosageInfo = "N/A", TargetCondition = "Play & Enrichment" },
    ];
}
