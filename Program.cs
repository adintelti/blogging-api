
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace BloggingAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // Configurar o DbContext com MariaDB
            builder.Services.AddDbContext<BlogDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(10, 5, 12))
                )
            );


            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                if (builder.Configuration.GetValue<bool>("Settings:UseScalar") == true)
                {
                    app.MapScalarApiReference();
                }
                else
                {
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/openapi/v1.json", "Weather API v1");
                    });
                }
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
