using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RDPConnecter
{
    public partial class RemoteDesktopWindow : Form
    {
        public RemoteDesktopWindow(string title, string server, int port, string username, string password, int width, int height,
            bool fullScreen)
        {
            InitializeComponent();
            InitializeControlEvents();
            setRemoteDesktop(server, port, username, password, width, height, fullScreen);
            this.Text = title;
        }

        public void setRemoteDesktop(string server, int port, string username, string password, int width, int height,
            bool fullScreen, int colorDepth = 16)
        {
            rdpClient.Server = server;
            rdpClient.AdvancedSettings2.RDPPort = port;
            rdpClient.UserName = username;
            //rdpClient.Domain = sd.dom
            rdpClient.AdvancedSettings2.ClearTextPassword = password;
            rdpClient.ColorDepth = colorDepth;
            rdpClient.DesktopWidth = width;
            rdpClient.DesktopHeight = height;
            rdpClient.FullScreen = fullScreen;
            this.panel.Size = new System.Drawing.Size(width, height);
            this.rdpClient.Size = new System.Drawing.Size(width, height);
            this.ClientSize = new System.Drawing.Size(width, height);
            if (fullScreen)
            {
                rdpClient.DesktopWidth = Screen.PrimaryScreen.Bounds.Width;
                rdpClient.DesktopHeight = Screen.PrimaryScreen.Bounds.Height;
            }

            // this fixes the rdp control locking issue
            // when lossing its focus
            //rdpClient.AdvancedSettings3.ContainerHandledFullScreen = -1;
            //rdpClient.AdvancedSettings3.DisplayConnectionBar = true;
            //rdpClient.FullScreen = true;
            //rdpClient.AdvancedSettings3.SmartSizing = true;
            //rdpClient.AdvancedSettings3.PerformanceFlags = 0x00000100;

            //rdpClient.AdvancedSettings2.allowBackgroundInput = -1;
            rdpClient.AdvancedSettings2.AcceleratorPassthrough = -1;
            rdpClient.AdvancedSettings2.Compress = -1;
            rdpClient.AdvancedSettings2.BitmapPersistence = -1;
            rdpClient.AdvancedSettings2.BitmapPeristence = -1;
            //rdpClient.AdvancedSettings2.BitmapCacheSize = 512;
            rdpClient.AdvancedSettings2.CachePersistenceActive = -1;
            rdpClient.AdvancedSettings.DisableRdpdr = 0;            // 启用重定向
            rdpClient.AdvancedSettings2.RedirectDrives = true;      // 磁盘重定向
            rdpClient.AdvancedSettings2.RedirectPorts = true;       // 串口重定向
            rdpClient.AdvancedSettings2.RedirectPrinters = true;    // 打印机重定向
            rdpClient.AdvancedSettings2.RedirectSmartCards = true;  // 智能卡重定向

            rdpClient.AdvancedSettings6.RedirectDevices = true;     // 设备重定向
            rdpClient.AdvancedSettings6.RedirectPOSDevices = true;  // POS设备重定向
            rdpClient.AdvancedSettings6.RedirectClipboard = true;   // 剪贴板重定向

        }

        public void setRDGW(string hostname, string username, string password, uint usageMethod = 1, uint credsSource = 0, uint profileUsageMethod = 1)
        {
            rdpClient.TransportSettings2.GatewayHostname = hostname;
            rdpClient.TransportSettings2.GatewayUsername = username;
            rdpClient.TransportSettings2.GatewayPassword = password;
            rdpClient.TransportSettings2.GatewayUsageMethod = usageMethod;
            rdpClient.TransportSettings2.GatewayCredsSource = credsSource;
            rdpClient.TransportSettings2.GatewayProfileUsageMethod = profileUsageMethod;
        }

        public void connect()
        {
            rdpClient.Connect();
        }

        public void InitializeControlEvents()
        {
            this.FormClosing += new FormClosingEventHandler(RdpClientWindow_FormClosing);
            this.rdpClient.OnDisconnected += new AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEventHandler(rdpClient_OnDisconnected);
        }

        void RdpClientWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("确定要断开连接并关闭窗口？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dr == DialogResult.Yes)
            {
                if (rdpClient.Connected == 1)
                    rdpClient.Disconnect();
                rdpClient.Dispose();
                Dispose();
            }
            else
            {
                e.Cancel = true;
            }
        }

        void rdpClient_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            rdpClient.Dispose();
            Dispose();
        }
    }
}
