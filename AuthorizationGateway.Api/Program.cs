using AuthorizationGateway.Infra.IoC;

var builder = WebApplication.CreateBuilder(args);

// Load crypto configuration from appsettings.json or environment variables
var env = builder.Environment;
var configuration = builder.Configuration;

var aesKey = configuration["Crypto:AES:KeyBase64"] ?? throw new InvalidOperationException("Missing AES Key");
var aesIv = configuration["Crypto:AES:IvBase64"] ?? throw new InvalidOperationException("Missing AES IV");
var hmac = configuration["Crypto:HMAC:Secret"] ?? throw new InvalidOperationException("Missing HMAC Secret");

// Dependency Injection
builder.Services.AddAuthorizationGatewayServices(aesKey, aesIv, hmac);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddControllers();

// Add Razor compilation
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("api", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5236/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapRazorPages();

app.UseAuthorization();

app.MapControllers();

app.Run();
