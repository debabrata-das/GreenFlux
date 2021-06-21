using System;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Core;

namespace GreenFlux.SmartCharging.Api
{
    public interface ILoggerManager
    {
        void Info(string message);

        void Debug(string message);

        void Error(string message);
    }

    public class LoggerManager : ILoggerManager
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(LoggerManager));

        public LoggerManager()
        {
            try
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                using (var fs = File.OpenRead(Path.Combine(path, "log4net.config")))
                {
                    XmlDocument log4NetConfig = new XmlDocument();
                    log4NetConfig.Load(fs);
                    var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
                    XmlConfigurator.Configure(repo, log4NetConfig["log4net"]);
                    _logger.Info("Log System Initialized");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error", ex);
            }
        }
        public void Info(string message)
        {
            if (_logger.IsInfoEnabled)
            {
                Log(Level.Info, message);
            }
        }

        public void Debug(string message)
        {
            if (_logger.IsDebugEnabled)
            {
                Log(Level.Debug, message);
            }
        }

        public void Error(string message)
        {
            if (_logger.IsErrorEnabled)
            {
                Log(Level.Error, message);
            }
        }

        public bool IsEnabled(Level logLevel)
        {
            if (logLevel == Level.Debug)
            {   return _logger.IsDebugEnabled;}

            if (logLevel == Level.Error)
            {   return _logger.IsErrorEnabled;}

            if (logLevel == Level.Info)
            {   return _logger.IsInfoEnabled;}

            throw new ArgumentOutOfRangeException(nameof(logLevel));
        }

        public void Log(Level logLevel, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (logLevel == Level.Debug)
            {
                _logger.Debug(message);
            }
            else if (logLevel == Level.Error)
            {
                _logger.Error(message);
            }
            else if (logLevel == Level.Info)
            {
                _logger.Info(message);
            }
            else
            {
                _logger.Info($"Encountered unknown log level {logLevel}, writing out as Info.");
                _logger.Info(message);
            }
        }
    }
}