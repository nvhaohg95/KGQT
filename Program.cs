
using KGQT.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSession();
builder.Services.AddMvc().AddSessionStateTempDataProvider();
builder.Services.AddCors(o => o.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddControllersWithViews();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddDbContext<nhanshiphangContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("KGQT"));
});
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
// NToastNotify
builder.Services.AddRazorPages().AddNToastNotifyNoty(new NotyOptions
{
    ProgressBar = true,
    Timeout = 3000
});
#region App
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseSession();
app.UseStaticFiles();
app.MapControllers();
app.UseRouting();
app.UseAuthorization();
app.UseNToastNotify();
app.MapRazorPages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}/{id?}", new { controller = "auth", action = "login" });
    endpoints.MapAreaControllerRoute(
      name: "admin",
      areaName: "admin",
      pattern: "admin/{controller}/{action}/{id?}", new { controller = "home", action = "index" }
    );
});
app.Run();
#endregion
