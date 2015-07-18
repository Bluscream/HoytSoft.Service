/*
 HoytSoft Self installing .NET service using the Win32 API
 by Dror Gluska
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoytSoft.ServiceBase.Base.Notifications
{
    public static class SessionNotification
    {
        /// <summary>
        /// Translated Session Event Info
        /// </summary>
        public struct SessionEventInfo
        {
            public SessionChangeReason? ChangeReason;
            public int SessionId;
            public SessionEventData? EventData;
            public bool? IsRemotelyControlled;
        }

        
        /// <summary>
        /// Session Event Data
        /// </summary>
        public struct SessionEventData
        {
            /// <summary>
            /// string that contains the name of the initial program that Remote Desktop Services runs when the user logs on.
            /// </summary>
            public string WTSInitialProgram;
            /// <summary>
            /// string that contains the published name of the application that the session is running.
            /// </summary>
            public string WTSApplicationName;
            /// <summary>
            /// string that contains the default directory used when launching the initial program.
            /// </summary>
            public string WTSWorkingDirectory;
            /// <summary>
            /// This value is not used.
            /// </summary>
            public string WTSOEMId;
            /// <summary>
            /// contains the session identifier.
            /// </summary>
            public uint? WTSSessionId;
            /// <summary>
            /// string that contains the name of the user associated with the session.
            /// </summary>
            public string WTSUserName;
            /// <summary>
            /// string that contains the name of the Remote Desktop Services session.
            /// </summary>
            public string WTSWinStationName;
            /// <summary>
            /// string that contains the name of the domain to which the logged-on user belongs.
            /// </summary>
            public string WTSDomainName;
            /// <summary>
            /// The session's current connection state.
            /// </summary>
            public WTS_CONNECTSTATE_CLASS? WTSConnectState;
            /// <summary>
            /// contains the build number of the client.
            /// </summary>
            public uint? WTSClientBuildNumber;
            /// <summary>
            /// the name of the client.
            /// </summary>
            public string WTSClientName;
            /// <summary>
            /// contains the directory in which the client is installed.
            /// </summary>
            public string WTSClientDirectory;
            /// <summary>
            /// client-specific product identifier.
            /// </summary>
            public ushort? WTSClientProductId;
            /// <summary>
            /// contains a client-specific hardware identifier.
            /// </summary>
            public ulong? WTSClientHardwareId;
            /// <summary>
            /// The network type and network address of the client
            /// </summary>
            public WTS_CLIENT_ADDRESS? WTSClientAddress;
            /// <summary>
            /// Information about the display resolution of the client. 
            /// </summary>
            public WTS_CLIENT_DISPLAY? WTSClientDisplay;
            /// <summary>
            /// the protocol type for the session. 
            /// </summary>
            public WTSClientProtocolType? WTSClientProtocolType;
            /// <summary>
            /// This value returns FALSE.
            /// </summary>
            public string WTSIdleTime;
            /// <summary>
            /// This value returns FALSE.
            /// </summary>
            public string WTSLogonTime;
            /// <summary>
            /// This value returns FALSE.
            /// </summary>
            public string WTSIncomingBytes;
            /// <summary>
            /// This value returns FALSE.
            /// </summary>
            public string WTSOutgoingBytes;
            /// <summary>
            /// This value returns FALSE.
            /// </summary>
            public string WTSIncomingFrames;
            /// <summary>
            /// This value returns FALSE.
            /// </summary>
            public string WTSOutgoingFrames;
            /// <summary>
            /// Information about a Remote Desktop Connection (RDC) client.
            /// </summary>
            public WTSCLIENT? WTSClientInfo;
            /// <summary>
            /// Information about a client session on an RD Session Host server. 
            /// </summary>
            public WTSINFO? WTSSessionInfo;
            /// <summary>
            /// Extended information about a session on an RD Session Host server.
            /// </summary>
            public WTSINFOEX? WTSSessionInfoEx;
            /// <summary>
            /// Information about the configuration of an RD Session Host server.
            /// </summary>
            public string WTSConfigInfo;
            /// <summary>
            /// This value is not supported.
            /// </summary>
            public string WTSValidationInfo;
            /// <summary>
            ///  IPv4 address assigned to the session. If the session does not have a virtual IP address, the WTSQuerySessionInformation function returns ERROR_NOT_SUPPORTED.
            /// </summary>
            public WTS_SESSION_ADDRESS? WTSSessionAddressV4;
            /// <summary>
            /// Determines whether the current session is a remote session. The WTSQuerySessionInformation function returns a value of TRUE to indicate that the current session is a remote session, and FALSE to indicate that the current session is a local session. This value can only be used for the local machine, so the hServer parameter of the WTSQuerySessionInformation function must contain WTS_CURRENT_SERVER_HANDLE.
            /// </summary>
            public bool? WTSIsRemoteSession;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                if (!string.IsNullOrEmpty(WTSInitialProgram)) { sb.AppendFormat(" WTSInitialProgram {0}", WTSInitialProgram); }
                if (!string.IsNullOrEmpty(WTSApplicationName)) { sb.AppendFormat(" WTSApplicationName {0}", WTSApplicationName); }
                if (!string.IsNullOrEmpty(WTSWorkingDirectory)) { sb.AppendFormat(" WTSWorkingDirectory {0}", WTSWorkingDirectory); }
                if (!string.IsNullOrEmpty(WTSOEMId)) { sb.AppendFormat(" WTSOEMId {0}", WTSOEMId); }
                if (WTSSessionId != null) { sb.AppendFormat(" WTSSessionId {0}", WTSSessionId); }
                if (!string.IsNullOrEmpty(WTSUserName)) { sb.AppendFormat(" WTSUserName {0}", WTSUserName); }
                if (!string.IsNullOrEmpty(WTSWinStationName)) { sb.AppendFormat(" WTSWinStationName {0}", WTSWinStationName); }
                if (!string.IsNullOrEmpty(WTSDomainName)) { sb.AppendFormat(" WTSDomainName {0}", WTSDomainName); }
                if (WTSConnectState != null) { sb.AppendFormat(" WTSConnectState {0}", WTSConnectState); }
                if (WTSClientBuildNumber != null) { sb.AppendFormat(" WTSClientBuildNumber {0}", WTSClientBuildNumber); }
                if (!string.IsNullOrEmpty(WTSClientName)) { sb.AppendFormat(" WTSClientName {0}", WTSClientName); }
                if (!string.IsNullOrEmpty(WTSClientDirectory)) { sb.AppendFormat(" WTSClientDirectory {0}", WTSClientDirectory); }
                if (WTSClientProductId != null) { sb.AppendFormat(" WTSClientProductId {0}", WTSClientProductId); }
                if (WTSClientHardwareId != null) { sb.AppendFormat(" WTSClientHardwareId {0}", WTSClientHardwareId); }
                if (WTSClientAddress != null) { sb.AppendFormat(" WTSClientAddress {0}", WTSClientAddress); }
                if (WTSClientDisplay != null) { sb.AppendFormat(" WTSClientDisplay {0}", WTSClientDisplay); }
                if (WTSClientProtocolType != null) { sb.AppendFormat(" WTSClientProtocolType {0}", WTSClientProtocolType); }
                if (!string.IsNullOrEmpty(WTSIdleTime)) { sb.AppendFormat(" WTSIdleTime {0}", WTSIdleTime); }
                if (!string.IsNullOrEmpty(WTSLogonTime)) { sb.AppendFormat(" WTSLogonTime {0}", WTSLogonTime); }
                if (!string.IsNullOrEmpty(WTSIncomingBytes)) { sb.AppendFormat(" WTSIncomingBytes {0}", WTSIncomingBytes); }
                if (!string.IsNullOrEmpty(WTSOutgoingBytes)) { sb.AppendFormat(" WTSOutgoingBytes {0}", WTSOutgoingBytes); }
                if (!string.IsNullOrEmpty(WTSIncomingFrames)) { sb.AppendFormat(" WTSIncomingFrames {0}", WTSIncomingFrames); }
                if (!string.IsNullOrEmpty(WTSOutgoingFrames)) { sb.AppendFormat(" WTSOutgoingFrames {0}", WTSOutgoingFrames); }
                if (WTSClientInfo != null) { sb.AppendFormat(" WTSClientInfo {0}", WTSClientInfo); }
                if (WTSSessionInfo != null) { sb.AppendFormat(" WTSSessionInfo {0}", WTSSessionInfo); }
                if (WTSSessionInfoEx != null) { sb.AppendFormat(" WTSSessionInfoEx {0}", WTSSessionInfoEx); }
                if (!string.IsNullOrEmpty(WTSConfigInfo)) { sb.AppendFormat(" WTSConfigInfo {0}", WTSConfigInfo); }
                if (!string.IsNullOrEmpty(WTSValidationInfo)) { sb.AppendFormat(" WTSValidationInfo {0}", WTSValidationInfo); }
                if (WTSSessionAddressV4 != null) { sb.AppendFormat(" WTSSessionAddressV4 {0}", WTSSessionAddressV4); }
                if (WTSIsRemoteSession != null) { sb.AppendFormat(" WTSIsRemoteSession {0}", WTSIsRemoteSession); }

                return sb.ToString();
            }
        }

        /// <summary>
        /// A USHORT value that specifies information about the protocol type for the session.
        /// </summary>
        public enum WTSClientProtocolType : ushort
        {
            /// <summary>
            /// The console session.
            /// </summary>
            Console = 0,
            /// <summary>
            /// This value is retained for legacy purposes.
            /// </summary>
            Legacy = 1,
            /// <summary>
            /// The RDP protocol.
            /// </summary>
            RDP = 2
        }

        /// <summary>
        /// The state of the session
        /// </summary>
        public enum SessionStateFlags : uint
        {
            /// <summary>
            /// The session state is not known.
            /// </summary>
             WTS_SESSIONSTATE_UNKNOWN  =  0xFFFFFFFF,
            /// <summary>
             /// The session is locked.
            /// </summary>
             WTS_SESSIONSTATE_LOCK     =  0x00000000,
            /// <summary>
             /// The session is unlocked.
            /// </summary>
             WTS_SESSIONSTATE_UNLOCK   =  0x00000001
        }

        /// <summary>
        /// Contains extended information about a Remote Desktop Services session.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct WTSINFOEX_LEVEL1
        {
            /// <summary>
            /// The session identifier.
            /// </summary>
            public int SessionId;
            /// <summary>
            /// A value of the WTS_CONNECTSTATE_CLASS enumeration type that specifies the connection state of a Remote Desktop Services session.
            /// </summary>
            public WTS_CONNECTSTATE_CLASS SessionState;
            /// <summary>
            /// The state of the session.
            /// </summary>
            public SessionStateFlags SessionFlags;

            /// <summary>
            /// string that contains the name of the window station for the session.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
            public string WinStationName;

            /// <summary>
            /// string that contains the name of the user who owns the session.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
            public string UserName;

            /// <summary>
            /// string that contains the name of the domain that the user belongs to.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
            public string DomainName;

            public int Unknown;

            /// <summary>
            /// The time that the user logged on to the session. This value is stored as a large integer that represents the number of 100-nanosecond intervals since January 1, 1601 Coordinated Universal Time (Greenwich Mean Time).
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            public Int64 LogonTime;

            /// <summary>
            /// The time of the most recent client connection to the session. This value is stored as a large integer that represents the number of 100-nanosecond intervals since January 1, 1601 Coordinated Universal Time.
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            public Int64 ConnectTime;
            /// <summary>
            /// The time of the most recent client disconnection to the session. This value is stored as a large integer that represents the number of 100-nanosecond intervals since January 1, 1601 Coordinated Universal Time.
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            public Int64 DisconnectTime;
            /// <summary>
            /// The time of the last user input in the session. This value is stored as a large integer that represents the number of 100-nanosecond intervals since January 1, 1601 Coordinated Universal Time.
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            public Int64 LastInputTime;

            /// <summary>
            /// The time that this structure was filled. This value is stored as a large integer that represents the number of 100-nanosecond intervals since January 1, 1601 Coordinated Universal Time.
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            public Int64 CurrentTime;

            /// <summary>
            /// The number of bytes of uncompressed Remote Desktop Protocol (RDP) data sent from the client to the server since the client connected.
            /// </summary>
            public int IncomingBytes;

            /// <summary>
            /// The number of bytes of uncompressed RDP data sent from the server to the client since the client connected.
            /// </summary>
            public int OutgoingBytes;

            /// <summary>
            /// The number of frames of RDP data sent from the client to the server since the client connected.
            /// </summary>
            public int IncomingFrames;

            /// <summary>
            /// The number of frames of RDP data sent from the server to the client since the client connected.
            /// </summary>
            public int OutgoingFrames;

            /// <summary>
            /// The number of bytes of compressed RDP data sent from the client to the server since the client connected.
            /// </summary>
            public int IncomingCompressedBytes;

            /// <summary>
            /// The number of bytes of compressed RDP data sent from the client to the server since the client connected.
            /// </summary>
            public int OutgoingCompressedBytes;

            /// <summary>
            /// The time that the user logged on to the session
            /// </summary>
            public DateTime LogonTimeAsDateTime
            {
                get{return DateTime.FromFileTime(this.LogonTime);}
            }

            /// <summary>
            /// The time of the most recent client connection to the session. 
            /// </summary>
            public DateTime ConnectTimeAsDateTime
            {
                get { return DateTime.FromFileTime(this.ConnectTime); }
            }

            /// <summary>
            /// The time of the most recent client disconnection to the session.
            /// </summary>
            public DateTime DisconnectTimeAsDateTime
            {
                get { return DateTime.FromFileTime(this.DisconnectTime); }
            }

            /// <summary>
            /// The time of the last user input in the session. 
            /// </summary>
            public DateTime LastInputTimeAsDateTime
            {
                get { return DateTime.FromFileTime(this.LastInputTime); }
            }

            /// <summary>
            /// The time that this structure was filled.
            /// </summary>
            public DateTime CurrentTimeAsDateTime
            {
                get { return DateTime.FromFileTime(this.CurrentTime); }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(" SessionId: {0}",SessionId);
                sb.AppendFormat(" SessionState: {0}", SessionState);
                sb.AppendFormat(" SessionFlags: {0}", SessionFlags);
                sb.AppendFormat(" WinStationName: {0}", WinStationName);
                sb.AppendFormat(" UserName: {0}\\{0}", DomainName,UserName);
                sb.AppendFormat(" LogonTime: {0}", LogonTimeAsDateTime);
                sb.AppendFormat(" ConnectTime: {0}", ConnectTimeAsDateTime);
                sb.AppendFormat(" DisconnectTime: {0}", DisconnectTimeAsDateTime);
                sb.AppendFormat(" LastInputTime: {0}", LastInputTimeAsDateTime);
                sb.AppendFormat(" CurrentTime: {0}", CurrentTimeAsDateTime);
                sb.AppendFormat(" IncomingBytes: {0}", IncomingBytes);
                sb.AppendFormat(" OutgoingBytes: {0}", OutgoingBytes);
                sb.AppendFormat(" IncomingFrames: {0}",IncomingFrames);
                sb.AppendFormat(" OutgoingFrames: {0}", OutgoingFrames);
                sb.AppendFormat(" IncomingCompressedBytes: {0}",IncomingCompressedBytes);
                sb.AppendFormat(" OutgoingCompressedBytes: {0}",OutgoingCompressedBytes);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Contains a WTSINFOEX_LEVEL union that contains extended information about a Remote Desktop Services session. This structure is returned by the WTSQuerySessionInformation function when you specify "WTSSessionInfoEx" for the WTSInfoClass parameter.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WTSINFOEX
        {
            /// <summary>
            /// Specifies the level of information contained in the Data member. This can be the following value.
            /// </summary>
            public int Level;
            public int Unknown;
            /// <summary>
            /// A WTSINFOEX_LEVEL union. The type of structure contained here is specified by the Level member.
            /// </summary>
            public WTSINFOEX_LEVEL1 Data;

            public override string ToString()
            {
 	             StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Level: {0}",Level);
                sb.AppendFormat(" Data: {0}",Data);
                return sb.ToString();
            }
        }


        public enum AddressFamilyType : uint
        {
            AF_INET = 2,
            AF_INET6 = 23,
            AF_NS = 6,
            AF_IPX = AF_NS,
            AF_NETBIOS = 17,
            AF_UNSPEC = 0
        }

        public struct WTS_SESSION_ADDRESS {
            public uint AddressFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Address;

            //public override string ToString()
            //{
            //    switch (this.AddressFamily)
            //    {
            //        case AddressFamilyType.AF_INET:
            //            return System.Text.UTF8Encoding.UTF8.GetString(this.Address);
            //        case AddressFamilyType.AF_INET6:
            //            return (new IPAddress(Address)).ToString();
            //        default:
            //            System.Diagnostics.Debug.Fail("address family not implemented");
            //            break;
            //    }
            //    return "";
            //}
        } 


        public const int WINSTATIONNAME_LENGTH = 32;
        public const int DOMAIN_LENGTH = 17;
        public const int USERNAME_LENGTH = 20;
        public static readonly IntPtr WTS_CURRENT_SERVER = IntPtr.Zero;

        /// <summary>
        /// Contains information about a Remote Desktop Services session.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct WTSINFO
        {
            /// <summary>
            /// A value of the WTS_CONNECTSTATE_CLASS enumeration type that indicates the session's current connection state.
            /// </summary>
            public WTS_CONNECTSTATE_CLASS State;
            /// <summary>
            /// The session identifier.
            /// </summary>
            public UInt32 SessionId;
            /// <summary>
            /// Uncompressed Remote Desktop Protocol (RDP) data from the client to the server.
            /// </summary>
            public UInt32 IncomingBytes;
            /// <summary>
            /// Uncompressed RDP data from the server to the client.
            /// </summary>
            public UInt32 OutgoingBytes;
            /// <summary>
            /// The number of frames of RDP data sent from the client to the server since the client connected.
            /// </summary>
            public UInt32 IncomingFrames;
            /// <summary>
            /// The number of frames of RDP data sent from the server to the client since the client connected.
            /// </summary>
            public UInt32 OutgoingFrames;
            /// <summary>
            /// Compressed RDP data from the client to the server.
            /// </summary>
            public UInt32 IncomingCompressedBytes;
            /// <summary>
            /// Compressed RDP data from the server to the client.
            /// </summary>
            public UInt32 OutgoingCompressedBytes;

            /// <summary>
            /// string that contains the name of the WinStation for the session.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = WINSTATIONNAME_LENGTH)]
            public String WinStationName;
            /// <summary>
            /// string that contains the name of the domain that the user belongs to.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DOMAIN_LENGTH)]
            public String Domain;
            /// <summary>
            /// A null-terminated string that contains the name of the user who owns the session.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = USERNAME_LENGTH + 1)]
            public String UserName;

            /// <summary>
            /// The most recent client connection time, use ConnectTimeAsDateTime
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            public Int64 ConnectTime;

            /// <summary>
            /// The last client disconnection time, use DisconnectTimeAsDateTime
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            public Int64 DisconnectTime;

            /// <summary>
            /// The time of the last user input in the session, use LastInputTimeAsDateTime
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            public Int64 LastInputTime;
            
            /// <summary>
            /// The time that the user logged on to the session, use LogonTimeAsDateTime
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            public Int64 LogonTime;

            /// <summary>
            /// The time that the WTSINFO data structure was called, use CurrentTimeAsDateTime
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            public Int64 CurrentTime;

            /// <summary>
            /// The time that the user logged on to the session.
            /// </summary>
            public DateTime? LogonTimeAsDateTime
            {
                get { return (this.LogonTime > 0) ? DateTime.FromFileTime(this.LogonTime) : (DateTime?)null; }
            }

            /// <summary>
            /// The most recent client connection time.
            /// </summary>
            public DateTime? ConnectTimeAsDateTime
            {
                get { return (this.ConnectTime > 0) ? DateTime.FromFileTime(this.ConnectTime) : (DateTime?)null; }
            }

            /// <summary>
            /// The last client disconnection time.
            /// </summary>
            public DateTime? DisconnectTimeAsDateTime
            {
                get { return (this.DisconnectTime > 0) ? DateTime.FromFileTime(this.DisconnectTime) : (DateTime?)null; }
            }

            /// <summary>
            /// The time of the last user input in the session.
            /// </summary>
            public DateTime? LastInputTimeAsDateTime
            {
                get { return DateTime.FromFileTime(this.LastInputTime); }
            }

            /// <summary>
            /// The time that the WTSINFO data structure was called.
            /// </summary>
            public DateTime? CurrentTimeAsDateTime
            {
                get { return DateTime.FromFileTime(this.CurrentTime); }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(" State: {0}", State);
                sb.AppendFormat(" SessionId: {0}",SessionId);
                sb.AppendFormat(" IncomingBytes: {0}", IncomingBytes);
                sb.AppendFormat(" OutgoingBytes: {0}", OutgoingBytes);

                sb.AppendFormat(" IncomingFrames: {0}", IncomingFrames);
                sb.AppendFormat(" OutgoingFrames: {0}", OutgoingFrames);

                sb.AppendFormat(" IncomingCompressedBytes: {0}", IncomingCompressedBytes);
                sb.AppendFormat(" OutgoingCompressedBytes: {0}", OutgoingCompressedBytes);


                sb.AppendFormat(" WinStationName: {0}", WinStationName);
                sb.AppendFormat(" UserName: {0}\\{0}", Domain,UserName);

                sb.AppendFormat(" ConnectTime: {0}", ConnectTimeAsDateTime);
                sb.AppendFormat(" DisconnectTime: {0}", DisconnectTimeAsDateTime);
                sb.AppendFormat(" LastInputTime: {0}", LastInputTimeAsDateTime);
                sb.AppendFormat(" LogonTime: {0}", LogonTimeAsDateTime);
                sb.AppendFormat(" CurrentTime: {0}", CurrentTimeAsDateTime);

                return sb.ToString();
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_CLIENT_ADDRESS
        {
            public AddressFamilyType AddressFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Address;

            public override string ToString()
            {
                switch (this.AddressFamily)
                {
                    case AddressFamilyType.AF_INET:
                        return System.Text.UTF8Encoding.UTF8.GetString(this.Address);
                    case AddressFamilyType.AF_INET6:
                        return (new IPAddress(Address)).ToString();
                    default:
                        System.Diagnostics.Debug.Fail("address family not implemented");
                        break;
                }
                return "";
            }
        }

        /// <summary>
        /// Color depth of the client's display enum
        /// </summary>
        public enum ColorDepthType : uint
        {
            /// <summary>
            /// 4 bits per pixel.
            /// </summary>
            BPP_4 = 1,
            /// <summary>
            /// 8 bits per pixel.
            /// </summary>
            BPP_8 = 2,
            /// <summary>
            /// 16 bits per pixel.
            /// </summary>
            BPP_16 = 4,
            /// <summary>
            /// A 3-byte RGB values for a maximum of 2^24 colors.
            /// </summary>
            RGB_24 = 8,
            /// <summary>
            /// 15 bits per pixel.
            /// </summary>
            BPP_15 = 16,
            /// <summary>
            /// 24 bits per pixel.
            /// </summary>
            BPP_24 = 24,
            /// <summary>
            /// 32 bits per pixel.
            /// </summary>
            BPP_32 = 32
        }

        /// <summary>
        /// Contains information about the display of a Remote Desktop Connection (RDC) client.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_CLIENT_DISPLAY
        {
            /// <summary>
            /// Horizontal dimension, in pixels, of the client's display.
            /// </summary>
            public uint HorizontalResolution;
            /// <summary>
            /// Vertical dimension, in pixels, of the client's display.
            /// </summary>
            public uint VerticalResolution;
            /// <summary>
            /// Color depth of the client's display. 
            /// </summary>
            public ColorDepthType ColorDepth;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("Resolution: {0}x{1}x{2}", HorizontalResolution,VerticalResolution, ColorDepth);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Contains information about a Remote Desktop Connection (RDC) client.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WTSCLIENT
        {
            /// <summary>
            /// The NetBIOS name of the client computer.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
            public String ClientName;

            /// <summary>
            /// The client domain name.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
            public String Domain;

            /// <summary>
            /// The client user name.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
            public String UserName;

            /// <summary>
            /// The folder for the initial program.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
            public String WorkDirectory;

            /// <summary>
            /// The program to start on connection.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
            public String InitialProgram;

            /// <summary>
            /// The security level of encryption.
            /// </summary>
            public byte EncryptionLevel;

            /// <summary>
            /// The address family. This member can be AF_INET, AF_INET6, AF_IPX, AF_NETBIOS, or AF_UNSPEC.
            /// </summary>
            public UInt32 ClientAddressFamily;

            /// <summary>
            /// The client network address.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
            public UInt16[] ClientAddress;

            /// <summary>
            /// Horizontal dimension, in pixels, of the client's display.
            /// </summary>
            public UInt16 HRes;

            /// <summary>
            /// Vertical dimension, in pixels, of the client's display.
            /// </summary>
            public UInt16 VRes;

            /// <summary>
            /// Color depth of the client's display. 
            /// </summary>
            public ColorDepthType ColorDepth;

            /// <summary>
            /// The location of the client ActiveX control DLL.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
            public String ClientDirectory;

            /// <summary>
            /// The client build number.
            /// </summary>
            public UInt32 BuildNumber;

            /// <summary>
            /// Reserved.
            /// </summary>
            public UInt32 HardwareId;

            /// <summary>
            /// Reserved.
            /// </summary>
            public UInt16 ProductId;

            /// <summary>
            /// The number of output buffers on the server per session.
            /// </summary>
            public UInt16 OutBufCountHost;

            /// <summary>
            /// The number of output buffers on the client.
            /// </summary>
            public UInt16 OutBufCountClient;

            /// <summary>
            /// The length of the output buffers, in bytes.
            /// </summary>
            public UInt16 OutBufLengh;

            /// <summary>
            /// The device ID of the network adapter.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 261)]
            public String DeviceId;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("ClientName: {0}", ClientName);
                sb.AppendFormat(" User: {0}\\{1}", Domain, UserName);
                sb.AppendFormat(" WorkDirectory: {0}", WorkDirectory);
                sb.AppendFormat(" InitialProgram: {0}", InitialProgram);
                sb.AppendFormat(" EncryptionLevel: {0}", EncryptionLevel);
                sb.AppendFormat(" ClientAddressFamily: {0}", ClientAddressFamily);
                sb.AppendFormat(" ClientAddress: {0}", ClientAddress);
                sb.AppendFormat(" Resolution: {0}x{1}x{2}", HRes, VRes, ColorDepth);
                sb.AppendFormat(" ClientDirectory: {0}", ClientDirectory);
                sb.AppendFormat(" BuildNumber: {0}", BuildNumber);
                sb.AppendFormat(" ProductId: {0}", ProductId);
                sb.AppendFormat(" OutBufCountHost: {0}", OutBufCountHost);
                sb.AppendFormat(" OutBufCountClient: {0}", OutBufCountClient);
                sb.AppendFormat(" OutBufLengh: {0}", OutBufLengh);

                return sb.ToString();
            }
        } 


        /// <summary>
        /// the type of session information
        /// </summary>
        private enum WTS_INFO_CLASS
        {
            /// <summary>
            /// A null-terminated string that contains the name of the initial program that Remote Desktop Services runs when the user logs on.
            /// </summary>
            WTSInitialProgram = 0,
            /// <summary>
            /// A null-terminated string that contains the published name of the application that the session is running.
            /// </summary>
            WTSApplicationName = 1,
            /// <summary>
            /// A null-terminated string that contains the default directory used when launching the initial program.
            /// </summary>
            WTSWorkingDirectory = 2,
            /// <summary>
            /// This value is not used.
            /// </summary>
            WTSOEMId = 3,
            /// <summary>
            /// A ULONG value that contains the session identifier.
            /// </summary>
            WTSSessionId = 4,
            /// <summary>
            /// A null-terminated string that contains the name of the user associated with the session.
            /// </summary>
            WTSUserName = 5,
            /// <summary>
            /// A null-terminated string that contains the name of the Remote Desktop Services session.
            /// </summary>
            WTSWinStationName = 6,
            /// <summary>
            /// A null-terminated string that contains the name of the domain to which the logged-on user belongs.
            /// </summary>
            WTSDomainName = 7,
            /// <summary>
            /// The session's current connection state. For more information, see WTS_CONNECTSTATE_CLASS.
            /// </summary>
            WTSConnectState = 8,
            /// <summary>
            /// A ULONG value that contains the build number of the client.
            /// </summary>
            WTSClientBuildNumber = 9,
            /// <summary>
            /// A null-terminated string that contains the name of the client.
            /// </summary>
            WTSClientName = 10,
            /// <summary>
            /// A null-terminated string that contains the directory in which the client is installed.
            /// </summary>
            WTSClientDirectory = 11,
            /// <summary>
            /// A USHORT client-specific product identifier.
            /// </summary>
            WTSClientProductId = 12,
            /// <summary>
            /// A ULONG value that contains a client-specific hardware identifier. This option is reserved for future use. WTSQuerySessionInformation will always return a value of 0.
            /// </summary>
            WTSClientHardwareId = 13,
            /// <summary>
            /// The network type and network address of the client. For more information, see WTS_CLIENT_ADDRESS.
            /// </summary>
            WTSClientAddress = 14,
            /// <summary>
            /// Information about the display resolution of the client. For more information, see WTS_CLIENT_DISPLAY.
            /// </summary>
            WTSClientDisplay = 15,
            /// <summary>
            /// A USHORT value that specifies information about the protocol type for the session. 
            /// </summary>
            WTSClientProtocolType = 16,
            /// <summary>
            /// This value returns FALSE. If you call GetLastError to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
            /// </summary>
            WTSIdleTime = 17,
            /// <summary>
            /// This value returns FALSE. If you call GetLastError to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
            /// </summary>
            WTSLogonTime = 18,
            /// <summary>
            /// This value returns FALSE. If you call GetLastError to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
            /// </summary>
            WTSIncomingBytes = 19,
            /// <summary>
            /// This value returns FALSE. If you call GetLastError to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
            /// </summary>
            WTSOutgoingBytes = 20,
            /// <summary>
            /// This value returns FALSE. If you call GetLastError to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
            /// </summary>
            WTSIncomingFrames = 21,
            /// <summary>
            /// This value returns FALSE. If you call GetLastError to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
            /// </summary>
            WTSOutgoingFrames = 22,
            /// <summary>
            /// Information about a Remote Desktop Connection (RDC) client. For more information, see WTSCLIENT.
            /// </summary>
            WTSClientInfo = 23,
            /// <summary>
            /// Information about a client session on an RD Session Host server. For more information, see WTSINFO.
            /// </summary>
            WTSSessionInfo = 24,
            /// <summary>
            /// Extended information about a session on an RD Session Host server. For more information, see WTSINFOEX.
            /// </summary>
            WTSSessionInfoEx = 25,
            /// <summary>
            /// Information about the configuration of an RD Session Host server.
            /// </summary>
            WTSConfigInfo = 26,
            /// <summary>
            /// This value is not supported.
            /// </summary>
            WTSValidationInfo = 27,
            /// <summary>
            /// A WTS_SESSION_ADDRESS structure that contains the IPv4 address assigned to the session. If the session does not have a virtual IP address, the WTSQuerySessionInformation function returns ERROR_NOT_SUPPORTED.
            /// </summary>
            WTSSessionAddressV4 = 28,
            /// <summary>
            /// Determines whether the current session is a remote session. The WTSQuerySessionInformation function returns a value of TRUE to indicate that the current session is a remote session, and FALSE to indicate that the current session is a local session. This value can only be used for the local machine, so the hServer parameter of the WTSQuerySessionInformation function must contain WTS_CURRENT_SERVER_HANDLE.
            /// </summary>
            WTSIsRemoteSession = 29
        }

        /// <summary>
        /// connection state of a Remote Desktop Services session.
        /// </summary>
        public enum WTS_CONNECTSTATE_CLASS {
            /// <summary>
            /// User logged on to WinStation
            /// </summary>
            WTSActive = 0,             
            /// <summary>
            /// WinStation connected to client 
            /// </summary>
            WTSConnected = 1,           
            /// <summary>
            /// In the process of connecting to client 
            /// </summary>
            WTSConnectQuery = 2,        
            /// <summary>
            /// Shadowing another WinStation 
            /// </summary>
            WTSShadow = 3,              
            /// <summary>
            /// WinStation logged on without client 
            /// </summary>
            WTSDisconnected = 4,        
            /// <summary>
            /// Waiting for client to connect 
            /// </summary>
            WTSIdle = 5,                
            /// <summary>
            /// WinStation is listening for connection 
            /// </summary>
            WTSListen = 6,              
            /// <summary>
            /// WinStation is being reset 
            /// </summary>
            WTSReset = 7,               
            /// <summary>
            /// WinStation is down due to error 
            /// </summary>
            WTSDown = 8,                
            /// <summary>
            /// WinStation in initialization
            /// </summary>
            WTSInit  =9,                
        }

        /// <summary>
        /// Contains information about a client session on a Remote Desktop Session Host (RD Session Host) server.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct WTS_SESSION_INFO
        {
            /// <summary>
            /// Session identifier of the session.
            /// </summary>
            public uint SessionId;

            /// <summary>
            /// Pointer to a null-terminated string that contains the WinStation name of this session. The WinStation name is a name that Windows associates with the session, for example, "services", "console", or "RDP-Tcp#0".
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pWinStationName;

            /// <summary>
            /// A value from the WTS_CONNECTSTATE_CLASS enumeration type that indicates the session's current connection state.
            /// </summary>
            public WTS_CONNECTSTATE_CLASS State;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("SessionId: {0}", SessionId);
                sb.AppendFormat(" pWinStationName: {0}", pWinStationName);
                sb.AppendFormat(" State: {0}", State);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Retrieves session information for the specified session on the specified Remote Desktop Session Host (RD Session Host) server. It can be used to query session information on local and remote RD Session Host servers.
        /// </summary>
        /// <param name="hServer">A handle to an RD Session Host server. Specify a handle opened by the WTSOpenServer function, or specify WTS_CURRENT_SERVER_HANDLE to indicate the RD Session Host server on which your application is running.</param>
        /// <param name="sessionId">A Remote Desktop Services session identifier. To indicate the session in which the calling application is running (or the current session) specify WTS_CURRENT_SESSION. Only specify WTS_CURRENT_SESSION when obtaining session information on the local server. If WTS_CURRENT_SESSION is specified when querying session information on a remote server, the returned session information will be inconsistent. Do not use the returned data.
        /// <para>You can use the WTSEnumerateSessions function to retrieve the identifiers of all sessions on a specified RD Session Host server.</para>
        /// <para>To query information for another user's session, you must have Query Information permission. For more information, see Remote Desktop Services Permissions. To modify permissions on a session, use the Remote Desktop Services Configuration administrative tool.</param></para>
        /// <param name="wtsInfoClass">A value of the WTS_INFO_CLASS enumeration that indicates the type of session information to retrieve in a call to the WTSQuerySessionInformation function.</param>
        /// <param name="ppBuffer">A pointer to a variable that receives a pointer to the requested information. The format and contents of the data depend on the information class specified in the WTSInfoClass parameter. To free the returned buffer, call the WTSFreeMemory function.</param>
        /// <param name="pBytesReturned">A pointer to a variable that receives the size, in bytes, of the data returned in ppBuffer.</param>
        /// <returns>If the function succeeds, the return value is true</returns>
        [DllImport("Wtsapi32.dll", SetLastError = true)]
        static extern bool WTSQuerySessionInformation(
            IntPtr hServer,
            uint sessionId,
            WTS_INFO_CLASS wtsInfoClass,
            out IntPtr ppBuffer,
            out uint pBytesReturned
        );

        /// <summary>
        /// Opens a handle to the specified Remote Desktop Session Host (RD Session Host) server.
        /// </summary>
        /// <param name="pServerName">string specifying the NetBIOS name of the RD Session Host server.</param>
        /// <returns>If the function succeeds, the return value is a handle to the specified server.</returns>
        [DllImport("wtsapi32.dll")]
        static extern IntPtr WTSOpenServer([MarshalAs(UnmanagedType.LPStr)] String pServerName);

        /// <summary>
        /// Closes an open handle to a Remote Desktop Session Host (RD Session Host) server.
        /// </summary>
        /// <param name="hServer">A handle to an RD Session Host server opened by a call to the WTSOpenServer or WTSOpenServerEx function.
        /// <para>Do not pass WTS_CURRENT_SERVER_HANDLE for this parameter.</param></para>
        [DllImport("wtsapi32.dll")]
        static extern void WTSCloseServer(IntPtr hServer);

        /// <summary>
        /// you will get a list of sessions on the specified terminal server.
        /// </summary>
        /// <param name="hServer">[Input] I will specify a terminal server handle. WTSOpenServer use the handle to open the function. When you specify a terminal server running the application, you can use the WTS_CURRENT_SERVER_HANDLE.</param>
        /// <param name="Reserved">You are reserved. Please be sure to specify 0.</param>
        /// <param name="Version">I specify the version of [input] enumeration request. Please be sure to specify the 1.</param>
        /// <param name="ppSessionInfo">I specify a variable that receives a pointer to an array of [output] structure. To each structure in the array, contains information about the specified terminal server session. buffer information is returned, WTSFreeMemory Please be freed using the function.</param>
        /// <param name="pCount">[Output] PpSessionInfo parameter points are in the buffer, WTS_SESSION_INFO I specify a pointer to a variable that receives the number of structure.</param>
        /// <returns>If the function succeeds, the return value is nonzero. </returns>
        [DllImport("wtsapi32.dll")]
        static extern Int32 WTSEnumerateSessions(
            IntPtr hServer,
            [MarshalAs(UnmanagedType.U4)] Int32 Reserved,
            [MarshalAs(UnmanagedType.U4)] Int32 Version,
            ref IntPtr ppSessionInfo,
            [MarshalAs(UnmanagedType.U4)] ref Int32 pCount);

        /// <summary>
        /// Frees memory allocated by a Remote Desktop Services function.
        /// </summary>
        /// <param name="pMemory">Pointer to the memory to free.</param>
        [DllImport("wtsapi32.dll")]
        static extern void WTSFreeMemory(IntPtr pMemory);

        /// <summary>
        /// Target Session Notification
        /// </summary>
        public enum SessionTarget : int
        {
            /// <summary>
            /// Only session notifications involving the session attached to by the window identified by the hWnd parameter value are to be received.
            /// </summary>
            NOTIFY_FOR_THIS_SESSION = 0,
            /// <summary>
            /// All session notifications are to be received.
            /// </summary>
            NOTIFY_FOR_ALL_SESSIONS = 1
        }

        /// <summary>
        /// Registers the specified window to receive session change notifications.
        /// </summary>
        /// <param name="hServer">Handle of the server returned from WTSOpenServer or WTS_CURRENT_SERVER.</param>
        /// <param name="hWnd">Handle of the window to receive session change notifications.</param>
        /// <param name="dwFlags">Specifies which session notifications are to be received. </param>
        /// <returns>If the function succeeds, the return value is TRUE. Otherwise, it is FALSE. To get extended error information, call GetLastError.</returns>
        [DllImport("wtsapi32.dll", SetLastError = true)]
        internal static extern bool WTSRegisterSessionNotificationEx(IntPtr hServer, IntPtr hWnd, SessionTarget dwFlags);


        /// <summary>
        /// Unregisters the specified window so that it receives no further session change notifications.
        /// </summary>
        /// <param name="hServer">Handle of the server returned from WTSOpenServer or WTS_CURRENT_SERVER.</param>
        /// <param name="hWnd">Handle of the window to be unregistered from receiving session notifications.</param>
        /// <returns>If the function succeeds, the return value is TRUE. Otherwise, it is FALSE. To get extended error information, call GetLastError.</returns>
        [DllImport("wtsapi32.dll", SetLastError=true)]
        internal static extern bool WTSUnRegisterSessionNotificationEx(IntPtr hServer, IntPtr hWnd);

        /// <summary>
        /// Retrieves the Remote Desktop Services session that is currently attached to the physical console. The physical console is the monitor, keyboard, and mouse. Note that it is not necessary that Remote Desktop Services be running for this function to succeed.
        /// </summary>
        /// <returns>The session identifier of the session that is attached to the physical console. If there is no session attached to the physical console, (for example, if the physical console session is in the process of being attached or detached), this function returns 0xFFFFFFFF.</returns>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal static extern int WTSGetActiveConsoleSessionId();

        /// <summary>
        /// Retrieves session information for the specified session on the specified Remote Desktop Session Host (RD Session Host) server. It can be used to query session information on local and remote RD Session Host servers.
        /// </summary>
        /// <param name="hServer">A handle to an RD Session Host server. Specify a handle opened by the WTSOpenServer function, or specify WTS_CURRENT_SERVER_HANDLE to indicate the RD Session Host server on which your application is running.</param>
        /// <param name="sessionId">A Remote Desktop Services session identifier. To indicate the session in which the calling application is running (or the current session) specify WTS_CURRENT_SESSION. Only specify WTS_CURRENT_SESSION when obtaining session information on the local server. If WTS_CURRENT_SESSION is specified when querying session information on a remote server, the returned session information will be inconsistent. Do not use the returned data.
        /// <para>You can use the WTSEnumerateSessions function to retrieve the identifiers of all sessions on a specified RD Session Host server.</para>
        /// <para>To query information for another user's session, you must have Query Information permission. For more information, see Remote Desktop Services Permissions. To modify permissions on a session, use the Remote Desktop Services Configuration administrative tool.</para></param>
        /// <param name="wtsInfoClass">A value of the WTS_INFO_CLASS enumeration that indicates the type of session information to retrieve in a call to the WTSQuerySessionInformation function.</param>
        /// <param name="ppBuffer">A pointer to a variable that receives a pointer to the requested information. The format and contents of the data depend on the information class specified in the WTSInfoClass parameter. To free the returned buffer, call the WTSFreeMemory function.</param>
        /// <param name="pBytesReturned">A pointer to a variable that receives the size, in bytes, of the data returned in ppBuffer.</param>
        /// <returns>If the function succeeds, the return value is true</returns>
        [DllImport("Wtsapi32.dll")]
        static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, out System.IntPtr ppBuffer, out uint pBytesReturned);

        private static T ChangeType<T>(object value)
        {
            var t = typeof(T);

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return default(T);
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return (T)Convert.ChangeType(value, t);
        }

        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }

        private static T QuerySessionInformation<T>(uint sessionId, WTS_INFO_CLASS info)
        {
            T retval = default(T);
            IntPtr valuePtr = IntPtr.Zero;
            uint bytes = 0;
            if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, info, out valuePtr, out bytes))
            {
                Type retvalType = typeof(T);
                bool isNullable = (retvalType.IsGenericType && retvalType.GetGenericTypeDefinition() == typeof(Nullable<>));
                if (isNullable)
                    retvalType = Nullable.GetUnderlyingType(retvalType);

                if (retvalType == typeof(string))
                {
                    retval = (T)(object)Marshal.PtrToStringAnsi(valuePtr);
                }else if (retvalType == typeof(uint) || retvalType == typeof(int))
                {
                    retval = ChangeType<T>(Marshal.ReadInt32(valuePtr));
                }
                else if (retvalType == typeof(ushort) || retvalType == typeof(short))
                {
                    retval = ChangeType<T>(Marshal.ReadInt16(valuePtr));
                }
                else if (retvalType == typeof(bool))
                {
                    retval = (T)(object)Marshal.ReadByte(valuePtr);
                }
                else if (retvalType.IsEnum) //enum
                {
                    retval = (T)Enum.Parse(retvalType, ChangeType<string>(Marshal.ReadInt32(valuePtr)));
                }
                else if (retvalType.IsValueType && !retvalType.IsPrimitive) //struct
                {
                    T val = (T)Marshal.PtrToStructure(valuePtr, retvalType);
                    retval = val;
                }
                
            }

            if (valuePtr != IntPtr.Zero)
            {
                WTSFreeMemory(valuePtr);
            }

            return retval;
        }

        /// <summary>
        /// Transforms a SessionId to SessionEventData
        /// </summary>
        /// <param name="SessionId"></param>
        /// <returns></returns>
        public static SessionEventData Transform(uint SessionId)
        {
            SessionEventData sei = new SessionEventData();
            sei.WTSApplicationName = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSApplicationName);

            sei.WTSInitialProgram = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSInitialProgram);
            sei.WTSApplicationName = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSApplicationName);
            sei.WTSWorkingDirectory = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSWorkingDirectory);
            sei.WTSOEMId = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSOEMId);
            sei.WTSSessionId = QuerySessionInformation<uint?>(SessionId, WTS_INFO_CLASS.WTSSessionId);
            sei.WTSUserName = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSUserName);
            sei.WTSWinStationName = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSWinStationName);
            sei.WTSDomainName = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSDomainName);
            sei.WTSConnectState = QuerySessionInformation<WTS_CONNECTSTATE_CLASS?>(SessionId, WTS_INFO_CLASS.WTSConnectState);
            sei.WTSClientBuildNumber = QuerySessionInformation<uint?>(SessionId, WTS_INFO_CLASS.WTSClientBuildNumber);
            sei.WTSClientName = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSClientName);
            sei.WTSClientDirectory = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSClientDirectory);
            sei.WTSClientProductId = QuerySessionInformation<ushort?>(SessionId, WTS_INFO_CLASS.WTSClientProductId);
            sei.WTSClientHardwareId = QuerySessionInformation<ulong?>(SessionId, WTS_INFO_CLASS.WTSClientHardwareId);
            sei.WTSClientAddress = QuerySessionInformation<WTS_CLIENT_ADDRESS?>(SessionId, WTS_INFO_CLASS.WTSClientAddress);
            sei.WTSClientDisplay = QuerySessionInformation<WTS_CLIENT_DISPLAY?>(SessionId, WTS_INFO_CLASS.WTSClientDisplay);
            sei.WTSClientProtocolType = QuerySessionInformation<WTSClientProtocolType?>(SessionId, WTS_INFO_CLASS.WTSClientProtocolType);
            sei.WTSIdleTime = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSIdleTime);
            sei.WTSLogonTime = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSLogonTime);
            sei.WTSIncomingBytes = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSIncomingBytes);
            sei.WTSOutgoingBytes = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSOutgoingBytes);
            sei.WTSIncomingFrames = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSIncomingFrames);
            sei.WTSOutgoingFrames = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSOutgoingFrames);
            sei.WTSClientInfo = QuerySessionInformation<WTSCLIENT?>(SessionId, WTS_INFO_CLASS.WTSClientInfo);
            sei.WTSSessionInfo = QuerySessionInformation<WTSINFO?>(SessionId, WTS_INFO_CLASS.WTSSessionInfo);
            sei.WTSSessionInfoEx = QuerySessionInformation<WTSINFOEX?>(SessionId, WTS_INFO_CLASS.WTSSessionInfoEx);
            sei.WTSConfigInfo = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSConfigInfo);
            sei.WTSValidationInfo = QuerySessionInformation<string>(SessionId, WTS_INFO_CLASS.WTSValidationInfo);
            sei.WTSSessionAddressV4 = QuerySessionInformation<WTS_SESSION_ADDRESS?>(SessionId, WTS_INFO_CLASS.WTSSessionAddressV4);
            sei.WTSIsRemoteSession = QuerySessionInformation<bool?>(SessionId, WTS_INFO_CLASS.WTSIsRemoteSession);

            return sei;
        }

        const int WM_WTSSESSION_CHANGE = 0x2b1;

        public enum SessionChangeReason
        {
            /// <summary>
            /// The session identified by lParam was connected to the console terminal or RemoteFX session.
            /// </summary>
            WTS_CONSOLE_CONNECT = 0x1,
            /// <summary>
            /// The session identified by lParam was disconnected from the console terminal or RemoteFX session.
            /// </summary>
            WTS_CONSOLE_DISCONNECT = 0x2,

            /// <summary>
            /// The session identified by lParam was connected to the remote terminal.
            /// </summary>
            WTS_REMOTE_CONNECT = 0x3,

            /// <summary>
            /// The session identified by lParam was disconnected from the remote terminal.
            /// </summary>
            WTS_REMOTE_DISCONNECT = 0x4,

            /// <summary>
            /// A user has logged on to the session identified by lParam.
            /// </summary>
            WTS_SESSION_LOGON = 0x5,

            /// <summary>
            /// A user has logged off the session identified by lParam.
            /// </summary>
            WTS_SESSION_LOGOFF = 0x6,

            /// <summary>
            /// The session identified by lParam has been locked.
            /// </summary>
            WTS_SESSION_LOCK = 0x7,

            /// <summary>
            /// The session identified by lParam has been unlocked.
            /// </summary>
            WTS_SESSION_UNLOCK = 0x8,

            /// <summary>
            /// The session identified by lParam has changed its remote controlled status. To determine the status, call GetSystemMetrics and check the SM_REMOTECONTROL metric.
            /// </summary>
            WTS_SESSION_REMOTE_CONTROL = 0x9,

            /// <summary>
            /// Reserved for future use.
            /// </summary>
            WTS_SESSION_CREATE = 0xA,

            /// <summary>
            /// Reserved for future use.
            /// </summary>
            WTS_SESSION_TERMINATE = 0xB,

        }

        /// <summary>
        /// Transform a Window Message to SessionEventInfo
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static SessionEventInfo? TransformMessage(ref Message m)
        {
            if (m.Msg == WM_WTSSESSION_CHANGE)
            {
                switch ((SessionChangeReason)(int)m.WParam)
                {
                    case SessionChangeReason.WTS_CONSOLE_CONNECT:
                        return new SessionEventInfo { ChangeReason = (SessionChangeReason)(int)m.WParam, SessionId = (int)m.LParam, EventData = Transform((uint)m.LParam) };
                    case SessionChangeReason.WTS_CONSOLE_DISCONNECT:
                        return new SessionEventInfo { ChangeReason = (SessionChangeReason)(int)m.WParam, SessionId = (int)m.LParam, EventData = Transform((uint)m.LParam) };
                    case SessionChangeReason.WTS_REMOTE_CONNECT:
                        return new SessionEventInfo { ChangeReason = (SessionChangeReason)(int)m.WParam, SessionId = (int)m.LParam, EventData = Transform((uint)m.LParam) };
                    case SessionChangeReason.WTS_REMOTE_DISCONNECT:
                        return new SessionEventInfo { ChangeReason = (SessionChangeReason)(int)m.WParam, SessionId = (int)m.LParam, EventData = Transform((uint)m.LParam) };
                    case SessionChangeReason.WTS_SESSION_CREATE:
                        return new SessionEventInfo { ChangeReason = (SessionChangeReason)(int)m.WParam, SessionId = (int)m.LParam, EventData = Transform((uint)m.LParam) };
                    case SessionChangeReason.WTS_SESSION_LOCK:
                        return new SessionEventInfo { ChangeReason = (SessionChangeReason)(int)m.WParam, SessionId = (int)m.LParam, EventData = Transform((uint)m.LParam) };
                    case SessionChangeReason.WTS_SESSION_LOGOFF:
                        return new SessionEventInfo { ChangeReason = (SessionChangeReason)(int)m.WParam, SessionId = (int)m.LParam, EventData = Transform((uint)m.LParam) };
                    case SessionChangeReason.WTS_SESSION_LOGON:
                        return new SessionEventInfo { ChangeReason = (SessionChangeReason)(int)m.WParam, SessionId = (int)m.LParam, EventData = Transform((uint)m.LParam) };
                    case SessionChangeReason.WTS_SESSION_REMOTE_CONTROL:
                        var isRemotlyControlled = (SystemMetrics.GetSystemMetrics(SystemMetrics.MetricsType.SM_REMOTECONTROL) != 0) ? true : false;
                        return new SessionEventInfo { ChangeReason = (SessionChangeReason)(int)m.WParam, SessionId = (int)m.LParam, EventData = Transform((uint)m.LParam), IsRemotelyControlled=isRemotlyControlled };
                    case SessionChangeReason.WTS_SESSION_TERMINATE:
                        return new SessionEventInfo { ChangeReason = (SessionChangeReason)(int)m.WParam, SessionId = (int)m.LParam, EventData = Transform((uint)m.LParam) };
                    case SessionChangeReason.WTS_SESSION_UNLOCK:
                        return new SessionEventInfo { ChangeReason = (SessionChangeReason)(int)m.WParam, SessionId = (int)m.LParam, EventData = Transform((uint)m.LParam) };
                    default:
                        //TODO: decide error reporting, perhaps silently fail?
                        System.Diagnostics.Debug.Fail("Session Change Event is Invalid");
                        break;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves a list of sessions on a Remote Desktop Session Host (RD Session Host) server
        /// </summary>
        public static List<SessionEventData> ListSessions(string serverName)
        {
            var retval = new List<SessionEventData>();

            IntPtr serverHandle = WTSOpenServer(serverName);
            Int32 sessionCount = 0;
            IntPtr sessionInfoPtr = IntPtr.Zero;
            Int32 dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
            Int32 retVal = WTSEnumerateSessions(serverHandle, 0, 1, ref sessionInfoPtr, ref sessionCount);
            if (retVal != 0)
            {
                for (var i = 0; i < sessionCount; i++)
                {
                    WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure(sessionInfoPtr, typeof(WTS_SESSION_INFO));
                    sessionInfoPtr += dataSize;
                    
                    retval.Add(Transform(si.SessionId));
                }
            }

            if (serverHandle != IntPtr.Zero)
            {
                WTSCloseServer(serverHandle);
            }

            return retval;
        }

    }

}