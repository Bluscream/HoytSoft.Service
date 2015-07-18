/*
 HoytSoft Self installing .NET service using the Win32 API
 
 
 Extended by Dror Gluska
 */

using System;
using System.Diagnostics;

namespace HoytSoft.ServiceBase.Base.Logging
{

    /// <summary>
    /// Service Logger Interface
    /// </summary>
    public interface IServiceLogger : IDisposable
    {
        void Create(string logName);
        void Delete(string logName);
        bool Exists(string logName);
        void Open(string logName, string sourceName);
        void WriteEntry(EventLogEntryType type, string message, params object[] parameters);
    }
}
