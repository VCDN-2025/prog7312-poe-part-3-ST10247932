using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
// Add these new using directives
using MunicipalReporter.DataStructures;
using MunicipalReporter.Managers;
using MunicipalReporter.Models;
using MunicipalReporter.Repositories;
using MunicipalReporter.Services;

namespace MunicipalReporter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<ChatService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<IssueRepository>();

            builder.Services.AddSingleton<ServiceRequestManager>();


            // *** ADD GEOCODING SERVICE REGISTRATION ***
            builder.Services.AddScoped<IGeocodingService, NominatimGeocodingService>();
            builder.Services.AddHttpClient<IGeocodingService, NominatimGeocodingService>(client =>
            {
                client.BaseAddress = new Uri("https://nominatim.openstreetmap.org");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "MunicipalReporterApp/1.0 (contact@yourmunicipality.gov.za)");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            // *** ADD AUTH AND CHAT SERVICES (STEP 4) ***
            builder.Services.AddSingleton<UserRepository>(); // For user management
            builder.Services.AddSingleton<MessageLinkedList>(); // Linked list for chat messages
            builder.Services.AddHttpContextAccessor(); // For session management
            builder.Services.AddSession(options => // Add session support with options
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // *** ADD SESSION MIDDLEWARE (This is crucial!) ***
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}