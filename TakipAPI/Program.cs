using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using StajTakipUygulamasý.Application.Interfaces;
using StajTakipUygulamasý.Data;                 // StajContext
using StajTakipUygulamasý.Infrastructure.Services; // StajService, StajyerService, BelgeService, BelgeTipiService, BasvuruService, RaporService

var builder = WebApplication.CreateBuilder(args);


//TakipAPI program.cs

// ---------------- DB ----------------
builder.Services.AddDbContext<StajContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString("Default");
    opt.UseSqlServer(cs);
});

// -------------- DI ------------------
builder.Services.AddScoped<IStajyerService, StajyerService>();
builder.Services.AddScoped<IStajService, StajService>();
builder.Services.AddScoped<IBelgeService, BelgeService>();
builder.Services.AddScoped<IBelgeTipiService, BelgeTipiService>();
builder.Services.AddScoped<IBasvuruService, BasvuruService>();   // varsa
builder.Services.AddScoped<IRaporService, RaporService>();       // varsa

// File storage kökünü webroot'a göre ayarlayalým:
builder.Services.AddSingleton<IFileStorage>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new FileSystemFileStorage(env.WebRootPath); // wwwroot/...
});
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

// -------- Controllers + Swagger ------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------- Upload limitleri ----------
builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 200L * 1024L * 1024L; // 200 MB
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartHeadersLengthLimit = int.MaxValue;
});

// ---------------- CORS ---------------
const string CorsPolicy = "AllowWeb";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(CorsPolicy, p => p
        .WithOrigins(
            "http://localhost:5173",
            "http://localhost:3000",
            "http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

// -------------- Pipeline -------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();   // wwwroot/belgeler gibi dosyalar
app.UseRouting();
app.UseCors(CorsPolicy);

// auth yoksa kapalý:
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();
