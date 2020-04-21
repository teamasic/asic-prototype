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
using AttendanceSystemIPCamera.Services.RoomService;
using AttendanceSystemIPCamera.Services.ChangeRequestService;
using System;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using AttendanceSystemIPCamera.Services.NetworkService;
using AttendanceSystemIPCamera.Services.RecognitionService;
using AttendanceSystemIPCamera.Services.UnitService;
using AttendanceSystemIPCamera.Framework.GlobalStates;
using Microsoft.Extensions.Logging;
using AttendanceSystemIPCamera.Services.UserService;
using AttendanceSystemIPCamera.Services.SettingsService;
using System.IO;
using Microsoft.Extensions.FileProviders;
using static AttendanceSystemIPCamera.Framework.Constants;
using AttendanceSystemIPCamera.Services.LogService;
using AttendanceSystemIPCamera.Services.OtherSettingsService;
using Microsoft.AspNetCore.Http.Connections;

namespace AttendanceSystemIPCamera
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private MyConfiguration myConfig;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
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
            SetupMyConfiguration(services);
            SetupFilesConfiguration(services);
            SetupBackgroundService(services);
            setupSwagger(services);
        }

        private void SetupMyConfiguration(IServiceCollection services)
        {
            myConfig = Configuration.GetSection("MyConfiguration").Get<MyConfiguration>();
            services.AddSingleton(myConfig);
        }
        private void SetupFilesConfiguration(IServiceCollection services)
        {
            services.AddSingleton(Configuration.GetSection("FilesConfiguration").Get<FilesConfiguration>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
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
            SetupStaticFiles(app);

            app.UseSpaStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Supervisor ASIC API"));

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<RealTimeService>("/hub", options =>
                {
                    options.Transports =
                        HttpTransportType.WebSockets |
                        HttpTransportType.LongPolling;
                });
            });
            loggerFactory.AddFile(string.Format($"{Constant.LOG_TEMPLATE}", "{Date}"));

            //app.UseSpa(spa =>
            //{
            //    spa.Options.SourcePath = "ClientApp";

            //    if (env.IsDevelopment())
            //    {
            //        spa.UseReactDevelopmentServer(npmScript: "start");
            //    }
            //});

            
        }

        private void setupSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Supervisor ASIC", Version = "v1" });
            });
        }


        private void SetupDatabaseContext(IServiceCollection services)
        {
            services.AddDbContext<MainDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("SqliteDB"));
            });
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
            services.AddSignalR(hubOptions =>
            {
                hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(15);
            });

            SetupServices(services);
            SetupRepositories(services);
            SetupUnitConfig(services);
            SetupOtherSettings(services);
            SetupGlobalStateManager(services);
        }

        private void SetupServices(IServiceCollection services)
        {
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IRecordService, RecordService>();
            services.AddScoped<IAttendeeService, AttendeeService>();
            services.AddScoped<IRealTimeService, RealTimeService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IAttendeeGroupService, AttendeeGroupService>();
            services.AddScoped<SupervisorNetworkService>();
            services.AddScoped<IRecognitionService, RecognitionService>();
            services.AddScoped<IChangeRequestService, ChangeRequestService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<SettingsUtils>();
            services.AddHttpClient();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGlobalStateService, GlobalStateService>();
            services.AddScoped<ILogService, LogService>();
        }
        private void SetupRepositories(IServiceCollection services)
        {
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IRecordRepository, RecordRepository>();
            services.AddScoped<IAttendeeRepository, AttendeeRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IAttendeeGroupRepository, AttendeeGroupRepository>();
            services.AddScoped<IChangeRequestRepository, ChangeRequestRepository>();
        }

        private void SetupBackgroundService(IServiceCollection services)
        {
            services.AddHostedService<WindowAppRunnerService>();
            services.AddHostedService<SupervisorRunnerService>();
        }

        private void SetupUnitConfig(IServiceCollection services)
        {
            UnitService unitServiceInstance = UnitServiceFactory.Create(Configuration.GetValue<string>("UnitConfigFile"));
            services.AddSingleton(unitServiceInstance);
        }

        private void SetupOtherSettings(IServiceCollection services)
        {
            OtherSettingsService otherSettingsService = OtherSettingsServiceFactory
                                .Create(Configuration.GetValue<string>("FilesConfiguration:SettingsConfigFile"));
            services.AddSingleton(otherSettingsService);
        }

        private void SetupGlobalStateManager(IServiceCollection services)
        {
            var globalState = new GlobalState();
            services.AddSingleton(globalState);
        }

        private void SetupStaticFiles(IApplicationBuilder app)
        {
            if (myConfig == null)
            {
                myConfig = Configuration.GetSection("MyConfiguration").Get<MyConfiguration>();
            }
            UseStaticFiles(app, myConfig.AvatarFolderPath, "/api/avatars");
            UseStaticFiles(app, myConfig.UnknownFolderPath, "/api/unknown");
            UseStaticFiles(app, myConfig.LogoFolderPath, "/api/logo");
        }

        private void UseStaticFiles(IApplicationBuilder app, string path, string requestPath) {
            var combinedPath = Path.Combine(Directory.GetCurrentDirectory(), path);
            Directory.CreateDirectory(combinedPath);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(combinedPath),
                RequestPath = requestPath
            });
        }
    }
}
