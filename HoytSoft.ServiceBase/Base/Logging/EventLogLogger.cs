/*
 HoytSoft Self installing .NET service using the Win32 API
 
 
 Extended by Dror Gluska
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoytSoft.ServiceBase.Base.Logging
{
    /// <summary>
    /// EventLog Logger
    /// </summary>
    public class EventLogLogger : IServiceLogger
    {
        private EventLog _log;

        public void Open(string logName, string sourceName)
        {
            _log = new EventLog(logName);
            _log.Source = sourceName;
        }

        public void Create(string logName)
        {
            //TODO: implement
            //EventLog.CreateEventSource(new EventSourceCreationData{ LogName})
        }

        public void Delete(string logName)
        {
            //TODO: implement, but need to see what to do about system logs (System, Application etc')
            //EventLog.Delete(logName);
        }

        public bool Exists(string logName)
        {
            return EventLog.Exists(logName);
        }

        public void WriteEntry(EventLogEntryType type, string message, params object[] parameters)
        {
            
            _log.WriteEntry(string.Format(message, parameters), type);
        }

        public void Dispose()
        {
            if (_log != null)
            {
                _log.Dispose();
                _log = null;
            }
        }

    }
}
