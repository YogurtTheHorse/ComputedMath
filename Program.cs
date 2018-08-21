using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ComputedMath {
    public static class Program {
        private static void Main(string[] args) {
            IWebHost webHost = new WebHostBuilder()
                .UseUrls(args.Length > 0 ? args[0] : "http://*:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            webHost.Run();
        }
    }
}