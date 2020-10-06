using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleApp.Services;

namespace SampleApp
{
   
    public class Program
    {
        public static object _sa;
        public static IHost _host;
        public static IHostedService _hostedServicesA;
        public static async Task Main(string[] args)
        {
            //await CreateHostBuilder(args).Build().RunAsync();

             _host = CreateHostBuilder(args).Build();

            _sa = _host.Services.GetService(typeof(ServiceA));
            _hostedServicesA =(ServiceA) _host.Services.GetService<IEnumerable<IHostedService>>().ToList()[0];

            await _host.RunAsync();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ServiceA>();
                    services.AddHostedService<ServiceB>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
