using System.Diagnostics;
using CS_Tutorial.Data;
using Microsoft.EntityFrameworkCore;
using CS_Tutorial.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
