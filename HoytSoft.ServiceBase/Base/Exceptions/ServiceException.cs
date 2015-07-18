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

namespace HoytSoft.ServiceBase.Base.Exceptions
{
    [Serializable]
    public class ServiceException : Exception
    {
        public ServiceException(string Message) : base(Message) { }
        public ServiceException(string Message, Exception InnerException) : base(Message, InnerException) { }
    }
}
