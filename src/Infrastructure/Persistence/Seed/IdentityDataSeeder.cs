using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Domain.Entities;

namespace Infrastructure.Persistence.Seed;

public class IdentityDataSeeder
{
    // Default admin credentials (can be overridden via environment variables)
    private const string DefaultAdminEmail = "admin@x.pl";
    private const string DefaultAdminPassword = "Pass1234!@#$";
    
    public static async Task SeedRolesAndAdminUserAsync(IServiceProvider services, bool seedTestUsers = true)
    {
        var logger = services.GetRequiredService<ILogger<IdentityDataSeeder>>();
        var configuration = services.GetRequiredService<IConfiguration>();
        
        try
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            string[] roleNames = { "Administrator", "User" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation($"Rola '{roleName}' została utworzona.");
                    }
                    else
                    {
                        logger.LogError(
                            $"Błąd podczas tworzenia roli '{roleName}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }
            
            // Read admin credentials from environment/config or use defaults
            var adminEmail = configuration["AdminEmail"] ?? DefaultAdminEmail;
            var adminPassword = configuration["AdminPassword"] ?? DefaultAdminPassword;

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "Systemu",
                    EmailConfirmed = true 
                };
                var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createUserResult.Succeeded)
                {
                    logger.LogInformation($"Użytkownik admin '{adminEmail}' został utworzony.");

                    var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "Administrator");
                    if (addToRoleResult.Succeeded)
                    {
                        logger.LogInformation($"Rola 'Administrator' została przypisana użytkownikowi '{adminEmail}'.");
                    }
                    else
                    {
                        logger.LogError(
                            $"Błąd podczas przypisywania roli 'Administrator' użytkownikowi '{adminEmail}': {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    logger.LogError(
                        $"Błąd podczas tworzenia użytkownika admin '{adminEmail}': {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
                }
            }
            
            
            // --- Seed 20 Test Users (Software House Staff) ---
            if (seedTestUsers)
            {
                // --- Seed Realistic Software House Staff ---
                var personas = new[]
                {
                    new { First = "Jan", Last = "Kowalski", Email = "jan.kowalski@x.pl", Role = "User", Note = "Senior Dev" },
                    new { First = "Anna", Last = "Nowak", Email = "anna.nowak@x.pl", Role = "User", Note = "Project Manager" },
                    new { First = "Piotr", Last = "Wiśniewski", Email = "piotr.wisniewski@x.pl", Role = "User", Note = "QA Lead" },
                    new { First = "Katarzyna", Last = "Wójcik", Email = "katarzyna.wojcik@x.pl", Role = "User", Note = "UX Designer" },
                    new { First = "Michał", Last = "Lewandowski", Email = "michal.lewandowski@x.pl", Role = "User", Note = "DevOps" },
                    new { First = "Tomasz", Last = "Zieliński", Email = "tomasz.zielinski@x.pl", Role = "User", Note = "Backend Dev" },
                    new { First = "Magdalena", Last = "Kamińska", Email = "magdalena.kaminska@x.pl", Role = "User", Note = "Frontend Dev" },
                    new { First = "Paweł", Last = "Woźniak", Email = "pawel.wozniak@x.pl", Role = "User", Note = "Mobile Dev" },
                    new { First = "Agnieszka", Last = "Dąbrowska", Email = "agnieszka.dabrowska@x.pl", Role = "User", Note = "HR Manager" },
                    new { First = "Krzysztof", Last = "Jankowski", Email = "krzysztof.jankowski@x.pl", Role = "Administrator", Note = "CTO" }
                };
    
                foreach (var p in personas)
                {
                    if (await userManager.FindByEmailAsync(p.Email) == null)
                    {
                        var user = new User
                        {
                            UserName = p.Email,
                            Email = p.Email,
                            FirstName = p.First,
                            LastName = p.Last,
                            EmailConfirmed = true
                        };
                        
                        var result = await userManager.CreateAsync(user, "Pass1234!@#$");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, p.Role);
                            logger.LogInformation($"Seeded User: {p.First} {p.Last} ({p.Note})");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Wystąpił błąd podczas seedowania danych Identity.");
        }
    }
}