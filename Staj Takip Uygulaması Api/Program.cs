
using StajTakipUygulamasý.Application.Interfaces;
using StajTakipUygulamasý.Data;                 // StajContext
using StajTakipUygulamasý.Infrastructure.Services; // StajService, StajyerService, BelgeService, BelgeTipiService, BasvuruService, RaporService

var builder = WebApplication.CreateBuilder(args);

// ---------------- DB ----------------
builder.Services.AddSingleton<IFileStorage>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new FileSystemFileStorage(env.WebRootPath);
});

// -------------- DI ------------------
builder.Services.AddScoped<IStajyerService, StajyerService>();
builder.Services.AddScoped<IStajService, StajService>();
builder.Services.AddScoped<IBelgeService, BelgeService>();
builder.Services.AddScoped<IBelgeTipiService, BelgeTipiService>();
builder.Services.AddScoped<IBasvuruService, BasvuruService>();   // UI controller'larýnda kullanýlýyor
builder.Services.AddScoped<IRaporService, RaporService>();

builder.Services.AddSingleton<IFileStorage>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new FileSystemFileStorage(env.WebRootPath);
});
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

// ------------- MVC Views ------------
builder.Services.AddControllersWithViews();

var app = builder.Build();

// -------------- Pipeline -------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// auth yoksa kapalý:
// app.UseAuthentication();
// app.UseAuthorization();

// default MVC yönlendirmesi
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
