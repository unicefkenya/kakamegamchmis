using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Models;
using MCHMIS.Models.Automapper;
using MCHMIS.Repository;
using Hangfire;
using Hangfire.SqlServer;
//using MCHMIS.Repository;
using MCHMIS.Services;
using MCHMIS.Services.Email;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Postal.AspNetCore;
using ElmahCore.Mvc;
using ElmahCore;
using DinkToPdf.Contracts;
using DinkToPdf;
using MCHMIS.Areas.Legacy.Data;

namespace MCHMIS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config2 =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    config2.Filters.Add(new AuthorizeFilter(policy));
                }).AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                }).AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContextPool<ApplicationDbContext>(_ =>
            {
                _.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), options =>
                {
                    options.CommandTimeout(180); // 3 minutes
                });
            });
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(30),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(30),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                PrepareSchemaIfNecessary = true
            }));
            services.AddHangfireServer();

            services.AddDbContext<LegacyDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LegacyConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddScoped<IDBService, DBService>();
            services.AddScoped<IEncryptDecrypt, EncryptDecrypt>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ISMSService, SMSService>();
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<ISingleRegistryService, SingleRegistryService>();
            services.AddScoped<ILogService, LogService>();
            // services.AddSingleton<IDatabaseService, DatabaseService>();
            // services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IEmailSenderEnhance, EmailSender>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>(); // <= Add this
            // add session support
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
            });
            var config = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile(new AutomapperProfile()));
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            services.AddElmah<XmlFileErrorLog>(options =>
            {
                options.CheckPermissionAction = context => context.User.Identity.IsAuthenticated;
                options.LogPath = "~/Logs";
            });
            //  services.AddMvc();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Expiration = TimeSpan.FromHours(1);
                });
            services.AddHttpsRedirection(options =>
            {
                //  options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 443;
            });
            //services.AddJsReport(new LocalReporting()
            //    .UseBinary(JsReportBinary.GetBinary())
            //    .AsUtility()
            //    .Create());
            services.AddTransient<DBService>();

            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, MyUserClaimsPrincipalFactory>();
            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.Name = "MCHTestCookie";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
                options.LoginPath = "/Account/Login";
                // ReturnUrlParameter requires
                //using Microsoft.AspNetCore.Authentication.Cookies;
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Permission", policyBuilder =>
                {
                    policyBuilder.Requirements.Add(new PermissionAuthorizationRequirement());
                });
            });
            services.Configure<EmailSenderOptions>(Configuration.GetSection("EmailSender"));
            services.AddPostal();
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseBrowserLink();
                app.UseExceptionHandler("/error");
            }

            app.UseBrowserLink();
            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();
            app.UseHsts();
            app.UseHttpsRedirection();
            // add session support
            app.UseSession();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseElmah();
            app.UseHangfireDashboard("/hangfire");
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "dashboardRoute",
                    template: "{area=Dashboard}/{controller=Landing}/{action=Index}/{id?}");

                routes.MapRoute(
                name: "adminRoute",
                template: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{area=admin}/{controller=Dashboard}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "reportsRoute",
                    template: "{area=reports}/{controller=Landing}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "exportRoute",
                    template: "{area=export}/{controller=Landing}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "legacyRoute",
                    template: "{area=legacy}/{controller=Mothers}/{action=Index}/{id?}");
            });
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                // context.Database.Migrate();
                // context.Database.EnsureCreated();
            }
        }
    }
}