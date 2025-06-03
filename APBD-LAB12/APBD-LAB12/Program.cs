using APBD_LAB12.Data;
using APBD_LAB12.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. 
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddDbContext<MasterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(name:"Default")));

builder.Services.AddScoped<IDbService, DbService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); 

app.Run();