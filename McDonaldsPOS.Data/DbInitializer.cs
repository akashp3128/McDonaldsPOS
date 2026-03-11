using McDonaldsPOS.Core.Enums;
using McDonaldsPOS.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace McDonaldsPOS.Data;

/// <summary>
/// Seeds the database with initial McDonald's-style menu data
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Skip if already seeded
        if (await context.Users.AnyAsync())
            return;

        // Seed Users
        var users = new List<User>
        {
            new() { Username = "crew1", DisplayName = "Alex (Crew)", Pin = "1234", Role = UserRole.Crew },
            new() { Username = "crew2", DisplayName = "Jordan (Crew)", Pin = "2345", Role = UserRole.Crew },
            new() { Username = "manager", DisplayName = "Sam (Manager)", Pin = "9999", Role = UserRole.Manager }
        };
        context.Users.AddRange(users);

        // Seed Modifiers
        var modifiers = new List<Modifier>
        {
            // "No" modifiers (free)
            new() { Name = "No Pickles", ShortName = "NO PKL", PriceAdjustment = 0, IsAddition = false, SortOrder = 1 },
            new() { Name = "No Onions", ShortName = "NO ONI", PriceAdjustment = 0, IsAddition = false, SortOrder = 2 },
            new() { Name = "No Lettuce", ShortName = "NO LET", PriceAdjustment = 0, IsAddition = false, SortOrder = 3 },
            new() { Name = "No Tomato", ShortName = "NO TOM", PriceAdjustment = 0, IsAddition = false, SortOrder = 4 },
            new() { Name = "No Ketchup", ShortName = "NO KET", PriceAdjustment = 0, IsAddition = false, SortOrder = 5 },
            new() { Name = "No Mustard", ShortName = "NO MUS", PriceAdjustment = 0, IsAddition = false, SortOrder = 6 },
            new() { Name = "No Mayo", ShortName = "NO MAY", PriceAdjustment = 0, IsAddition = false, SortOrder = 7 },
            new() { Name = "No Cheese", ShortName = "NO CHZ", PriceAdjustment = 0, IsAddition = false, SortOrder = 8 },
            new() { Name = "No Salt", ShortName = "NO SLT", PriceAdjustment = 0, IsAddition = false, SortOrder = 9 },
            new() { Name = "No Ice", ShortName = "NO ICE", PriceAdjustment = 0, IsAddition = false, SortOrder = 10 },
            // "Add/Extra" modifiers (some with upcharge)
            new() { Name = "Extra Cheese", ShortName = "XTR CHZ", PriceAdjustment = 0.50m, IsAddition = true, SortOrder = 11 },
            new() { Name = "Add Bacon", ShortName = "ADD BCN", PriceAdjustment = 1.50m, IsAddition = true, SortOrder = 12 },
            new() { Name = "Extra Pickles", ShortName = "XTR PKL", PriceAdjustment = 0, IsAddition = true, SortOrder = 13 },
            new() { Name = "Extra Onions", ShortName = "XTR ONI", PriceAdjustment = 0, IsAddition = true, SortOrder = 14 },
            new() { Name = "Add Mac Sauce", ShortName = "ADD MAC", PriceAdjustment = 0.30m, IsAddition = true, SortOrder = 15 },
            new() { Name = "Light Ice", ShortName = "LT ICE", PriceAdjustment = 0, IsAddition = false, SortOrder = 16 },
        };
        context.Modifiers.AddRange(modifiers);
        await context.SaveChangesAsync();

        // Seed Menu Items
        var menuItems = new List<MenuItem>
        {
            // === BURGERS ===
            new() { Name = "Big Mac", ShortName = "BIG MAC", BasePrice = 5.99m, Category = MenuCategory.Burgers, IsComboEligible = true, CanHaveModifiers = true, BackgroundColor = "#D32F2F", SortOrder = 1 },
            new() { Name = "Quarter Pounder w/ Cheese", ShortName = "QPC", BasePrice = 6.49m, Category = MenuCategory.Burgers, IsComboEligible = true, CanHaveModifiers = true, BackgroundColor = "#D32F2F", SortOrder = 2 },
            new() { Name = "McDouble", ShortName = "MCDBL", BasePrice = 3.19m, Category = MenuCategory.Burgers, IsComboEligible = true, CanHaveModifiers = true, BackgroundColor = "#D32F2F", SortOrder = 3 },
            new() { Name = "Cheeseburger", ShortName = "CHZBGR", BasePrice = 2.49m, Category = MenuCategory.Burgers, IsComboEligible = true, CanHaveModifiers = true, BackgroundColor = "#D32F2F", SortOrder = 4 },
            new() { Name = "Hamburger", ShortName = "HMBGR", BasePrice = 1.99m, Category = MenuCategory.Burgers, IsComboEligible = true, CanHaveModifiers = true, BackgroundColor = "#D32F2F", SortOrder = 5 },
            new() { Name = "Double Cheeseburger", ShortName = "DBL CHZ", BasePrice = 3.99m, Category = MenuCategory.Burgers, IsComboEligible = true, CanHaveModifiers = true, BackgroundColor = "#D32F2F", SortOrder = 6 },

            // === CHICKEN ===
            new() { Name = "10pc McNuggets", ShortName = "10 NUG", BasePrice = 5.99m, Category = MenuCategory.Chicken, IsComboEligible = true, CanHaveModifiers = false, BackgroundColor = "#FF9800", SortOrder = 1 },
            new() { Name = "6pc McNuggets", ShortName = "6 NUG", BasePrice = 4.49m, Category = MenuCategory.Chicken, IsComboEligible = true, CanHaveModifiers = false, BackgroundColor = "#FF9800", SortOrder = 2 },
            new() { Name = "4pc McNuggets", ShortName = "4 NUG", BasePrice = 2.99m, Category = MenuCategory.Chicken, IsComboEligible = false, CanHaveModifiers = false, BackgroundColor = "#FF9800", SortOrder = 3 },
            new() { Name = "McChicken", ShortName = "MCCHKN", BasePrice = 2.49m, Category = MenuCategory.Chicken, IsComboEligible = true, CanHaveModifiers = true, BackgroundColor = "#FF9800", SortOrder = 4 },
            new() { Name = "Crispy Chicken Sandwich", ShortName = "CRSPY CHK", BasePrice = 5.49m, Category = MenuCategory.Chicken, IsComboEligible = true, CanHaveModifiers = true, BackgroundColor = "#FF9800", SortOrder = 5 },

            // === FRIES & SIDES ===
            new() { Name = "French Fries", ShortName = "FRIES", BasePrice = 2.49m, Category = MenuCategory.FriesSides, IsSizable = true, SmallPrice = 1.89m, MediumPrice = 2.49m, LargePrice = 3.19m, CanHaveModifiers = false, BackgroundColor = "#FFC107", SortOrder = 1 },
            new() { Name = "Hash Browns", ShortName = "HASHBRN", BasePrice = 1.99m, Category = MenuCategory.FriesSides, CanHaveModifiers = false, BackgroundColor = "#FFC107", SortOrder = 2 },
            new() { Name = "Apple Slices", ShortName = "APPLES", BasePrice = 1.49m, Category = MenuCategory.FriesSides, CanHaveModifiers = false, BackgroundColor = "#4CAF50", SortOrder = 3 },
            new() { Name = "Side Salad", ShortName = "SALAD", BasePrice = 3.49m, Category = MenuCategory.FriesSides, CanHaveModifiers = false, BackgroundColor = "#4CAF50", SortOrder = 4 },

            // === DRINKS ===
            new() { Name = "Coca-Cola", ShortName = "COKE", BasePrice = 1.99m, Category = MenuCategory.Drinks, IsSizable = true, SmallPrice = 1.29m, MediumPrice = 1.79m, LargePrice = 2.19m, CanHaveModifiers = true, BackgroundColor = "#F44336", SortOrder = 1 },
            new() { Name = "Sprite", ShortName = "SPRITE", BasePrice = 1.99m, Category = MenuCategory.Drinks, IsSizable = true, SmallPrice = 1.29m, MediumPrice = 1.79m, LargePrice = 2.19m, CanHaveModifiers = true, BackgroundColor = "#8BC34A", SortOrder = 2 },
            new() { Name = "Fanta Orange", ShortName = "FANTA", BasePrice = 1.99m, Category = MenuCategory.Drinks, IsSizable = true, SmallPrice = 1.29m, MediumPrice = 1.79m, LargePrice = 2.19m, CanHaveModifiers = true, BackgroundColor = "#FF9800", SortOrder = 3 },
            new() { Name = "Dr Pepper", ShortName = "DR PEP", BasePrice = 1.99m, Category = MenuCategory.Drinks, IsSizable = true, SmallPrice = 1.29m, MediumPrice = 1.79m, LargePrice = 2.19m, CanHaveModifiers = true, BackgroundColor = "#7B1FA2", SortOrder = 4 },
            new() { Name = "Sweet Tea", ShortName = "SWTEA", BasePrice = 1.49m, Category = MenuCategory.Drinks, IsSizable = true, SmallPrice = 1.00m, MediumPrice = 1.29m, LargePrice = 1.49m, CanHaveModifiers = true, BackgroundColor = "#795548", SortOrder = 5 },
            new() { Name = "Bottled Water", ShortName = "WATER", BasePrice = 1.49m, Category = MenuCategory.Drinks, CanHaveModifiers = false, BackgroundColor = "#2196F3", SortOrder = 6 },

            // === DESSERTS ===
            new() { Name = "McFlurry Oreo", ShortName = "MCFL OREO", BasePrice = 3.99m, Category = MenuCategory.Desserts, CanHaveModifiers = false, BackgroundColor = "#9C27B0", SortOrder = 1 },
            new() { Name = "McFlurry M&M", ShortName = "MCFL M&M", BasePrice = 3.99m, Category = MenuCategory.Desserts, CanHaveModifiers = false, BackgroundColor = "#9C27B0", SortOrder = 2 },
            new() { Name = "Hot Fudge Sundae", ShortName = "HF SUND", BasePrice = 2.49m, Category = MenuCategory.Desserts, CanHaveModifiers = false, BackgroundColor = "#795548", SortOrder = 3 },
            new() { Name = "Vanilla Cone", ShortName = "V CONE", BasePrice = 1.49m, Category = MenuCategory.Desserts, CanHaveModifiers = false, BackgroundColor = "#E3F2FD", SortOrder = 4 },
            new() { Name = "Apple Pie", ShortName = "APPLE PIE", BasePrice = 1.99m, Category = MenuCategory.Desserts, CanHaveModifiers = false, BackgroundColor = "#FFEB3B", SortOrder = 5 },
            new() { Name = "Chocolate Chip Cookie", ShortName = "COOKIE", BasePrice = 1.29m, Category = MenuCategory.Desserts, CanHaveModifiers = false, BackgroundColor = "#8D6E63", SortOrder = 6 },

            // === BREAKFAST ===
            new() { Name = "Egg McMuffin", ShortName = "EGG MCM", BasePrice = 4.49m, Category = MenuCategory.Breakfast, IsComboEligible = true, CanHaveModifiers = true, BackgroundColor = "#FFEB3B", SortOrder = 1 },
            new() { Name = "Sausage McMuffin", ShortName = "SAUS MCM", BasePrice = 2.99m, Category = MenuCategory.Breakfast, IsComboEligible = true, CanHaveModifiers = true, BackgroundColor = "#FFEB3B", SortOrder = 2 },
            new() { Name = "Sausage McMuffin w/ Egg", ShortName = "SAUS EGG", BasePrice = 4.99m, Category = MenuCategory.Breakfast, IsComboEligible = true, CanHaveModifiers = true, BackgroundColor = "#FFEB3B", SortOrder = 3 },
            new() { Name = "Hotcakes", ShortName = "HOTCAKES", BasePrice = 4.49m, Category = MenuCategory.Breakfast, CanHaveModifiers = false, BackgroundColor = "#FFB74D", SortOrder = 4 },
            new() { Name = "Sausage Burrito", ShortName = "BURRITO", BasePrice = 2.49m, Category = MenuCategory.Breakfast, CanHaveModifiers = true, BackgroundColor = "#FF7043", SortOrder = 5 },
            new() { Name = "Fruit & Yogurt Parfait", ShortName = "PARFAIT", BasePrice = 2.99m, Category = MenuCategory.Breakfast, CanHaveModifiers = false, BackgroundColor = "#EC407A", SortOrder = 6 },

            // === HAPPY MEALS ===
            new() { Name = "Cheeseburger Happy Meal", ShortName = "CHZ HM", BasePrice = 5.99m, Category = MenuCategory.HappyMeals, CanHaveModifiers = true, BackgroundColor = "#E91E63", SortOrder = 1 },
            new() { Name = "McNuggets Happy Meal (4pc)", ShortName = "NUG HM", BasePrice = 5.99m, Category = MenuCategory.HappyMeals, CanHaveModifiers = false, BackgroundColor = "#E91E63", SortOrder = 2 },
            new() { Name = "Hamburger Happy Meal", ShortName = "HBG HM", BasePrice = 5.49m, Category = MenuCategory.HappyMeals, CanHaveModifiers = true, BackgroundColor = "#E91E63", SortOrder = 3 },

            // === McCAFE ===
            new() { Name = "Premium Roast Coffee", ShortName = "COFFEE", BasePrice = 1.49m, Category = MenuCategory.McCafe, IsSizable = true, SmallPrice = 1.00m, MediumPrice = 1.49m, LargePrice = 1.79m, CanHaveModifiers = false, BackgroundColor = "#5D4037", SortOrder = 1 },
            new() { Name = "Iced Coffee", ShortName = "ICED COF", BasePrice = 2.49m, Category = MenuCategory.McCafe, IsSizable = true, SmallPrice = 1.99m, MediumPrice = 2.49m, LargePrice = 2.99m, CanHaveModifiers = true, BackgroundColor = "#795548", SortOrder = 2 },
            new() { Name = "Caramel Frappe", ShortName = "CARM FRP", BasePrice = 4.29m, Category = MenuCategory.McCafe, IsSizable = true, SmallPrice = 3.49m, MediumPrice = 4.29m, LargePrice = 4.99m, CanHaveModifiers = false, BackgroundColor = "#A1887F", SortOrder = 3 },
            new() { Name = "Mocha Frappe", ShortName = "MOCHA FRP", BasePrice = 4.29m, Category = MenuCategory.McCafe, IsSizable = true, SmallPrice = 3.49m, MediumPrice = 4.29m, LargePrice = 4.99m, CanHaveModifiers = false, BackgroundColor = "#6D4C41", SortOrder = 4 },
            new() { Name = "Hot Chocolate", ShortName = "HOT CHOC", BasePrice = 2.29m, Category = MenuCategory.McCafe, IsSizable = true, SmallPrice = 1.79m, MediumPrice = 2.29m, LargePrice = 2.79m, CanHaveModifiers = false, BackgroundColor = "#4E342E", SortOrder = 5 },
        };

        context.MenuItems.AddRange(menuItems);
        await context.SaveChangesAsync();

        // Link modifiers to appropriate menu items
        var burgerModifierIds = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 11, 12, 13, 14, 15 }; // All burger-related modifiers
        var drinkModifierIds = new[] { 10, 16 }; // No Ice, Light Ice

        var menuItemModifiers = new List<MenuItemModifier>();

        foreach (var item in menuItems)
        {
            if (!item.CanHaveModifiers) continue;

            if (item.Category == MenuCategory.Burgers || item.Category == MenuCategory.Chicken ||
                item.Category == MenuCategory.Breakfast || item.Category == MenuCategory.HappyMeals)
            {
                foreach (var modId in burgerModifierIds)
                {
                    menuItemModifiers.Add(new MenuItemModifier { MenuItemId = item.Id, ModifierId = modId });
                }
            }
            else if (item.Category == MenuCategory.Drinks || item.Category == MenuCategory.McCafe)
            {
                foreach (var modId in drinkModifierIds)
                {
                    menuItemModifiers.Add(new MenuItemModifier { MenuItemId = item.Id, ModifierId = modId });
                }
            }
        }

        context.MenuItemModifiers.AddRange(menuItemModifiers);
        await context.SaveChangesAsync();
    }
}
