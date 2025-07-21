using MongoDB.Driver;
using runSyncBackend.service.RunSync.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//Configure MongoDB connection
var connectionString = builder.Configuration.GetSection("MongoDb:ConnectionString").Value;
var databaseName = builder.Configuration.GetSection("MongoDb:DatabaseName").Value;
var client = new MongoClient(connectionString);
var database = client.GetDatabase(databaseName);

builder.Services.AddTransient<DataSeeder>();
builder.Services.AddSingleton(database);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
