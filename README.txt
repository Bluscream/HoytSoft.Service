Self installing .NET service using the Win32 API
DavidHoyt, 28 Oct 2005 CPOL (see CPOL.htm)
http://www.codeproject.com/Articles/11880/Self-installing-NET-service-using-the-Win-API


This is an enchancement of David Hoyt's Windows Service, which went an extensive refactoring and extension by Dror Gluska.

Highlights:
- Console Container
- Detection of Power Events
- Detection of Device Events
- Hardware Profile changes
- NetBind events
- Pre Shutdown events
- Session Change events
- Time Change events.

This was initially designed as a framework component, but was abandoned, I've made a few cleanups and decided to contribute it as incomplete code, feel free to finish it.

Currently its missing a bit of functionality:
- Console Container needs to be finished with proper messages send to the service along with the device and power events.
- EventLog logger needs to be finished, specifically deciding how to behave when a service is uninstalled.
- RequestAdditionalTime is missing
- SendCustomCommand is not implemented/verified.
