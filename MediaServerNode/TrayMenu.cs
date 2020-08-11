using MediaServerNode.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaServerNode {
    public class TrayMenu : ApplicationContext {
        public NotifyIcon trayIcon;

        public TrayMenu() {
            trayIcon = new NotifyIcon() {
                Icon = Resources.icon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Выйти", exit),
                    new MenuItem("Обновить", update),
                    new MenuItem("Настройки", settings)
                }),
                Visible = true
            };
            trayIcon.MouseClick += TrayIcon_Click;
            update(null, null);
        }

        private void TrayIcon_Click(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(trayIcon, null);
            }
        }

        void exit(object sender, EventArgs args) {
            trayIcon.Visible = false;
            Application.Exit();
        }
        public void update(object sender, EventArgs args) {
            try {
                string localIP = Program.getLocalIP();
                string[] files = { };
                try {
                    files = Directory.GetFiles(Program.mediaDirectory);
                } catch (Exception) {
                }
                StringBuilder dataStr = new StringBuilder();
                foreach (string file in files) {
                    dataStr.Append("http://" + localIP + ":" + Program.port + "/mediaServerNode/" + file.Substring(file.LastIndexOf('\\') + 1));
                    dataStr.Append("\t");
                    dataStr.Append(file.Substring(file.LastIndexOf('\\') + 1));
                    dataStr.Append("\n");
                }
                if (dataStr.Length > 0)
                    dataStr.Remove(dataStr.Length - 1, 1);
                byte[] data = Encoding.UTF8.GetBytes(dataStr.ToString());

                WebRequest request = WebRequest.CreateHttp("http://" + Program.mainServerUrl + "/update?pc_name=" + Program.machineName);
                request.Method = "POST";
                request.GetRequestStream().Write(data, 0, data.Length);
                request.GetResponse();

                if (Program.fileServer != null)
                    Program.fileServer.stop();
                Program.fileServer = new FileServer();
            } catch (Exception e) {
                Console.Error.WriteLine(e.ToString());
            }
        }
        void settings(object sender, EventArgs args) {
            new SettingsForm().ShowDialog();
        }
    }
}
