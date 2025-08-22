using Microsoft.EntityFrameworkCore;
using StajTakipUygulamasý.Application.Interfaces;
using StajTakipUygulamasý.Data;
using StajTakipUygulamasý.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// DB
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StajContext>(options => options.UseSqlServer(conn));

// DI
builder.Services.AddScoped<IStajyerService, StajyerService>();
builder.Services.AddScoped<IStajService, StajService>();
builder.Services.AddScoped<IBelgeService, BelgeService>();
builder.Services.AddScoped<IBelgeTipiService, BelgeTipiService>();
builder.Services.AddScoped<IBasvuruService, BasvuruService>();
builder.Services.AddScoped<IRaporService, RaporService>();
builder.Services.AddSingleton<IFileStorage>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new FileSystemFileStorage(env.WebRootPath);
});
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();


// IHttpClientFactory kaydý
builder.Services.AddHttpClient("Api", client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"]
                  ?? throw new InvalidOperationException("Api:BaseUrl missing");
    client.BaseAddress = new Uri(baseUrl);
});



// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
