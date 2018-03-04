using Adora_Scratchpad.Properties;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Adora_Scratchpad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Start Notification Icon Code

            var notificationIcon = new System.Windows.Forms.NotifyIcon()
            {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name),
                Visible = true
            };

            notificationIcon.Click += delegate
            {
                ShowApplication();
            };

            var notificationIconContextMenu = new System.Windows.Forms.ContextMenu();
            notificationIconContextMenu.MenuItems.Add("Exit", (s, e) => Application.Current.Shutdown());
            notificationIcon.ContextMenu = notificationIconContextMenu;

            // End Notification Icon Code

            Loaded += delegate
            {
                DataContext = new ViewModel();
            };

            ContentRendered += delegate
            {
                textbox.Focus();
                textbox.SelectionStart = textbox.Text.Length;
                textbox.SelectionLength = 0;
            };

            Closing += delegate
            {
                Settings.Default.MainWindowPlacement = this.GetPlacement();
                Settings.Default.Save();
            };
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.SetPlacement(Settings.Default.MainWindowPlacement);

            // related to controlling the visible behavior of the single instance of the application
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        public void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.Enter) // Convert Ctrl + Enter to Enter
            {
                var editor = sender as TextBox;
                var caretIndex = editor.CaretIndex;
                editor.Text = editor.Text.Insert(caretIndex, Environment.NewLine);
                editor.CaretIndex = caretIndex + 1;
            }
        }

        private void ShowApplication()
        {
            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }
            Show();
            Activate();
        }

        // minimize to system tray when applicaiton is closed
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            base.OnClosing(e);
        }

        // related to controlling the visible behavior of the single instance of the application
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_SHOWME)
            {
                ShowApplication();
            }
            return IntPtr.Zero;
        }
    }
}
