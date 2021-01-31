using HappyIRCClientLibrary.Config;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyIRCConsoleClient
{
    public class ContainerBuilder
    {
        public IServiceProvider Build()
        {
            IConfig config = new Config();

            var container = new ServiceCollection();

            container.AddSingleton(_ => config)
                .AddSingleton(_ => container);

            return container.BuildServiceProvider();
        }
    }
}
