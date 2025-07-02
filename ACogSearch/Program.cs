using ACogSearch.SearchTools;
using Microsoft.Extensions.Options;

namespace ACogSearch
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.Configure<AzureSearchOptions>(builder
                .Configuration.GetSection("AzureSearch"));
            builder.Services.AddTransient<SearchService>();
            builder.Services.AddTransient<DocumentUploader>();
            
            var app = builder.Build();

            // Hämta config-inställningar
            var searchOptions = app.Services.GetRequiredService<IOptions<AzureSearchOptions>>().Value;
            // Skapa index om det saknas
            await IndexCreator.CreateIndexIfNotExistsAsync(searchOptions.Endpoint, searchOptions.ApiKey);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Upload}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
