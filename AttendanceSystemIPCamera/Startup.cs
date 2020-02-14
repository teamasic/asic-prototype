using AttendanceSystemIPCamera.BackgroundServices;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using AttendanceSystemIPCamera.Framework.Database;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.GroupService;
using AttendanceSystemIPCamera.Services.SessionService;
using AttendanceSystemIPCamera.Services.RecordService;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AttendanceSystemIPCamera.Services.AttendeeService;
using AttendanceSystemIPCamera.Services.AttendeeGroupService;

namespace AttendanceSystemIPCamera
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
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options => {
                    options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
                });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            SetupDatabaseContext(services);
            SetupAutoMapper(services);
            SetupDependencyInjection(services);
            SetupBackgroundService(services);
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
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }

        private void SetupDatabaseContext(IServiceCollection services)
        {
            services.AddDbContext<MainDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqliteDB")));
        }
        private void SetupAutoMapper(IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        private void SetupDependencyInjection(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddSingleton(Configuration);
            // services.AddSingleton<IMapper>(Mapper.Instance);

            services.AddScoped<DbContext, MainDbContext>();
            services.AddScoped<MyUnitOfWork>();
            services.AddScoped<GroupValidation>();

            SetupServices(services);
            SetupRepositories(services);
        }

        private void SetupServices(IServiceCollection services)
        {
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IRecordService, RecordService>();
            services.AddScoped<IAttendeeService, AttendeeService>();
            services.AddScoped<IAttendeeGroupService, AttendeeGroupService>();
        }
        private void SetupRepositories(IServiceCollection services)
        {
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IRecordRepository, RecordRepository>();
            services.AddScoped<IAttendeeRepository, AttendeeRepository>();
            services.AddScoped<IAttendeeGroupRepository, AttendeeGroupRepository>();
        }

        private void SetupBackgroundService(IServiceCollection services)
        {
            services.AddSingleton<IHostedService, WindowAppRunnerService>();
        }
    }
}
