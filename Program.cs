using MongoDB.Driver;
using runSyncBackend.service.RunSync.Services;
using MongoDB.Bson;
using runSyncBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//Configure MongoDB connection
var connectionString = builder.Configuration.GetSection("MongoDb:ConnectionString").Value;
var databaseName = builder.Configuration.GetSection("MongoDb:DatabaseName").Value;
var client = new MongoClient(connectionString);
var database = client.GetDatabase(databaseName);

builder.Services.AddTransient<DataSeeder>();
builder.Services.AddSingleton(database);
builder.Services.AddSingleton<CryptoService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

try
{
    var databaseTest = app.Services.GetRequiredService<IMongoDatabase>();
    // Send a ping command to the server.
    await databaseTest.RunCommandAsync((Command<BsonDocument>)"{ping: 1}");
    Console.WriteLine("✅ Successfully connected to MongoDB!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Failed to connect to MongoDB. Error: {ex.Message}");
    // Optionally, you can decide to stop the application from starting if the DB is down.
    // return;
}

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    seeder.SeedData().Wait();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseRouting(); 

app.UseAuthorization();

app.MapControllers();

app.Run();
