using AirportAutomation.Core.Converters;
using AirportAutomation.Infrastructure.Middlewares;
using AirportAutomation.Web.Binders;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
	.MinimumLevel.Is(LogEventLevel.Information)
	.WriteTo.Logger(lc => lc
		.Filter.ByIncludingOnly(le => le.Level >= LogEventLevel.Information)
		.WriteTo.Console()
	)
	.WriteTo.Logger(lc => lc
		.Filter.ByIncludingOnly(le => le.Level >= LogEventLevel.Warning)
		.WriteTo.File("Logs/AirportAutomationAPI.txt", rollingInterval: RollingInterval.Day)
	)
	.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.Services.AddCors(options =>
{
	options.AddPolicy("_AllowSpecificMethods", builder =>
	{
		builder.AllowAnyOrigin();
		builder.AllowAnyHeader();
		builder.WithMethods("GET", "POST");
	});
});

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
	options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
});

BinderConfiguration.Binders(builder.Services);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("_AllowSpecificMethods");
app.UseRouting();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandler>();
app.UseSession();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "passenger",
	pattern: "{controller=Passenger}/{action=Index}/{id?}");

app.Run();