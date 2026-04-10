using Microsoft.EntityFrameworkCore;
using ShopOnline.Auth.Server.Utils;
using ShopOnline.Infrastructure.Database;

namespace WebApp.Internal;


public static class AppBootstrap
{
    public static async Task RunAsync(IServiceProvider services)
    {


        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        // 1. Kiểm tra & Chạy Migration
        Console.WriteLine("[Bootstrap] Checking Migrations...");
        var db = provider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        // 2. Quét & Load Phân quyền (Hàm static của ông ở module Auth)
        Console.WriteLine("[Bootstrap] Scanning System Permissions...");
        PermissionHub.ScanAll();
        // 3. Seed User/Data nếu chưa có
        //Console.WriteLine("[Bootstrap] Seeding Default Data...");
        //await SeedInitialData.EnsureCreated(provider, perms);

        //Console.WriteLine("[Bootstrap] System is ready.");
    }
}