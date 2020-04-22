using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PartyPlaylists.Contexts;
using PartyPlaylists.Models.DataModels;
using PartyPlaylists.MVC.Hubs;
using PartyPlaylists.Services;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.IO;
using NSwag.AspNetCore;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace PartyPlaylists.MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSignalR();

            var connectionString = Configuration.GetConnectionString("PartyPlaylistConnectionString");

            services.AddDbContext<PlaylistContext>(options => options.UseMySql(connectionString));
            services.AddSingleton<IConfiguration>(provider => Configuration);
            // Using JWT tokens as a sort of "soft" authentication
            // The user is never creating an account, but we're generating one for them if they
            // don't have one when connecting to a room
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetValue<string>("Jwt:Key"))),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            services.AddSwaggerDocument(config =>
            {
                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT Token"));
                config.AddSecurity("JWT Token", Enumerable.Empty<string>(),
                    new OpenApiSecurityScheme()
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        BearerFormat = "Bearer {token}",
                        Description = "Copy this into the value field: Bearer {token}"
                    }
                );
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "PartyPlaylists API";
                    document.Info.Description = "An open API for PartyPlaylists";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "John Singh",
                        Email = "wethegreenpeople@gmail.com",
                        Url = "https://github.com/wethegreenpeople/partyplaylists"
                    };
                };
            });

            services.AddMvc().AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<RoomHub>("/roomhub");
            });
        }
    }
}
