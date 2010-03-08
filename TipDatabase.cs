using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeaTimer {
    class TipDatabase {
        private static List<string> tips = new List<string> {
            "Right-click on icon in taskbar and select \"Pin this program to taskbar\" for easy access.",
            "Minimize or hide this window.  Timer status is visible in taskbar.",
            "Minimize or hide this window.  You will be properly notified when the timer is done.",
        };
        private static Random rng = new Random();

        public static string GetTip() {
            return tips[rng.Next(tips.Count)];
        }
    }
}
