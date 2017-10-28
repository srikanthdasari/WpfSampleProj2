using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace WpfSampleProj2.Services
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var logger = LogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
