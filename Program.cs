using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ToxicFitnessAPI.Data;
using ToxicFitnessAPI.Models;
using ToxicFitnessAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// -------------------- MongoDB Configuration --------------------
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDB")
);

builder.Services.AddSingleton<IMongoClient>(s =>
{
    var settings = builder.Configuration.GetSection("MongoDB").Get<MongoDBSettings>();
    return new MongoClient(settings.ConnectionURI);
});

builder.Services.AddSingleton(s =>
{
    var mongoSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDBSettings>();
    var client = s.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoSettings.DatabaseName);
});

// -------------------- Register Services --------------------
builder.Services.AddSingleton<UserGoalService>();
builder.Services.AddSingleton<WorkoutGeneratorService>();
builder.Services.AddSingleton<NutritionService>();
builder.Services.AddSingleton<MealPlanService>();
builder.Services.AddSingleton<WorkoutPlanService>();

// -------------------- API Behavior --------------------
builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// -------------------- Swagger & Middleware --------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Always enable Swagger (for production on Render)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToxicFitnessAPI v1");
    c.RoutePrefix = string.Empty; // makes Swagger UI available at root "/"
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// -------------------- Database Seeder --------------------
var mongoSection = builder.Configuration.GetSection("MongoDB");
var conn = mongoSection.GetValue<string>("ConnectionURI");
var dbName = mongoSection.GetValue<string>("DatabaseName");

await DataSeeder.SeedAsync(conn, dbName);

// -------------------- Run App --------------------
app.Run();
