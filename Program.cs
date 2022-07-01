using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;

namespace Program
{
    class Program
    {
        private static WindowPattern GetWindowPattern(AutomationElement targetControl)
        {
            WindowPattern windowPattern = null;

            try
            {
                windowPattern =
                    targetControl.GetCurrentPattern(WindowPattern.Pattern)
                    as WindowPattern;
            }
            catch (InvalidOperationException)
            {
                // Object doesn't support the WindowPattern control pattern
                return null;
            }
            // Make sure the element is usable.
            if (false == windowPattern.WaitForInputIdle(10000))
            {
                // Object not responding in a timely manner
                return null;
            }
            return windowPattern;
        }

        private static void RestartApplication()
        {
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.Arguments = "/C ping 127.0.0.1 -n 2 && \"" + Process.GetCurrentProcess().MainModule.FileName + "\"";
            Info.WindowStyle = ProcessWindowStyle.Hidden;
            Info.CreateNoWindow = true;
            Info.FileName = "cmd.exe";
            Console.WriteLine("Initiating restart...");
            Process.Start(Info);

            Process currentProcess = Process.GetCurrentProcess();
            var currentProcessId = currentProcess.Id;
            Process.GetProcessById(currentProcessId).CloseMainWindow();
            Process.GetProcessById(currentProcessId).Close();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("Application has been started. Now listening to windows...");

            Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, AutomationElement.RootElement, TreeScope.Children, (sender, e) =>
            {
                var element = (AutomationElement)sender;
                var name = element.Current.Name;

                Console.WriteLine("open: " + name + " hwnd:" + element.Current.NativeWindowHandle);

                // Determine which windows to maximize
                // Set if(true) to maximize every window, or remove it & set your own conditions in if-statement for which windows to maximize
                // Multiple name.Contains() conditions with OR operators can be used
                if (true || name.Contains("Google Chrome")) // Remove true to only maximize Google Chrome or set own conditions
                {
                    var windowPattern = GetWindowPattern(element);
                    windowPattern.SetWindowVisualState(WindowVisualState.Maximized);
                    Console.WriteLine("Maximized a " + name + " window");
                }

                Automation.AddAutomationEventHandler(WindowPattern.WindowClosedEvent, element, TreeScope.Element, (s, e2) =>
                {
                    Console.WriteLine("close: " + name + " hwnd:" + element.Current.NativeWindowHandle);
                });
            });

            /*
            // Focus Changed Event Handler (for other usage options, disabled to reduce memory usage)
            Automation.AddAutomationFocusChangedEventHandler((sender, e) =>
            {
                var element = (AutomationElement)sender;
                var name = element.Current.Name;
                Console.WriteLine("focused: " + name + " hwnd:" + element.Current.NativeWindowHandle);
            });
            */

            // 6 hour wait while processing thread to handle maximizing of windows, before a scheduled app restart
            // Change wait time if desired (parameter is in milliseconds), or exclude number to wait indefinitely (until an error occurs)
            new ManualResetEvent(false).WaitOne(21600000);
            Console.WriteLine("Application has closed handlers for some unexpected error or it has been 6 hours. Restarting app...");
            Automation.RemoveAllEventHandlers();
            // Automatic restart for when the handlers close unexpectedly, and to clear memory usage buildup
            RestartApplication();
        }
    }
}