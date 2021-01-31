using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace HappyIRCClientLibrary.Config
{
    public interface IConfig
    {
        ILog GetLogger(string name);
    }
}
