// ApiServer.cs
using Microsoft.EntityFrameworkCore;
using src.Models;

namespace src
{
    public class ApiServer
    {
        public static WebApplication CreateApiServer(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication("Cookies")
            .AddCookie("Cookies", options =>
            {
                options.LoginPath = "/api/auth/login";
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
            });


            // Ajouter la connexion à SQLite
            builder.Services.AddDbContext<UserDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();

            var app = builder.Build();
             
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}
