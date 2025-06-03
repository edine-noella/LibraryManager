
using LibraryManager.API.Models;
using LibraryManager.API.Repositories;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(); 

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LibraryDb")));

//all possible lifetimes
// builder.Services.AddTransient<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
// builder.Services.AddSingleton<IBookRepository, BookRepository>();
//we are using scoped because we are relying on a services (repository) that is scoped


// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Map your controllers
app.MapControllers();


app.Run();