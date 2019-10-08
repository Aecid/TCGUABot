using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using TCGUABot.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using TCGUABot.Data.Models;
using TCGUABot.Helpers.TelegramOAuth.Middleware;
using TCGUABot.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using TCGUABot.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Cors;

namespace TCGUABot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Bot.Get();
            SecondaryBot.Get();
            CardData.Initalize();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));


            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
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
            services.AddProgressiveWebApp();

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

            services.AddMvc(options =>
             {
                 options.InputFormatters.Insert(0, new TextPlainInputFormatter());
             }
            )
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson()
                .AddRazorPagesOptions(ops =>
                {
                    ops.Conventions.AuthorizePage("/Tournaments/Create", "Event Organizer");
                });

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddHostedService<TimeService>();
            services.AddHostedService<TimeServiceGetSpoilers>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithHeaders("Access-Control-Allow-Origin"));
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "simple",
            //        template: "{controller=Home}/{action=Index}");
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
            app.UseRouting();
            app.UseCors("AllowAnyOrigin");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints
                .MapRazorPages().RequireAuthorization();
                

                endpoints
                .MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
