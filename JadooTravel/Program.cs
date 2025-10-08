using JadooTravel.DataAccess.Context;

using JadooTravel.UI.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddServiceExtensions();

var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDbConnection");
var mongoDatabaseName = builder.Configuration.GetValue<string>("MongoDbSettings:DatabaseName");

if (string.IsNullOrEmpty(mongoConnectionString))
    throw new ArgumentNullException("MongoDbConnection boþ!");

if (string.IsNullOrEmpty(mongoDatabaseName))
    throw new ArgumentNullException("MongoDbSettings:DatabaseName boþ!");

var mongoClient = new MongoClient(mongoConnectionString);
var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);
builder.Services.AddDbContext<JadooContext>(options =>
{
    options.UseMongoDB(mongoClient, mongoDatabaseName);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
