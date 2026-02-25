using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Elastic.Clients.Elasticsearch;
using JadooTravel.Business.Abstract;
using JadooTravel.Business.Extensions;
using JadooTravel.Business.Mappings;
using JadooTravel.Business.Options;
using JadooTravel.DataAccess.Extensions;
using JadooTravel.Entity.Entities;
using JadooTravel.Services;
using JadooTravel.UI.Extensions;
using JadooTravel.UI.Logging;
using JadooTravel.UI.Models;
using JadooTravel.UI.Options;
using JadooTravel.UI.Services;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<AwsS3Options>(builder.Configuration.GetSection("AWS:S3"));

var awsS3Options = builder.Configuration.GetSection("AWS:S3").Get<AwsS3Options>() ?? new AwsS3Options();
builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    var regionEndpoint = RegionEndpoint.GetBySystemName(string.IsNullOrWhiteSpace(awsS3Options.Region) ? "us-east-1" : awsS3Options.Region);

    if (!string.IsNullOrWhiteSpace(awsS3Options.AccessKey) && !string.IsNullOrWhiteSpace(awsS3Options.SecretKey))
    {
        var credentials = new BasicAWSCredentials(awsS3Options.AccessKey, awsS3Options.SecretKey);
        return new AmazonS3Client(credentials, regionEndpoint);
    }

    return new AmazonS3Client(regionEndpoint);
});
builder.Services.AddScoped<IAwsS3Service, AwsS3Service>();
builder.Services.Configure<ElasticLoggingOptions>(builder.Configuration.GetSection("ElasticConfiguration"));

var elasticUri = builder.Configuration["ElasticConfiguration:Uri"] ?? "http://localhost:9200";
builder.Services.AddSingleton(_ =>
{
    var settings = new ElasticsearchClientSettings(new Uri(elasticUri));
    return new ElasticsearchClient(settings);
});
builder.Services.AddScoped<IElasticAuditLogger, ElasticAuditLogger>();

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


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapDefaultControllerRoute();
await app.SeedAdminAsync();
app.Run();


