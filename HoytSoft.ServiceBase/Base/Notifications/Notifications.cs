/*
 HoytSoft Self installing .NET service using the Win32 API
 by Dror Gluska
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoytSoft.ServiceBase.Base.Notifications
{
    /// <summary>
    /// Notification Event Details
    /// </summary>
    public class NotificationDetail
    {
        public DeviceNotification.DeviceEventInfo Device;
        public PowerNotification.PowerEventInfo Power;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Device != null)
                sb.Append(Device.ToString());
            if (Power != null)
                sb.Append(Power.ToString());

            return sb.ToString();
        }
    }

    /// <summary>
    /// Notifications Controller
    /// </summary>
    public class Notifications
    {
        /// <summary>
        /// This makes sure only one Notifications object can be created
        /// </summary>
        private static readonly SemaphoreSlim _instanceCounter = new SemaphoreSlim(1, 1);

        /// <summary>
        /// ignore standard window messages (like WM_MOVE etc')
        /// </summary>
        private static readonly int[] _ignoreMessages = new[] {0x7d,0xd, 0x46, 0x24, 0x81, 0x1, 0x80, 0xe, 0x7c, 
            0x47, 0x86, 0x83, 0x7f, 0x18, 0x1c, 0x281, 0x7, 0x282, 0x6, 0x85, 0x14, 0x5, 0x30, 0x31f, 0x8, 0x348, 0xc0f2 ,0x3,0xc348};


        public Notifications()
        {
            if (!_instanceCounter.Wait(1))
                throw new ApplicationException("Notifications class can only be instantiated once!");
        }

        ~Notifications()
        {
            _instanceCounter.Release();
        }

        public enum NotifyType
        {
            Window,
            Service
        }


        private static readonly HashSet<IntPtr> _registeredDeviceNotifications = new HashSet<IntPtr>();
        private static readonly HashSet<IntPtr> _registeredPowerNotifications = new HashSet<IntPtr>();

        /// <summary>
        /// Transforms a Message to NotificationDetail
        /// </summary>
        public static NotificationDetail Transform(ref Message m)
        {
            NotificationDetail nd = new NotificationDetail();
            
            if (_ignoreMessages.Contains(m.Msg)) return null;

            nd.Power = PowerNotification.TransformMessage(ref m);
            nd.Device = DeviceNotification.TransformMessage(ref m);
            if ((nd.Power != null) || (nd.Device != null))
                return nd;

            return null;
        }

        /// <summary>
        /// Registers a Device Notification 
        /// </summary>
        private void RegisterDeviceNotification(IntPtr handle, NotifyType notifyType, Guid device)
        {
            switch (notifyType)
            {
                case NotifyType.Window:
                    _registeredDeviceNotifications.Add(DeviceNotification.RegisterDeviceInterface(handle, DeviceNotification.DeviceNotifyType.DEVICE_NOTIFY_WINDOW_HANDLE | DeviceNotification.DeviceNotifyType.DEVICE_NOTIFY_ALL_INTERFACE_CLASSES, device));
                    break;
                case NotifyType.Service:
                    _registeredDeviceNotifications.Add(DeviceNotification.RegisterDeviceInterface(handle, DeviceNotification.DeviceNotifyType.DEVICE_NOTIFY_SERVICE_HANDLE, device));
                    break;
            }
        }

        /// <summary>
        /// Registers Power Notification 
        /// </summary>
        private void RegisterPowerNotification(IntPtr handle, NotifyType notifyType, Guid powerSettings)
        {
            switch (notifyType)
            {
                case NotifyType.Window:
                    _registeredPowerNotifications.Add(PowerNotification.RegisterPowerSettingNotification(handle,ref  powerSettings, PowerNotification.DeviceNotifyType.DEVICE_NOTIFY_WINDOW_HANDLE));
                    break;
                case NotifyType.Service:
                    _registeredPowerNotifications.Add(PowerNotification.RegisterPowerSettingNotification(handle, ref powerSettings, PowerNotification.DeviceNotifyType.DEVICE_NOTIFY_SERVICE_HANDLE));
                    break;
            }
        }

        /// <summary>
        /// Registers a Preset of Device and Power Notifications to match the default service notifications
        /// </summary>
        /// <param name="handle">Window handle</param>
        /// <param name="notifyType">Window/Service</param>
        public void RegisterNotifications(IntPtr handle, NotifyType notifyType)
        {
            //device notifications
            RegisterDeviceNotification(handle,notifyType,DeviceNotification.IOEvents.GUID_DEVCLASS_USB);
            RegisterDeviceNotification(handle,notifyType,DeviceNotification.IOEvents.GUID_DEVCLASS_VOLUME);
            RegisterDeviceNotification(handle,notifyType,DeviceNotification.IOEvents.GUID_IO_VOLUME_MOUNT);
            RegisterDeviceNotification(handle,notifyType,DeviceNotification.IOEvents.GUID_DEVCLASS_WCEUSBS);
            RegisterDeviceNotification(handle,notifyType,DeviceNotification.IOEvents.GUID_DEVINTERFACE_DISK);
            RegisterDeviceNotification(handle,notifyType,DeviceNotification.IOEvents.GUID_DEVINTERFACE_USB_DEVICE);

            //power notifications

            RegisterPowerNotification(handle, notifyType, PowerNotification.PowerSettings.GUID_ACDC_POWER_SOURCE);
            RegisterPowerNotification(handle, notifyType, PowerNotification.PowerSettings.GUID_ACTIVE_POWERSCHEME);
            RegisterPowerNotification(handle, notifyType, PowerNotification.PowerSettings.GUID_BATTERY_PERCENTAGE_REMAINING);
            RegisterPowerNotification(handle, notifyType, PowerNotification.PowerSettings.GUID_CONSOLE_DISPLAY_STATE);
            RegisterPowerNotification(handle, notifyType, PowerNotification.PowerSettings.GUID_GLOBAL_USER_PRESENCE);
            RegisterPowerNotification(handle, notifyType, PowerNotification.PowerSettings.GUID_IDLE_BACKGROUND_TASK);
            RegisterPowerNotification(handle, notifyType, PowerNotification.PowerSettings.GUID_LIDSWITCH_STATE_CHANGE);
            RegisterPowerNotification(handle, notifyType, PowerNotification.PowerSettings.GUID_MONITOR_POWER_ON);
            RegisterPowerNotification(handle, notifyType, PowerNotification.PowerSettings.GUID_POWERSCHEME_PERSONALITY);
            RegisterPowerNotification(handle, notifyType, PowerNotification.PowerSettings.GUID_SESSION_DISPLAY_STATUS);
            RegisterPowerNotification(handle, notifyType, PowerNotification.PowerSettings.GUID_SESSION_USER_PRESENCE);
            RegisterPowerNotification(handle, notifyType, PowerNotification.PowerSettings.GUID_SYSTEM_AWAYMODE);

            SessionNotification.WTSRegisterSessionNotificationEx(SessionNotification.WTS_CURRENT_SERVER, handle, SessionNotification.SessionTarget.NOTIFY_FOR_ALL_SESSIONS);

        }

        /// <summary>
        /// Unregisters all Device and Power Notifications previously registered with RegisterPowerNotification and RegisterDeviceNotification
        /// </summary>
        /// <param name="handle">Window handle</param>
        public void UnRegisterNotifications(IntPtr handle)
        {
            foreach (var item in _registeredDeviceNotifications)
            {
                DeviceNotification.UnregisterDeviceNotification(item);
            }

            foreach (var item in _registeredPowerNotifications)
            {
                PowerNotification.UnregisterPowerSettingNotification(item);
            }

            SessionNotification.WTSUnRegisterSessionNotificationEx(SessionNotification.WTS_CURRENT_SERVER, handle);
        }
    }
}
