using WebApplcationDogFoodEcom.Server.Models;

namespace WebApplcationDogFoodEcom.Server.Data;

public static class ProductStore
{
    public static List<Product> Products { get; } =
    [
        // Medicines
        new() { Id = 1, Name = "Heartgard Plus", Description = "Monthly chewable for heartworm prevention in pets", Price = 45.99m, Category = ProductCategory.Medicine, PetType = PetType.Dog, Brand = "Boehringer Ingelheim", StockQuantity = 120, DosageInfo = "1 chewable per month", TargetCondition = "Heartworm Prevention" },
        new() { Id = 2, Name = "NexGard", Description = "Flea and tick treatment chewable tablet for pets", Price = 38.50m, Category = ProductCategory.Medicine, PetType = PetType.Dog, Brand = "Boehringer Ingelheim", StockQuantity = 95, DosageInfo = "1 tablet monthly", TargetCondition = "Flea & Tick" },
        new() { Id = 3, Name = "Apoquel", Description = "Itch relief for allergic dermatitis in pets", Price = 72.00m, Category = ProductCategory.Medicine, PetType = PetType.Dog, Brand = "Zoetis", StockQuantity = 60, DosageInfo = "Twice daily for 14 days, then once daily", TargetCondition = "Allergies & Itching" },
        new() { Id = 4, Name = "Rimadyl", Description = "Pain relief and anti-inflammatory for pets", Price = 35.00m, Category = ProductCategory.Medicine, PetType = PetType.Dog, Brand = "Zoetis", StockQuantity = 80, DosageInfo = "2mg per pound body weight daily", TargetCondition = "Pain & Inflammation" },
        new() { Id = 5, Name = "Metronidazole", Description = "Antibiotic for gastrointestinal infections in pets", Price = 18.99m, Category = ProductCategory.Medicine, PetType = PetType.Cat, Brand = "Generic", StockQuantity = 150, DosageInfo = "As prescribed by vet", TargetCondition = "GI Infections" },
        new() { Id = 6, Name = "Simparica Trio", Description = "All-in-one parasite protection for pets", Price = 55.00m, Category = ProductCategory.Medicine, PetType = PetType.Dog, Brand = "Zoetis", StockQuantity = 70, DosageInfo = "1 chewable monthly", TargetCondition = "Heartworm, Flea, Tick & Worms" },

        // Vaccines
        new() { Id = 7, Name = "DHPP Vaccine", Description = "Core vaccine: Distemper, Hepatitis, Parainfluenza, Parvovirus", Price = 25.00m, Category = ProductCategory.Vaccine, PetType = PetType.Dog, Brand = "Nobivac", StockQuantity = 200, DosageInfo = "3 doses at 6-8, 10-12, 14-16 weeks", TargetCondition = "Distemper, Hepatitis, Parvo" },
        new() { Id = 8, Name = "Rabies Vaccine", Description = "Required core vaccine for rabies prevention in all pets", Price = 20.00m, Category = ProductCategory.Vaccine, PetType = PetType.Dog, Brand = "Imrab", StockQuantity = 250, DosageInfo = "Single dose at 12-16 weeks, booster at 1 year", TargetCondition = "Rabies" },
        new() { Id = 9, Name = "Bordetella Vaccine", Description = "Kennel cough prevention nasal spray or injection", Price = 22.00m, Category = ProductCategory.Vaccine, PetType = PetType.Dog, Brand = "Nobivac", StockQuantity = 180, DosageInfo = "Annual or semi-annual", TargetCondition = "Kennel Cough" },
        new() { Id = 10, Name = "Leptospirosis Vaccine", Description = "Protection against leptospira bacteria for pets", Price = 28.00m, Category = ProductCategory.Vaccine, PetType = PetType.Dog, Brand = "Nobivac", StockQuantity = 140, DosageInfo = "2 doses 2-4 weeks apart, annual booster", TargetCondition = "Leptospirosis" },
        new() { Id = 11, Name = "FVRCP Vaccine", Description = "Core feline vaccine for rhinotracheitis, calicivirus, and panleukopenia", Price = 30.00m, Category = ProductCategory.Vaccine, PetType = PetType.Cat, Brand = "Nobivac", StockQuantity = 100, DosageInfo = "3 doses at 6-8, 10-12, 14-16 weeks", TargetCondition = "Feline Respiratory & Panleukopenia" },
        new() { Id = 12, Name = "Lyme Disease Vaccine", Description = "Prevention of Lyme disease from tick bites in pets", Price = 32.00m, Category = ProductCategory.Vaccine, PetType = PetType.Dog, Brand = "Nobivac", StockQuantity = 90, DosageInfo = "2 doses 2-4 weeks apart, annual booster", TargetCondition = "Lyme Disease" },

        // Accessories
        new() { Id = 13, Name = "Adjustable Pet Collar", Description = "Durable nylon collar with quick-release buckle for pets of all sizes", Price = 12.99m, Category = ProductCategory.Accessory, PetType = PetType.Dog, Brand = "PetSafe", StockQuantity = 300, DosageInfo = "N/A", TargetCondition = "Everyday Wear" },
        new() { Id = 14, Name = "Retractable Leash", Description = "16 ft retractable leash with ergonomic grip for comfortable walks", Price = 24.99m, Category = ProductCategory.Accessory, PetType = PetType.Dog, Brand = "Flexi", StockQuantity = 200, DosageInfo = "N/A", TargetCondition = "Walking & Exercise" },
        new() { Id = 15, Name = "Stainless Steel Pet Bowl", Description = "Non-slip stainless steel food and water bowl for any pet", Price = 9.99m, Category = ProductCategory.Accessory, PetType = PetType.Cat, Brand = "PetSafe", StockQuantity = 250, DosageInfo = "N/A", TargetCondition = "Feeding" },
        new() { Id = 16, Name = "Pet Grooming Brush", Description = "Self-cleaning slicker brush for removing loose fur and tangles", Price = 14.50m, Category = ProductCategory.Accessory, PetType = PetType.Dog, Brand = "FURminator", StockQuantity = 180, DosageInfo = "N/A", TargetCondition = "Grooming" },
        new() { Id = 17, Name = "Pet Carrier Bag", Description = "Airline-approved soft-sided carrier for small pets", Price = 39.99m, Category = ProductCategory.Accessory, PetType = PetType.Cat, Brand = "Sherpa", StockQuantity = 75, DosageInfo = "N/A", TargetCondition = "Travel" },
        new() { Id = 18, Name = "Interactive Toy Ball", Description = "Treat-dispensing puzzle ball to keep pets mentally stimulated", Price = 11.99m, Category = ProductCategory.Accessory, PetType = PetType.Dog, Brand = "KONG", StockQuantity = 220, DosageInfo = "N/A", TargetCondition = "Play & Enrichment" },

        // Toys
        new() { Id = 19, Name = "Chew Toy", Description = "Durable rubber chew toy for aggressive chewers", Price = 9.99m, Category = ProductCategory.Toys, PetType = PetType.Dog, Brand = "PetFun", StockQuantity = 300, DosageInfo = "N/A", TargetCondition = "Play & Enrichment" },
        new() { Id = 20, Name = "Chew Toy", Description = "Small catnip-infused chew toy for cats", Price = 6.99m, Category = ProductCategory.Toys, PetType = PetType.Cat, Brand = "MeowPlay", StockQuantity = 250, DosageInfo = "N/A", TargetCondition = "Play & Enrichment" },
        new() { Id = 21, Name = "Scratch Post", Description = "Tall sisal scratch post with plush base for cats", Price = 29.99m, Category = ProductCategory.Toys, PetType = PetType.Cat, Brand = "MeowPlay", StockQuantity = 80, DosageInfo = "N/A", TargetCondition = "Play & Enrichment" },
        new() { Id = 22, Name = "Chew Toy", Description = "Wooden chew toy safe for rabbits and small pets", Price = 5.49m, Category = ProductCategory.Toys, PetType = PetType.Rabbit, Brand = "PetFun", StockQuantity = 200, DosageInfo = "N/A", TargetCondition = "Play & Enrichment" },
        new() { Id = 23, Name = "Scratch Post", Description = "Hanging bird toy with colorful beads and bell", Price = 7.99m, Category = ProductCategory.Toys, PetType = PetType.Bird, Brand = "PetFun", StockQuantity = 150, DosageInfo = "N/A", TargetCondition = "Play & Enrichment" },

        // Feeding
        new() { Id = 24, Name = "Food Bowl", Description = "Stainless steel non-tip food bowl for dogs", Price = 12.99m, Category = ProductCategory.Feeding, PetType = PetType.Dog, Brand = "PetServe", StockQuantity = 200, DosageInfo = "N/A", TargetCondition = "Feeding" },
        new() { Id = 25, Name = "Food Bowl", Description = "Elevated ceramic food bowl for cats", Price = 14.99m, Category = ProductCategory.Feeding, PetType = PetType.Cat, Brand = "PetServe", StockQuantity = 180, DosageInfo = "N/A", TargetCondition = "Feeding" },
        new() { Id = 26, Name = "Water Dispenser", Description = "Automatic gravity water dispenser for pets", Price = 19.99m, Category = ProductCategory.Feeding, PetType = PetType.Dog, Brand = "AquaPet", StockQuantity = 120, DosageInfo = "N/A", TargetCondition = "Feeding" },
        new() { Id = 27, Name = "Water Dispenser", Description = "Clip-on water bottle dispenser for rabbit cages", Price = 8.99m, Category = ProductCategory.Feeding, PetType = PetType.Rabbit, Brand = "AquaPet", StockQuantity = 160, DosageInfo = "N/A", TargetCondition = "Feeding" },
        new() { Id = 28, Name = "Food Bowl", Description = "Seed cup feeder for bird cages", Price = 6.49m, Category = ProductCategory.Feeding, PetType = PetType.Bird, Brand = "PetServe", StockQuantity = 220, DosageInfo = "N/A", TargetCondition = "Feeding" },

        // Grooming
        new() { Id = 29, Name = "Grooming Brush", Description = "Double-sided deshedding brush for dogs", Price = 15.99m, Category = ProductCategory.Grooming, PetType = PetType.Dog, Brand = "CleanPaws", StockQuantity = 140, DosageInfo = "N/A", TargetCondition = "Grooming" },
        new() { Id = 30, Name = "Grooming Brush", Description = "Soft slicker brush for cats with sensitive skin", Price = 12.49m, Category = ProductCategory.Grooming, PetType = PetType.Cat, Brand = "CleanPaws", StockQuantity = 130, DosageInfo = "N/A", TargetCondition = "Grooming" },
        new() { Id = 31, Name = "Nail Clipper", Description = "Professional-grade nail clipper for dogs", Price = 10.99m, Category = ProductCategory.Grooming, PetType = PetType.Dog, Brand = "SafePet", StockQuantity = 170, DosageInfo = "N/A", TargetCondition = "Grooming" },
        new() { Id = 32, Name = "Nail Clipper", Description = "Small nail clipper for rabbits and birds", Price = 7.99m, Category = ProductCategory.Grooming, PetType = PetType.Rabbit, Brand = "SafePet", StockQuantity = 150, DosageInfo = "N/A", TargetCondition = "Grooming" },
        new() { Id = 33, Name = "Grooming Brush", Description = "Gentle feather-safe grooming brush for birds", Price = 8.49m, Category = ProductCategory.Grooming, PetType = PetType.Bird, Brand = "CleanPaws", StockQuantity = 100, DosageInfo = "N/A", TargetCondition = "Grooming" },

        // Hygiene
        new() { Id = 34, Name = "Litter Box", Description = "Large hooded litter box with carbon filter for odor control", Price = 34.99m, Category = ProductCategory.Hygiene, PetType = PetType.Cat, Brand = "CleanPaws", StockQuantity = 90, DosageInfo = "N/A", TargetCondition = "Hygiene" },
        new() { Id = 35, Name = "Litter Box", Description = "Corner litter box for rabbits with easy-clean design", Price = 16.99m, Category = ProductCategory.Hygiene, PetType = PetType.Rabbit, Brand = "CleanPaws", StockQuantity = 110, DosageInfo = "N/A", TargetCondition = "Hygiene" },
        new() { Id = 36, Name = "Pet Collar", Description = "Flea and tick prevention collar for dogs", Price = 22.99m, Category = ProductCategory.Hygiene, PetType = PetType.Dog, Brand = "SafePet", StockQuantity = 160, DosageInfo = "N/A", TargetCondition = "Hygiene" },
        new() { Id = 37, Name = "Pet Collar", Description = "Flea prevention collar for cats", Price = 18.99m, Category = ProductCategory.Hygiene, PetType = PetType.Cat, Brand = "SafePet", StockQuantity = 140, DosageInfo = "N/A", TargetCondition = "Hygiene" },

        // Travel
        new() { Id = 38, Name = "Travel Carrier", Description = "Airline-approved hard-shell travel carrier for dogs", Price = 49.99m, Category = ProductCategory.Travel, PetType = PetType.Dog, Brand = "WalkMate", StockQuantity = 60, DosageInfo = "N/A", TargetCondition = "Travel" },
        new() { Id = 39, Name = "Travel Carrier", Description = "Soft-sided travel carrier for cats", Price = 39.99m, Category = ProductCategory.Travel, PetType = PetType.Cat, Brand = "WalkMate", StockQuantity = 70, DosageInfo = "N/A", TargetCondition = "Travel" },
        new() { Id = 40, Name = "Leash", Description = "Heavy-duty retractable leash for large dogs", Price = 27.99m, Category = ProductCategory.Travel, PetType = PetType.Dog, Brand = "WalkMate", StockQuantity = 180, DosageInfo = "N/A", TargetCondition = "Walking & Exercise" },
        new() { Id = 41, Name = "Leash", Description = "Lightweight harness leash for rabbits", Price = 14.99m, Category = ProductCategory.Travel, PetType = PetType.Rabbit, Brand = "WalkMate", StockQuantity = 100, DosageInfo = "N/A", TargetCondition = "Walking & Exercise" },
        new() { Id = 42, Name = "Travel Carrier", Description = "Compact travel carrier for birds with perch", Price = 32.99m, Category = ProductCategory.Travel, PetType = PetType.Bird, Brand = "WalkMate", StockQuantity = 55, DosageInfo = "N/A", TargetCondition = "Travel" },
    ];
}
