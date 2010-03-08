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
        private Thread bt;
        private NotifyIcon n;

        public MainWindow() {
            InitializeComponent();
        }

        void n_BalloonTipClicked(object sender, EventArgs e) {
            this.Close();
        }

        void n_BalloonTipClosed(object sender, EventArgs e) {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e) {
            n.Visible = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
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
            if (Thread.CurrentThread != bt) throw new Exception("plz to run in background thread k?");
            DateTime start = DateTime.Now;
            DateTime end = start.AddMinutes(3);
            TimeSpan total = (end - start);
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
            Dispatcher.BeginInvoke(new Action(() => { pbar.Maximum = total.Ticks; }));
            while (true) {
                DateTime now = DateTime.Now;
                if (now > end) break;
                TimeSpan progress = (now - start);
                TimeSpan remaining = (end - now).Add(TimeSpan.FromSeconds(1));
                var reportText = String.Format("{0}:{1:D2}", remaining.Minutes, remaining.Seconds);
                Dispatcher.BeginInvoke(new Action(() => { plabel.Content = reportText; }));
                Dispatcher.BeginInvoke(new Action(() => { pbar.Value = progress.Ticks; }));
                TaskbarManager.Instance.SetProgressValue((int)progress.Ticks, (int)total.Ticks);
                Thread.Sleep(10);
            }
            Dispatcher.BeginInvoke(new Action(() => { pbar.Value = total.Ticks; }));
            TaskbarManager.Instance.SetProgressValue((int)total.Ticks, (int)total.Ticks);
            var balloonTimeout = 1000 * 60 * 10; // 10 minutes
            Dispatcher.BeginInvoke(new Action(() => {
                n.Visible = true;
                n.ShowBalloonTip(balloonTimeout);
            }));
            Thread.Sleep(balloonTimeout);
            Dispatcher.BeginInvoke(new Action(this.Close));
        }
    }
}
