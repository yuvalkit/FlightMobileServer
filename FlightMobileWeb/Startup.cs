using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Rewrite;
using System.IO;
using FlightMobileApp.Models;

namespace FlightMobileApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // get all the simulator connection parameters
            string ip = Configuration.GetValue<string>("SimulatorInfo:IP");
            int telnetPort = Configuration.GetValue<int>("SimulatorInfo:TelnetPort");
            int httpPort = Configuration.GetValue<int>("SimulatorInfo:HttpPort");
            // create the 2 managers
            var commandManager = new CommandManager(ip, telnetPort);
            var screenshotManager = new ScreenshotManager(ip, httpPort);
            // make the 2 managers singletons
            services.AddSingleton(commandManager);
            services.AddSingleton(screenshotManager);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
