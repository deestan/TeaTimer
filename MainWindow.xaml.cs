using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TeaTimer {
    using Microsoft.WindowsAPICodePack;
    using Microsoft.WindowsAPICodePack.Taskbar;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Thread bt = null;
        private NotifyIcon n;
        private bool taskbarProgress;
        private bool progressInTitle;
        private string origTitle;
        private string progressTitle;
        private const int BALLOON_TIMEOUT = 1000 * 10; // 10 seconds

        public MainWindow() {
            InitializeComponent();
        }

        void n_BalloonTipClicked(object sender, EventArgs e) {
            this.Close();
        }

        void n_BalloonTipClosed(object sender, EventArgs e) {
            n.ShowBalloonTip(BALLOON_TIMEOUT);
        }

        private void Window_Closed(object sender, EventArgs e) {
            n.Visible = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.origTitle = this.Title + " " + StartupParams.DurationDescription;
            this.taskbarProgress = TaskbarManager.IsPlatformSupported;
            tipbox.Text = "Tip: " + TipDatabase.GetTip();
            n = new NotifyIcon();
            n.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            n.Text = "TeaTimer";
            n.BalloonTipTitle = "Ding dong!  Tea is done!";
            n.BalloonTipText = "Click to dismiss.";
            n.BalloonTipClosed += new EventHandler(n_BalloonTipClosed);
            n.BalloonTipClicked += new EventHandler(n_BalloonTipClicked);
            bt = new Thread(RunThread);
            bt.IsBackground = true;
            bt.Start();
        }

        private void RunThread() {
            bool turnedRed = false;
            if (Thread.CurrentThread != bt) throw new Exception("plz to run in background thread k?");
            DateTime start = DateTime.Now;
            DateTime end = (start + StartupParams.Duration);
            DateTime almostDone = end.Subtract(TimeSpan.FromSeconds(10));
            if (taskbarProgress) TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
            Dispatcher.BeginInvoke(new Action(() => { pbar.Maximum = StartupParams.Duration.Ticks; }));
            while (true) {
                DateTime now = DateTime.Now;
                if (now > almostDone && !turnedRed) {
                    if (taskbarProgress) TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                    turnedRed = true;
                }
                if (now > end) break;
                TimeSpan progress = (now - start);
                TimeSpan remaining = (end - now).Add(TimeSpan.FromSeconds(1));
                var reportText = String.Format("{0}:{1:D2}", remaining.Minutes, remaining.Seconds);
                Dispatcher.BeginInvoke(new Action(() => { this.progressTitle = reportText; this.SetTitle(); }));
                Dispatcher.BeginInvoke(new Action(() => { plabel.Content = reportText; }));
                Dispatcher.BeginInvoke(new Action(() => { pbar.Value = progress.Ticks; }));
                if (taskbarProgress) TaskbarManager.Instance.SetProgressValue((int)progress.TotalMilliseconds, (int)StartupParams.Duration.TotalMilliseconds);
                Thread.Sleep(100);
            }
            Dispatcher.BeginInvoke(new Action(() => { plabel.Content = ":-D"; }));
            Dispatcher.BeginInvoke(new Action(() => { pbar.Value = StartupParams.Duration.Ticks; }));
            if (taskbarProgress) TaskbarManager.Instance.SetProgressValue((int)StartupParams.Duration.TotalMilliseconds, (int)StartupParams.Duration.TotalMilliseconds);
            Dispatcher.BeginInvoke(new Action(() => {
                n.Visible = true;
                n.ShowBalloonTip(BALLOON_TIMEOUT);
            }));
        }

        private void SetTitle() {
            if (this.progressInTitle)
                this.Title = this.progressTitle + " " + this.origTitle;
            else
                this.Title = this.origTitle;
        }

        private void Window_StateChanged(object sender, EventArgs e) {
            this.progressInTitle = this.WindowState == WindowState.Minimized;
            this.SetTitle();
        }
    }
}
