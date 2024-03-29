﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend;

public class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

        if (context.Users.Any())
        {
            return;
        }

        string[] roles = [ "Administrator", "Manager" ];
        using var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        using var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        var user = new AppUser
        {
            Email = "leela@contoso.com",
            NormalizedEmail = "LEELA@CONTOSO.COM",
            UserName = "leela@contoso.com",
            NormalizedUserName = "LEELA@CONTOSO.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D")
        };

        var password = new PasswordHasher<AppUser>();
        var hashed = password.HashPassword(user, "Passw0rd!");
        user.PasswordHash = hashed;

        await userManager.AddToRolesAsync(user, roles);

        var userStore = new UserStore<AppUser>(context);
        var result = userStore.CreateAsync(user);

        await context.SaveChangesAsync();
    }
}
