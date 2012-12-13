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
    public partial class RDPConnecter : Form
    {
        public RDPConnecter()
        {
            InitializeComponent();
        }

        private void connect_Click(object sender, EventArgs e)
        {
            string server = serverText.Text;
            int port = Int32.Parse(portText.Text);
            string username = usernameText.Text;
            string password = passwordText.Text;
            int width = 800;
            int height = 600;
            bool fullScreen = isFullScreen.Checked;
            if (!fullScreen)
            {
                width = Int32.Parse(widthText.Text);
                height = Int32.Parse(heightText.Text);
            }
            RemoteDesktopWindow rdw = new RemoteDesktopWindow(server, port, username, password, width, height, fullScreen);
            rdw.Show();
            rdw.BringToFront();
        }
    }
}
