using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Text.RegularExpressions;

namespace TeaTimer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e) {
            var timerDuration = parseParams(e.Args);
            StartupParams.Duration = timerDuration;
            var desc = new StringWriter();
            if (timerDuration.TotalHours >= 1.0d) desc.Write("a long time + ");
            if (timerDuration.Minutes > 0) desc.Write("" + timerDuration.Minutes + "m");
            if (timerDuration.Seconds > 0) desc.Write("" + timerDuration.Seconds + "s");
            StartupParams.DurationDescription = desc.ToString();
        }

        private TimeSpan parseParams(IEnumerable<string> clParams) {
            if (clParams.Count() == 0) return TimeSpan.FromMinutes(3);
            var sb = new StringWriter();
            foreach (var x in clParams) sb.Write(x);
            var allParams = sb.ToString().Replace(" ", "");
            var minMatch = new Regex("([0-9]+)m");
            var secMatch = new Regex("([0-9]+)s");
            TimeSpan acc = new TimeSpan();
            var txtMin = minMatch.Match(allParams);
            var txtSec = secMatch.Match(allParams);
            if (txtMin.Success) acc += TimeSpan.FromMinutes(int.Parse(txtMin.Groups[1].Value));
            if (txtSec.Success) acc += TimeSpan.FromSeconds(int.Parse(txtSec.Groups[1].Value));
            if (acc < TimeSpan.FromSeconds(1)) return TimeSpan.FromSeconds(1);
            return acc;
        }
    }
}
