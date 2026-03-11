# McDonald's POS System

A modern, touch-friendly Point of Sale (POS) system inspired by McDonald's NP6, built with .NET 8 WPF to showcase MVVM architecture, Entity Framework Core, and professional UI design.

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![WPF](https://img.shields.io/badge/WPF-Desktop-blue?style=flat-square)
![SQLite](https://img.shields.io/badge/SQLite-Database-003B57?style=flat-square&logo=sqlite)
![Material Design](https://img.shields.io/badge/Material%20Design-UI-6200EE?style=flat-square)

## Features

### Order Taking
- **Category-based navigation** - Quick access to Burgers, Chicken, Fries & Sides, Drinks, Desserts, Breakfast, Happy Meals, and McCafe
- **Touch-optimized menu grid** - Large, colorful buttons for easy item selection
- **Size selection** - Small, Medium, Large options for applicable items
- **Item modifiers** - Add/remove ingredients (No Pickles, Extra Cheese, Add Bacon, etc.)
- **Combo/Meal builder** - "Make it a Meal?" prompt with automatic fries + drink pairing
- **Real-time order totals** - Live subtotal, tax (8%), and total calculation

### Order Management
- **Quantity adjustment** - Easy +/- controls per item
- **Item voiding** - Manager PIN-protected void functionality
- **Order clearing** - Start fresh with one tap

### Payment Processing (Simulated)
- **Cash payment** - Enter tendered amount, auto-calculate change
- **Quick tender buttons** - $10, $20, $50, $100 presets
- **Card payment** - One-tap "Card Approved" simulation
- **Receipt preview** - Professional receipt with order number

### Kitchen Display System (KDS)
- **Separate KDS window** - Simulates kitchen operations
- **Order status workflow** - Pending → In Prep → Ready → Complete
- **Visual indicators** - Color-coded status columns
- **Timer tracking** - Elapsed time per order with overdue highlighting
- **Auto-refresh** - Automatic updates every 5 seconds

### Authentication
- **Crew/Manager roles** - Different access levels
- **4-digit PIN login** - Touch-friendly numeric keypad
- **User selection** - Quick user switching

## Tech Stack

| Layer | Technology |
|-------|------------|
| **UI Framework** | WPF (.NET 8 Windows) |
| **Architecture** | MVVM with CommunityToolkit.Mvvm |
| **Database** | SQLite with Entity Framework Core 8 |
| **UI Components** | MaterialDesignInXamlToolkit |
| **DI Container** | Microsoft.Extensions.DependencyInjection |

## Project Structure

```
McDonaldsPOS/
├── McDonaldsPOS.Core/          # Domain models and enums
│   ├── Enums/
│   │   ├── ItemSize.cs
│   │   ├── MenuCategory.cs
│   │   ├── OrderStatus.cs
│   │   ├── PaymentMethod.cs
│   │   └── UserRole.cs
│   └── Models/
│       ├── MenuItem.cs
│       ├── MenuItemModifier.cs
│       ├── Modifier.cs
│       ├── Order.cs
│       ├── OrderItem.cs
│       ├── OrderItemModifier.cs
│       └── User.cs
│
├── McDonaldsPOS.Data/          # Data access layer
│   ├── AppDbContext.cs
│   ├── DbInitializer.cs        # Seeds menu data
│   └── Repositories/
│       ├── IRepository.cs
│       ├── Repository.cs
│       ├── IMenuRepository.cs
│       ├── MenuRepository.cs
│       ├── IOrderRepository.cs
│       ├── OrderRepository.cs
│       ├── IUserRepository.cs
│       └── UserRepository.cs
│
├── McDonaldsPOS.Services/      # Business logic
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── IMenuService.cs
│   ├── MenuService.cs
│   ├── IOrderService.cs
│   └── OrderService.cs
│
└── McDonaldsPOS.UI/            # WPF Application
    ├── App.xaml                # Theme & resources
    ├── App.xaml.cs             # DI configuration
    ├── Converters/
    │   └── Converters.cs
    ├── ViewModels/
    │   ├── ViewModelBase.cs
    │   ├── MainViewModel.cs
    │   ├── LoginViewModel.cs
    │   ├── POSViewModel.cs
    │   └── KDSViewModel.cs
    └── Views/
        ├── MainWindow.xaml
        ├── LoginView.xaml
        ├── POSView.xaml
        └── KDSWindow.xaml
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10/11 (WPF requirement)
- Visual Studio 2022+ or VS Code with C# extension

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/McDonaldsPOS.git
   cd McDonaldsPOS
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run --project McDonaldsPOS.UI
   ```

The database will be automatically created and seeded with menu items on first run.

### Default Users

| Username | PIN | Role |
|----------|-----|------|
| crew1 | 1234 | Crew |
| crew2 | 2345 | Crew |
| manager | 9999 | Manager |

## Screenshots

### Login Screen
- Touch-friendly PIN pad
- User selection list
- McDonald's branding

### Main POS Screen
- Category navigation bar
- Menu item grid
- Current order panel
- Payment totals

### Kitchen Display System
- Three-column layout (Pending / In Prep / Ready)
- Order cards with items and modifiers
- Bump/status buttons
- Order timing

## Key Design Patterns

- **MVVM** - Clean separation of concerns with CommunityToolkit.Mvvm
- **Repository Pattern** - Abstract data access for testability
- **Service Layer** - Business logic isolation
- **Dependency Injection** - Loose coupling via Microsoft.Extensions.DependencyInjection
- **Observable Properties** - Reactive UI updates with source generators

## Seeded Menu Items

The database is pre-populated with realistic McDonald's-style menu items:

**Burgers**: Big Mac, Quarter Pounder w/ Cheese, McDouble, Cheeseburger, Hamburger, Double Cheeseburger

**Chicken**: 10pc/6pc/4pc McNuggets, McChicken, Crispy Chicken Sandwich

**Fries & Sides**: French Fries (S/M/L), Hash Browns, Apple Slices, Side Salad

**Drinks**: Coca-Cola, Sprite, Fanta Orange, Dr Pepper, Sweet Tea, Bottled Water

**Desserts**: McFlurry (Oreo/M&M), Hot Fudge Sundae, Vanilla Cone, Apple Pie, Chocolate Chip Cookie

**Breakfast**: Egg McMuffin, Sausage McMuffin, Hotcakes, Sausage Burrito, Fruit & Yogurt Parfait

**Happy Meals**: Cheeseburger, McNuggets (4pc), Hamburger

**McCafe**: Premium Roast Coffee, Iced Coffee, Caramel Frappe, Mocha Frappe, Hot Chocolate

## Future Enhancements

- [ ] Order history and reporting
- [ ] Inventory management
- [ ] Employee scheduling
- [ ] Multi-language support
- [ ] Receipt printing integration
- [ ] Real payment gateway integration

## License

This project is for educational and portfolio purposes only. McDonald's and all related trademarks are property of McDonald's Corporation.

---

Built with ❤️ using .NET 8, WPF, and Material Design
