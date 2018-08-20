using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ComputedMath {
    public static class Program {
        private static void Main(string[] args) {
            IWebHost webHost = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            webHost.Run();
        }
    }
}