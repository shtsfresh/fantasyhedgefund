using Fantasy_Hedge_Fund.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DAL;
using BLL.Interfaces;
using BLL.Services;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MainDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<MainDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IUtilitiesService, UtilitiesService>();
builder.Services.AddTransient<ITransactionService, TransactionService>();
builder.Services.AddTransient<IAssetService, AssetService>();

var app = builder.Build();

//ITrigger trigger = TriggerBuilder.Create()
//    .StartNow()
//    .WithSimpleSchedule(x => x.WithIntervalInMinutes(10).RepeatForever())
//    .Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseMigrationsEndPoint();
//}
//else
//{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
