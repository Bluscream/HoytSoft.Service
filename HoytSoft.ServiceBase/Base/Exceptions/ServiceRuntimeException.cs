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
    public class ServiceRuntimeException : ServiceException
    {
        public ServiceRuntimeException(string Message) : base(Message) { }
        public ServiceRuntimeException(string Message, Exception InnerException) : base(Message, InnerException) { }
    }
}
