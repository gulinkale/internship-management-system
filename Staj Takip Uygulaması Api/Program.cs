using StajTakipUygulamasý.Application.Interfaces;
using StajTakipUygulamasý.Infrastructure.Services;
using StajTakipUygulamasý.Models;
using StajTakipUygulamasý.Persistence.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IFileStorage, FileSystemFileStorage>();
builder.Services.AddScoped<IBelgeService, BelgeService>();
builder.Services.AddScoped<IStajyerService, StajyerService>();

builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

builder.Services.AddScoped<IBelgeTipiService, BelgeTipiService>();

builder.Services.AddSingleton<IFileStorage>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new FileSystemFileStorage(env.WebRootPath); // webroot'ý burada veriyoruz
});

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

app.Run();
