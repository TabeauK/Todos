using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Data.Entity;

namespace Todos
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using TaskContext ctx = new();
            //UpdateAll(null, null);
            //var myTimer = new Timer(60 * 60 * 1000);
            //myTimer.Elapsed += UpdateAll;
            //myTimer.Start();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        //public static void UpdateAll(object e, ElapsedEventArgs args)
        //{
        //    using TaskContext ctx = new();
        //    foreach (var task in ctx.Tasks.Include(i => i.User).Include(i => i.Checks)) //todo zdefiniowaæ warunek na usera
        //        task.Validate();
        //    ctx.SaveChanges();
        //}
    }
}
