using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Taski.Scheduler.Core;

namespace Taski.Scheduler
{
    public class Scheduler
    {
        //use Cron Job format https://cloud.google.com/scheduler/docs/configuring/cron-job-schedules

        private readonly List<Plugin> plugins = new List<Plugin>();
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        public Scheduler(ILogger logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public void LoadPlugins()
        {
            var pluginsPath = configuration["Plugins:RootFolder"];
            string[] directories = Directory.GetDirectories(pluginsPath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var dir in directories)
            {
                var dllName = new DirectoryInfo(dir).Name;
                string[] files = Directory.GetFiles(dir, $"{dllName}.dll", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                    LoadAssembly(file);
            }
        }

        public async Task Run()
        {
            List<Task> executionTasks = new List<Task>();

            foreach (var plugin in plugins)
            {
                foreach (var type in plugin.ClassTypes)
                {
                    //TODO check schedule before calling ExecuteAsync
                    executionTasks.Add(ExecuteAsync(plugin, type));
                }
            }

            //TODO better to use multi-thread? do we need to await for all completed?
            await Task.WhenAll(executionTasks);
        }

        private void LoadAssembly(string assemblyName)
        {
            var pluginFolderPath = Path.GetDirectoryName(assemblyName);
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(pluginFolderPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var pluginConfiguration = configBuilder.Build();

            //TODO do we need to unload assemblies after getting all classes?
            Assembly assembly = Assembly.LoadFrom(assemblyName);
            var pluginTypes = assembly.GetTypes().Where(t => typeof(IDynamicTask).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract).ToList();
            if (pluginTypes.Count > 0)
            {
                Plugin plugin = new Plugin(assemblyName, pluginTypes.Select(t => t).ToList(), pluginConfiguration);
                plugins.Add(plugin);

                var strFramework = "N/A";
                var targetFremworkAttr = assembly.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(TargetFrameworkAttribute));
                if (targetFremworkAttr != null)
                    strFramework = (string)targetFremworkAttr.ConstructorArguments[0].Value;

                logger.LogInformation($"Assembly {assemblyName} - Version: {strFramework} loaded.");
                foreach (var type in plugin.ClassTypes)
                    logger.LogInformation($"\t{type.Name} found.");
            }
            else
                logger.LogInformation($"No DynamicTask found in assembly {assemblyName}");
        }

        private async Task ExecuteAsync(Plugin plugin, Type classType)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(plugin.AssemblyName);
                var taskType = assembly.GetTypes().FirstOrDefault(t => t.Name == classType.Name);
                if (taskType == null)
                {
                    logger.LogError($"Class {classType.Name} not found in assembly {plugin.AssemblyName}");
                    return;
                }

                var task = (IDynamicTask)Activator.CreateInstance(taskType);
                if (task != null && task.Status == DynamicTaskStatus.Ready)
                {
                    logger.LogInformation($"Executing task {classType.Name} from assembly {plugin.AssemblyName}...");
                    //TODO create cancellationtoken
                    await task.ExecuteAsync(logger, plugin.Configuration, CancellationToken.None);
                }
                else
                {
                    logger.LogInformation($"Task {classType.Name} from assembly {plugin.AssemblyName} is not ready to execute.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error executing task {classType.Name} from assembly {plugin.AssemblyName}: {ex.Message}");
            }
        }
    }
}