using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using UserManagementWithIdentity.Data;
using UserManagementWithIdentity.Models;
using UserManagementWithIdentity.Services.Contract;
using UserManagementWithIdentity.Services.Implementation;
using UserManagementWithIdentity.Utilities;

var builder = WebApplication.CreateBuilder(args);


#region Add services to the IOC container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
	throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
	.AddIdentity<ApplicationUser, IdentityRole>(options =>
	{
		options.Password.RequiredUniqueChars = 1;
		options.Password.RequireNonAlphanumeric = true;
		options.Password.RequiredLength = 6;
		options.Password.RequireDigit = true;
		options.Password.RequireLowercase = true;
		options.Password.RequireLowercase = true;

		options.User.RequireUniqueEmail = true;

		options.SignIn.RequireConfirmedAccount = true;
	})
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultUI()
	.AddDefaultTokenProviders();

builder.Services.Configure<Admin>(builder.Configuration.GetSection("Admin"));
builder.Services.AddScoped(typeof(ISeedingIdentityData), typeof(SeedingIdentityData));
builder.Services.Configure<ApplicationEmail>(builder.Configuration.GetSection("ApplicationEmail"));
builder.Services.AddTransient<IEmailSender, EmailSender>();


builder.Services.AddControllersWithViews();

#endregion

var app = builder.Build();

using var scope = app.Services.CreateScope();
var seedDataService = scope.ServiceProvider.GetRequiredService<ISeedingIdentityData>();
await seedDataService.SeedDataAsync();

#region Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
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

app.Run();

#endregion

