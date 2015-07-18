/*
 HoytSoft Self installing .NET service using the Win32 API
 David Hoyt CPOL 2005
 
 Extended by Dror Gluska
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HoytSoft.ServiceBase.Base.Exceptions;
using HoytSoft.ServiceBase.Base.Logging;

namespace HoytSoft.ServiceBase.Base
{
    /// <summary>
    /// Service Installer
    /// </summary>
    public static class ServiceInstaller
    {
        //predefined standard access types
          public const int  DELETE                          = (0x00010000);
          public const int  READ_CONTROL                    = (0x00020000);
          public const int  WRITE_DAC                       = (0x00040000);
          public const int  WRITE_OWNER                     = (0x00080000);
          public const int  SYNCHRONIZE                     = (0x00100000);
          public const int  STANDARD_RIGHTS_REQUIRED        = (0x000F0000);
          public const int  STANDARD_RIGHTS_READ            = (READ_CONTROL);
          public const int  STANDARD_RIGHTS_WRITE           = (READ_CONTROL);
          public const int  STANDARD_RIGHTS_EXECUTE         = (READ_CONTROL);
          public const int  STANDARD_RIGHTS_ALL             = (0x001F0000);
          public const int  SPECIFIC_RIGHTS_ALL             = (0x0000FFFF);


          /// <summary>
          /// service start options
          /// </summary>
          public enum ServiceStartType : int
          {
              /// <summary>
              /// A device driver started by the system loader. This value is valid only for driver services.
              /// </summary>
              SERVICE_BOOT_START = 0x0,
              SERVICE_SYSTEM_START = 0x1,
              /// <summary>
              /// A service started automatically by the service control manager during system startup.
              /// </summary>
              SERVICE_AUTO_START = 0x2,
              /// <summary>
              /// A service started by the service control manager when a process calls the StartService function. 
              /// </summary>
              SERVICE_DEMAND_START = 0x3,
              /// <summary>
              /// A service that cannot be started. Attempts to start the service result in the error code ERROR_SERVICE_DISABLED.
              /// </summary>
              SERVICE_DISABLED = 0x4,
              SERVICESTARTTYPE_NO_CHANGE = ServicesAPI.SERVICE_NO_CHANGE
          }

          /// <summary>
          /// The severity of the error, and action taken, if this service fails to start.
          /// </summary>
          public enum ServiceErrorControl
          {
              /// <summary>
              /// The startup program ignores the error and continues the startup operation.
              /// </summary>
              SERVICE_ERROR_IGNORE = 0x00000000,
              /// <summary>
              /// The startup program logs the error in the event log but continues the startup operation.
              /// </summary>
              SERVICE_ERROR_NORMAL = 0x00000001,
              /// <summary>
              /// The startup program logs the error in the event log. If the last-known-good configuration is being started, the startup operation continues. Otherwise, the system is restarted with the last-known-good configuration.
              /// </summary>
              SERVICE_ERROR_SEVERE = 0x00000002,
              /// <summary>
              /// The startup program logs the error in the event log, if possible. If the last-known-good configuration is being started, the startup operation fails. Otherwise, the system is restarted with the last-known good configuration.
              /// </summary>
              SERVICE_ERROR_CRITICAL = 0x00000003
          }

        /// <summary>
        /// Access Rights for the Service Control Manager
        /// <remarks>See http://msdn.microsoft.com/en-us/library/windows/desktop/ms685981(v=vs.85).aspx#access_rights_for_the_service_control_manager</remarks>
        /// </summary>
        public enum ServiceControlManagerAccessType : int
        {
            /// <summary>Required to connect to the service control manager.</summary>
            SC_MANAGER_CONNECT = 0x1,
            /// <summary>Required to call the CreateService function to create a service object and add it to the database.</summary>
            SC_MANAGER_CREATE_SERVICE = 0x2,
            /// <summary>Required to call the EnumServicesStatus or EnumServicesStatusEx function to list the services that are in the database.Required to call the NotifyServiceStatusChange function to receive notification when any service is created or deleted.</summary>
            SC_MANAGER_ENUMERATE_SERVICE = 0x4,
            /// <summary>Required to call the LockServiceDatabase function to acquire a lock on the database.</summary>
            SC_MANAGER_LOCK = 0x8,
            /// <summary>Required to call the QueryServiceLockStatus function to retrieve the lock status information for the database.</summary>
            SC_MANAGER_QUERY_LOCK_STATUS = 0x10,
            /// <summary>Required to call the NotifyBootConfigStatus function.</summary>
            SC_MANAGER_MODIFY_BOOT_CONFIG = 0x20,
            /// <summary>Includes STANDARD_RIGHTS_REQUIRED, in addition to all access rights</summary>
            SC_MANAGER_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SC_MANAGER_CONNECT | SC_MANAGER_CREATE_SERVICE | SC_MANAGER_ENUMERATE_SERVICE | SC_MANAGER_LOCK | SC_MANAGER_QUERY_LOCK_STATUS | SC_MANAGER_MODIFY_BOOT_CONFIG,

            GENERIC_READ = STANDARD_RIGHTS_READ | SC_MANAGER_ENUMERATE_SERVICE | SC_MANAGER_QUERY_LOCK_STATUS,
            GENERIC_WRITE = STANDARD_RIGHTS_WRITE | SC_MANAGER_CREATE_SERVICE | SC_MANAGER_MODIFY_BOOT_CONFIG,
            GENERIC_EXECUTE = STANDARD_RIGHTS_EXECUTE | SC_MANAGER_CONNECT | SC_MANAGER_LOCK,
            GENERIC_ALL = SC_MANAGER_ALL_ACCESS,
            DELETE = ServiceInstaller.DELETE
        }

        /// <summary>
        /// specific access rights for a service
        /// </summary>
        [Flags]
        public enum ServiceAccessType : int
        {
            /// <summary>
            /// Required to call the QueryServiceConfig and QueryServiceConfig2 functions to query the service configuration.
            /// </summary>
            SERVICE_QUERY_CONFIG = 0x0001,
            /// <summary>
            /// Required to call the ChangeServiceConfig or ChangeServiceConfig2 function to change the service configuration. Because this grants the caller the right to change the executable file that the system runs, it should be granted only to administrators.
            /// </summary>
            SERVICE_CHANGE_CONFIG = 0x0002,
            /// <summary>
            /// Required to call the QueryServiceStatus or QueryServiceStatusEx function to ask the service control manager about the status of the service.
            /// <para>Required to call the NotifyServiceStatusChange function to receive notification when a service changes status.</para>
            /// </summary>
            SERVICE_QUERY_STATUS = 0x0004,
            /// <summary>
            /// Required to call the EnumDependentServices function to enumerate all the services dependent on the service.
            /// </summary>
            SERVICE_ENUMERATE_DEPENDENTS = 0x0008,
            /// <summary>
            /// Required to call the StartService function to start the service.
            /// </summary>
            SERVICE_START = 0x0010,
            /// <summary>
            /// Required to call the ControlService function to stop the service.
            /// </summary>
            SERVICE_STOP = 0x0020,
            /// <summary>
            /// Required to call the ControlService function to pause or continue the service.
            /// </summary>
            SERVICE_PAUSE_CONTINUE = 0x0040,
            /// <summary>
            /// Required to call the ControlService function to ask the service to report its status immediately.
            /// </summary>
            SERVICE_INTERROGATE = 0x0080,
            /// <summary>
            /// Required to call the ControlService function to specify a user-defined control code.
            /// </summary>
            SERVICE_USER_DEFINED_CONTROL = 0x0100,
            /// <summary>
            /// Includes STANDARD_RIGHTS_REQUIRED in addition to all access rights in this table.
            /// </summary>
            SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
                                            SERVICE_QUERY_CONFIG |
                                            SERVICE_CHANGE_CONFIG |
                                            SERVICE_QUERY_STATUS |
                                            SERVICE_ENUMERATE_DEPENDENTS |
                                            SERVICE_START |
                                            SERVICE_STOP |
                                            SERVICE_PAUSE_CONTINUE |
                                            SERVICE_INTERROGATE |
                                            SERVICE_USER_DEFINED_CONTROL)
        }

        /// <summary>
        /// Opens an existing service.
        /// </summary>
        /// <param name="hSCManager">A handle to the service control manager database. The OpenSCManager function returns this handle. For more information, see Service Security and Access Rights.</param>
        /// <param name="lpServiceName">The name of the service to be opened. This is the name specified by the lpServiceName parameter of the CreateService function when the service object was created, not the service display name that is shown by user interface applications to identify the service.
        /// <para>The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are invalid service name characters.</para> 
        /// </param>
        /// <param name="dwDesiredAccess">
        /// The access to the service. For a list of access rights, see Service Security and Access Rights.
        /// <para>Before granting the requested access, the system checks the access token of the calling process against the discretionary access-control list of the security descriptor associated with the service object.</para>
        /// </param>
        /// <returns></returns>
        [DllImport("advapi32.dll")]
        internal static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, ServiceControlManagerAccessType dwDesiredAccess);

        /// <summary>
        /// Establishes a connection to the service control manager on the specified computer and opens the specified service control manager database.
        /// </summary>
        /// <param name="lpMachineName">The name of the target computer. If the pointer is NULL or points to an empty string, the function connects to the service control manager on the local computer.</param>
        /// <param name="lpDatabaseName">The name of the service control manager database. This parameter should be set to SERVICES_ACTIVE_DATABASE. If it is NULL, the SERVICES_ACTIVE_DATABASE database is opened by default.</param>
        /// <param name="dwDesiredAccess">The access to the service control manager.</param>
        /// <returns>If the function succeeds, the return value is a handle to the specified service control manager database.</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern IntPtr OpenSCManagerA(string lpMachineName, string lpDatabaseName, ServiceControlManagerAccessType dwDesiredAccess);

        /// <summary>
        /// Closes a handle to a service control manager or service object.
        /// </summary>
        /// <param name="hSCObject">A handle to the service control manager object or the service object to close. Handles to service control manager objects are returned by the OpenSCManager function, and handles to service objects are returned by either the OpenService or CreateService function.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("advapi32.dll")]
        internal static extern bool CloseServiceHandle(IntPtr hSCObject);

        /// <summary>
        /// Marks the specified service for deletion from the service control manager database.
        /// </summary>
        /// <param name="svHandle">A handle to the service. This handle is returned by the OpenService or CreateService function, and it must have the DELETE access right. For more information, see Service Security and Access Rights.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern int DeleteService(IntPtr svHandle);

        /// <summary>
        /// Creates a service object and adds it to the specified service control manager database.
        /// </summary>
        /// <param name="scHandle">A handle to the service control manager database. This handle is returned by the OpenSCManager function and must have the SC_MANAGER_CREATE_SERVICE access right. For more information, see Service Security and Access Rights.</param>
        /// <param name="lpSvcName">The name of the service to install. The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are not valid service name characters.</param>
        /// <param name="lpDisplayName">The display name to be used by user interface programs to identify the service. This string has a maximum length of 256 characters. The name is case-preserved in the service control manager. Display name comparisons are always case-insensitive.</param>
        /// <param name="dwDesiredAccess">The access to the service. Before granting the requested access, the system checks the access token of the calling process. For a list of values, see Service Security and Access Rights.</param>
        /// <param name="dwServiceType">The service type</param>
        /// <param name="dwStartType">The service start options.</param>
        /// <param name="dwErrorControl">The severity of the error, and action taken, if this service fails to start. </param>
        /// <param name="lpPathName">The fully qualified path to the service binary file. If the path contains a space, it must be quoted so that it is correctly interpreted. For example, "d:\\my share\\myservice.exe" should be specified as "\"d:\\my share\\myservice.exe\"".
        /// <para>The path can also include arguments for an auto-start service. For example, "d:\\myshare\\myservice.exe arg1 arg2". These arguments are passed to the service entry point (typically the main function).</para>
        /// <para>If you specify a path on another computer, the share must be accessible by the computer account of the local computer because this is the security context used in the remote call. However, this requirement allows any potential vulnerabilities in the remote computer to affect the local computer. Therefore, it is best to use a local file.</param></para>
        /// <param name="lpLoadOrderGroup">The names of the load ordering group of which this service is a member. Specify NULL or an empty string if the service does not belong to a group.
        /// <para>The startup program uses load ordering groups to load groups of services in a specified order with respect to the other groups. The list of load ordering groups is contained in the following registry value:</para>
        /// <para>HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\ServiceGroupOrder</para>
        /// </param>
        /// <param name="lpdwTagId">A pointer to a variable that receives a tag value that is unique in the group specified in the lpLoadOrderGroup parameter. Specify NULL if you are not changing the existing tag.
        /// <para>You can use a tag for ordering service startup within a load ordering group by specifying a tag order vector in the following registry value:</para>
        /// <para>HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\GroupOrderList</para>
        /// <para>Tags are only evaluated for driver services that have SERVICE_BOOT_START or SERVICE_SYSTEM_START start types.</para>
        /// </param>
        /// <param name="lpDependencies">A pointer to a double null-terminated array of null-separated names of services or load ordering groups that the system must start before this service. Specify NULL or an empty string if the service has no dependencies. Dependency on a group means that this service can run if at least one member of the group is running after an attempt to start all members of the group.
        /// <para>You must prefix group names with SC_GROUP_IDENTIFIER so that they can be distinguished from a service name, because services and service groups share the same name space.</para>
        /// </param>
        /// <param name="lpServiceStartName">The name of the account under which the service should run. If the service type is SERVICE_WIN32_OWN_PROCESS, use an account name in the form DomainName\UserName. The service process will be logged on as this user. If the account belongs to the built-in domain, you can specify .\UserName.
        /// <para>If this parameter is NULL, CreateService uses the LocalSystem account. If the service type specifies SERVICE_INTERACTIVE_PROCESS, the service must run in the LocalSystem account.</para>
        /// <para>If this parameter is NT AUTHORITY\LocalService, CreateService uses the LocalService account. If the parameter is NT AUTHORITY\NetworkService, CreateService uses the NetworkService account.</para>
        /// <para>A shared process can run as any user.</para>
        /// <para>If the service type is SERVICE_KERNEL_DRIVER or SERVICE_FILE_SYSTEM_DRIVER, the name is the driver object name that the system uses to load the device driver. Specify NULL if the driver is to use a default object name created by the I/O system.</para>
        /// <para>A service can be configured to use a managed account or a virtual account. If the service is configured to use a managed service account, the name is the managed service account name. If the service is configured to use a virtual account, specify the name as NT SERVICE\ServiceName. For more information about managed service accounts and virtual accounts, see the Service Accounts Step-by-Step Guide.</para>
        /// <para>Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP:  Managed service accounts and virtual accounts are not supported until Windows 7 and Windows Server 2008 R2.</param></para>
        /// <param name="lpPassword">The password to the account name specified by the lpServiceStartName parameter. Specify an empty string if the account has no password or if the service runs in the LocalService, NetworkService, or LocalSystem account. For more information, see Service Record List.
        /// <para>If the account name specified by the lpServiceStartName parameter is the name of a managed service account or virtual account name, the lpPassword parameter must be NULL.</para>
        /// <para>Passwords are ignored for driver services.</para>
        /// </param>
        /// <returns>If the function succeeds, the return value is a handle to the service.</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern IntPtr CreateService(IntPtr scHandle,
            string lpSvcName,
            string lpDisplayName,
            ServiceInstaller.ServiceAccessType dwDesiredAccess,
            ServicesAPI.ServiceType dwServiceType,
            ServiceInstaller.ServiceStartType dwStartType,
            ServiceInstaller.ServiceErrorControl dwErrorControl,
            string lpPathName,
            string lpLoadOrderGroup,
            IntPtr lpdwTagId,
            string lpDependencies,
            string lpServiceStartName,
            string lpPassword);



        /// <summary>
        /// Checks if Service is Installed
        /// </summary>
        /// <param name="Name">The name of the service to check</param>
        /// <returns>true if installed</returns>
        internal static bool IsInstalled(string Name)
        {
            if (Name.Length > 256) throw new ServiceStartupException("The maximum length for a service name is 256 characters.");
            bool ret = false;
            IntPtr sc_handle = IntPtr.Zero;
            IntPtr sv_handle = IntPtr.Zero;
            try
            {
                sc_handle = OpenSCManagerA(null, null, ServiceControlManagerAccessType.SC_MANAGER_CREATE_SERVICE);
                if (sc_handle == IntPtr.Zero)
                {
                    throw new ServiceStartupException("Unable to open Service Control Manager",
                        new Win32Exception(Marshal.GetLastWin32Error()));
                }

                sv_handle = OpenService(sc_handle, Name, ServiceControlManagerAccessType.GENERIC_READ);
                ret = (sv_handle != IntPtr.Zero);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (sv_handle != IntPtr.Zero) CloseServiceHandle(sv_handle);
                if (sc_handle != IntPtr.Zero) CloseServiceHandle(sc_handle);
            }
            return ret;
        }

        /// <summary>
        /// Marks the specified service for deletion from the service control manager database.
        /// </summary>
        /// <param name="Name">The name of the service to delete</param>
        /// <returns>true if successful</returns>
        internal static bool Uninstall(string Name)
        {
            if (Name.Length > 256) throw new ServiceInstallException("The maximum length for a service name is 256 characters.");

            IntPtr sc_hndl= IntPtr.Zero;
            IntPtr svc_hndl = IntPtr.Zero;

            try
            {
                sc_hndl = OpenSCManagerA(null, null, ServiceControlManagerAccessType.GENERIC_WRITE);
                if (sc_hndl == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                svc_hndl = OpenService(sc_hndl, Name, ServiceControlManagerAccessType.DELETE);
                if (svc_hndl != IntPtr.Zero)
                {
                    int i = DeleteService(svc_hndl);
                    if (i == 0)
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                    return true;
                }

            }
            catch
            {
                throw;
            }
            finally
            {
                if (sc_hndl != IntPtr.Zero)
                {
                    CloseServiceHandle(sc_hndl);
                }
                if (svc_hndl != IntPtr.Zero)
                {
                    CloseServiceHandle(svc_hndl);
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a service object and adds it to the specified service control manager database.
        /// </summary>
        /// <param name="ServicePath">
        /// The fully qualified path to the service binary file. If the path contains a space, it must be quoted so that it is correctly interpreted. For example, "d:\\my share\\myservice.exe" should be specified as "\"d:\\my share\\myservice.exe\"".
        /// <para>The path can also include arguments for an auto-start service. For example, "d:\\myshare\\myservice.exe arg1 arg2". These arguments are passed to the service entry point (typically the main function).</para>
        /// <para>If you specify a path on another computer, the share must be accessible by the computer account of the local computer because this is the security context used in the remote call. However, this requirement allows any potential vulnerabilities in the remote computer to affect the local computer. Therefore, it is best to use a local file.</para>
        /// </param>
        /// <param name="Name">The name of the service to install. The maximum string length is 256 characters. The service control manager database preserves the case of the characters, but service name comparisons are always case insensitive. Forward-slash (/) and backslash (\) are not valid service name characters.</param>
        /// <param name="DisplayName">The display name to be used by user interface programs to identify the service. This string has a maximum length of 256 characters. The name is case-preserved in the service control manager. Display name comparisons are always case-insensitive.</param>
        /// <param name="Description">The description to be used by user interface programs</param>
        /// <param name="ServType">The service type.</param>
        /// <param name="ServAccessType"></param>
        /// <param name="ServStartType"></param>
        /// <param name="ServErrorControl"></param>
        /// <returns></returns>
        internal static bool Install(string ServicePath, string Name, string DisplayName, string Description, ServicesAPI.ServiceType ServType, ServiceInstaller.ServiceAccessType ServAccessType, ServiceInstaller.ServiceStartType ServStartType, ServiceInstaller.ServiceErrorControl ServErrorControl)
        {
            if (Name.Length > 256) throw new ServiceInstallException("The maximum length for a service name is 256 characters.");
            if (Name.IndexOf(@"\") >= 0 || Name.IndexOf(@"/") >= 0) throw new ServiceInstallException(@"Service names cannot contain \ or / characters.");
            if (DisplayName.Length > 256) throw new ServiceInstallException("The maximum length for a display name is 256 characters.");
            
            //The spec says that if a service's path has a space in it, then we must quote it...
            //if (ServicePath.IndexOf(" ") >= 0)
            //  ServicePath = "\"" + ServicePath + "\"";
            //ServicePath = ServicePath.Replace(@"\", @"\\");

            IntPtr sc_handle = IntPtr.Zero;
            IntPtr sv_handle = IntPtr.Zero;

            try
            {
                sc_handle = OpenSCManagerA(null, null, ServiceControlManagerAccessType.SC_MANAGER_CREATE_SERVICE);
                if (sc_handle == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                sv_handle = CreateService(sc_handle, Name, DisplayName, ServAccessType, ServType, ServStartType,
                    ServErrorControl, ServicePath, null, IntPtr.Zero, null, null, null);
                //IntPtr sv_handle = ServicesAPI.CreateService(sc_handle, Name, DisplayName, 0xF0000 | 0x0001 | 0x0002 | 0x0004 | 0x0008 | 0x0010 | 0x0020 | 0x0040 | 0x0080 | 0x0100, 0x00000010, 0x00000002, 0x00000001, ServicePath, null, 0, null, null, null);
                if (sv_handle == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                

                //Sets a service's description by adding a registry entry for it.
                if (!string.IsNullOrEmpty(Description))
                {
                    try
                    {
                        using (
                            Microsoft.Win32.RegistryKey serviceKey =
                                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                                    @"System\CurrentControlSet\Services\" + Name, true))
                        {
                            if (serviceKey != null)
                            {
                                serviceKey.SetValue("Description", Description);
                            }
                            else
                            {
                                DebugLogger.WriteLine("Unable to find service in registry, can't set Description");
                            }
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (sv_handle != IntPtr.Zero)
                {
                    CloseServiceHandle(sv_handle);
                }
                if (sc_handle != IntPtr.Zero)
                {
                    CloseServiceHandle(sc_handle);
                }
            }
        }
    }
}
