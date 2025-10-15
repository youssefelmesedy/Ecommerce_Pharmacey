using Pharmacy.Api.SwaggerExtenion;
using Pharmacy.Infarstructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// AddOpenApiDocument
builder.Services.AddSwaggerCustom();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCustomSwagger();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
