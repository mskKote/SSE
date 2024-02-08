using Microsoft.AspNetCore.Mvc.Formatters;
using task_1_books_service.Models;

namespace task_1_books_service;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers(options =>
        {
            // [ERROR] This JsonSerializerOptions instance is read-only or has already been used in serialization or deserialization.
            // Forcing Pascal Case for JSON
            // var jsonOutputFormatters = options.OutputFormatters.OfType<SystemTextJsonOutputFormatter>();
            // jsonOutputFormatters.AsParallel().ForAll(x => x.SerializerOptions.PropertyNamingPolicy = null);
            // Adding XML support
            options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Mapping Dependencies
        builder.Services.AddScoped<IBookRepository, BookXmlRepository>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}