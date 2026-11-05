using Microsoft.EntityFrameworkCore;
using POS.Models;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


builder.Services.AddSwaggerGen();

var connectionString =
    builder.Configuration.GetConnectionString("cadenaSQL");
builder.Services.AddDbContext<PosAbarrotesContext>(options =>
options.UseSqlServer(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("NuevaPoliticas", app =>
    {
        app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseCors("NuevaPoliticas");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
