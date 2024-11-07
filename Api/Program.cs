using Application.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options => options.AddDefaultPolicy(
        policy => policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()));

// Register the Swagger generator, defining 1 or more Swagger documents
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "VSA ecomerce API", Version = "v1" }));

builder.Services.AddProblemDetails();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpoints();
builder.Services.AddAuthentication();
var app = builder.Build();



app.UseCors();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
    app.UseExceptionHandler("/error-development");
}
else
{
    app.UseExceptionHandler("/error");
}
app.UseAuthentication();
app.MapEndpoints();
app.MapHealthChecks();
app.Run();
