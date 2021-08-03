using Azure.Storage.Table.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace App
{
    class Program
    {
        public static IConfiguration Configuration { get; private set; }

        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                        .ConfigureServices(ConfigureServices)
                        .RunConsoleAsync();
        }
         
        private static void ConfigureServices(IServiceCollection services)
        {
            Configuration = new ConfigurationBuilder()
                                       .AddJsonFile("appsettings.json", true, true)
                                           .Build();

            services.AddSingleton(new RepositorySettings(Configuration["AzureWebJobsStorage"]));
            services.AddTransient<ConfigRepository>();
            services.AddHostedService<AppHostService>();
        }
    }

    internal class AppHostService : IHostedService
    {
        private readonly ConfigRepository _configRepository;

        public AppHostService(ConfigRepository configRepository)
        {
            this._configRepository = configRepository;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var entity = new ConfigEntity();
            entity.Name = "teste entity";
            _configRepository.Insert(entity);

            var lst = _configRepository.GetAll();

            foreach (var item in lst)
                Console.WriteLine(item.Name);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {


            return Task.CompletedTask;
        }
    }
}
