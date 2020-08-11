using MediaServerNode.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaServerNode {
    static class Program {
        public static string[] files = { };
        public static TrayMenu trayMenu;
        public static string port = "13124";
        public static string mainServerUrl = "media.loc";
        public static string mediaDirectory = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Desktop\\SharedMedia";
        public static string machineName = Environment.MachineName;
        public static FileServer fileServer;

        [STAThread]
        static void Main() {
            if (File.Exists("MediaServerNode.cfg")) {
                string[] settings = File.ReadAllLines("MediaServerNode.cfg");
                if (settings.Length >= 4) {
                    port = settings[0];
                    mainServerUrl = settings[1];
                    mediaDirectory = settings[2];
                    machineName = settings[3];
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            trayMenu = new TrayMenu();
            Application.Run(trayMenu);
        }

        public static string getLocalIP() {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            return null;
        }
    }
}
