using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using blockPlayDataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using oneBlockWeb.DI;
using Newtonsoft.Json;
using System;

namespace oneBlockWeb {
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("webSetting.json", optional: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)//从 appsettings.json 读取配置参数 Configuration.Get("Data:DefaultConnection:ConnectionString");
                //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true) //增加环境配置文件，新建项目默认有
                .AddEnvironmentVariables();//从环境变量读取配置?

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                //调试模式修改 JS 和CSS后立刻生效?
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();


        }


        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //权限相关 目前去掉似乎也没问题
            //services.AddAuthorization();

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));
            //});

            //数据库配置
            //var connection = "Filename=App_Data/blockPlayDB.db"; //正式发布
            var connection = "Filename=../../../App_Data/blockPlayDB.db"; //debug
            services.AddDbContext<blockPlayDBContext>(options => options.UseSqlite(connection));

            //注入用户相关操作对象
            services.AddSingleton<userLV>();

            //自定义配置文件
            services.AddOptions();
            services.Configure<WebSetting>(Configuration.GetSection("WebSetting"));


            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);


            //应该可以和下面的合并
            Newtonsoft.Json.JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings();
            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                setting.DateFormatString = "yyyy-MM-dd hh:mm:ss";//时间格式化
                setting.NullValueHandling = NullValueHandling.Ignore;//忽略null值
                return setting;
            });

            services
                .AddMvc()
                .AddJsonOptions(r => {//设置json相关参数
                    r.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();//设置默认规则,首字母不强制小写
                    r.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;//忽略null值
                    r.SerializerSettings.DateFormatString = "yyyy-MM-dd hh:mm:ss";//时间格式化
                });

           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var defaultcon = Configuration.GetConnectionString("DefaultConnection");

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            //自定义中间件
            app.UseMiddleware<RequestFilter>();
            
            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();
            
            //权限相关
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                //CookieHttpOnly = true,
                //ExpireTimeSpan = TimeSpan.FromDays(1),
                AuthenticationScheme = "mcookie",
                LoginPath = new PathString("/Account/Login/"),
                AccessDeniedPath = new PathString("/Account/Forbidden/"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                //CookieName = ".mcookie",
                //CookiePath = "/",
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { action = "Index" });
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }


}
