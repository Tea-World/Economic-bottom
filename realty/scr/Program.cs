using ServiceDesk.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI: JSON repositories (без EF Core / без БД)
builder.Services.AddSingleton<IEquipmentRepository, JsonEquipmentRepository>();
builder.Services.AddSingleton<IServiceRequestRepository, JsonServiceRequestRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
