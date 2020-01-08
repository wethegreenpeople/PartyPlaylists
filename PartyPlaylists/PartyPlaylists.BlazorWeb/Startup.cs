using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PartyPlaylists.BlazorWeb.Data;
using PartyPlaylists.Services;
using System.Net.Http;
using SpotifyApi.NetCore;
using PartyPlaylists.BlazorWeb.Shared;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using PartyPlaylists.MobileAppService.Contexts;

namespace PartyPlaylists.BlazorWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor().AddCircuitOptions(opt => { opt.DetailedErrors = true; });
            services.AddHttpContextAccessor();
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<RoomDataStore>((s) => new RoomDataStore(new PlaylistContextFactory().CreateDbContext(null)));
            services.AddSingleton<SpotifyService>();
            services.AddSingleton<RefreshService>();
            services.AddSingleton<TokenService>();
            services.AddStorage();
            services.AddSingleton(new UserAccountsService(new HttpClient(), Configuration));

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseForwardedHeaders();
            app.UsePathBase("");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
