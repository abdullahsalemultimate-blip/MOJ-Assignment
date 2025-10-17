using Bogus;
using InventorySys.Domain.Constants;
using InventorySys.Domain.Entities;
using InventorySys.Domain.Enums;
using InventorySys.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InventorySys.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAdminUserAsync();
            await TrySeedSuppliersAndProductsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAdminUserAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole(Roles.Administrator);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
            }
        }
    }

    // this seeder method created with help of AI
    public async Task TrySeedSuppliersAndProductsAsync()
    {

        if (_context.Suppliers.Any() || _context.Products.Any())
        {
            // DB has been seeded
            return;
        }

        // --- 1. Seed Suppliers ---
        await SeedSuppliers();

        // --- 2. Seed Products and Audit Trail ---
        await SeedProductsAndAuditTrail();

        // Save changes for all audit entries created
        await _context.SaveChangesAsync();
    }

    private async Task SeedSuppliers()
    {
        // 1. Create a "Largest Supplier" for statistics requirement
        var largestSupplier = new Faker<Supplier>()
            .CustomInstantiator(f => Supplier.Create("Global Megacorp (Largest)"));

        // 2. Create the "Smallest Supplier" for future testing
        var smallestSupplier = new Faker<Supplier>()
            .CustomInstantiator(f => Supplier.Create("Local Goods (Smallest Orders)"));

        // 3. Create other generic suppliers using Bogus
        var supplierFaker = new Faker<Supplier>()
            .CustomInstantiator(f => Supplier.Create(f.Company.CompanyName()));

        var genericSuppliers = supplierFaker.Generate(8);

        _context.Suppliers.AddRange(largestSupplier, smallestSupplier);
        _context.Suppliers.AddRange(genericSuppliers);

        // Save the suppliers now so we can use their IDs for Products
        await _context.SaveChangesAsync();
    }

    private async Task SeedProductsAndAuditTrail()
    {
        var allSupplierIds = _context.Suppliers.Select(s => s.Id).ToList();
        if (!allSupplierIds.Any()) return;

        var largestSupplierId = _context.Suppliers
            .First(s => s.Name.Contains("Largest")).Id;

        var smallestSupplierId = _context.Suppliers
            .First(s => s.Name.Contains("Smallest")).Id;

        var randomSupplierId = new Faker().PickRandom(allSupplierIds);

        // --- Product Factory ---
        var productFaker = new Faker<Product>()
            .CustomInstantiator(f =>
            {
                var reorderLevel = f.Random.Number(10, 50);
                var unitsInStock = f.Random.Number(reorderLevel + 10, 200);

                return Product.Create(
                    name: f.Commerce.ProductName(),
                    quantityPerUnit: f.PickRandom<QuantityUnit>(),
                    supplierId: f.PickRandom(allSupplierIds),
                    unitPrice: f.Finance.Amount(5, 500),
                    unitsInStock: unitsInStock,
                    unitsOnOrder: f.Random.Number(0, 50),
                    reorderLevel: reorderLevel
                );
            });


        // 1. Products needing Reorder (Stock <= ReorderLevel)
        var reorderProducts = productFaker.CustomInstantiator(f =>
        {
            var reorderLevel = f.Random.Number(50, 100);
            var unitsInStock = f.Random.Number(5, reorderLevel); // UnitsInStock <= ReorderLevel

            return Product.Create(
                name: "LOW_STOCK: " + f.Commerce.ProductName(),
                quantityPerUnit: f.PickRandom<QuantityUnit>(),
                supplierId: randomSupplierId,
                unitPrice: f.Finance.Amount(50, 200),
                unitsInStock: unitsInStock,
                unitsOnOrder: 0,
                reorderLevel: reorderLevel
            );
        }).Generate(5);
        _context.Products.AddRange(reorderProducts);


        // 2. Products from the "Largest Supplier" (to satisfy the stats API)
        var largestSupplierProducts = productFaker.CustomInstantiator(f =>
        {
            return Product.Create(
                name: "MEGACORP: " + f.Commerce.ProductName(),
                quantityPerUnit: f.PickRandom<QuantityUnit>(),
                supplierId: largestSupplierId,
                unitPrice: f.Finance.Amount(100, 1000),
                unitsInStock: f.Random.Number(300, 500), // High stock for visibility
                unitsOnOrder: f.Random.Number(0, 0),
                reorderLevel: 50
            );
        }).Generate(15); // A large number to make this supplier dominant
        _context.Products.AddRange(largestSupplierProducts);


        // 3. Products with few/minimum orders (to identify for stopping)
        var minimumOrderProducts = productFaker.CustomInstantiator(f =>
        {
            return Product.Create(
                name: "MIN_ORDER: " + f.Commerce.ProductName(),
                quantityPerUnit: f.PickRandom<QuantityUnit>(),
                supplierId: smallestSupplierId, // Tie to the smallest supplier
                unitPrice: f.Finance.Amount(5, 50),
                unitsInStock: f.Random.Number(10, 30),
                unitsOnOrder: 1, // Only 1 unit on order
                reorderLevel: 5
            );
        }).Generate(3);
        _context.Products.AddRange(minimumOrderProducts);

        SeedProductsWithDuplicateNames(allSupplierIds);


        // 4. Products for Audit Trail demonstration
        var auditProduct1 = Product.Create("Audit Product A", QuantityUnit.Box, randomSupplierId, 10.00m, 100, 0, 20);
        var auditProduct2 = Product.Create("Audit Product B", QuantityUnit.Liter, randomSupplierId, 5.00m, 50, 0, 10);
        _context.Products.AddRange(auditProduct1, auditProduct2);

        // --- Save the initial creations to seed the database and create AuditTrail CREATE entries ---
        await _context.SaveChangesAsync();


        // --- Create multiple Audit Trail entries (simulated updates) ---

        // A. Product A: Update Price
        auditProduct1.UpdateDetails(
            auditProduct1.Name,
            auditProduct1.QuantityPerUnit,
            12.50m, // Price change
            auditProduct1.SupplierId,
            auditProduct1.UnitsInStock,
            auditProduct1.UnitsOnOrder,
            auditProduct1.ReorderLevel);
        await _context.SaveChangesAsync();

        // B. Product A: Increase Stock
        auditProduct1.IncreaseStock(50);
        await _context.SaveChangesAsync();

        auditProduct1.DecreaseStock(11);
        await _context.SaveChangesAsync();

        auditProduct1.UpdateDetails(
            auditProduct1.Name,
            auditProduct1.QuantityPerUnit,
            auditProduct1.UnitPrice,
            auditProduct1.SupplierId,
            auditProduct1.UnitsInStock,
            200,
            75);
        await _context.SaveChangesAsync();



        // C. Product B: Decrease Stock (reaching reorder level)
        auditProduct2.DecreaseStock(45); // Stock goes from 50 to 5 (<= ReorderLevel 10)
        await _context.SaveChangesAsync();

        // D. Product B: Update Supplier (Change of FK)
        auditProduct2.UpdateDetails(
            auditProduct2.Name,
            auditProduct2.QuantityPerUnit,
            auditProduct2.UnitPrice,
            largestSupplierId, // SupplierId change
            auditProduct2.UnitsInStock,
            auditProduct2.UnitsOnOrder,
            auditProduct2.ReorderLevel);
        await _context.SaveChangesAsync();
    }

    private void SeedProductsWithDuplicateNames(List<int> allSupplierIds)
    {
        if (allSupplierIds.Count < 3) return; // Need at least 3 suppliers to demonstrate diversity

        // Define three product names that will be duplicated across different suppliers
        var duplicatedProductNames = new List<string>
    {
        "Classic Red Widget (Duplicated)",
        "Premium Blue Widget (Duplicated)",
        "Standard Green Widget (Duplicated)"
    };

        var duplicatedProducts = new List<Product>();
        var supplierIterator = 0;

        foreach (var name in duplicatedProductNames)
        {
            // Use three different suppliers for each product name
            for (int i = 0; i < 3; i++)
            {
                // Cycle through the supplier IDs to ensure diversity
                var supplierId = allSupplierIds[supplierIterator % allSupplierIds.Count];
                supplierIterator++;

                var faker = new Faker();

                // Create the product using the fixed name and unique supplier ID
                var product = Product.Create(
                    name: name,
                    quantityPerUnit: faker.PickRandom<QuantityUnit>(),
                    supplierId: supplierId,
                    unitPrice: faker.Finance.Amount(2.00m, 99.00m),
                    unitsInStock: faker.Random.Number(50, 150),
                    unitsOnOrder: faker.Random.Number(0, 10),
                    reorderLevel: 25
                );
                duplicatedProducts.Add(product);
            }
        }

        _context.Products.AddRange(duplicatedProducts);
    }

}
