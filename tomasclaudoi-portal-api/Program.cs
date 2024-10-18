
using Microsoft.EntityFrameworkCore;
using SAPB1SLayerWebAPI.Context;

var policyName = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// EF Core
builder.Services.AddDbContext<SboDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SBO_DB")));
builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AUTH_DB")));
builder.Services.AddDbContext<MainDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MAIN_DB")));

// HTTP CLIENT
builder.Services.AddHttpClient("API_GATEWAY", c =>
{
    c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("API_GW")!);
}).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policyName,
                      builder =>
                      {
                          builder
                            //.WithOrigins("http://superspeed-svr:8099")
                            .AllowAnyOrigin()
                            //.SetIsOriginAllowed(origin => true)
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                      });
});

// NEWTONSOFT
builder.Services.AddControllers().AddNewtonsoftJson();


var app = builder.Build();

// CORS
app.UseCors(policyName);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
