using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace HappyIRCClientLibrary.Config
{
    public class Config : IConfig
    {
        private ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());

        public Config()
        {
            var configFile = new FileInfo("log4net.config");
            XmlConfigurator.Configure(logRepository, configFile);
        }

        public ILog GetLogger(string name)
        {
            return LogManager.GetLogger(name);
        }
    }
}
