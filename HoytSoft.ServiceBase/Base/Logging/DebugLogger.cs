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
    /// Used for Debugging
    /// Will log to Trace.WriteLine if in Service Mode or to Console.WriteLine if in Console Mode
    /// Will be completely Disabled in Release
    /// </summary>
    class DebugLogger
    {
        public static bool IsConsole = true;

        public static void WriteLine(Exception ex)
        {
            WriteLine(ex.ToString());
        }

        public static void WriteLine(string msg, params object[] arg)
        {
#if DEBUG
            if (IsConsole)
            {
                Console.WriteLine(msg,arg);
            }
            else
            {
                Trace.WriteLine(string.Format(msg,arg));
            }
#endif
        }
    }
}
