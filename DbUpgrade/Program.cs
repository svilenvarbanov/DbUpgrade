using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using DbUpgrade.Abstractions;
using DbUpgrade.EF;
using DbUpgrade.Helpers;
using DbUpgrade.Models;
using DbUpgrade.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DbUpgrade
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Argument number incorrect. Pass the module to be updated.");

                return;
            }
            var moduleParamCorect = Enum.TryParse(args[0], true, out ModuleName moduleName);
            if (!moduleParamCorect)
            {
                Console.WriteLine("Module name '{0}' incorrect. Selecting payments module by default.", args[0]);
            }
            // Setup Host
            var host = CreateDefaultBuilder(moduleParamCorect ? moduleName : ModuleName.Pay).Build();

            // Invoke DatabaseUpgradeService
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            var workerInstance = provider.GetRequiredService<IDatabaseUpgradeService>();
            workerInstance.UpgradeDatabase();

            host.Run();
        }

        static IHostBuilder CreateDefaultBuilder(ModuleName moduleName)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(app =>
                {
                    app.AddJsonFile("appsettings.json").AddEnvironmentVariables();
                })
                .ConfigureServices((ctx, services) =>
                {
                    services.AddSingleton<IDatabaseUpgradeService, DatabaseUpgradeService>();
                    services.AddSingleton(ctx.Configuration.GetSection("Services")?.Get<List<DbUpConfiguration>>()?.SingleOrDefault(dbc => dbc.Name.ToLower() == moduleName.ToString().ToLower()) ?? throw new ConfigurationErrorsException($"Invalid configuration for module {moduleName}"));
                    services.AddLogging(configure => configure.AddSystemdConsole());
                    //services.AddDbContext<DbUpVersionContext>(options => options.UseSqlServer("name=Services:0:ConnectionString"));
                    services.AddDbContext<DbUpVersionContext>(options => options.UseSqlServer("name=VersioningDatabaseConnectionString"));
                    services.AddTransient<IGitRepoService, GitRepoService>();
                });

        }
    }
}
