using FlappyAlby.API.Extensions;
using FlappyAlby.API.Infrastructure;
using FlappyAlby.API.Options;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

Log.Information("Starting web host");

// Add services to the container.
builder.Services
    .AddOptions<ConnectionStringOptions>()
    .Bind(builder.Configuration.GetSection("ConnectionStrings"))
    .ValidateDataAnnotations();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddLogging(b => b.AddSerilog(Log.Logger, true));

builder.Services.AddDbContext<FlappyAlbyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultDatabase")));

var app = builder.Build();

await app.MigrateContextAsync<FlappyAlbyContext>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();