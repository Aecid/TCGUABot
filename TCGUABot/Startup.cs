using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCGUABot.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TCGUABot.Helpers.TelegramOAuth.Middleware;
using TCGUABot.Data.Models;
using TCGUABot.Helpers;
using React.AspNet;

namespace TCGUABot
{
    public class Startup
    {
        public static IServiceCollection StaticServices {get; private set;}
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Bot.Get();
            CardData.Initalize();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication()
                .AddTelegram(options =>
                {
                    options.BotUsername = Configuration.GetSection("TelegramSettings").GetSection("TelegramBotName").Value;
                    options.ClientId = Configuration.GetSection("TelegramSettings").GetSection("TelegramClientId").Value;
                    options.ClientSecret = Configuration.GetSection("TelegramSettings").GetSection("TelegramBotToken").Value;
                }
                );

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddReact();

            services.AddAuthorization(ops =>
            {
                ops.AddPolicy("Admin", policy =>
                {
                    policy.RequireRole("Admin");
                });
                ops.AddPolicy("Store Owner", policy =>
                {
                    policy.RequireRole("Store Owner");
                });
                ops.AddPolicy("Event Organizer", policy =>
                {
                    policy.RequireRole("Event Organizer");
                });
                ops.AddPolicy("Judge", policy =>
                {
                    policy.RequireRole("Judge");
                });
            });

            services.AddMvc( options =>
              {
                  options.InputFormatters.Insert(0, new TextPlainInputFormatter());
              }
            )
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.DateFormatString = "0:MM/dd/yy, H:mm";
                })
                .AddRazorPagesOptions(ops =>
                {

                });

            services.AddHostedService<TimeService>();

            Startup.StaticServices = services;

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseReact(config =>
            {
                // If you want to use server-side rendering of React components,
                // add all the necessary JavaScript files here. This includes
                // your components as well as all of their dependencies.
                // See http://reactjs.net/ for more information. Example:
                //config
                //  .AddScript("~/js/First.jsx")
                //  .AddScript("~/js/Second.jsx");

                // If you use an external build too (for example, Babel, Webpack,
                // Browserify or Gulp), you can improve performance by disabling
                // ReactJS.NET's version of Babel and loading the pre-transpiled
                // scripts. Example:
                //config
                //  .SetLoadBabel(false)
                //  .AddScriptWithoutTransform("~/js/bundle.server.js");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "simple",
                    template: "{controller=Home}/{action=Index}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
