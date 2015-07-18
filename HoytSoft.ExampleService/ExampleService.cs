/*
 HoytSoft Self installing .NET service using the Win32 API
 David Hoyt CPOL 2005
 
 Extended by Dror Gluska
 */
using System;
using System.Diagnostics;
using HoytSoft.ServiceBase.Base;
using HoytSoft.ServiceBase.Base.Notifications;

namespace HoytSoft.ExampleService {
      ///<summary>A service for testing out the base service class.</summary>
      [Service(
      "HoytSoft_ExampleService", 
      DisplayName             = "HoytSoft Example Service",
      Description             = "Isn't this just absolutely amazing?",
      ServiceType             = ServicesAPI.ServiceType.SERVICE_WIN32_OWN_PROCESS,
      ServiceAccessType       = ServiceInstaller.ServiceAccessType.SERVICE_ALL_ACCESS,
      ServiceStartType        = ServiceInstaller.ServiceStartType.SERVICE_AUTO_START,
      ServiceErrorControl     = ServiceInstaller.ServiceErrorControl.SERVICE_ERROR_NORMAL,
      ServiceControls         = ServicesAPI.ControlsAccepted.SERVICE_ACCEPT_ALL,
      LogName = "Application",
      IServiceLoggerType =  typeof(HoytSoft.ServiceBase.Base.Logging.EventLogLogger)//typeof(HoytSoft.ServiceBase.Base.Logging.ConsoleLogger)
      )]
      public class ExampleService : HoytSoft.ServiceBase.Base.ServiceBase
      {
          public static new void Main(string[] Args)
          {
              RunService(Args, typeof(ExampleService));
          }
  
          protected override bool Initialize(string[] arguments) {
              this.Log(System.Diagnostics.EventLogEntryType.Information, "Example service initialized correctly, starting up...");
              return true;
          }

          protected override void Start(string[] args)
          {
              this.Log(System.Diagnostics.EventLogEntryType.Information, "Service started");
          }
  
          protected override void Stop() {
              this.Log(System.Diagnostics.EventLogEntryType.Information, "Service stopped");
          }
  
          protected override void Pause() {
              this.Log(System.Diagnostics.EventLogEntryType.Information, "Service paused");
          }
  
          protected override void Continue() {
              this.Log(System.Diagnostics.EventLogEntryType.Information, "Service continued");
          }
  
          protected override void Interrogate() {
              this.Log(System.Diagnostics.EventLogEntryType.Information, "Service interrogated");
          }
  
          protected override void Shutdown() {
              this.Log(System.Diagnostics.EventLogEntryType.Information, "Service shutdown");
          }

          protected override void CustomCommand(int command)
          {
              this.Log(EventLogEntryType.Information, "Custom Command");
          }

          protected override void DeviceEvent(DeviceNotification.DeviceEventInfo e)
          {
              this.Log(EventLogEntryType.Information, string.Format("DeviceEvent {0}", e));
          }

          protected override void HardwareProfileChange(ServicesAPI.SERVICE_CONTROL_HARDWAREPROFILECHANGE_Control dwControl)
          {
              this.Log(EventLogEntryType.Information, string.Format("HardwareProfileChange {0}", dwControl));
          }

          protected override void NetBind(ServicesAPI.NetBindControl control)
          {
              this.Log(EventLogEntryType.Information, string.Format("NetBind {0}", control));
          }

          protected override void PowerEvent(PowerNotification.PowerEventInfo e)
          {
              this.Log(EventLogEntryType.Information, string.Format("PowerEvent {0}", e));
          }

          protected override void PreShutdown()
          {
              this.Log(EventLogEntryType.Information, "PreShutdown");
          }

          protected override void SessionChange(ServicesAPI.SERVICE_CONTROL_SESSIONCHANGE_Control dwControl, SessionNotification.SessionEventData e)
          {
              this.Log(EventLogEntryType.Information, string.Format("SessionChange {0}", dwControl));
          }

          protected override void TimeChange(DateTime oldtime, DateTime newtime)
          {
              this.Log(EventLogEntryType.Information,string.Format("TimeChange from {0} to {1}",oldtime,newtime));
          }

          protected override bool Install() {
              this.Log(System.Diagnostics.EventLogEntryType.Information, "Service installed");
              return true;
          }
  
          protected override bool Uninstall() {
              this.Log(System.Diagnostics.EventLogEntryType.Information, "Service uninstalled");
              return true;
          }
      }
  }