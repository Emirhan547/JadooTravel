using JadooTravel.Business.Abstract;
using JadooTravel.Business.Concrete;
using JadooTravel.DataAccess.Context;
using JadooTravel.Business.Extensions;
using JadooTravel.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepositoryExtensions();
builder.Services.AddServiceExtensions();
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());



var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDbConnection");
var mongoDatabaseName = builder.Configuration.GetValue<string>("MongoDbSettings:DatabaseName");

if (string.IsNullOrWhiteSpace(mongoConnectionString))
    throw new ArgumentNullException(nameof(mongoConnectionString), "MongoDbConnection boş!");

if (string.IsNullOrWhiteSpace(mongoDatabaseName))
    throw new ArgumentNullException(nameof(mongoDatabaseName), "MongoDbSettings:DatabaseName boş!");

var mongoClient = new MongoClient(mongoConnectionString);

builder.Services.AddDbContext<JadooContext>(options =>
{
    options.UseMongoDB(mongoClient, mongoDatabaseName);
});



var app = builder.Build();



if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

    app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
