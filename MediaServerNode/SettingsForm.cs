using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetFwTypeLib;


namespace MediaServerNode {
    public partial class SettingsForm : Form {
        public SettingsForm() {
            InitializeComponent();

            textBox2.Text = Program.port;
            textBox1.Text = Program.mediaDirectory;
            textBox3.Text = Program.mainServerUrl;
            textBox4.Text = Program.machineName;
            try {
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                firewallPolicy.Rules.Item("MediaServer Node");
                button3.Text = "удалить исключение из фаервола";
            } catch (FileNotFoundException) {

            }
        }

        private void button3_Click(object sender, EventArgs e) {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            try {
                firewallPolicy.Rules.Item("MediaServer Node");
                firewallPolicy.Rules.Remove("MediaServer Node");
                button3.Text = "добавить исключение в фаервол";
                Program.trayMenu.trayIcon.ShowBalloonTip(3000, "МедиаСервер", "Правило удалено", Program.trayMenu.trayIcon.BalloonTipIcon);
            } catch (FileNotFoundException) {
                INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                firewallRule.Enabled = true;
                firewallRule.InterfaceTypes = "All";
                firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                firewallRule.LocalPorts = "*";
                firewallRule.RemotePorts = "*";
                firewallRule.Name = "MediaServer Node";
                firewallRule.ApplicationName = Application.ExecutablePath;
                firewallPolicy.Rules.Add(firewallRule);
                button3.Text = "удалить исключение из фаервола";
                Program.trayMenu.trayIcon.ShowBalloonTip(3000, "МедиаСервер", "Правило добавлено", Program.trayMenu.trayIcon.BalloonTipIcon);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            Program.port = textBox2.Text;
            Program.mediaDirectory = textBox1.Text;
            Program.mainServerUrl = textBox3.Text;
            Program.machineName = textBox4.Text;
            Program.trayMenu.update(null, null);

            File.WriteAllLines("MediaServerNode.cfg", new string[] {
                Program.port,
                Program.mainServerUrl,
                Program.mediaDirectory,
                Program.machineName
            });

            Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            textBox1.Text = folderBrowser.SelectedPath;
        }
    }
}
