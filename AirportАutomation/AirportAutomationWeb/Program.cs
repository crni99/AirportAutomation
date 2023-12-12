using AirportAutomationWeb.Converters;
using AirportАutomationWeb.Binders;
using AirportАutomationWeb.Converters;

var builder = WebApplication.CreateBuilder(args);

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
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "passenger",
	pattern: "{controller=Passenger}/{action=Index}/{id?}");

app.Run();