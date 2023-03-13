using Microsoft.EntityFrameworkCore;
using PRSApplicationCapstone.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<PrsDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("PrsDbContext"));
});

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthorization();

app.MapControllers();

app.Run();
