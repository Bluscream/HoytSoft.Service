/*
 HoytSoft Self installing .NET service using the Win32 API
 by Dror Gluska
 */

using HoytSoft.ServiceBase.Base.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HoytSoft.ServiceBase.Base.Logging;

namespace HoytSoft.ServiceBase.Base
{
    /// <summary>
    /// Service Container
    /// <para>Used to host a service and send events from console/windows messages</para>
    /// </summary>
    class ServiceContainer : IDisposable
    {
        private readonly ServiceBase _service;

        private ConsoleNotificationWindow<Notifications.NotificationDetail> _consoleNotificationWindow = null;
        private Notifications.Notifications _notificationRegister = null;

        private void NotificationDetailHandler(ref NotificationDetail nd)
        {
            DebugLogger.WriteLine("event " + nd.ToString());
        }

        public ServiceContainer(ServiceBase service)
        {
            _service = service;

            Console.WriteLine("Starting Service Container");

            if (_consoleNotificationWindow != null || _notificationRegister != null)
            {
                throw new ApplicationException("Unable to start twice");
            }

            _notificationRegister = new Notifications.Notifications();

            _consoleNotificationWindow = new ConsoleNotificationWindow<Notifications.NotificationDetail>();
            _consoleNotificationWindow.TransformMessage = Notifications.Notifications.Transform;
            _consoleNotificationWindow.WndProc += new ConsoleNotificationWindow<NotificationDetail>.WndProcHandler(NotificationDetailHandler);
            _consoleNotificationWindow.Start();

            _notificationRegister.RegisterNotifications(_consoleNotificationWindow.Handle, Notifications.Notifications.NotifyType.Window);
        }

        public void Start()
        {
            while (true)
            {
                if (!ProcessCommand())
                {
                    break;
                }
                Thread.Sleep(100);

            }
        }

        void DisplayMenu()
        {
            Console.WriteLine("Select Action:");
            Console.WriteLine("1 - Start");
            Console.WriteLine("2 - Stop");
            Console.WriteLine("3 - Pause");
            Console.WriteLine("4 - Continue");
            Console.WriteLine("5 - Interrogate");
            Console.WriteLine("6 - Shutdown");
            Console.WriteLine("7 - Request Additional Time (ms)");
            Console.WriteLine("8 - SessionChange (SessionChangeDescription...)");
            Console.WriteLine("9 - PowerEvent (PowerBroadcastStatus...)");
            Console.WriteLine("0 - Custom Command");
            Console.WriteLine("l - Force Service Logger to Console");
            Console.WriteLine("State {0}",_service.ServiceState.ToString());
        }


        int? ParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace( value))
            {
                return null;
            }

            int retval;
            if (int.TryParse(value, out retval))
                return retval;

            return null;
        }

        int ReadInt()
        {
            int? value = null;

            while (value == null){
            Console.Write("Value: ");
            value = ParseInt(Console.ReadLine());
            }

            return value.Value;
        }

        bool ProcessCommand()
        {
            if (!Console.KeyAvailable) return true;

            var keyinfo = Console.ReadKey();
            var key = keyinfo.KeyChar;
            if (keyinfo.Key == ConsoleKey.Escape)
            {
                return false;
            }
                
            switch (key)
            {
                case '0':
                    //MIGHT BE A MISTAKE!!! PARAMCHANGE <> Custom command
                    //_service.ChangeServiceState(ServiceProcess.ServicesAPI.ServiceControlType.SERVICE_CONTROL_PARAMCHANGE, ReadInt());
                        
                    break;
                case '1':
                    _service.DebugStart();
                    break;
                case '2':
                    _service.ChangeServiceState(ServicesAPI.ServiceControlType.SERVICE_CONTROL_STOP);
                    break;
                case '3':
                    _service.ChangeServiceState(ServicesAPI.ServiceControlType.SERVICE_CONTROL_PAUSE);
                    break;
                case '4':
                    _service.ChangeServiceState(ServicesAPI.ServiceControlType.SERVICE_CONTROL_CONTINUE);
                    break;
                case '5':
                    _service.ChangeServiceState(ServicesAPI.ServiceControlType.SERVICE_CONTROL_INTERROGATE);
                    break;
                case '6':
                    _service.ChangeServiceState(ServicesAPI.ServiceControlType.SERVICE_CONTROL_SHUTDOWN);
                    break;
                case '7':
                    //wrong thing to test, the request should tell me to wait before failing to stop...
                    //_service.RequestAdditionalTime(ReadInt());
                    break;
                case '8':
                    _service.ChangeServiceState(ServicesAPI.ServiceControlType.SERVICE_CONTROL_SESSIONCHANGE);
                    break;
                case '9':
                    _service.ChangeServiceState(ServicesAPI.ServiceControlType.SERVICE_CONTROL_POWEREVENT);
                    break;
                case 'l':
                    _service.IServiceLogger = new ConsoleLogger();
                    break;
                case '\r':
                    DisplayMenu();
                    break;
            }

            return true;
        }




        #region IDisposable Members

        public void Dispose()
        {
            _notificationRegister.UnRegisterNotifications(_consoleNotificationWindow.Handle);
            _consoleNotificationWindow.Stop();

            _notificationRegister = null;
            _consoleNotificationWindow = null;
        }

        #endregion
    }
}
