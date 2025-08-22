using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StajTakipUygulamasý.Application.Interfaces;
using StajTakipUygulamasý.Data;                 // StajContext
using StajTakipUygulamasý.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// DB
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StajContext>(opt => opt.UseSqlServer(cs));

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

// API + Swagger
builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Staj Takip API", Version = "v1" });
});

// Upload limit (opsiyonel)
builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 200L * 1024L * 1024L;
});

const string CorsPolicy = "AllowWeb";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(CorsPolicy, p => p
        .WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:4200")
        .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});


var app = builder.Build();

// Middleware pipeline
app.UseDeveloperExceptionPage(); // opsiyonel: dev exception page her ortamda açýlýr
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Staj Takip API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(CorsPolicy);
// app.UseAuthentication();
// app.UseAuthorization();
app.MapControllers();
app.Run();
