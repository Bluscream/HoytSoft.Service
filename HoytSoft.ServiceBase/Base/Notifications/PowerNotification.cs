/*
 HoytSoft Self installing .NET service using the Win32 API
 by Dror Gluska
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HoytSoft.ServiceBase.Base.Logging;

namespace HoytSoft.ServiceBase.Base.Notifications
{
    /// <summary>
    /// Power State Notification 
    /// </summary>
    public class PowerNotification
    {
        /// <summary>
        /// Power Setting
        /// </summary>
        public struct PowerSettings
        {
            /// <summary>
            /// The system power source has changed. The Data member is a DWORD with values from the SYSTEM_POWER_CONDITION enumeration that indicates the current power source.
            /// <para>PoAc (0) - The computer is powered by an AC power source (or similar, such as a laptop powered by a 12V automotive adapter).</para>
            /// <para>PoDc (1) - The computer is powered by an onboard battery power source.</para>
            /// <para>PoHot (2) - The computer is powered by a short-term power source such as a UPS device.</para>
            /// </summary>
            public static readonly Guid GUID_ACDC_POWER_SOURCE = Guid.Parse("5d3e9a59-e9D5-4b00-a6bd-ff34ff516548");

            /// <summary>
            /// The remaining battery capacity has changed. The granularity varies from system to system but the finest granularity is 1 percent. The Data member is a DWORD that indicates the current battery capacity remaining as a percentage from 0 through 100.
            /// </summary>
            public static readonly Guid GUID_BATTERY_PERCENTAGE_REMAINING = Guid.Parse("a7ad8041-b45a-4cae-87a3-eecbb468a9e1");

            /// <summary>
            /// The current monitor's display state has changed.
            /// <para>Windows 7, Windows Server 2008 R2, Windows Vista, and Windows Server 2008:  This notification is available starting with Windows 8 and Windows Server 2012.</para>
            /// <para>The Data member is a DWORD with one of the following values.</para>
            /// <para>0x0 - The display is off.</para>
            /// <para>0x1 - The display is on.</para>
            /// <para>0x2 - The display is dimmed.</para>
            /// </summary>
            public static readonly Guid GUID_CONSOLE_DISPLAY_STATE = Guid.Parse("6fe69556-704a-47a0-8f24-c28d936fda47");

            /// <summary>
            /// The user status associated with any session has changed. This represents the combined status of user presence across all local and remote sessions on the system.
            /// <para>This notification is sent only services and other programs running in session 0. User-mode applications should register for GUID_SESSION_USER_PRESENCE instead.</para>
            /// <para>Windows 7, Windows Server 2008 R2, Windows Vista, and Windows Server 2008:  This notification is available starting with Windows 8 and Windows Server 2012.</para>
            /// <para>The Data member is a DWORD with one of the following values.</para>
            /// <para>PowerUserPresent (0) - The user is present in any local or remote session on the system.</para>
            /// <para>PowerUserInactive (2) - The user is not present in any local or remote session on the system.</para>
            /// </summary>
            public static readonly Guid GUID_GLOBAL_USER_PRESENCE = Guid.Parse("786E8A1D-B427-4344-9207-09E70BDCBEA9");

            /// <summary>
            /// The system is busy. This indicates that the system will not be moving into an idle state in the near future and that the current time is a good time for components to perform background or idle tasks that would otherwise prevent the computer from entering an idle state.
            /// <para>There is no notification when the system is able to move into an idle state. The idle background task notification does not indicate whether a user is present at the computer. The Data member has no information and can be ignored.</para>
            /// </summary>
            public static readonly Guid GUID_IDLE_BACKGROUND_TASK = Guid.Parse("515c31d8-f734-163d-a0fd-11a08c91e8f1");

            /// <summary>
            /// The primary system monitor has been powered on or off. This notification is useful for components that actively render content to the display device, such as media visualization. These applications should register for this notification and stop rendering graphics content when the monitor is off to reduce system power consumption. The Data member is a DWORD that indicates the current monitor state.
            /// <para>0x0 - The monitor is off.</para>
            /// <para>0x1 - The monitor is on.</para>
            /// <para>Windows 8 and Windows Server 2012:  New applications should use GUID_CONSOLE_DISPLAY_STATE instead of this notification.</para>
            /// </summary>
            public static readonly Guid GUID_MONITOR_POWER_ON = Guid.Parse("02731015-4510-4526-99e6-e5a17ebd1aea");

            /// <summary>
            /// The active power scheme personality has changed. All power schemes map to one of these personalities. The Data member is a GUID that indicates the new active power scheme personality.
            /// <para>GUID_MIN_POWER_SAVINGS (8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c)</para>
            /// <para>High Performance - The scheme is designed to deliver maximum performance at the expense of power consumption savings.</para>
            /// <para>GUID_MAX_POWER_SAVINGS (a1841308-3541-4fab-bc81-f71556f20b4a)</para>
            /// <para>Power Saver - The scheme is designed to deliver maximum power consumption savings at the expense of system performance and responsiveness.</para>
            /// <para>GUID_TYPICAL_POWER_SAVINGS (381b4222-f694-41f0-9685-ff5bb260df2e)</para>
            /// <para>Automatic - The scheme is designed to automatically balance performance and power consumption savings.</para>
            /// </summary>
            public static readonly Guid GUID_POWERSCHEME_PERSONALITY = Guid.Parse("245d8541-3943-4422-b025-13A784F679B7");

            /// <summary>
            /// The display associated with the application's session has been powered on or off.
            /// <para>Windows 7, Windows Server 2008 R2, Windows Vista, and Windows Server 2008:  This notification is available starting with Windows 8 and Windows Server 2012.</para>
            /// <para>This notification is sent only to user-mode applications. Services and other programs running in session 0 do not receive this notification. The Data member is a DWORD with one of the following values.</para>
            /// <para>0x0 - The display is off.</para>
            /// <para>0x1 - The display is on.</para>
            /// <para>0x2 - The display is dimmed.</para>
            /// </summary>
            public static readonly Guid GUID_SESSION_DISPLAY_STATUS = Guid.Parse("2B84C20E-AD23-4ddf-93DB-05FFBD7EFCA5");

            /// <summary>
            /// The user status associated with the application's session has changed.
            /// <para>Windows 7, Windows Server 2008 R2, Windows Vista, and Windows Server 2008:  This notification is available starting with Windows 8 and Windows Server 2012.</para>
            /// <para>This notification is sent only to user-mode applications running in an interactive session. Services and other programs running in session 0 should register for GUID_GLOBAL_USER_PRESENCE. The Data member is a DWORD with one of the following values.</para>
            /// <para>PowerUserPresent (0) - The user is providing input to the session.</para>
            /// <para>PowerUserInactive (2) - The user activity timeout has elapsed with no interaction from the user.</para>
            /// <para>Note  All applications that run in an interactive user-mode session should use this setting. When kernel mode applications register for monitor status they should use GUID_CONSOLE_DISPLAY_STATUS instead.</para>
            /// </summary>
            public static readonly Guid GUID_SESSION_USER_PRESENCE = Guid.Parse("3C0F4548-C03F-4c4d-B9F2-237EDE686376");

            /// <summary>
            /// The system is entering or exiting away mode. The Data member is a DWORD that indicates the current away mode state.
            /// <para>0x0 - The computer is exiting away mode.</para>
            /// <para>0x1 - The computer is entering away mode.</para>
            /// </summary>
            public static readonly Guid GUID_SYSTEM_AWAYMODE = Guid.Parse("98a7f580-01f7-48aa-9c0f-44352c29e5C0");

            /// <summary>
            /// Define a special GUID which will be used to define the active power scheme.
            /// User will register for this power setting GUID, and when the active power
            /// scheme changes, they'll get a callback where the payload is the GUID
            /// representing the active powerscheme.
            /// </summary>
            public static readonly Guid GUID_ACTIVE_POWERSCHEME = Guid.Parse("31F9F286-5084-42FE-B720-2B0264993763");

            /// <summary>
            /// notebook lid being closed & opened
            /// When you receive this notification through WM_POWERBROADCAST (or PBT_POWERSETTINGCHANGE for services) the lParam will point to 
            /// POWERBROADCAST_SETTING struct, with DataLength evidently set to sizeof(DWORD) and Data set as follows:
            /// <para>0 - lid closed</para>
            /// <para>1 - lid opened</para>
            /// </summary>
            public static readonly Guid GUID_LIDSWITCH_STATE_CHANGE = Guid.Parse("BA3E0F4D-B817-4094-A2D1-D56379E6A0F3");


            /// <summary>
            /// Maximum Power Savings - indicates that very aggressive power savings measures will be used to help stretch battery life.
            /// </summary>
            public static readonly Guid GUID_MAX_POWER_SAVINGS = Guid.Parse("a1841308-3541-4fab-bc81-f71556f20b4a");
            /// <summary>
            /// No Power Savings - indicates that almost no power savings measures will be used.
            /// </summary>
            public static readonly Guid GUID_MIN_POWER_SAVINGS = Guid.Parse("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
            /// <summary>
            /// Typical Power Savings - indicates that fairly aggressive power savings measures will be used.
            /// </summary>
            public static readonly Guid GUID_TYPICAL_POWER_SAVINGS = Guid.Parse("381b4222-f694-41f0-9685-ff5bb260df2e");


        }

        [Flags]
        public enum DeviceNotifyType
        {
            /// <summary>
            /// The hRecipient parameter is a window handle.
            /// </summary>
            DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000,
            /// <summary>
            /// The hRecipient parameter is a service status handle.
            /// </summary>
            DEVICE_NOTIFY_SERVICE_HANDLE = 0x00000001,
            /// <summary>
            /// Notifies the recipient of device interface events for all device interface classes. (The dbcc_classguid member is ignored.)
            /// <para>This value can be used only if the dbch_devicetype member is DBT_DEVTYP_DEVICEINTERFACE.</para>
            /// </summary>
            DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004
        }

        public enum PowerSavings
        {
            /// <summary>
            /// No Power Savings - indicates that almost no power savings measures will be used.
            /// </summary>
            Min,
            /// <summary>
            /// 
            /// </summary>
            Max,
            /// <summary>
            /// 
            /// </summary>
            Typical
        }

        /// <summary>
        /// Registers the application to receive power setting notifications for the specific power setting event.
        /// </summary>
        /// <param name="hRecipient">Handle indicating where the power setting notifications are to be sent. For interactive applications, the Flags parameter should be zero, and the hRecipient parameter should be a window handle. For services, the Flags parameter should be one, and the hRecipient parameter should be a SERVICE_STATUS_HANDLE as returned from RegisterServiceCtrlHandlerEx.</param>
        /// <param name="PowerSettingGuid">The GUID of the power setting for which notifications are to be sent</param>
        /// <returns>Returns a notification handle for unregistering for power notifications. If the function fails, the return value is NULL. To get extended error information, call GetLastError.</returns>
        [DllImport("User32", SetLastError = true, EntryPoint = "RegisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid PowerSettingGuid, DeviceNotifyType Flags);

        /// <summary>
        /// Unregisters the power setting notification.
        /// </summary>
        /// <param name="handle">The handle returned from the RegisterPowerSettingNotification function.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("User32", EntryPoint = "UnregisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        internal static extern bool UnregisterPowerSettingNotification(IntPtr handle);

        /// <summary>
        /// The AC power status
        /// </summary>
        public enum ACLineStatus : byte
        {
            /// <summary>
            /// Offline
            /// </summary>
            Offline = 0, 
            /// <summary>
            /// Online
            /// </summary>
            Online = 1, 
            /// <summary>
            /// Unknown status
            /// </summary>
            Unknown = 255
        }

        /// <summary>
        /// The battery charge status.
        /// </summary>
        public enum BatteryFlag : byte
        {
            /// <summary>
            /// High—the battery capacity is at more than 66 percent
            /// </summary>
            High = 1,
            /// <summary>
            /// Low—the battery capacity is at less than 33 percent
            /// </summary>
            Low = 2,
            /// <summary>
            /// Critical—the battery capacity is at less than five percent
            /// </summary>
            Critical = 4,
            /// <summary>
            /// Charging
            /// </summary>
            Charging = 8,
            /// <summary>
            /// No system battery
            /// </summary>
            NoSystemBattery = 128,
            /// <summary>
            /// Unknown status—unable to read the battery flag information
            /// </summary>
            Unknown = 255
        }

        /// <summary>
        /// The system power source
        /// </summary>
        public enum PoweredBy : int
        {
            /// <summary>
            /// The computer is powered by an AC power source (or similar, such as a laptop powered by a 12V automotive adapter).
            /// </summary>
            Ac = 0,
            /// <summary>
            /// The computer is powered by an onboard battery power source.
            /// </summary>
            Dc = 1,
            /// <summary>
            /// The computer is powered by a short-term power source such as a UPS device.
            /// </summary>
            Hot = 2
        }

        /// <summary>
        /// The current monitor's display state
        /// </summary>
        public enum MonitorDisplayState : int
        {
            /// <summary>
            /// The display is off.
            /// </summary>
            Off = 0,
            /// <summary>
            /// The display is on.
            /// </summary>
            On = 1,
            /// <summary>
            /// The display is dimmed.
            /// </summary>
            Dimmed = 2
        }

        /// <summary>
        /// information about the power status of the system.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SYSTEM_POWER_STATUS
        {
            /// <summary>
            /// The AC power status.
            /// </summary>
            public ACLineStatus ACLineStatus;
            /// <summary>
            /// The battery charge status
            /// </summary>
            public BatteryFlag BatteryFlag;
            /// <summary>
            /// The percentage of full battery charge remaining. This member can be a value in the range 0 to 100, or 255 if status is unknown.
            /// </summary>
            public byte BatteryLifePercent;
            /// <summary>
            /// Reserved; must be zero.
            /// </summary>
            public byte Reserved1;
            /// <summary>
            /// The number of seconds of battery life remaining, or –1 if remaining seconds are unknown.
            /// </summary>
            public int BatteryLifeTime;
            /// <summary>
            /// The number of seconds of battery life when at full charge, or –1 if full battery lifetime is unknown.
            /// <para>
            /// The system is only capable of estimating BatteryFullLifeTime based on calculations on BatteryLifeTime and 
            /// BatteryLifePercent. Without smart battery subsystems, this value may not be accurate enough to be useful.
            /// </para>
            /// </summary>
            public int BatteryFullLifeTime;

            public override string ToString()
            {
                return string.Format("AC: {0} Battery: {1} Battery Percent: {2} Battery Life Time: {3} Battery Full Life Time: {4}", this.ACLineStatus, this.BatteryFlag, this.BatteryLifePercent, this.BatteryLifeTime, this.BatteryFullLifeTime);
            }
        }

        /// <summary>
        /// Sent with a power setting event and contains data about the specific change
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct POWERBROADCAST_SETTING
        {
            public Guid PowerSetting;
            public uint DataLength;
            //public byte Data;
        }

        /// <summary>
        /// Retrieves the power status of the system. The status indicates whether the system is running on AC or DC power, whether the battery is currently charging, and how much battery life remains.
        /// </summary>
        /// <param name="lpSystemPowerStatus">A pointer to a SYSTEM_POWER_STATUS structure that receives status information.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("kernel32.dll")]
        internal static extern bool GetSystemPowerStatus(out SYSTEM_POWER_STATUS lpSystemPowerStatus);


        public class PowerEventInfo
        {
            public ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control PowerEvent;
            public SYSTEM_POWER_STATUS? SYSTEM_POWER_STATUS;
            public PowerSavings? PowerSavingsMode;
            public int? OEMEventCode;
            public PoweredBy? PoweredBy;
            public Guid? CurrentPowerSchemeId;
            public string CurrentPowerScheme;
            public MonitorDisplayState? CurrentMonitorDisplayState;
            public int? BatteryPowerRemaining;
            public bool? LidOpen;

            public DateTime? Idle;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(" Event: {0}", PowerEvent);

                if (SYSTEM_POWER_STATUS != null)
                    sb.AppendFormat(" Status: {0}", SYSTEM_POWER_STATUS.ToString());
                if (PowerSavingsMode != null)
                    sb.AppendFormat(" Mode: {0}", PowerSavingsMode);
                if (OEMEventCode != null)
                    sb.AppendFormat(" OEM Code: {0}", OEMEventCode);
                if (PoweredBy != null)
                    sb.AppendFormat(" Powered By: {0}", PoweredBy);
                if (CurrentPowerSchemeId != null)
                    sb.AppendFormat(" Current Power Scheme: {0}({1})", CurrentPowerScheme,CurrentPowerSchemeId);
                if (CurrentMonitorDisplayState != null)
                    sb.AppendFormat(" Monitor Display State: {0}", CurrentMonitorDisplayState);
                if (Idle != null)
                    sb.AppendFormat(" System is Idle: {0}", Idle);
                if (BatteryPowerRemaining != null)
                    sb.AppendFormat(" Battery Power Remaining: {0}", BatteryPowerRemaining);
                if (LidOpen != null)
                    sb.AppendFormat(" Lid Open: {0}", LidOpen);

                return sb.ToString();
            }
        }

        /// <summary>
        /// The subgroup of power settings
        /// </summary>
        public struct SubGroupOfPowerSettingsGuid
        {
            /// <summary>
            /// Settings in this subgroup are part of the default power scheme.
            /// </summary>
            private static Guid NO_SUBGROUP_GUID = new Guid("fea3413e-7e05-4911-9a71-700331f1c294");
            /// <summary>
            /// Settings in this subgroup control power management configuration of the system's hard disk drives.
            /// </summary>
            private static Guid GUID_DISK_SUBGROUP = new Guid("0012ee47-9041-4b5d-9b77-535fba8b1442");
            /// <summary>
            /// Settings in this subgroup control configuration of the system power buttons.
            /// </summary>
            private static Guid GUID_SYSTEM_BUTTON_SUBGROUP = new Guid("4f971e89-eebd-4455-a8de-9e59040e7347");
            /// <summary>
            /// Settings in this subgroup control configuration of processor power management features.
            /// </summary>
            private static Guid GUID_PROCESSOR_SETTINGS_SUBGROUP = new Guid("54533251-82be-4824-96c1-47b60b740d00");
            /// <summary>
            /// Settings in this subgroup control configuration of the video power management features.
            /// </summary>
            private static Guid GUID_VIDEO_SUBGROUP = new Guid("7516b95f-f776-4464-8c53-06167f40cc99");
            /// <summary>
            /// Settings in this subgroup control battery alarm trip points and actions.
            /// </summary>
            private static Guid GUID_BATTERY_SUBGROUP = new Guid("e73a048d-bf27-4f12-9731-8b2076e8891f");
            /// <summary>
            /// Settings in this subgroup control system sleep settings.
            /// </summary>
            private static Guid GUID_SLEEP_SUBGROUP = new Guid("238C9FA8-0AAD-41ED-83F4-97BE242C8F20");
            /// <summary>
            /// Settings in this subgroup control PCI Express settings.
            /// </summary>
            private static Guid GUID_PCIEXPRESS_SETTINGS_SUBGROUP = new Guid("501a4d13-42af-4429-9fd1-a8218c268e20");

        }

        /// <summary>
        /// Retrieves the friendly name for the specified power setting, subgroup, or scheme. If the SchemeGuid parameter is 
        /// not NULL but both the SubGroupOfPowerSettingsGuid and PowerSettingGuid parameters are NULL, the friendly name of 
        /// the power scheme will be returned. If the SchemeGuid and SubGroupOfPowerSettingsGuid parameters are not NULL
        /// and the PowerSettingGuid parameter is NULL, the friendly name of the subgroup will be returned. If the 
        /// SchemeGuid, SubGroupOfPowerSettingsGuid, and PowerSettingGuid parameters are not NULL, the friendly name of 
        /// the power setting will be returned.
        /// </summary>
        /// <param name="RootPowerKey">This parameter is reserved for future use and must be set to NULL.</param>
        /// <param name="SchemeGuid">The identifier of the power scheme.</param>
        /// <param name="SubGroupOfPowerSettingGuid">The subgroup of power settings. </param>
        /// <param name="PowerSettingGuid">The identifier of the power setting that is being used.</param>
        /// <param name="Buffer">A pointer to a buffer that receives the friendly name. If this parameter is NULL, the BufferSize parameter 
        /// receives the required buffer size. The strings returned are all wide (Unicode) strings.</param>
        /// <param name="BufferSize">A pointer to a variable that contains the size of the buffer pointed to by the Buffer parameter.
        /// <para>If the Buffer parameter is NULL, the function returns ERROR_SUCCESS and the variable receives the required buffer size.</para>
        /// <para>If the specified buffer size is not large enough to hold the requested data, the function returns ERROR_MORE_DATA and the variable
        /// receives the required buffer size.</para>
        /// </param>
        /// <returns>Returns ERROR_SUCCESS (zero) if the call was successful, and a nonzero value if the call failed. If the buffer size specified 
        /// by the BufferSize parameter is too small, ERROR_MORE_DATA will be returned and the DWORD pointed to by the BufferSize parameter
        ///  will be filled in with the required buffer size.</returns>
        [DllImport("PowrProf.dll")]
        internal static extern UInt32 PowerReadFriendlyName(
            IntPtr RootPowerKey, 
            ref Guid SchemeGuid, 
            IntPtr SubGroupOfPowerSettingGuid, 
            IntPtr PowerSettingGuid, 
            IntPtr Buffer, 
            ref UInt32 BufferSize);


        /// <summary>
        /// Retrieves the friendly name for the specified power setting, subgroup, or scheme.
        /// </summary>
        /// <param name="schemeGuid">The identifier of the power scheme.</param>
        /// <returns>friendly name</returns>
        internal static string ReadPowerSchemeFriendlyName(Guid schemeGuid)
        {
            uint sizeName = 1024;
            IntPtr pSizeName = Marshal.AllocHGlobal((int)sizeName);

            string friendlyName;

            try
            {
                PowerReadFriendlyName(IntPtr.Zero, ref schemeGuid, IntPtr.Zero, IntPtr.Zero, pSizeName, ref sizeName);
                friendlyName = Marshal.PtrToStringUni(pSizeName);
            }
            finally
            {
                Marshal.FreeHGlobal(pSizeName);
            }

            return friendlyName;
        }

        private const int WM_POWERBROADCAST = 0x218;

        /// <summary>
        /// Transforms a Window/Service Message to PowerEventInfo
        /// </summary>
        public static PowerEventInfo TransformMessage(ref Message m)
        {
            if (m.Msg == WM_POWERBROADCAST)
            {
                switch ((int)m.WParam)
                {
                    case (int)ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control.PBT_APMBATTERYLOW:
                        return new PowerEventInfo { PowerEvent = (ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control)(int)(m.WParam) };
                    case (int)ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control.PBT_APMOEMEVENT:
                        return new PowerEventInfo { PowerEvent = (ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control)(int)(m.WParam), OEMEventCode = (int)m.LParam };
                    case (int)ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control.PBT_APMPOWERSTATUSCHANGE:
                        {
                            var pei = new PowerEventInfo { PowerEvent = (ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control)(int)(m.WParam) };
                            SYSTEM_POWER_STATUS sps = new SYSTEM_POWER_STATUS();
                            if (GetSystemPowerStatus(out sps))
                            {
                                pei.SYSTEM_POWER_STATUS = sps;
                            }
                            return pei;
                        }
                    case (int)ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control.PBT_APMQUERYSTANDBY:
                        return new PowerEventInfo { PowerEvent = (ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control)(int)(m.WParam) };
                    case (int)ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control.PBT_APMQUERYSTANDBYFAILED:
                        return new PowerEventInfo { PowerEvent = (ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control)(int)(m.WParam) };
                    case (int)ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control.PBT_APMQUERYSUSPEND:
                        return new PowerEventInfo { PowerEvent = (ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control)(int)(m.WParam) };
                    case (int)ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control.PBT_APMQUERYSUSPENDFAILED:
                        return new PowerEventInfo { PowerEvent = (ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control)(int)(m.WParam) };
                    case (int)ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control.PBT_APMRESUMEAUTOMATIC:
                        return new PowerEventInfo { PowerEvent = (ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control)(int)(m.WParam) };
                    case (int)ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control.PBT_APMRESUMECRITICAL:
                        return new PowerEventInfo { PowerEvent = (ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control)(int)(m.WParam) };
                    case (int)ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control.PBT_APMRESUMESTANDBY:
                        return new PowerEventInfo { PowerEvent = (ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control)(int)(m.WParam) };
                    case (int)ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control.PBT_APMRESUMESUSPEND:
                        return new PowerEventInfo { PowerEvent = (ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control)(int)(m.WParam) };
                    case (int)ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control.PBT_APMSTANDBY:
                        return new PowerEventInfo { PowerEvent = (ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control)(int)(m.WParam) };
                    case (int)ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control.PBT_APMSUSPEND:
                        return new PowerEventInfo { PowerEvent = (ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control)(int)(m.WParam) };
                    case (int)ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control.PBT_POWERSETTINGCHANGE:
                        {
                            var pei = new PowerEventInfo { PowerEvent = (ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control)(int)(m.WParam) };
                            

                            POWERBROADCAST_SETTING ps = (POWERBROADCAST_SETTING)Marshal.PtrToStructure(m.LParam, typeof(POWERBROADCAST_SETTING));
                            IntPtr pData = (IntPtr)(m.LParam.ToInt32() + Marshal.SizeOf(ps));  // (*1)

                            if (ps.PowerSetting == PowerSettings.GUID_POWERSCHEME_PERSONALITY && ps.DataLength == Marshal.SizeOf(typeof(Guid)))
                            {
                                Guid newPersonality = (Guid)Marshal.PtrToStructure(pData, typeof(Guid));

                                if (newPersonality == PowerSettings.GUID_MAX_POWER_SAVINGS)
                                {
                                    pei.PowerSavingsMode = PowerSavings.Max;
                                }
                                else if (newPersonality == PowerSettings.GUID_MIN_POWER_SAVINGS)
                                {
                                    pei.PowerSavingsMode = PowerSavings.Min;
                                }
                                else if (newPersonality == PowerSettings.GUID_TYPICAL_POWER_SAVINGS)
                                {
                                    pei.PowerSavingsMode = PowerSavings.Typical;
                                }
                                else
                                {
                                    DebugLogger.WriteLine("switched to unknown Power savings");
                                }
                            }
                            else if (ps.PowerSetting == PowerSettings.GUID_ACDC_POWER_SOURCE && ps.DataLength == Marshal.SizeOf(typeof(Int32)))
                            {
                                Int32 iData = (Int32)Marshal.PtrToStructure(pData, typeof(Int32));
                                DebugLogger.WriteLine("ACDC: " + (PoweredBy)iData);

                                pei.PoweredBy = (PoweredBy)iData;
                                //onBattery = iData != 0;
                            }
                            else if (ps.PowerSetting == PowerSettings.GUID_ACTIVE_POWERSCHEME && ps.DataLength == Marshal.SizeOf(typeof(Guid)))
                            {
                                Guid schemeId = (Guid)Marshal.PtrToStructure(pData, typeof(Guid));
                                pei.CurrentPowerSchemeId = schemeId;
                                pei.CurrentPowerScheme = ReadPowerSchemeFriendlyName(schemeId);
                            }
                            else if ((ps.PowerSetting == PowerSettings.GUID_CONSOLE_DISPLAY_STATE || ps.PowerSetting == PowerSettings.GUID_MONITOR_POWER_ON) && ps.DataLength == Marshal.SizeOf(typeof(Int32)))
                            {
                                Int32 iData = (Int32)Marshal.PtrToStructure(pData, typeof(Int32));
                                DebugLogger.WriteLine("monitor's display state: " + (MonitorDisplayState)iData);
                                pei.CurrentMonitorDisplayState = (MonitorDisplayState)iData;
                            }
                            else if (ps.PowerSetting == PowerSettings.GUID_IDLE_BACKGROUND_TASK)
                            {
                                pei.Idle = DateTime.UtcNow;
                            }
                            else if (ps.PowerSetting == PowerSettings.GUID_BATTERY_PERCENTAGE_REMAINING)
                            {
                                Int32 iData = (Int32)Marshal.PtrToStructure(pData, typeof(Int32));
                                DebugLogger.WriteLine("Battery Power Remaining: " + iData);

                                pei.BatteryPowerRemaining = iData;
                            }
                            else if (ps.PowerSetting == PowerSettings.GUID_LIDSWITCH_STATE_CHANGE)
                            {
                                Int32 iData = (Int32)Marshal.PtrToStructure(pData, typeof(Int32));
                                DebugLogger.WriteLine("Lid Open: " + iData);

                                pei.LidOpen = (iData == 1);
                            }
                            else
                            {
                                DebugLogger.WriteLine("Unknown power setting {0}", ps.PowerSetting);
                            }
                            return pei;
                        }

                }
            }

            DebugLogger.WriteLine("Unhandled Power notification " + m.ToString());
            return null;
        }

    }
}
