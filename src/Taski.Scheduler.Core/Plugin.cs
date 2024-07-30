using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Taski.Scheduler.Core
{
    public class Plugin
    {
        private string assemblyName;
        private List<Type> classTypes;
        private IConfiguration configuration;

        public Plugin(string assemblyName, List<Type> classTypes, IConfiguration configuration)
        {
            this.assemblyName = assemblyName;
            this.classTypes = classTypes;
            this.configuration = configuration;
        }

        public string AssemblyName => assemblyName;
        public List<Type> ClassTypes => classTypes;
        public IConfiguration Configuration => configuration;
    }
}