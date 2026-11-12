using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Data;
using OCULIS.Data.Repositories;
using OCULIS.Models;
using OCULIS.Services.Email;
using OCULIS.Services.Identity;
using OCULIS.Services.Obavijest;
using OCULIS.Services.Placanje;
using OCULIS.Services.Popust;
using OCULIS.Services.Seed;
using OCULIS.Services.Termin;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<Korisnik>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddErrorDescriber<BosanskiIdentityErrorDescriber>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPopustStrategija, LojalnostPopustStrategija>();
builder.Services.AddScoped<IPopustStrategija, KolicinaPopustStrategija>();
builder.Services.AddScoped<IPopustStrategija, AkcijaPopustStrategija>();
builder.Services.AddScoped<IPopustKalkulatorService, PopustKalkulatorService>();
builder.Services.AddScoped<IPlacanjeStrategija, KarticnoPlacanjeStrategija>();
builder.Services.AddScoped<IPlacanjeStrategija, GotovinskoPlacanjeStrategija>();
builder.Services.AddScoped<IPlacanjeServisFactory, PlacanjeServisFactory>();
builder.Services.AddScoped<IObavijestServis, ObavijestServis>();
builder.Services.AddScoped<ITerminServis, TerminServis>();
builder.Services.AddTransient<IEmailSender, EmailService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
   var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
   await DataSeeder.SeedAsync(app.Services);
}

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
