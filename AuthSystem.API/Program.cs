using AuthSystem.API.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configurar servicios de la aplicación
builder.Services.AddApplicationServices(builder.Configuration);

// Configurar autenticación JWT
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configurar Swagger
builder.Services.AddSwaggerDocumentation();

// Configurar CORS
builder.Services.AddCorsPolicy(builder.Configuration);

// Configurar compresión de respuestas
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthSystem API v1"));
}

app.UseHttpsRedirection();

// Usar CORS
app.UseCors("CorsPolicy");

// Usar compresión de respuestas
app.UseResponseCompression();

// Usar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores
app.MapControllers();

app.Run();
