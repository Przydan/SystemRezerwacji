using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Domain.Entities;

namespace Infrastructure.Persistence.Seed;

public class IdentityDataSeeder
{
    public static async Task SeedRolesAndAdminUserAsync(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<IdentityDataSeeder>>();
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
            
            var adminEmail = "admin@x.pl"; 
            var adminPassword = "Pass1234!@#$"; 

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
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Wystąpił błąd podczas seedowania danych Identity.");
        }
    }
}