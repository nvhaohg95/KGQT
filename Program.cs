
using KGQT.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.FileProviders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSession();
builder.Services.AddMvc().AddRazorPagesOptions(options =>
{
    options.Conventions.AddPageRoute("/Login/Index", "");
}).AddSessionStateTempDataProvider();
builder.Services.AddCors(o => o.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddControllersWithViews();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
TaskRun.Run();
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

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "uploads")),
    RequestPath = "/uploads"
});
app.MapControllers();
app.UseRouting();
app.UseAuthorization();
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
