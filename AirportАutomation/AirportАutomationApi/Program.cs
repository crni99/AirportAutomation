using AirportAutomationApi.Binders;
using AirportAutomationApi.Data;
using AirportАutomationApi.Helpers;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;

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

builder.Services.AddCors(options =>
{
	options.AddPolicy("_AllowAll", builder =>
	{
		builder.AllowAnyOrigin();
		builder.AllowAnyHeader();
		builder.AllowAnyMethod();
	});
});

builder.Services.AddControllers(
		options => options.UseDateOnlyTimeOnlyStringConverters())
	.AddJsonOptions(options => options.UseDateOnlyTimeOnlyStringConverters())
	.AddNewtonsoftJson();
builder.Host.UseSerilog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
	setupAction.SwaggerDoc("v1", new OpenApiInfo
	{
		Description = "Airport Automation Api",
		Title = "AirportAutomationApi",
		Version = "v1",
		Contact = new OpenApiContact
		{
			Name = "Ognjen Andjelic",
			Email = "andjelicb.ognjen@gmail.com",
			Url = new Uri("https://github.com/crni99")
		},

	});

	setupAction.AddSecurityDefinition("AirportAutomationApiBearerAuth", new OpenApiSecurityScheme()
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Input a valid token to access this API"
	});

	setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference {
					Type = ReferenceType.SecurityScheme,
					Id = "AirportAutomationApiBearerAuth" }
			}, new List<string>() }
	});
	var filePath = Path.Combine(AppContext.BaseDirectory, "AirportАutomationApi.xml");
	setupAction.IncludeXmlComments(filePath);
	setupAction.UseDateOnlyTimeOnlyStringConverters();
	setupAction.DocumentFilter<JsonPatchDocumentFilter>();
});

BinderConfiguration.Binders(builder.Services);

builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(
	builder.Configuration.GetConnectionString("Default")
));

builder.Services.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(o =>
	{
		o.IncludeErrorDetails = true;
		o.TokenValidationParameters = new()
		{
			RoleClaimType = "Admin",
			ValidTypes = new[] { "JWT" },
			ValidIssuer = builder.Configuration["Authentication:Issuer"],
			ValidAudience = builder.Configuration["Authentication:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretForKey"])),
			ValidateIssuer = true,
			ValidateAudience = false,
			ValidateLifetime = false,
			ValidateIssuerSigningKey = true
		};
	}
	);
builder.Services.AddAuthorization();

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
	options.EnableEndpointRateLimiting = true;
	options.StackBlockedRequests = false;
	options.HttpStatusCode = 429;
	options.RealIpHeader = "X-Real-IP";
	options.ClientIdHeader = "X-ClientId";
	options.GeneralRules = new List<RateLimitRule>
	{
		new RateLimitRule
		{
			Endpoint = "*",
			Period = "60s",
			Limit = 120
		}
	};
});

builder.Services.AddApiVersioning(setupAction =>
{
	setupAction.AssumeDefaultVersionWhenUnspecified = true;
	setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
	setupAction.ReportApiVersions = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.DefaultModelsExpandDepth(-1);
	});
}

app.UseHttpsRedirection();
app.UseCors("_AllowAll");
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
