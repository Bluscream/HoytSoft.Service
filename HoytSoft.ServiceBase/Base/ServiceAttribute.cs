/*
 HoytSoft Self installing .NET service using the Win32 API
 David Hoyt CPOL 2005
 
 Extended by Dror Gluska
 */

using System;
  /*
   Copyright (c) 2005, 2006 David Hoyt
   
  This is entirely original and provides a mechanism to find out information about
  our service through reflection at runtime.
  */
namespace HoytSoft.ServiceBase.Base
{
      ///<summary>Describes a new Win32 service.</summary>
      [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
      public class ServiceAttribute : System.Attribute {
          #region Constructors
          ///<summary>Describes a new Win32 service.</summary>
          /// <param name="Name">The name of the service used in the service database.</param>
          /// <param name="DisplayName">The name of the service that will be displayed in the services snap-in.</param>
          /// <param name="Description">The description of the service that will be displayed in the service snap-in.</param>
          /// <param name="Run">Indicates if you want the service to run or not on program startup.</param>
          /// <param name="ServiceType">Indicates the type of service you will be running. By default this is "Default."</param>
          /// <param name="ServiceAccessType">Access to the service. Before granting the requested access, the system checks the access token of the calling process.</param>
          /// <param name="ServiceStartType">Service start options. By default this is "AutoStart."</param>
          /// <param name="ServiceErrorControl">Severity of the error, and action taken, if this service fails to start.</param>
          /// <param name="ServiceControls">The controls or actions the service responds to.</param>
          public ServiceAttribute(
              string Name, 
              string DisplayName = null, 
              string Description = null, 
              bool Run = true, 
              ServicesAPI.ServiceType ServiceType = ServicesAPI.ServiceType.SERVICE_WIN32_OWN_PROCESS, 
              ServiceInstaller.ServiceAccessType ServiceAccessType = ServiceInstaller.ServiceAccessType.SERVICE_ALL_ACCESS, 
              ServiceInstaller.ServiceStartType ServiceStartType = ServiceInstaller.ServiceStartType.SERVICE_AUTO_START, 
              ServiceInstaller.ServiceErrorControl ServiceErrorControl = ServiceInstaller.ServiceErrorControl.SERVICE_ERROR_NORMAL, 
              ServicesAPI.ControlsAccepted ServiceControls= ServicesAPI.ControlsAccepted.SERVICE_ACCEPT_ALL)
          {
              this.Name = Name;
              this.DisplayName = DisplayName ?? this.Name;
              this.Description = Description ?? this.Name;
              this.Run = Run;
              this.ServiceType = ServiceType;
              this.ServiceAccessType = ServiceAccessType;// ServiceInstaller.ServiceAccessType.SERVICE_ALL_ACCESS;
              this.ServiceStartType = ServiceStartType;
              this.ServiceErrorControl = ServiceErrorControl;
              this.ServiceControls = ServiceControls;
              this.LogName = "Services";
          }
         
          #endregion
  
          #region Properties
          ///<summary>The name of the service used in the service database.</summary>
          public string Name { get;set; }
          ///<summary>The name of the service that will be displayed in the services snap-in.</summary>
          public string DisplayName { get; set; }
          ///<summary>The description of the service that will be displayed in the service snap-in.</summary>
          public string Description { get; set; }
          ///<summary>Indicates if you want the service to run or not on program startup.</summary>
          public bool Run { get; set; }
          ///<summary>Indicates the type of service you want to run.</summary>
          public ServicesAPI.ServiceType ServiceType { get; set; }
          ///<summary>Access to the service. Before granting the requested access, the system checks the access token of the calling process.</summary>
          public ServiceInstaller.ServiceAccessType ServiceAccessType { get; set; }
          ///<summary>Service start options.</summary>
          public ServiceInstaller.ServiceStartType ServiceStartType { get; set; }
          ///<summary>Severity of the error, and action taken, if this service fails to start.</summary>
          public ServiceInstaller.ServiceErrorControl ServiceErrorControl { get; set; }
          ///<summary>The controls or actions the service responds to.</summary>
          public ServicesAPI.ControlsAccepted ServiceControls { get; set; }
          ///<summary>The name of the log you want to write to.</summary>
          public string LogName { get; set; }
          /// <summary>IServiceLogger typed logger</summary>
          public Type IServiceLoggerType { get; set; } 
          #endregion
      }
  }