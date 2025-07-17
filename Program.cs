using WorkflowEngine.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Workflow Engine API",
        Version = "v1",
        Description = "A configurable workflow engine (state-machine API) that allows you to define workflows, start instances, and execute state transitions.",
        Contact = new OpenApiContact
        {
            Name = "Infonetica Assignment"
        }
    });
    
    // Include XML comments for better API documentation
    var xmlFile = "WorkflowEngine.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Register workflow services
builder.Services.AddSingleton<IWorkflowStorage, InMemoryWorkflowStorage>();
builder.Services.AddScoped<IWorkflowEngine, WorkflowEngineService>();

// Add CORS support for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Workflow Engine API v1");
        c.RoutePrefix = string.Empty; // Serve the Swagger UI at the app's root
    });
    app.UseCors();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

// Add a simple health check endpoint
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow })
   .WithName("HealthCheck")
   .WithOpenApi();

app.Run();
