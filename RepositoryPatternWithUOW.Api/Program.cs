
using Microsoft.EntityFrameworkCore;
using RepositoryPatternWithUOW.Ef.Repository;
using RepositoryPatternWithUOW.Core.Repository;
using RepositoryPatternWithUOW.Ef;
namespace RepositoryPatternWithUOW.Api;
//http://localhost:5063/swagger/index.html
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<AppDbContext>(
           options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
       );
        //Inject BaseRepository in Repository Pattern
        //builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        //Inject UnitOfWork in Unit of Work Pattern with Repository Pattern
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        AddCors(builder);
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors("AllowAll");
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
    private static void AddCors(WebApplicationBuilder builder)
    {
        // this line add the cors policy to the container and this enable cross-origin requests
        // from the frontend (Angular in this case) to the backend (ASP.NET Core API).
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
            });
        });
    }
}
