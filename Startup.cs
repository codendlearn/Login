using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Services;
using is4aspid.Data;
using is4aspid.Models;
using Login.IdentityServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            services.AddControllersWithViews();
            services.Configure<IISOptions>(iis =>
                       {
                           iis.AuthenticationDisplayName = "Windows";
                           iis.AutomaticAuthentication = false;
                       });

            // configures IIS in-proc settings
            services.Configure<IISServerOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });
            services.AddAuthentication()
            .AddGoogle("Google", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                options.ClientId = "1032039370296-0a8p0eumo4pqet064m9g735beskfsfv1.apps.googleusercontent.com";
                options.ClientSecret = "D2_moj6aTJ0cCIncyLa1sc5V";
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                //options.UserSql(Configuration.GetConnectionString("DefaultConnection"), mySqlOptions =>
                //{
                //    mySqlOptions.ServerVersion(new Version(5, 6, 10), ServerType.MySql);
                //}));
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddCors(setup =>
            {
                setup.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.WithOrigins("http://localhost:4200");
                    policy.AllowCredentials();
                });
            });

            var builder = services.AddIdentityServer(options =>
            {
                //options.Events.RaiseErrorEvents = true;
                //options.Events.RaiseInformationEvents = true;
                //options.Events.RaiseFailureEvents = true;
                //options.Events.RaiseSuccessEvents = true;
                //options.UserInteraction.LoginUrl = "http://192.168.0.117:8080/login.html";
                //options.UserInteraction.ErrorUrl = "http://192.168.0.117:8080/error.html";
                //options.UserInteraction.LogoutUrl = "http://192.168.0.117:8080/logout.html";

                if (!string.IsNullOrEmpty(Configuration["Issuer"]))
                {
                    options.IssuerUri = Configuration["Issuer"];
                }
            }).AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                //.AddTestUsers(Config.GetTestUsers());
                .AddAspNetIdentity<ApplicationUser>();

            builder.AddSigningCredential(new X509Certificate2(Path.Join(Environment.WebRootPath, "IdentityServer4Auth.pfx"), "ApnePassword"));

            var cors = new DefaultCorsPolicyService(new LoggerFactory().CreateLogger<DefaultCorsPolicyService>())
            {
                AllowedOrigins = { "http://localhost:4200" },
                AllowAll = true,

            };

            services.AddControllers();
            services.AddSingleton<ICorsPolicyService>(cors);
            //services.AddTransient<IReturnUrlParser, ReturnUrlParser>();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<GlobalExceptionHandler>();
            }

            app.UseStaticFiles();
            app.UseCors();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}