# AutomationMaxWindow
Auto-maximizes newly created windows using Automation (to work with FancyZones' current active monitor)

# Requirements
To run without customizing which windows to maximize, download the .exe and run it. To build the program after customizing options, download .NET Framework 4.8 and change the assembly reference paths in AutomationMaxWindow.csproj if required. This version is required as some UI Automation methods will not work otherwise. 

# What this app is for
If you are using dual monitors, you may want the functionality of having windows open on the current active monitor (i.e. the monitor where the cursor is). 

FancyZones can do this with the new experimental option "move newly created windows to the current active window", but when it moves the window, it resets the window size (and unmaximizes it) if it was maximized. It currently does not have the option to maximize newly moved windows. 

Program parameters to maximize windows upon opening exist, but they occur before FancyZones moves the window, so a small delay is required. Other third-party programs that handle window behavior did not solve this, so this app does so using the accessibility framework Microsoft UI Automation. 

# Customization
Multiple conditions can be set for determining which windows should be maximized, instead of the default which maximizes every window. For example, name.Contains("Notepad") will maximize every window that has "Notepad" as a substring in its window title. 

The app also restarts on a schedule, for when the handlers close unexpectedly, and to clear memory usage buildup (which is minor, but still worth clearing). This schedule can be changed by editing the value in WaitOne(21600000), which is the default schedule of 6 hours (in milliseconds). You can exclude the number to wait indefinitely (until an error occurs). If you don't want the app to restart upon crashing or waiting, remove RestartApplication(). 

# Run this app in the background
Change the output type in AutomationMaxWindow.csproj to winexe instead of exe to run the app in the background, before building the program. You can also remove the Console.WriteLine() lines if you don't want the app to log your window titles in the console. 