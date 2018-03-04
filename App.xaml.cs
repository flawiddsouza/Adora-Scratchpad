// From: https://stackoverflow.com/questions/21789899/how-to-create-single-instance-wpf-application-that-restores-the-open-window-when/21791103#21791103

using System;
using System.Threading;
using System.Windows;

namespace Adora_Scratchpad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex mutex = new Mutex(true, "Adora_Scratchpad");
        private static MainWindow mainWindow = null;

        App()
        {
            InitializeComponent();
        }

        [STAThread]
        static void Main(string[] args)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                App app = new App();
                mainWindow = new MainWindow();
                if (args.Length > 0 && args[0] == "/StartMinimized")
                {
                    app.Run();
                }
                else
                {
                    app.Run(mainWindow);
                }
                mutex.ReleaseMutex();
            }
            else
            {
                // send our Win32 message to make the currently running instance
                // jump on top of all the other windows
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }
        }
    }
}
