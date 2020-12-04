using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.DataAccess;
using Chat.SignalRChat.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Chat.Business;

namespace Chat
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
            services.AddLogging();
            
            services.AddSession();
            services.AddControllersWithViews()
            //.AddNewtonsoftJson(option => option.SerializerSettings.ContractResolver = new DefaultContractResolver());// mvc NewtonsoftJson中设置json序列化 PascalCase格式
            .AddJsonOptions(option => option.JsonSerializerOptions.PropertyNamingPolicy = null); // mvc System.Text.Json中设置json序列化 PascalCase格式

            services.AddSignalR(option =>
            {
                option.KeepAliveInterval = TimeSpan.FromMinutes(5);//默认15秒
                option.ClientTimeoutInterval = TimeSpan.FromMinutes(2);//默认30秒
                option.MaximumReceiveMessageSize = 32 * 100;//默认32kb
                option.EnableDetailedErrors = true;
            })
            //.AddNewtonsoftJsonProtocol(option => option.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver());//signalr NewtonsoftJson json序列化 PascalCase格式
            .AddJsonProtocol(option => option.PayloadSerializerOptions.PropertyNamingPolicy = null); // signalr System.Text.Json中设置json序列化 PascalCase格式

            services.AddDbContext<MyDbContext>(option =>
            {
                option.UseInMemoryDatabase("ChatTest");
            });

            services.AddTransient<DataBusiness>();
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
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                //endpoints.MapHub<ChatHub>("ChatHub");
                endpoints.MapHub<ChatHubT>("ChatHubT");
            });
        }
    }
}
