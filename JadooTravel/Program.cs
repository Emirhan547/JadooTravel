using JadooTravel.Business.Abstract;
using JadooTravel.Business.Extensions;
using JadooTravel.Business.Mappings;
using JadooTravel.Business.Options;
using JadooTravel.DataAccess.Extensions;
using JadooTravel.Entity.Entities;
using JadooTravel.Services;
using JadooTravel.UI.Extensions;
using JadooTravel.UI.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddServiceExtensions();
builder.Services.AddRepositoryExtensions();
builder.Services.AddHttpClient<IAIService, OpenAiService>();
builder.Services.Configure<OpenAiOptions>(builder.Configuration.GetSection("OpenAI"));
builder.Services.AddAutoMapper(typeof(GeneralMapping).Assembly);


var mongoConnectionString = builder.Configuration["MongoSettings:ConnectionString"];
var mongoDatabaseName = builder.Configuration["MongoSettings:DatabaseName"];

if (string.IsNullOrWhiteSpace(mongoConnectionString))
    throw new ArgumentNullException(nameof(mongoConnectionString));

if (string.IsNullOrWhiteSpace(mongoDatabaseName))
    throw new ArgumentNullException(nameof(mongoDatabaseName));

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(mongoConnectionString));

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDatabaseName);
});

builder.Services
    .AddIdentity<AppUser, AppRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;

        options.User.RequireUniqueEmail = true;
    })
    .AddMongoDbStores<AppUser, AppRole, string>(
        mongoConnectionString,
        mongoDatabaseName)
    .AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
});

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();

var app = builder.Build();
await app.Services.EnsureMongoIndexesAsync();



if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
// Program.cs'e ekle:
app.UseMiddleware<AnalyticsMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
 // Add this using directive at the top of the file
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapDefaultControllerRoute();
await app.SeedAdminAsync();
app.Run();


