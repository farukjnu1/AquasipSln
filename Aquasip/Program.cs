using Aquasip.EF;
using Aquasip.Models;
using Aquasip.Services.EmailServices;
using Aquasip.Services.TokenServices;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache(); // Required for Session

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7); // Optional
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Required for GDPR compliance
});

builder.Services.AddControllersWithViews();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddDataProtection();

builder.Services.AddScoped<ITokenService, TokenService>();

//string Aquasip = builder.Configuration["ConnectionStrings:AquasipContext"];
builder.Services.AddDbContext<AquasipContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AquasipContext")));

var app = builder.Build();

// 👇 enable static file serving from wwwroot
app.UseStaticFiles();

// Configure the HTTP request pipeline.
/*if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}*/
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else 
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
