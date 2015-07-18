/*
 HoytSoft Self installing .NET service using the Win32 API
 David Hoyt CPOL 2005
 
 Extended by Dror Gluska
 */

using HoytSoft.ServiceBase.Base.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoytSoft.ServiceBase.Base
{
    public abstract class IService
    {
        protected string[] args;

        protected abstract bool Install();
        protected abstract bool Uninstall();
        protected abstract bool Initialize(string[] arguments);
        protected abstract void Start(string[] args);
        protected abstract void Pause();
        protected abstract void Stop();
        protected abstract void Continue();
        protected abstract void Shutdown();
        protected abstract void PreShutdown();
        protected abstract void Interrogate();
        protected abstract void DeviceEvent(DeviceNotification.DeviceEventInfo e);
        protected abstract void HardwareProfileChange(ServicesAPI.SERVICE_CONTROL_HARDWAREPROFILECHANGE_Control dwControl);
        //protected abstract  void RequestAdditionalTime(int milliseconds);
        protected abstract void SessionChange(ServicesAPI.SERVICE_CONTROL_SESSIONCHANGE_Control dwControl, SessionNotification.SessionEventData e);
        protected abstract void PowerEvent(PowerNotification.PowerEventInfo e);
        protected abstract void TimeChange(DateTime oldtime, DateTime newtime);
        protected abstract void NetBind(ServicesAPI.NetBindControl control);
        //could be a mistake
        protected abstract void CustomCommand(int command);

        #region Service Commands, used in ServiceContainer

        internal enum Command
        {
            Start,
            Pause,
            Stop,
            Continue,
            Shutdown,
            PreShutdown,
            Interrogate
        }

        internal void ServiceSendCustomCommand(int command)
        {
            CustomCommand(command);
        }

        internal void ServiceSendNetBind(ServicesAPI.NetBindControl control)
        {
            NetBind(control);
        }

        internal void ServiceSendTimeChange(DateTime oldtime, DateTime newtime)
        {
            TimeChange(oldtime,newtime);
        }

        internal void ServiceSendPowerEvent(PowerNotification.PowerEventInfo e)
        {
            PowerEvent(e);
        }

        internal void ServiceSendSessionChange(ServicesAPI.SERVICE_CONTROL_SESSIONCHANGE_Control dwControl, SessionNotification.SessionEventData e)
        {
            SessionChange(dwControl, e);
        }

        internal void ServiceSendDeviceEvent(DeviceNotification.DeviceEventInfo e)
        {
            this.DeviceEvent(e);
        }

        internal void ServiceSendHardwareProfileChange(ServicesAPI.SERVICE_CONTROL_HARDWAREPROFILECHANGE_Control dwControl)
        {
            HardwareProfileChange(dwControl);
        }

        internal void ServiceSendCommand(Command _command)
        {
            switch (_command)
            {
                case Command.Start:
                    this.Start(this.args);
                    break;
                case Command.Stop:
                    this.Stop();
                    break;
                case Command.Pause:
                    this.Pause();
                    break;
                case Command.Continue:
                    this.Continue();
                    break;
                case Command.Shutdown:
                    this.Shutdown();
                    break;
                case Command.PreShutdown:
                    this.PreShutdown();
                    break;
                case Command.Interrogate:
                    this.Interrogate();
                    break;
            }
        }

        #endregion

    }
}
