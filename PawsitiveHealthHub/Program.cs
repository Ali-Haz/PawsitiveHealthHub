using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PawsitiveHealthHub.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // Creating roles
    string[] roles = { "Owner", "Vet" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Creating VET USER
    string vetEmail = "vet1@hub.com";
    string vetPassword = "Vet123!";
    var vetUser = await userManager.FindByEmailAsync(vetEmail);
    if (vetUser == null)
    {
        vetUser = new ApplicationUser
        {
            UserName = vetEmail,
            Email = vetEmail,
            EmailConfirmed = true,
            FirstName = "Clara",
            LastName = "Williams"
        };
        var result = await userManager.CreateAsync(vetUser, vetPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(vetUser, "Vet");
        }
    }

    // Creating OWNER USER
    string ownerEmail = "owner1@hub.com";
    string ownerPassword = "Owner123!";
    var ownerUser = await userManager.FindByEmailAsync(ownerEmail);
    if (ownerUser == null)
    {
        ownerUser = new ApplicationUser
        {
            UserName = ownerEmail,
            Email = ownerEmail,
            EmailConfirmed = true,
            FirstName = "Max",
            LastName = "Davis"
        };
        var result = await userManager.CreateAsync(ownerUser, ownerPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(ownerUser, "Owner");
        }
    }
}


app.Run();
