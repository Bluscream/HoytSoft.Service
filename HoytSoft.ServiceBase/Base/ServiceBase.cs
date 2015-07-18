/*
 HoytSoft Self installing .NET service using the Win32 API
 David Hoyt CPOL 2005
 
 Extended by Dror Gluska
 */

using HoytSoft.ServiceBase.Base;
using HoytSoft.ServiceBase.Base.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using HoytSoft.ServiceBase.Base.Logging;
using HoytSoft.ServiceBase.Base.Exceptions;
using HoytSoft.ServiceBase.Base.Permissions;

/*
   Copyright (c) 2005, 2006 David Hoyt
   
   A huge thank-you to C.V Anish for his article: http://www.codeproject.com/system/windows_nt_service.asp
   It paved the way for this class. Although I used his article to better understand the internals of Win32
   services, almost everything here is original except where noted.
   

  */
  namespace HoytSoft.ServiceBase.Base
 {
      #region Public Enums
      public enum ServiceState : byte {
          Running,
          Stopped,
          Paused,
          ShuttingDown,
          Interrogating
      }
      #endregion
  
      ///<summary>A base class for installing, starting, pausing, etc. a Windows service.</summary>
      public abstract class ServiceBase : IService, IDisposable {
          #region Entry Point
          ///<summary>If you do not want to specify your own main entry point, you can use this one.</summary>
          public static void Main(string[] Args)
          {
              Assembly a = Assembly.GetExecutingAssembly();
              if (a == null) throw new ServiceException("No currently executing assembly.");
              RunServices(Args, a.GetTypes());
          }
  
          ///<summary>Executes your service. If multiple services are defined in the assembly, it will run them all in separate threads.</summary>
          /// <param name="Args">The arguments passed in from the command line.</param>
          /// <param name="ServiceType">The class' type you want to inspect for services.</param>
          public static void RunService(string[] Args, Type ServiceType) {
              RunServices(Args, new Type[] { ServiceType });
          }
  
          ///<summary>Executes your service. If multiple services are defined in the assembly, it will run them all in separate threads.</summary>
          /// <param name="Args">The arguments passed in from the command line.</param>
          /// <param name="Types">An array of types we want to inspect for services.</param>
          public static void RunServices(string[] Args, Type[] Types) {
              Console.WriteLine("Service Installer");


              //Reads in all the classes in the assembly and finds one that derives
              //from this class. If it finds one, it checks the attributes to see
              //if we should run it. If we should, we create an instance of it and 
              //start it on its way...
  
              //Type[] types = a.GetTypes();
              
              System.Collections.ArrayList alDispatchTables = new System.Collections.ArrayList();
              List<ServiceBase> alServices = new List<ServiceBase>();
              foreach(Type t in Types) {
                  if (t.IsClass && t.BaseType != null && t.BaseType.Equals(typeof(ServiceBase))) {
                      //Gets all the custom attributes of type ServiceAttribute in the class.
                      object[] attributes = t.GetCustomAttributes(typeof(ServiceAttribute), true);
                      foreach(ServiceAttribute info in attributes) {
                          if (info.Run) {
                              ServiceBase s = (ServiceBase)Activator.CreateInstance(t);
                              alServices.Add(s);
  
                              //Make sure we have a name set for this guy...
                              if (string.IsNullOrWhiteSpace( s.Name))
                                  throw new ServiceRuntimeException("A service was created without a name.");

                              if (Args.Length == 0)
                              {
                                  Console.WriteLine();
                                  Console.WriteLine("{0} Service", s.Name);
                                  Console.WriteLine("==================================");
                                  Console.WriteLine("Install");
                                  Console.WriteLine("\t{0} i", System.AppDomain.CurrentDomain.FriendlyName);
                                  Console.WriteLine("Uninstall");
                                  Console.WriteLine("\t{0} u", System.AppDomain.CurrentDomain.FriendlyName);
                                  Console.WriteLine("Interactive Mode");
                                  Console.WriteLine("\t{0} c", System.AppDomain.CurrentDomain.FriendlyName);
                              }

                              bool isUacEnabled = UacHelper.IsUacEnabled;
                              bool isUacElevated = UacHelper.IsProcessElevated;

                              if (isUacEnabled && !isUacElevated)
                              {
                                  Console.WriteLine(
                                      "Warning: UAC is enabled but not process is not elevated, some functionality is not possible");

                                  ServicesAPI.SERVICE_TABLE_ENTRY entry = new ServicesAPI.SERVICE_TABLE_ENTRY();
                                  entry.lpServiceName = info.Name;
                                  entry.lpServiceProc = new ServicesAPI.ServiceMainProc(s.baseServiceMain);
                                  alDispatchTables.Add(entry);
                                  s.Debug = false;
                              }
                              else
                              {


                                  if (Args.Length > 0 && (Args[0].ToLower() == "u" || Args[0].ToLower() == "uninstall"))
                                  {
                                      //Nothing to uninstall if it's not installed...
                                      if (!IsServiceInstalled(info.Name)) break;
                                      if (!ServiceInstaller.Uninstall(info.Name))
                                          throw new ServiceUninstallException("Unable to remove service \"" + info.DisplayName + "\"");
                                      if (!s.Uninstall())
                                          throw new ServiceUninstallException("Service \"" + info.DisplayName + "\" was unable to uninstall itself correctly.");
                                  }
                                  else if (Args.Length > 0 && (Args[0].ToLower() == "i" || Args[0].ToLower() == "install"))
                                  {
                                      //Just install the service if we pass in "i" or "install"...
                                      //Always check to see if the service is installed and if it isn't,
                                      //then go ahead and install it...
                                      if (!IsServiceInstalled(info.Name))
                                      {
                                          string[] envArgs = Environment.GetCommandLineArgs();
                                          if (envArgs.Length > 0)
                                          {
                                              System.IO.FileInfo fi = new System.IO.FileInfo(envArgs[0]);
                                              if (!ServiceInstaller.Install(fi.FullName, info.Name, info.DisplayName, info.Description, info.ServiceType, info.ServiceAccessType, info.ServiceStartType, info.ServiceErrorControl))
                                                  throw new ServiceInstallException("Unable to install service \"" + info.DisplayName + "\"");
                                              if (!s.Install())
                                                  throw new ServiceInstallException("Service was not able to install itself correctly.");
                                          }
                                      }
                                  }
                                  else
                                  {
                                      //Always check to see if the service is installed and if it isn't,
                                      //then go ahead and install it...
                                      if (!IsServiceInstalled(info.Name))
                                      {
                                          string[] envArgs = Environment.GetCommandLineArgs();
                                          if (envArgs.Length > 0)
                                          {
                                              System.IO.FileInfo fi = new System.IO.FileInfo(envArgs[0]);
                                              if (!ServiceInstaller.Install(fi.FullName, info.Name, info.DisplayName, info.Description, info.ServiceType, info.ServiceAccessType, info.ServiceStartType, info.ServiceErrorControl))
                                                  throw new ServiceInstallException("Unable to install service \"" + info.DisplayName + "\"");
                                              if (!s.Install())
                                                  throw new ServiceInstallException("Service was not able to install itself correctly.");
                                          }
                                      }

                                      ServicesAPI.SERVICE_TABLE_ENTRY entry = new ServicesAPI.SERVICE_TABLE_ENTRY();
                                      entry.lpServiceName = info.Name;
                                      entry.lpServiceProc = new ServicesAPI.ServiceMainProc(s.baseServiceMain);
                                      alDispatchTables.Add(entry);
                                      s.Debug = false;
                                  }
                              }
                          }
                          break; //We can break b/c we only allow ONE instance of this attribute per object...
                      }
                  }
              }
  
              if (alDispatchTables.Count > 0) {
                  //Add a null entry to tell the API it's the last entry in the table...
                  ServicesAPI.SERVICE_TABLE_ENTRY entry = new ServicesAPI.SERVICE_TABLE_ENTRY();
                  entry.lpServiceName = null;
                  entry.lpServiceProc = null;
                  alDispatchTables.Add(entry);
  
                  ServicesAPI.SERVICE_TABLE_ENTRY[] table = (ServicesAPI.SERVICE_TABLE_ENTRY[])alDispatchTables.ToArray(typeof(ServicesAPI.SERVICE_TABLE_ENTRY));
                  if (ServicesAPI.StartServiceCtrlDispatcher(table) == 0)
                  {
                      //There was an error. What was it?
                      switch (Marshal.GetLastWin32Error())
                      {
                          case ServicesAPI.ERROR_INVALID_DATA:
                              throw new ServiceStartupException(
                                  "The specified dispatch table contains entries that are not in the proper format.");
                          case ServicesAPI.ERROR_SERVICE_ALREADY_RUNNING:
                              throw new ServiceStartupException("A service is already running.");
                          case ServicesAPI.ERROR_FAILED_SERVICE_CONTROLLER_CONNECT:
                              //Executed when in Console Mode

                              foreach (var s in alServices)
                              {
                                  ServiceContainer sc = new ServiceContainer(s);
                                  s.Debug = true;
                                  s.args = Args;
                                  sc.Start();
                              }

                              //throw new ServiceStartupException("A service is being run as a console application");
                              //"A service is being run as a console application. Try setting the Service attribute's \"Debug\" property to true if you're testing an application."
                              //If we've started up as a console/windows app, then we'll get this error in which case we treat the program
                              //like a normal app instead of a service and start it up in "debug" mode...
                              break;
                          default:
                              throw new ServiceStartupException(
                                  "An unknown error occurred while starting up the service(s).");
                      }
                  }
                  else
                  {
                      //Service Mode
                      DebugLogger.IsConsole = false;
                  }
              }
          }

          /// <summary>
          /// Checks if serviceName is installed
          /// </summary>
          /// <param name="serviceName">the service name</param>
          /// <returns>true if installed, false if no access or isn't installed</returns>
          private static bool IsServiceInstalled(string serviceName)
          {
              bool isServiceInstalled;

              try
              {
                  isServiceInstalled = ServiceInstaller.IsInstalled(serviceName);
              }
              catch (Exception ex)
              {
                  Console.WriteLine(
                      "Unable to check if service is installed, this is usually because elevated privileges are needed");
#if DEBUG
                  DebugLogger.WriteLine(ex);
#endif
                  return false;
              }
              return isServiceInstalled;
          }

          #endregion
  
          #region Private Variables
          
          private ServicesAPI.SERVICE_STATUS _servStatus;
          private IntPtr _servStatusHandle;
          private readonly ServicesAPI.ServiceCtrlHandlerProcEx _servCtrlHandlerProc;
          #endregion
  
          #region Constructors
          public ServiceBase() {
              var attributes = this.GetType().GetCustomAttributes < ServiceAttribute>( true);
              foreach(var info in attributes) {
                  this.Name = info.Name;
                  this.DisplayName = info.DisplayName;
                  this.Description = info.Description;
                  this.Run = info.Run;
                  this.ServiceType = info.ServiceType;
                  this.ServiceAccessType = info.ServiceAccessType;
                  this.ServiceStartType = info.ServiceStartType;
                  this.ServiceErrorControl = info.ServiceErrorControl;
                  this.ServiceControls = info.ServiceControls;
                  this.LogName = info.LogName;
                  this.IServiceLoggerType = info.IServiceLoggerType;
                  this.Debug = false;
              }
              this._servCtrlHandlerProc = new ServicesAPI.ServiceCtrlHandlerProcEx(this.baseServiceControlHandler);
              this._servStatus = new ServicesAPI.SERVICE_STATUS();
              this.ServiceState = ServiceState.Stopped;
              this.args = null;
          }
          #endregion
  
          #region Properties
          ///<summary>The name of the service used in the service database.</summary>
          public string Name {  get ; private set; } 
          ///<summary>The name of the service that will be displayed in the services snap-in.</summary>
          public string DisplayName {  get; private set; }
          ///<summary>The description of the service that will be displayed in the service snap-in.</summary>
          public string Description { get; private set; }
          ///<summary>Indicates if you want the service to run or not on program startup.</summary>
          public bool Run {  get; private set; }
          ///<summary>Indicates the type of service you want to run.</summary>
          public ServicesAPI.ServiceType ServiceType { get; private set; }
          ///<summary>Access to the service. Before granting the requested access, the system checks the access token of the calling process.</summary>
          public ServiceInstaller.ServiceAccessType ServiceAccessType { get; private set; }
          ///<summary>Service start options.</summary>
          public ServiceInstaller.ServiceStartType ServiceStartType { get; private set; }
          ///<summary>Severity of the error, and action taken, if this service fails to start.</summary>
          public ServiceInstaller.ServiceErrorControl ServiceErrorControl { get; private set; }
          ///<summary>The controls or actions the service responds to.</summary>
          public ServicesAPI.ControlsAccepted ServiceControls {  get; private set; }
          ///<summary>The current state of the service.</summary>
          public ServiceState ServiceState {  get; private set; }
          ///<summary>Treats the service as a console application instead of a normal service.</summary>
          public bool Debug {  get; private set; }
          ///<summary>Tells if the service has been disposed of already.</summary>
          public bool Disposed {  get; private set; }
          /// <summary>Logger Name</summary>
          public string LogName {  get; private set; }
          /// <summary>Logger Provider Type</summary>
          public Type IServiceLoggerType { get; private set; }
          /// <summary>Logger Provider</summary>
          public IServiceLogger IServiceLogger { get; internal set; }
          #endregion
  
          #region Override Methods
          protected override bool Install() { return true; }
          protected override bool Uninstall() { return true; }
          protected override bool Initialize(string[] arguments) { return true; }
          protected override void Start(string[] args) { }
          protected override void Pause() { }
          protected override void Stop() { }
          protected override void Continue() { }
          protected override void Shutdown() { }
          protected override void PreShutdown() {}
          protected override void Interrogate() { }
          /// <summary>
          /// TODO: Add device event lParam
          /// </summary>
          /// <param name="dwControl"></param>
          //protected override void DeviceEvent(ServicesAPI.SERVICE_CONTROL_DEVICEEVENT_Control dwControl) { }
          protected override void HardwareProfileChange(ServicesAPI.SERVICE_CONTROL_HARDWAREPROFILECHANGE_Control dwControl) { }
          //protected virtual void RequestAdditionalTime(int milliseconds) { throw new NotImplementedException(); }
          protected override void SessionChange(ServicesAPI.SERVICE_CONTROL_SESSIONCHANGE_Control dwControl, SessionNotification.SessionEventData e) { }
          //protected override void PowerEvent(ServicesAPI.SERVICE_CONTROL_POWEREVENT_Control dwControl, ServicesAPI.POWERBROADCAST_SETTING  setting) {  }
          protected override void TimeChange(DateTime oldtime, DateTime newtime){ }
          protected override void NetBind(ServicesAPI.NetBindControl control) { }
          //could be a mistake
          protected override void CustomCommand(int command) { throw new NotImplementedException(); }

          #endregion
  
  
          #region IDisposable Members
          public void Dispose() {
              if (!this.Disposed) {
                  this.Disposed = true;
                  if (this.IServiceLogger != null){
                      this.IServiceLogger.Dispose();
                  }
                  //if (this.Loggerger != null) {
                  //    this.Logger.Close();
                  //    this.Logger.Dispose();
                  //    this.Logger = null;
                  //}
              }
          }
          #endregion
  
          #region Helper Methods

          private IServiceLogger Logger
          {
              get
              {
                  if ((this.IServiceLogger == null) && (this.IServiceLoggerType != null))
                  {
                      this.IServiceLogger = (IServiceLogger)Activator.CreateInstance(this.IServiceLoggerType);
                      if (!this.IServiceLogger.Exists(this.LogName)){
                          this.IServiceLogger.Create(this.LogName);
                      }

                      this.IServiceLogger.Open(this.LogName,this.Name);
                  }

                  return this.IServiceLogger;
              }
          }

          public void Log(EventLogEntryType type, string Message) {
              if (this.Logger == null)
              {
                  DebugLogger.WriteLine("[No Logger Found!]" + Message);
                  return;
              }

              try {
                  this.Logger.WriteEntry(type, Message);
              } catch(System.ComponentModel.Win32Exception) {
                  //In case the event log is full....
              }
              catch (Exception ex)
              {
                  //in case other errors
                  DebugLogger.WriteLine(ex);
              }
          }

          
  
          #region Debug Mode Methods
          private void baseDebugInstall() {
              while(this.ServiceState == ServiceState.Interrogating)
                  ;
              if (!this.Install())
                  throw new ServiceInstallException("Service was not able to install itself correctly.");
          }
  
          private void baseDebugUninstall() {
              while(this.ServiceState == ServiceState.Interrogating)
                  ;
              if (!this.Uninstall())
                  throw new ServiceUninstallException("Service \"" + this.DisplayName + "\" was unable to uninstall itself correctly.");
          }
  
    
          #endregion
          #endregion


  
          internal void DebugStart() {
              if (this.Initialize(this.args)) {
                  this.ServiceState = ServiceState.Running;
                  this.Start(this.args);
              }
          }
  
          private void baseServiceMain(int argc, string[] argv) {
              if (this.ServiceType != ServicesAPI.ServiceType.SERVICE_FILE_SYSTEM_DRIVER && this.ServiceType != ServicesAPI.ServiceType.SERVICE_KERNEL_DRIVER)
                  this._servStatus.dwServiceType = ServicesAPI.ServiceType.SERVICE_WIN32_OWN_PROCESS | ServicesAPI.ServiceType.SERVICE_WIN32_SHARE_PROCESS;
              else
                  this._servStatus.dwServiceType = this.ServiceType;
              this._servStatus.dwCurrentState = ServicesAPI.ServiceCurrentStateType.SERVICE_START_PENDING;
              this._servStatus.dwControlsAccepted =  this.ServiceControls;
              this._servStatus.dwWin32ExitCode = 0;
              this._servStatus.dwServiceSpecificExitCode = 0;
              this._servStatus.dwCheckPoint = 0;
              this._servStatus.dwWaitHint = 0;
              this._servStatusHandle = ServicesAPI.RegisterServiceCtrlHandlerEx(this.Name, this._servCtrlHandlerProc,IntPtr.Zero); 
              if (_servStatusHandle == IntPtr.Zero) return;
              this._servStatus.dwCurrentState = ServicesAPI.ServiceCurrentStateType.SERVICE_RUNNING;
              this._servStatus.dwCheckPoint = 0;
              this._servStatus.dwWaitHint = 0;
              if (ServicesAPI.SetServiceStatus(this._servStatusHandle, ref this._servStatus) == 0) {
                  var errorid = Marshal.GetLastWin32Error();
                  throw new ServiceRuntimeException("\"" + this.DisplayName + "\" threw an error. Error Number: " + errorid);
              }
  
              //Call the initialize method...
              if (!this.Initialize(argv)) {
                  this._servStatus.dwCurrentState       = ServicesAPI.ServiceCurrentStateType.SERVICE_STOPPED; 
                  this._servStatus.dwCheckPoint         = 0; 
                  this._servStatus.dwWaitHint           = 0; 
                  this._servStatus.dwWin32ExitCode      = 1; 
                  this._servStatus.dwServiceSpecificExitCode = 1;
   
                  ServicesAPI.SetServiceStatus(this._servStatusHandle, ref this._servStatus); 
                  return; 
              }
  
              //Initialization complete - report running status. 
              this._servStatus.dwCurrentState = ServicesAPI.ServiceCurrentStateType.SERVICE_RUNNING; 
              this._servStatus.dwCheckPoint         = 0; 
              this._servStatus.dwWaitHint           = 0; 
   
              if (ServicesAPI.SetServiceStatus(this._servStatusHandle, ref this._servStatus) == 0) {
                  var errorid = Marshal.GetLastWin32Error();
                  throw new ServiceRuntimeException("\"" + this.DisplayName + "\" threw an error. Error Number: " + errorid);
              }
  
              this.ServiceState = ServiceState.Running;
              /*System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(this.Start));
              if (this.debug)
                  t.Name = "Start - " + this.DisplayName;
              t.Start();/**/


           
              this.Start(this.args);
          }


         

          internal void ChangeServiceState(ServicesAPI.ServiceControlType OpCode)
          {
              this.baseServiceControlHandler(OpCode,0,IntPtr.Zero,IntPtr.Zero);
          }
  
          /// <summary>
          /// This is called whenever a service control event happens such as pausing, stopping, etc...
          /// </summary>
          /// <param name="Opcode"></param>
          /// <param name="eventType"></param>
          /// <param name="eventData"></param>
          /// <param name="context"></param>
          /// <returns></returns>
          private int baseServiceControlHandler(ServicesAPI.ServiceControlType Opcode, int eventType, IntPtr eventData, IntPtr context)
          {

              System.Windows.Forms.Message m = new System.Windows.Forms.Message { Msg = DeviceNotification.WM_DEVICECHANGE, WParam = (IntPtr)eventType, LParam = eventData };

              
              switch(Opcode) {
                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_PAUSE: 
                      this.ServiceState = ServiceState.Paused;
                      this._servStatus.dwCurrentState = ServicesAPI.ServiceCurrentStateType.SERVICE_PAUSED;
                      try {
                          this.ServiceSendCommand(Command.Pause);
                      } catch(Exception e) {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to pause the service:" + e);
                      }
                      ServicesAPI.SetServiceStatus(this._servStatusHandle, ref this._servStatus);
                      break;

                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_CONTINUE:
                      this.ServiceState = ServiceState.Running;
                      this._servStatus.dwCurrentState = ServicesAPI.ServiceCurrentStateType.SERVICE_RUNNING;
                      ServicesAPI.SetServiceStatus(this._servStatusHandle, ref this._servStatus);
                      try {
                          this.ServiceSendCommand(Command.Continue);
                      } catch(Exception e) {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to continue the service:" + e);
                      }
                      break;

                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_STOP:
                      this.ServiceState = ServiceState.Stopped;
                      this._servStatus.dwWin32ExitCode = 0;
                      this._servStatus.dwCurrentState = ServicesAPI.ServiceCurrentStateType.SERVICE_STOPPED;
                      this._servStatus.dwCheckPoint = 0;
                      this._servStatus.dwWaitHint = 0;
                      ServicesAPI.SetServiceStatus(this._servStatusHandle, ref this._servStatus);
                      try {
                          this.ServiceSendCommand(Command.Stop);
                      } catch(Exception e) {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to stop the service:" + e);
                      }
                      break;

                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_SHUTDOWN:
                      this.ServiceState = ServiceState.ShuttingDown;
                      this._servStatus.dwCurrentState = ServicesAPI.ServiceCurrentStateType.SERVICE_STOPPED;
                      ServicesAPI.SetServiceStatus(this._servStatusHandle, ref this._servStatus);
                      try {
                          this.ServiceSendCommand(Command.Shutdown);
                      } catch(Exception e) {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to shut down the service:" + e);
                      }
                      break;

                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_INTERROGATE:
                      //this.ServiceState = ServiceState.Interrogating;
                      //this.servStatus.dwCurrentState = ServicesAPI.ServiceCurrentStateType. .SERVICE_INTERROGATE;
                      ServicesAPI.SetServiceStatus(this._servStatusHandle, ref this._servStatus);
                      try {
                          this.ServiceSendCommand(Command.Interrogate);
                      } catch(Exception e) {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to interrogate the service:" + e);
                      }
                      break; 
                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_DEVICEEVENT:
                      try
                      {
                          //TODO: Implement proper device event parsing!!
                          var devevt = Notifications.Notifications.Transform(ref m);
                          this.ServiceSendDeviceEvent(devevt.Device);
                      }
                      catch (Exception e)
                      {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to update Device Event" + e);
                      }
                      break;
                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_HARDWAREPROFILECHANGE:
                      try
                      {
                          this.ServiceSendHardwareProfileChange((ServicesAPI.SERVICE_CONTROL_HARDWAREPROFILECHANGE_Control)eventType);
                      }
                      catch (Exception e)
                      {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to change hardware profile" + e);
                      }
                      break;
                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_NETBINDADD:
                      try
                      {
                          this.ServiceSendNetBind(ServicesAPI.NetBindControl.NETBINDADD);
                      }
                      catch (Exception e)
                      {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to netbind add" + e);
                      }
                      break;
                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_NETBINDDISABLE:
                      try
                      {
                          this.ServiceSendNetBind(ServicesAPI.NetBindControl.NETBINDDISABLE);
                      }
                      catch (Exception e)
                      {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to netbind disable" + e);
                      }
                      break;
                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_NETBINDENABLE :
                      try
                      {
                          this.ServiceSendNetBind(ServicesAPI.NetBindControl.NETBINDENABLE);
                      }
                      catch (Exception e)
                      {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to netbind enable" + e);
                      }
                      break;
                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_NETBINDREMOVE:
                      try
                      {
                          this.ServiceSendNetBind(ServicesAPI.NetBindControl.NETBINDREMOVE);
                      }
                      catch (Exception e)
                      {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to netbind remove" + e);
                      }
                      break;
                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_PARAMCHANGE:
                      try
                      {
                          //TODO could be a mistake
                          this.ServiceSendCustomCommand((int)eventData);
                      }
                      catch (Exception e)
                      {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to send custom command" + e);
                      }
                      break;
                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_POWEREVENT:
                       try
                      {
                          var powevt = Notifications.Notifications.Transform(ref m);
                          this.ServiceSendPowerEvent(powevt.Power);
                      }
                      catch (Exception e)
                      {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to process power event" + e);
                      }
                      break;
                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_PRESHUTDOWN:
                       try
                      {
                          this.ServiceSendCommand(Command.PreShutdown);

                      }
                      catch (Exception e)
                      {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to execute Pre Shutdown event" + e);
                      }
                      break;
                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_SESSIONCHANGE:
                       try
                      {
                           var wtsnotification = (ServicesAPI.WTSSESSION_NOTIFICATION)Marshal.PtrToStructure(eventData,typeof(ServicesAPI.WTSSESSION_NOTIFICATION));
                           var sne = Notifications.SessionNotification.Transform( wtsnotification.dwSessionId);
                          this.ServiceSendSessionChange((ServicesAPI.SERVICE_CONTROL_SESSIONCHANGE_Control)eventType,sne);
                      }
                      catch (Exception e)
                      {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to process session change event" + e);
                      }
                      break;
                  case ServicesAPI.ServiceControlType.SERVICE_CONTROL_TIMECHANGE:
                      try
                      {
                          var sti = (ServicesAPI.SERVICE_TIMECHANGE_INFO)Marshal.PtrToStructure(eventData, typeof(ServicesAPI.SERVICE_TIMECHANGE_INFO));
                          this.ServiceSendTimeChange(sti.liOldTimeToDateTime(),sti.liNewTimeToDateTime());
                      }
                      catch (Exception e)
                      {
                          this.Logger.WriteEntry(System.Diagnostics.EventLogEntryType.Error, "An exception occurred while trying to process time change event" + e);
                      }
                      break;
                  //case ServicesAPI.ServiceControlType.SERVICE_CONTROL_TRIGGEREVENT:

                  //    break;
              }
              return 0; //NO_ERROR
          }
  
          
      }
  }