/*
 HoytSoft Self installing .NET service using the Win32 API
 David Hoyt CPOL 2005
 
 Extended by Dror Gluska
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoytSoft.ServiceBase.Base.Logging
{
    /// <summary>
    /// Console Logger
    /// <para>Logs all messages to console, use for debugging purposes</para>
    /// </summary>
    class ConsoleLogger : IServiceLogger
    {
        private string _logname;
        public void Create(string logName)
        {
            Console.WriteLine("Creating Logger {0}", logName);
        }

        public void Delete(string logName)
        {
            Console.WriteLine("Deleting Logger {0}", logName);
        }

        public bool Exists(string logName)
        {
            Console.WriteLine("Logger {0} Exists", logName);
            return true;
        }

        public void Open(string logName, string sourceName)
        {
            _logname = logName;
            Console.WriteLine("Opening Logger {0} Source {1}", logName,sourceName);
        }

        public void WriteEntry(System.Diagnostics.EventLogEntryType type, string message, params object[] parameters)
        {
            Console.WriteLine("[{0}] - {1} {2}", _logname, type, string.Format(message, parameters));
        }

        public void Dispose()
        {
        }
    }
}
