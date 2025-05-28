using BookStore.Application.Services;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Interfaces;
using BookStore.Infrastructure.Services;

namespace BookStore.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IBookGenerationService, BookGenerationService>();
            builder.Services.AddScoped<IRegionProvider, RegionProvider>();
            builder.Services.AddScoped<ICsvExportService, CsvExportService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
