/*
 HoytSoft Self installing .NET service using the Win32 API
 
 Dror Gluska
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HoytSoft.ServiceBase.Base.Logging;

namespace HoytSoft.ServiceBase.Base.Notifications
{

    //https://stackoverflow.com/questions/14514147/servicecontrolhandler-used-for-usb-device-notifications-onstop-not-implementa
    public static class DeviceNotification
    {

        /// <summary>
        /// Contains information about a class of devices.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEV_BROADCAST_DEVICEINTERFACE
        {
            /// <summary>
            /// The size of this structure, in bytes. This is the size of the members plus the actual length of the dbcc_name string (the null character is accounted for by the declaration of dbcc_name as a one-character array.)
            /// </summary>
            public int dbcc_size;
            /// <summary>
            /// Set to DBT_DEVTYP_DEVICEINTERFACE.
            /// </summary>
            public dbch_devicetype dbcc_devicetype;
            /// <summary>
            /// Reserved; do not use.
            /// </summary>
            public int dbcc_reserved;
            /// <summary>
            /// The GUID for the interface device class.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
            public byte[] dbcc_classguid;
            /// <summary>
            /// A null-terminated string that specifies the name of the device.
            /// <para>When this structure is returned to a window through the WM_DEVICECHANGE message, the dbcc_name string is converted to ANSI as appropriate. Services always receive a Unicode string, whether they call RegisterDeviceNotificationW or RegisterDeviceNotificationA.</para>
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] dbcc_name;
        }

        /// <summary>
        /// Serves as a standard header for information related to a device event reported through the WM_DEVICECHANGE message.
        /// <para>The members of the DEV_BROADCAST_HDR structure are contained in each device management structure. To determine which structure you have received through WM_DEVICECHANGE, treat the structure as a DEV_BROADCAST_HDR structure and check its dbch_devicetype member.</para>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEV_BROADCAST_HDR
        {
            public int dbch_size;
            public dbch_devicetype dbch_devicetype;
            public int dbch_reserved;
        }

        /// <summary>
        /// Contains information about a file system handle.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEV_BROADCAST_HANDLE
        {
            public int dbch_size;
            public dbch_devicetype dbch_devicetype;
            public int dbch_reserved;
            public IntPtr dbch_handle;
            public IntPtr dbch_hdevnotify;
            public Guid dbch_eventguid;
            public long dbch_nameoffset;
            public byte dbch_data;
            public byte dbch_data1;
        }

        /// <summary>
        /// Contains information about a OEM-defined device type.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEV_BROADCAST_OEM
        {
            public int dbco_size;
            public dbch_devicetype dbco_devicetype;
            public int dbco_reserved;
            public int dbco_identifier;
            public int dbco_suppfunc;
        }

        /// <summary>
        /// Contains information about a modem, serial, or parallel port.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEV_BROADCAST_PORT
        {
            public int dbcp_size;
            public dbch_devicetype dbcp_devicetype;
            public int dbcp_reserved;
            public char[] dbcp_name;
        }

        private enum dbcv_flags : short
        {
            None = 0x0000,
            /// <summary>
            /// Change affects media in drive. If not set, change affects physical device or drive.
            /// </summary>
            DBTF_MEDIA = 0x0001,
            /// <summary>
            /// Indicated logical volume is a network volume.
            /// </summary>
            DBTF_NET = 0x0002
        }

        [Flags]
        private enum dbcv_drives : uint
        {
            A = 0x00000001,
            B = 0x00000002,
            C = 0x00000004,
            D = 0x00000008,
            E = 0x00000010,
            F = 0x00000020,
            G = 0x00000040,
            H = 0x00000080,
            I = 0x00000100,
            J = 0x00000200,
            K = 0x00000400,
            L = 0x00000800,
            M = 0x00001000,
            N = 0x00002000,
            O = 0x00004000,
            P = 0x00008000,
            Q = 0x00010000,
            R = 0x00020000,
            S = 0x00040000,
            T = 0x00080000,
            U = 0x00100000,
            V = 0x00200000,
            W = 0x00400000,
            X = 0x00800000,
            Y = 0x01000000,
            Z = 0x02000000
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEV_BROADCAST_VOLUME
        {
            public int dbcv_size;
            public dbch_devicetype dbcv_devicetype;
            public int dbcv_reserved;
            public dbcv_drives dbcv_unitmask; //bitmask for drives, bit0 - drive a, bit1 - drive b etc'
            public dbcv_flags dbcv_flags;
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

        public enum dbch_devicetype : int
        {
            /// <summary>
            /// OEM- or IHV-defined device type. This structure is a DEV_BROADCAST_OEM structure.
            /// </summary>
            DBT_DEVTYP_OEM = 0x00000000,
            /// <summary>
            /// devnode number
            /// </summary>
            DBT_DEVTYP_DEVNODE = 0x00000001,
            /// <summary>
            /// Logical volume. This structure is a DEV_BROADCAST_VOLUME structure.
            /// </summary>
            DBT_DEVTYP_VOLUME = 0x00000002,
            /// <summary>
            /// Port device (serial or parallel). This structure is a DEV_BROADCAST_PORT structure.
            /// </summary>
            DBT_DEVTYP_PORT = 0x00000003,
            /// <summary>
            /// network resource
            /// </summary>
            DBT_DEVTYP_NET = 0x00000004,
            /// <summary>
            /// Class of devices. This structure is a DEV_BROADCAST_DEVICEINTERFACE structure.
            /// </summary>
            DBT_DEVTYP_DEVICEINTERFACE = 0x00000005,
            /// <summary>
            /// File system handle. This structure is a DEV_BROADCAST_HANDLE structure.
            /// </summary>
            DBT_DEVTYP_HANDLE = 0x00000006
        }




        /// <summary>
        /// Registers the device or type of device for which a window will receive notifications.
        /// </summary>
        /// <param name="recipient">
        /// A handle to the window or service that will receive device events for the devices specified in the NotificationFilter parameter. The same window handle can be used in multiple calls to RegisterDeviceNotification.
        /// <para>Services can specify either a window handle or service status handle.</para>
        /// </param>
        /// <param name="notificationFilter">
        /// A pointer to a block of data that specifies the type of device for which notifications should be sent. This block always begins with the DEV_BROADCAST_HDR structure. The data following this header is dependent on the value of the dbch_devicetype member, which can be DBT_DEVTYP_DEVICEINTERFACE or DBT_DEVTYP_HANDLE. For more information, see Remarks.
        /// </param>
        /// <param name="flags">Device Notify Type</param>
        /// <returns>
        /// If the function succeeds, the return value is a device notification handle.
        /// <para>If the function fails, the return value is NULL</para>
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, DeviceNotifyType flags);

        /// <summary>
        /// Closes the specified device notification handle.
        /// </summary>
        /// <param name="handle">Device notification handle returned by the RegisterDeviceNotification function.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll")]
        internal static extern bool UnregisterDeviceNotification(IntPtr handle);

        /// <summary>
        /// Device Events
        /// </summary>
        public struct IOEvents
        {
            /// <summary>
            /// The CD-ROM device has been locked for exclusive access.
            /// <para>Windows Server 2003 and Windows XP:  Support for this value requires IMAPI 2.0. For more information, see Image Mastering API.</para>
            /// </summary>
            public static readonly Guid GUID_IO_CDROM_EXCLUSIVE_LOCK = Guid.Parse("bc56c139-7a10-47ee-a294-4c6a38f0149a");

            /// <summary>
            /// A CD-ROM device that was locked for exclusive access has been unlocked.
            /// <para>Windows Server 2003 and Windows XP:  Support for this value requires IMAPI 2.0. For more information, see Image Mastering API.</para>
            /// </summary>
            public static readonly Guid GUID_IO_CDROM_EXCLUSIVE_UNLOCK = Guid.Parse("a3b6d27d-5e35-4885-81e5-ee18c00ed779");

            /// <summary>
            /// Media spin-up is in progress.
            /// </summary>
            public static readonly Guid GUID_IO_DEVICE_BECOMING_READY = Guid.Parse("d07433f0-a98e-11d2-917a-00a0c9068ff3");

            /// <summary>
            /// There are several possible causes for this event; for more information, refer to T10 MMC specification of the GET EVENT STATUS NOTIFICATION Command, at http://www.t10.org/.
            /// </summary>
            public static readonly Guid GUID_IO_DEVICE_EXTERNAL_REQUEST = Guid.Parse("d07433d0-a98e-11d2-917a-00a0c9068ff3");

            /// <summary>
            /// Removable media has been added to the device. The dbch_data member is a pointer to a CLASS_MEDIA_CHANGE_CONTEXT structure. The NewState member provides status information. For example, a value of MediaUnavailable indicates that the media is not available (for example, due to an active recording session).
            /// <para>Windows XP:  The dbch_data member is a ULONG value that represents the number of times that media has been changed since system startup.</para>
            /// </summary>
            public static readonly Guid GUID_IO_MEDIA_ARRIVAL = Guid.Parse("d07433c0-a98e-11d2-917a-00a0c9068ff3");

            /// <summary>
            /// The removable media's drive has received a request from the user to eject the specified slot or media.
            /// </summary>
            public static readonly Guid GUID_IO_MEDIA_EJECT_REQUEST = Guid.Parse("d07433d1-a98e-11d2-917a-00a0c9068ff3");

            /// <summary>
            /// Removable media has been removed from the device or is unavailable. The dbch_data member is a pointer to a CLASS_MEDIA_CHANGE_CONTEXT structure. The NewState member provides status information. For example, a value of MediaUnavailable indicates that the media is not available (for example, due to an active recording session).
            /// <para>Windows XP:  The dbch_data member is a ULONG value that represents the number of times that media has been changed since system startup.</para>
            /// </summary>
            public static readonly Guid GUID_IO_MEDIA_REMOVAL = Guid.Parse("d07433c1-a98e-11d2-917a-00a0c9068ff3");

            /// <summary>
            /// The volume label has changed.
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_CHANGE = Guid.Parse("7373654a-812a-11d0-bec7-08002be2092f");

            /// <summary>
            /// The size of the file system on the volume has changed.
            /// <para>Windows Server 2003 and Windows XP:  This value is not supported.</para>
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_CHANGE_SIZE = Guid.Parse("3a1625be-ad03-49f1-8ef8-6bbac182d1fd");

            /// <summary>
            /// An attempt to dismount the volume is in progress. You should close all handles to files and directories on the volume. This event will not necessarily be preceded by a GUID_IO_VOLUME_LOCK event.
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_DISMOUNT = Guid.Parse("d16a55e8-1059-11d2-8ffd-00a0c9a06d32");

            /// <summary>
            /// An attempt to dismount a volume failed. This often happens because another process failed to respond to a GUID_IO_VOLUME_DISMOUNT notice by closing its outstanding handles. Because the dismount failed, you may reopen any handles to the affected volume.
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_DISMOUNT_FAILED = Guid.Parse("e3c5b178-105d-11d2-8ffd-00a0c9a06d32");

            /// <summary>
            /// The volume's BitLocker Drive Encryption status has changed. This event is signaled when BitLocker is enabled or disabled, or when encryption begins, ends, pauses, or resumes.
            /// <para>Windows Server 2003 and Windows XP:  This value is not supported.</para>
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_FVE_STATUS_CHANGE = Guid.Parse("062998b2-ee1f-4b6a-b857-e76cbbe9a6da");

            /// <summary>
            /// Another process is attempting to lock the volume. You should close all handles to files and directories on the volume.
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_LOCK = Guid.Parse("50708874-c9af-11d1-8fef-00a0c9a06d32");

            /// <summary>
            /// An attempt to lock a volume failed. This often happens because another process failed to respond to a GUID_IO_VOLUME_LOCK event by closing its outstanding handles. Because the lock failed, you may reopen any handles to the affected volume.
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_LOCK_FAILED = Guid.Parse("ae2eed10-0ba8-11d2-8ffb-00a0c9a06d32");

            /// <summary>
            /// The volume has been mounted by another process. You may open one or more handles to it.
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_MOUNT = Guid.Parse("b5804878-1a96-11d2-8ffd-00a0c9a06d32");

            /// <summary>
            /// The volume name has been changed.
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_NAME_CHANGE = Guid.Parse("2de97f83-4c06-11d2-a532-00609713055a");

            /// <summary>
            /// A file system has detected corruption on the volume. The application should run CHKDSK on the volume or notify the user to do so.
            /// <para>Windows Server 2003 and Windows XP:  This value is not supported.</para>
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_NEED_CHKDSK = Guid.Parse("799a0960-0a0b-4e03-ad88-2fa7c6ce748a");

            /// <summary>
            /// The physical makeup or current physical state of the volume has changed.
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_PHYSICAL_CONFIGURATION_CHANGE = Guid.Parse("2de97f84-4c06-11d2-a532-00609713055a");

            /// <summary>
            /// The file system is preparing the disc to be ejected. For example, the file system is stopping a background formatting operation or closing the session on write-once media.
            /// <para>Windows Server 2003 and Windows XP:  This value is not supported.</para>
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_PREPARING_EJECT = Guid.Parse("c79eb16e-0dac-4e7a-a86c-b25ceeaa88f6");

            /// <summary>
            /// The volume's unique identifier has been changed. For more information about the unique identifier, see IOCTL_MOUNTDEV_QUERY_UNIQUE_ID.
            /// <para>Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP:  This value is not supported until Windows Server 2008 R2 and Windows 7.</para>
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_UNIQUE_ID_CHANGE = Guid.Parse("af39da42-6622-41f5-970b-139d092fa3d9");

            /// <summary>
            /// The volume has been unlocked by another process. You may open one or more handles to it.
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_UNLOCK = Guid.Parse("9a8c3d68-d0cb-11d1-8fef-00a0c9a06d32");

            /// <summary>
            /// The media is wearing out. This event is sent when a file system determines that the error rate on a volume is too high, or its defect replacement space is almost exhausted.
            /// <para>Windows Server 2003 and Windows XP:  This value is not supported.</para>
            /// </summary>
            public static readonly Guid GUID_IO_VOLUME_WEARING_OUT = Guid.Parse("873113ca-1486-4508-82ac-c3b2e5297aaa");



            public static readonly Guid GUID_DEVCLASS_1394 = Guid.Parse("6bdd1fc1-810f-11d0-bec7-08002be2092f");
            public static readonly Guid GUID_DEVCLASS_1394DEBUG = Guid.Parse("66f250d6-7801-4a64-b139-eea80a450b24");
            public static readonly Guid GUID_DEVCLASS_61883 = Guid.Parse("7ebefbc0-3200-11d2-b4c2-00a0c9697d07");
            public static readonly Guid GUID_DEVCLASS_ADAPTER = Guid.Parse("4d36e964-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_APMSUPPORT = Guid.Parse("d45b1c18-c8fa-11d1-9f77-0000f805f530");
            public static readonly Guid GUID_DEVCLASS_AVC = Guid.Parse("c06ff265-ae09-48f0-812c-16753d7cba83");
            public static readonly Guid GUID_DEVCLASS_BATTERY = Guid.Parse("72631e54-78a4-11d0-bcf7-00aa00b7b32a");
            public static readonly Guid GUID_DEVCLASS_BIOMETRIC = Guid.Parse("53d29ef7-377c-4d14-864b-eb3a85769359");
            public static readonly Guid GUID_DEVCLASS_BLUETOOTH = Guid.Parse("e0cbf06c-cd8b-4647-bb8a-263b43f0f974");
            public static readonly Guid GUID_DEVCLASS_CDROM = Guid.Parse("4d36e965-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_COMPUTER = Guid.Parse("4d36e966-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_DECODER = Guid.Parse("6bdd1fc2-810f-11d0-bec7-08002be2092f");
            public static readonly Guid GUID_DEVCLASS_DISKDRIVE = Guid.Parse("4d36e967-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_DISPLAY = Guid.Parse("4d36e968-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_DOT4 = Guid.Parse("48721b56-6795-11d2-b1a8-0080c72e74a2");
            public static readonly Guid GUID_DEVCLASS_DOT4PRINT = Guid.Parse("49ce6ac8-6f86-11d2-b1e5-0080c72e74a2");
            public static readonly Guid GUID_DEVCLASS_ENUM1394 = Guid.Parse("c459df55-db08-11d1-b009-00a0c9081ff6");
            public static readonly Guid GUID_DEVCLASS_FDC = Guid.Parse("4d36e969-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_FLOPPYDISK = Guid.Parse("4d36e980-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_GPS = Guid.Parse("6bdd1fc3-810f-11d0-bec7-08002be2092f");
            public static readonly Guid GUID_DEVCLASS_HDC = Guid.Parse("4d36e96a-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_HIDCLASS = Guid.Parse("745a17a0-74d3-11d0-b6fe-00a0c90f57da");
            public static readonly Guid GUID_DEVCLASS_IMAGE = Guid.Parse("6bdd1fc6-810f-11d0-bec7-08002be2092f");
            public static readonly Guid GUID_DEVCLASS_INFINIBAND = Guid.Parse("30ef7132-d858-4a0c-ac24-b9028a5cca3f");
            public static readonly Guid GUID_DEVCLASS_INFRARED = Guid.Parse("6bdd1fc5-810f-11d0-bec7-08002be2092f");
            public static readonly Guid GUID_DEVCLASS_KEYBOARD = Guid.Parse("4d36e96b-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_LEGACYDRIVER = Guid.Parse("8ecc055d-047f-11d1-a537-0000f8753ed1");
            public static readonly Guid GUID_DEVCLASS_MEDIA = Guid.Parse("4d36e96c-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_MEDIUM_CHANGER = Guid.Parse("ce5939ae-ebde-11d0-b181-0000f8753ec4");
            public static readonly Guid GUID_DEVCLASS_MEMORY = Guid.Parse("5099944a-f6b9-4057-a056-8c550228544c");
            public static readonly Guid GUID_DEVCLASS_MODEM = Guid.Parse("4d36e96d-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_MONITOR = Guid.Parse("4d36e96e-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_MOUSE = Guid.Parse("4d36e96f-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_MTD = Guid.Parse("4d36e970-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_MULTIFUNCTION = Guid.Parse("4d36e971-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_MULTIPORTSERIAL = Guid.Parse("50906cb8-ba12-11d1-bf5d-0000f805f530");
            public static readonly Guid GUID_DEVCLASS_NET = Guid.Parse("4d36e972-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_NETCLIENT = Guid.Parse("4d36e973-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_NETSERVICE = Guid.Parse("4d36e974-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_NETTRANS = Guid.Parse("4d36e975-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_NODRIVER = Guid.Parse("4d36e976-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_PCMCIA = Guid.Parse("4d36e977-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_PNPPRINTERS = Guid.Parse("4658ee7e-f050-11d1-b6bd-00c04fa372a7");
            public static readonly Guid GUID_DEVCLASS_PORTS = Guid.Parse("4d36e978-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_PRINTER = Guid.Parse("4d36e979-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_PRINTERUPGRADE = Guid.Parse("4d36e97a-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_PROCESSOR = Guid.Parse("50127dc3-0f36-415e-a6cc-4cb3be910B65");
            public static readonly Guid GUID_DEVCLASS_SBP2 = Guid.Parse("d48179be-ec20-11d1-b6b8-00c04fa372a7");
            public static readonly Guid GUID_DEVCLASS_SCSIADAPTER = Guid.Parse("4d36e97b-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_SECURITYACCELERATOR = Guid.Parse("268c95a1-edfe-11d3-95c3-0010dc4050a5");
            public static readonly Guid GUID_DEVCLASS_SENSOR = Guid.Parse("5175d334-c371-4806-b3ba-71fd53c9258d");
            public static readonly Guid GUID_DEVCLASS_SIDESHOW = Guid.Parse("997b5d8d-c442-4f2e-baf3-9c8e671e9e21");
            public static readonly Guid GUID_DEVCLASS_SMARTCARDREADER = Guid.Parse("50dd5230-ba8a-11d1-bf5d-0000f805f530");
            public static readonly Guid GUID_DEVCLASS_SOUND = Guid.Parse("4d36e97c-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_SYSTEM = Guid.Parse("4d36e97d-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_TAPEDRIVE = Guid.Parse("6d807884-7d21-11cf-801c-08002be10318");
            public static readonly Guid GUID_DEVCLASS_UNKNOWN = Guid.Parse("4d36e97e-e325-11ce-bfc1-08002be10318");
            public static readonly Guid GUID_DEVCLASS_USB = Guid.Parse("36fc9e60-c465-11cf-8056-444553540000");
            public static readonly Guid GUID_DEVCLASS_VOLUME = Guid.Parse("71a27cdd-812a-11d0-bec7-08002be2092f");
            public static readonly Guid GUID_DEVCLASS_VOLUMESNAPSHOT = Guid.Parse("533c5b84-ec70-11d2-9505-00c04f79deaf");
            public static readonly Guid GUID_DEVCLASS_WCEUSBS = Guid.Parse("25dbce51-6c8f-4a72-8a6d-b54c2b4fc835");
            public static readonly Guid GUID_DEVCLASS_WPD = Guid.Parse("eec5ad98-8080-425f-922a-dabf3de3f69a");
            public static readonly Guid GUID_DEVCLASS_EHSTORAGESILO = Guid.Parse("9da2b80f-f89f-4a49-a5c2-511b085b9e8a");
            public static readonly Guid GUID_DEVCLASS_FIRMWARE = Guid.Parse("f2e7dd72-6468-4e36-b6f1-6488f42c1b52");
            public static readonly Guid GUID_DEVCLASS_EXTENSION = Guid.Parse("e2f84ce7-8efa-411c-aa69-97454ca4cb57");


            public static readonly Guid GUID_DEVINTERFACE_DISK = Guid.Parse("53f56307-b6bf-11d0-94f2-00a0c91efb8b");
            public static readonly Guid GUID_DEVINTERFACE_USB_DEVICE = Guid.Parse("A5DCBF10-6530-11D2-901F-00C04FB951ED");

        }

        public static IntPtr RegisterDeviceInterface(IntPtr hwnd, DeviceNotifyType notificationType, Guid device)
        {
            DebugLogger.WriteLine("Registering {0}", device);

            DEV_BROADCAST_DEVICEINTERFACE devInt = new DEV_BROADCAST_DEVICEINTERFACE();
            devInt.dbcc_devicetype = dbch_devicetype.DBT_DEVTYP_DEVICEINTERFACE;
            devInt.dbcc_classguid = device.ToByteArray();
            devInt.dbcc_size = Marshal.SizeOf(devInt);


            IntPtr buffer = Marshal.AllocHGlobal(devInt.dbcc_size);
            try
            {
                Marshal.StructureToPtr(devInt, buffer, false);

                return RegisterDeviceNotification(hwnd, buffer, notificationType); //| DeviceNotifyType.DEVICE_NOTIFY_SERVICE_HANDLE
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

     


        public class DeviceEventInfo
        {
            public ServicesAPI.SERVICE_CONTROL_HARDWAREPROFILECHANGE_Control? HARDWAREPROFILECHANGE;
            public ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control? DEVICEEVENT;

            public dbch_devicetype? DeviceType;
            public string DeviceName;
            public string DeviceId;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                if (HARDWAREPROFILECHANGE != null)
                    sb.AppendFormat(" Hardware Profile: {0}", HARDWAREPROFILECHANGE);
                if (DEVICEEVENT != null)
                    sb.AppendFormat(" Device Event: {0}", DEVICEEVENT);
                if (DeviceType != null)
                    sb.AppendFormat(" Device Type: {0}", DeviceType);
                if (!string.IsNullOrEmpty(DeviceId))
                    sb.AppendFormat(" Device Id: {0}", DeviceId);
                if (!string.IsNullOrEmpty(DeviceName))
                    sb.AppendFormat(" Device Name: {0}", DeviceName);

                return sb.ToString();
            }
        }

        //private static ConsoleNotificationWindow<TransformedMessage> _consoleNotificationWindow = null;

        private static string cstr_to_string(char[] data)
        {
            //Encoding Enc = System.Text.UTF8Encoding.UTF8;
            int inx = Array.FindIndex(data, '\0', (x) => x == 0);//search for 0
            if (inx >= 0)
                return new string(data, 0, inx);
            else
                return new string(data);
        }

        private static void WndHandler(ref DeviceEventInfo m)
        {
            if (m.HARDWAREPROFILECHANGE != null)
                DebugLogger.WriteLine("Hardware profile changed: {0}", m.HARDWAREPROFILECHANGE.ToString());
            if (m.DEVICEEVENT != null)
                DebugLogger.WriteLine("Device Event: {0}", m.DEVICEEVENT.ToString());
        }

        internal static int WM_DEVICECHANGE = 0x0219;

        public static DeviceEventInfo TransformMessage(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                switch ((int)m.WParam)
                {
                    case (int)ServicesAPI.SERVICE_CONTROL_HARDWAREPROFILECHANGE_Control.DBT_CONFIGCHANGECANCELED:
                        return new DeviceEventInfo { HARDWAREPROFILECHANGE = (ServicesAPI.SERVICE_CONTROL_HARDWAREPROFILECHANGE_Control)(int)m.WParam };
                    case (int)ServicesAPI.SERVICE_CONTROL_HARDWAREPROFILECHANGE_Control.DBT_CONFIGCHANGED:
                        return new DeviceEventInfo { HARDWAREPROFILECHANGE = (ServicesAPI.SERVICE_CONTROL_HARDWAREPROFILECHANGE_Control)(int)m.WParam };
                    case (int)ServicesAPI.SERVICE_CONTROL_HARDWAREPROFILECHANGE_Control.DBT_QUERYCHANGECONFIG:
                        return new DeviceEventInfo { HARDWAREPROFILECHANGE = (ServicesAPI.SERVICE_CONTROL_HARDWAREPROFILECHANGE_Control)(int)m.WParam };
                    case (int)ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control.DBT_CUSTOMEVENT:
                        return TransformDeviceEvent(ref m);
                    case (int)ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control.DBT_DEVICEARRIVAL:
                        return TransformDeviceEvent(ref m);
                    case (int)ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control.DBT_DEVICEQUERYREMOVE:
                        return TransformDeviceEvent(ref m);
                    case (int)ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control.DBT_DEVICEQUERYREMOVEFAILED:
                        return TransformDeviceEvent(ref m);
                    case (int)ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control.DBT_DEVICEREMOVECOMPLETE:
                        return TransformDeviceEvent(ref m);
                    case (int)ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control.DBT_DEVICEREMOVEPENDING:
                        return TransformDeviceEvent(ref m);
                    case (int)ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control.DBT_DEVICETYPESPECIFIC:
                        return TransformDeviceEvent(ref m);
                    //case (int)ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control.DBT_DEVNODES_CHANGED:
                    //    return new TransformedMessage { DEVICEEVENT = (ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control)(int)m.WParam };
                    //case (int)ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control.DBT_QUERYCHANGECONFIG :
                    //    return new TransformedMessage { DEVICEEVENT = (ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control)(int)m.WParam };
                    //case (int)ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control.DBT_USERDEFINED  :
                    //    return new TransformedMessage { DEVICEEVENT = (ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control)(int)m.WParam };
                }
            }

            DebugLogger.WriteLine ("Unhandled Device notification " + m.ToString());
            return null;
        }

        private static DeviceEventInfo TransformDeviceEvent(ref Message m)
        {
            var tm = new DeviceEventInfo { DEVICEEVENT = (ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control)(int)m.WParam };

            DEV_BROADCAST_HDR dbh = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HDR));
            tm.DeviceType = dbh.dbch_devicetype;

            switch (dbh.dbch_devicetype)
            {
                case dbch_devicetype.DBT_DEVTYP_DEVICEINTERFACE:
                    DEV_BROADCAST_DEVICEINTERFACE dbdi = (DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_DEVICEINTERFACE));
                    tm.DeviceId = cstr_to_string(dbdi.dbcc_name);
                    tm.DeviceName = ConvertDbccNameToFriendlyName(cstr_to_string(dbdi.dbcc_name));
                    break;
                case dbch_devicetype.DBT_DEVTYP_HANDLE:
                    DEV_BROADCAST_HANDLE dbbh = (DEV_BROADCAST_HANDLE)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HANDLE));
                    tm.DeviceName = "Handle Id " + dbbh.dbch_handle.ToString();
                    break;
                case dbch_devicetype.DBT_DEVTYP_OEM:
                    DEV_BROADCAST_OEM dbo = (DEV_BROADCAST_OEM)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_OEM));
                    tm.DeviceName = string.Format("OEM: {0} Value: {1}", dbo.dbco_identifier, dbo.dbco_suppfunc);
                    break;
                case dbch_devicetype.DBT_DEVTYP_PORT:
                    DEV_BROADCAST_PORT dbp = (DEV_BROADCAST_PORT)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_PORT));
                    IntPtr pData = (IntPtr)(m.LParam.ToInt32() + dbp.dbcp_size);  // (*1)
                    IntPtr offsetDbcpName = Marshal.OffsetOf(typeof(DEV_BROADCAST_PORT), "dbcp_name");
                    int len = (int)pData - (int)offsetDbcpName;
                    tm.DeviceName = Marshal.PtrToStringAuto(offsetDbcpName, len);
                    break;
                case dbch_devicetype.DBT_DEVTYP_VOLUME:
                    DEV_BROADCAST_VOLUME dbv = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
                    tm.DeviceName = dbv.dbcv_unitmask.ToString();
                    if (dbv.dbcv_flags != dbcv_flags.None)
                    {
                        tm.DeviceName += " " + dbv.dbcv_flags.ToString();
                    }
                    break;
            }

            //dbh.dbch_devicetype == dbch_devicetype.
            //IntPtr pData = (IntPtr)(m.LParam.ToInt32() + Marshal.SizeOf(ps));  // (*1)


            return tm;
        }



        //http://www.codeproject.com/Questions/94034/USB-detection
        #region Convert dbcc name to friendly name

        private enum RegPropertyType : uint
        {
            SPDRP_DEVICEDESC = 0x00000000, // DeviceDesc (R/W)
            SPDRP_HARDWAREID = 0x00000001, // HardwareID (R/W)
            SPDRP_COMPATIBLEIDS = 0x00000002, // CompatibleIDs (R/W)
            SPDRP_UNUSED0 = 0x00000003, // unused
            SPDRP_SERVICE = 0x00000004, // Service (R/W)
            SPDRP_UNUSED1 = 0x00000005, // unused
            SPDRP_UNUSED2 = 0x00000006, // unused
            SPDRP_CLASS = 0x00000007, // Class (R--tied to ClassGUID)
            SPDRP_CLASSGUID = 0x00000008, // ClassGUID (R/W)
            SPDRP_DRIVER = 0x00000009, // Driver (R/W)
            SPDRP_CONFIGFLAGS = 0x0000000A, // ConfigFlags (R/W)
            SPDRP_MFG = 0x0000000B, // Mfg (R/W)
            SPDRP_FRIENDLYNAME = 0x0000000C, // FriendlyName (R/W)
            SPDRP_LOCATION_INFORMATION = 0x0000000D,// LocationInformation (R/W)
            SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0x0000000E, // PhysicalDeviceObjectName (R)
            SPDRP_CAPABILITIES = 0x0000000F, // Capabilities (R)
            SPDRP_UI_NUMBER = 0x00000010, // UiNumber (R)
            SPDRP_UPPERFILTERS = 0x00000011, // UpperFilters (R/W)
            SPDRP_LOWERFILTERS = 0x00000012, // LowerFilters (R/W)
            SPDRP_BUSTYPEGUID = 0x00000013, // BusTypeGUID (R)
            SPDRP_LEGACYBUSTYPE = 0x00000014, // LegacyBusType (R)
            SPDRP_BUSNUMBER = 0x00000015, // BusNumber (R)
            SPDRP_ENUMERATOR_NAME = 0x00000016, // Enumerator Name (R)
            SPDRP_SECURITY = 0x00000017, // Security (R/W, binary form)
            SPDRP_SECURITY_SDS = 0x00000018, // Security (W, SDS form)
            SPDRP_DEVTYPE = 0x00000019, // Device Type (R/W)
            SPDRP_EXCLUSIVE = 0x0000001A, // Device is exclusive-access (R/W)
            SPDRP_CHARACTERISTICS = 0x0000001B, // Device Characteristics (R/W)
            SPDRP_ADDRESS = 0x0000001C, // Device Address (R)
            SPDRP_UI_NUMBER_DESC_FORMAT = 0x0000001E, // UiNumberDescFormat (R/W)
            SPDRP_MAXIMUM_PROPERTY = 0x0000001F  // Upper bound on ordinals
        }

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVINFO_DATA
        {
            public int cbSize;
            public Guid ClassGuid;
            public int DevInst;
            public IntPtr Reserved;
        }

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInfo(
            IntPtr DeviceInfoSet,
            int Index,
            ref  SP_DEVINFO_DATA DeviceInfoData
            );

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiDeleteDeviceInterfaceData(
          IntPtr DeviceInfoSet,
          ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData
        );


        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiDestroyDeviceInfoList
        (
             IntPtr DeviceInfoSet
        );


        /// <summary>
        /// Device interface data
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid InterfaceClassGuid;
            public int Flags;
            public int Reserved;
        }


        //[DllImport("setupapi.dll", SetLastError = true)]
        //private static extern IntPtr SetupDiCreateDeviceInfoList(ref Guid ClassGuid, IntPtr hwndParent);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern IntPtr SetupDiCreateDeviceInfoList(IntPtr ClassGuid, IntPtr hwndParent);

        
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiGetDeviceRegistryProperty(
            IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData,
            RegPropertyType Property,
            out int PropertyRegDataType,
            byte[] PropertyBuffer,
            int PropertyBufferSize,
            out int RequiredSize
        );

        [DllImport("setupapi.dll", EntryPoint = "SetupDiOpenDeviceInterfaceW", SetLastError = true)]
        private static extern bool SetupDiOpenDeviceInterface(
                IntPtr DeviceInfoSet,
                [MarshalAs(UnmanagedType.LPTStr)]string DevicePath, // In the .h header, it's a PCSTR
                int OpenFlags,
                ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

      

       

        public static string ConvertDbccNameToFriendlyName(string aDeviceInterfaceDbccName)
        {
            IntPtr deviceInfoHandle;
            SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();
            SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA(); ;
            int memberIndex;

            // Create a new empty "device info set"
            deviceInfoHandle = SetupDiCreateDeviceInfoList(IntPtr.Zero, IntPtr.Zero);
            if (deviceInfoHandle != INVALID_HANDLE_VALUE)
            {
                try
                {
                    // Add "aDeviceInterfaceDbccName" to the device info set
                    deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);
                    if (SetupDiOpenDeviceInterface(deviceInfoHandle, aDeviceInterfaceDbccName, 0, ref deviceInterfaceData))
                    {
                        try
                        {
                            // iterate over the device info set
                            // (though I only expect it to contain one item)
                            memberIndex = 0;
                            while (true)
                            {
                                // get device info that corresponds to the next memberIndex
                                deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);
                                if (!SetupDiEnumDeviceInfo(deviceInfoHandle, memberIndex, ref deviceInfoData))
                                {
                                    // The enumerator is exhausted when SetupDiEnumDeviceInfo returns false
                                    break;
                                }
                                else
                                {
                                    memberIndex++;
                                }

                                // Get the friendly name for that device info
                                string friendlyName;
                                if (TryGetDeviceFriendlyName(deviceInfoHandle, deviceInfoData, RegPropertyType.SPDRP_FRIENDLYNAME, out friendlyName))
                                {
                                    return friendlyName;
                                }
                                if (TryGetDeviceFriendlyName(deviceInfoHandle, deviceInfoData, RegPropertyType.SPDRP_DEVICEDESC, out friendlyName))
                                {
                                    return friendlyName;
                                }
                            }
                        }
                        finally
                        {
                            SetupDiDeleteDeviceInterfaceData(deviceInfoHandle, ref deviceInterfaceData);
                        }
                    }
                }
                finally
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoHandle);
                }
            }
            return string.Empty;
        }


        private static bool TryGetDeviceFriendlyName(IntPtr aDeviceInfoHandle, SP_DEVINFO_DATA aDeviceInfoData, RegPropertyType regProperty, out string aFriendlyName)
        {
            byte[] valueBuffer = new byte[255];
            int propertyRegDataType = 0;
            var friendlyNameByteSize = 255;

            aFriendlyName = "";

            // Get the size of the friendly device name
           
            var success = SetupDiGetDeviceRegistryProperty(
                aDeviceInfoHandle,     // handle to device information set
                ref aDeviceInfoData,       // pointer to SP_DEVINFO_DATA structure
                regProperty,           // property to be retrieved
                out propertyRegDataType,   // pointer to variable that receives the data type of the property
                valueBuffer,                   // pointer to PropertyBuffer that receives the property
                friendlyNameByteSize,                     // size, in bytes, of the PropertyBuffer buffer.
                out friendlyNameByteSize);
            var error = Marshal.GetLastWin32Error();
            if (success)
            {
                aFriendlyName = System.Text.UTF8Encoding.Default.GetString(valueBuffer,0,friendlyNameByteSize-1);
                DebugLogger.WriteLine(aFriendlyName);
                return success;
            }

            throw new Win32Exception(error);
        }
        #endregion

    }

}
