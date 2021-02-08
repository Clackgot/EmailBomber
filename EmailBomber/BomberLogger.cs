using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using NLog.Fluent;

namespace EmailBomber
{
    public static class BomberLogger
    {
        public static Logger GetLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }
    }
}
