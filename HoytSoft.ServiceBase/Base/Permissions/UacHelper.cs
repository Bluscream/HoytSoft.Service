//or https://code.msdn.microsoft.com/windowsapps/CSUACSelfElevation-644673d3
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HoytSoft.ServiceBase.Base.Permissions
{
    /// <summary>
    /// User Access Control Helper
    /// </summary>
    public static class UacHelper
    {
        private const string uacRegistryKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
        private const string uacRegistryValue = "EnableLUA";

        private const uint STANDARD_RIGHTS_READ = 0x00020000;
        private const uint TOKEN_QUERY = 0x0008;
        private const uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);

        /// <summary>
        /// The OpenProcessToken function opens the access token associated with a process.
        /// </summary>
        /// <param name="ProcessHandle">A handle to the process whose access token is opened. The process must have the PROCESS_QUERY_INFORMATION access permission.</param>
        /// <param name="DesiredAccess">Specifies an access mask that specifies the requested types of access to the access token. These requested access types are compared with the discretionary access control list (DACL) of the token to determine which accesses are granted or denied.
        /// <para>For a list of access rights for access tokens, see Access Rights for Access-Token Objects.</para>
        /// </param>
        /// <param name="TokenHandle">A pointer to a handle that identifies the newly opened access token when the function returns.</param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

        /// <summary>
        /// The GetTokenInformation function retrieves a specified type of information about an access token. The calling process must have appropriate access rights to obtain the information.
        /// <para>To determine if a user is a member of a specific group, use the CheckTokenMembership function. To determine group membership for app container tokens, use the CheckTokenMembershipEx function.</para>
        /// </summary>
        /// <param name="TokenHandle">A handle to an access token from which information is retrieved. If TokenInformationClass specifies TokenSource, the handle must have TOKEN_QUERY_SOURCE access. For all other TokenInformationClass values, the handle must have TOKEN_QUERY access.</param>
        /// <param name="TokenInformationClass">Specifies a value from the TOKEN_INFORMATION_CLASS enumerated type to identify the type of information the function retrieves. Any callers who check the TokenIsAppContainer and have it return 0 should also verify that the caller token is not an identify level impersonation token. If the current token is not an app container but is an identity level token, you should return AccessDenied.</param>
        /// <param name="TokenInformation">A pointer to a buffer the function fills with the requested information. The structure put into this buffer depends upon the type of information specified by the TokenInformationClass parameter.</param>
        /// <param name="TokenInformationLength">Specifies the size, in bytes, of the buffer pointed to by the TokenInformation parameter. If TokenInformation is NULL, this parameter must be zero.</param>
        /// <param name="ReturnLength">A pointer to a variable that receives the number of bytes needed for the buffer pointed to by the TokenInformation parameter. If this value is larger than the value specified in the TokenInformationLength parameter, the function fails and stores no data in the buffer.
        /// <para>If the value of the TokenInformationClass parameter is TokenDefaultDacl and the token has no default DACL, the function sets the variable pointed to by ReturnLength to sizeof(TOKEN_DEFAULT_DACL) and sets the DefaultDacl member of the TOKEN_DEFAULT_DACL structure to NULL.</para>
        /// </param>
        /// <returns>if the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation, uint TokenInformationLength, out uint ReturnLength);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="hObject">A valid handle to an open object.</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// <para>If the function fails, the return value is zero. To get extended error information, call GetLastError.</para>
        /// <para>If the application is running under a debugger, the function will throw an exception if it receives either a handle value that is not valid or a pseudo-handle value. This can happen if you close a handle twice, or if you call CloseHandle on a handle returned by the FindFirstFile function instead of calling the FindClose function.</para>
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// The TOKEN_INFORMATION_CLASS enumeration contains values that specify the type of information being assigned to or retrieved from an access token.
        /// </summary>
        private enum TOKEN_INFORMATION_CLASS
        {
            /// <summary>
            /// The buffer receives a TOKEN_USER structure that contains the user account of the token.
            /// </summary>
            TokenUser = 1,
            /// <summary>
            /// The buffer receives a TOKEN_GROUPS structure that contains the group accounts associated with the token.
            /// </summary>
            TokenGroups,
            /// <summary>
            /// The buffer receives a TOKEN_PRIVILEGES structure that contains the privileges of the token.
            /// </summary>
            TokenPrivileges,
            /// <summary>
            /// The buffer receives a TOKEN_OWNER structure that contains the default owner security identifier (SID) for newly created objects.
            /// </summary>
            TokenOwner,
            /// <summary>
            /// The buffer receives a TOKEN_PRIMARY_GROUP structure that contains the default primary group SID for newly created objects.
            /// </summary>
            TokenPrimaryGroup,
            /// <summary>
            /// The buffer receives a TOKEN_DEFAULT_DACL structure that contains the default DACL for newly created objects.
            /// </summary>
            TokenDefaultDacl,
            /// <summary>
            /// The buffer receives a TOKEN_SOURCE structure that contains the source of the token. TOKEN_QUERY_SOURCE access is needed to retrieve this information.
            /// </summary>
            TokenSource,
            /// <summary>
            /// The buffer receives a TOKEN_TYPE value that indicates whether the token is a primary or impersonation token.
            /// </summary>
            TokenType,
            /// <summary>
            /// The buffer receives a SECURITY_IMPERSONATION_LEVEL value that indicates the impersonation level of the token. If the access token is not an impersonation token, the function fails.
            /// </summary>
            TokenImpersonationLevel,
            /// <summary>
            /// The buffer receives a TOKEN_STATISTICS structure that contains various token statistics.
            /// </summary>
            TokenStatistics,
            /// <summary>
            /// The buffer receives a TOKEN_GROUPS structure that contains the list of restricting SIDs in a restricted token.
            /// </summary>
            TokenRestrictedSids,
            /// <summary>
            /// The buffer receives a DWORD value that indicates the Terminal Services session identifier that is associated with the token.
            /// <para>If the token is associated with the terminal server client session, the session identifier is nonzero.</para>
            /// <para>Windows Server 2003 and Windows XP:  If the token is associated with the terminal server console session, the session identifier is zero.</para>
            /// <para>In a non-Terminal Services environment, the session identifier is zero.</para>
            /// <para>If TokenSessionId is set with SetTokenInformation, the application must have the Act As Part Of the Operating System privilege, and the application must be enabled to set the session ID in a token.</para>
            /// </summary>
            TokenSessionId,
            /// <summary>
            /// The buffer receives a TOKEN_GROUPS_AND_PRIVILEGES structure that contains the user SID, the group accounts, the restricted SIDs, and the authentication ID associated with the token.
            /// </summary>
            TokenGroupsAndPrivileges,
            /// <summary>
            /// Reserved.
            /// </summary>
            TokenSessionReference,
            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if the token includes the SANDBOX_INERT flag.
            /// </summary>
            TokenSandBoxInert,
            /// <summary>
            /// Reserved.
            /// </summary>
            TokenAuditPolicy,
            /// <summary>
            /// The buffer receives a TOKEN_ORIGIN value.
            /// <para>If the token resulted from a logon that used explicit credentials, such as passing a name, domain, and password to the LogonUser function, then the TOKEN_ORIGIN structure will contain the ID of the logon session that created it.</para>
            /// <para>If the token resulted from network authentication, such as a call to AcceptSecurityContext or a call to LogonUser with dwLogonType set to LOGON32_LOGON_NETWORK or LOGON32_LOGON_NETWORK_CLEARTEXT, then this value will be zero.</para>
            /// </summary>
            TokenOrigin,
            /// <summary>
            /// The buffer receives a TOKEN_ELEVATION_TYPE value that specifies the elevation level of the token.
            /// </summary>
            TokenElevationType,
            /// <summary>
            /// The buffer receives a TOKEN_LINKED_TOKEN structure that contains a handle to another token that is linked to this token.
            /// </summary>
            TokenLinkedToken,
            /// <summary>
            /// The buffer receives a TOKEN_ELEVATION structure that specifies whether the token is elevated.
            /// </summary>
            TokenElevation,
            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if the token has ever been filtered.
            /// </summary>
            TokenHasRestrictions,
            /// <summary>
            /// The buffer receives a TOKEN_ACCESS_INFORMATION structure that specifies security information contained in the token.
            /// </summary>
            TokenAccessInformation,
            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if virtualization is allowed for the token.
            /// </summary>
            TokenVirtualizationAllowed,
            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if virtualization is enabled for the token.
            /// </summary>
            TokenVirtualizationEnabled,
            /// <summary>
            /// The buffer receives a TOKEN_MANDATORY_LABEL structure that specifies the token's integrity level.
            /// </summary>
            TokenIntegrityLevel,
            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if the token has the UIAccess flag set.
            /// </summary>
            TokenUIAccess,
            /// <summary>
            /// The buffer receives a TOKEN_MANDATORY_POLICY structure that specifies the token's mandatory integrity policy.
            /// </summary>
            TokenMandatoryPolicy,
            /// <summary>
            /// The buffer receives a TOKEN_GROUPS structure that specifies the token's logon SID.
            /// </summary>
            TokenLogonSid,
            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if the token is an app container token. Any callers who check the TokenIsAppContainer and have it return 0 should also verify that the caller token is not an identify level impersonation token. If the current token is not an app container but is an identity level token, you should return AccessDenied.
            /// </summary>
            TokenIsAppContainer,
            /// <summary>
            /// The buffer receives a TOKEN_GROUPS structure that contains the capabilities associated with the token.
            /// </summary>
            TokenCapabilities,
            /// <summary>
            /// The buffer receives a TOKEN_APPCONTAINER_INFORMATION structure that contains the AppContainerSid associated with the token. If the token is not associated with an app container, the TokenAppContainer member of the TOKEN_APPCONTAINER_INFORMATION structure points to NULL.
            /// </summary>
            TokenAppContainerSid,
            /// <summary>
            /// The buffer receives a DWORD value that includes the app container number for the token. For tokens that are not app container tokens, this value is zero.
            /// </summary>
            TokenAppContainerNumber,
            /// <summary>
            /// The buffer receives a CLAIM_SECURITY_ATTRIBUTES_INFORMATION structure that contains the user claims associated with the token.
            /// </summary>
            TokenUserClaimAttributes,
            /// <summary>
            /// The buffer receives a CLAIM_SECURITY_ATTRIBUTES_INFORMATION structure that contains the device claims associated with the token.
            /// </summary>
            TokenDeviceClaimAttributes,
            /// <summary>
            /// This value is reserved.
            /// </summary>
            TokenRestrictedUserClaimAttributes,
            /// <summary>
            /// This value is reserved.
            /// </summary>
            TokenRestrictedDeviceClaimAttributes,
            /// <summary>
            /// The buffer receives a TOKEN_GROUPS structure that contains the device groups that are associated with the token.
            /// </summary>
            TokenDeviceGroups,
            /// <summary>
            /// The buffer receives a TOKEN_GROUPS structure that contains the restricted device groups that are associated with the token.
            /// </summary>
            TokenRestrictedDeviceGroups,
            /// <summary>
            /// This value is reserved.
            /// </summary>
            TokenSecurityAttributes,
            /// <summary>
            /// This value is reserved.
            /// </summary>
            TokenIsRestricted,
            /// <summary>
            /// The maximum value for this enumeration.
            /// </summary>
            MaxTokenInfoClass
        }

        /// <summary>
        ///  indicates the elevation type of token being queried by the GetTokenInformation function.
        /// </summary>
        private enum TOKEN_ELEVATION_TYPE
        {
            /// <summary>
            /// The token does not have a linked token.
            /// </summary>
            TokenElevationTypeDefault = 1,
            /// <summary>
            /// The token is an elevated token.
            /// </summary>
            TokenElevationTypeFull,
            /// <summary>
            /// The token is a limited token.
            /// </summary>
            TokenElevationTypeLimited
        }

        public static bool IsUacEnabled
        {
            get
            {
                RegistryKey uacKey = Registry.LocalMachine.OpenSubKey(uacRegistryKey, false);
                if (uacKey == null)
                {
                    throw new ApplicationException(
                        "Unable to open UAC registry key, this could indicate no privileges or unsupported system");
                }
                return uacKey.GetValue(uacRegistryValue).Equals(1);
            }
        }

        public static bool IsProcessElevated
        {
            get
            {
                if (IsUacEnabled)
                {
                    IntPtr tokenHandle;
                    if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_READ, out tokenHandle))
                    {
                        throw new ApplicationException("Could not get process token.  Win32 Error Code: " + Marshal.GetLastWin32Error());
                    }

                    TOKEN_ELEVATION_TYPE elevationResult = TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;

                    int elevationResultSize = Marshal.SizeOf((int)elevationResult);
                    uint returnedSize = 0;
                    IntPtr elevationTypePtr = Marshal.AllocHGlobal(elevationResultSize);

                    bool isProcessAdmin;
                    bool success = GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenElevationType, elevationTypePtr, (uint)elevationResultSize, out returnedSize);
                    if (success)
                    {
                        elevationResult = (TOKEN_ELEVATION_TYPE)Marshal.ReadInt32(elevationTypePtr);
                        isProcessAdmin = (elevationResult == TOKEN_ELEVATION_TYPE.TokenElevationTypeFull);
                    }
                    else
                    {
                        throw new ApplicationException("Unable to determine the current elevation.");
                    }

                    if (tokenHandle != IntPtr.Zero)
                    {
                        CloseHandle(tokenHandle);
                    }

                    return isProcessAdmin;
                }
                else
                {
                    WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
        }
    }
}
