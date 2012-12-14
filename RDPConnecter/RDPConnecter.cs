using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace RDPConnecter
{
    public partial class RDPConnecter : Form
    {
        private ArrayList f = new ArrayList();
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
            string formTitlePattern = "云晫 - {0}@{1}[{2}]";
            string formTitle = string.Format(formTitlePattern, username, "pool1", server);
            bool canCreateNewForm = true;
            foreach (Form ff in f)
            {
                if (ff != null && ff.Text == formTitle)
                {
                    ff.Activate();
                    canCreateNewForm = false;
                    break;
                }
            }
            if (canCreateNewForm)
            {
                RemoteDesktopWindow rdw = new RemoteDesktopWindow(formTitle, server, port, username, password, width, height, fullScreen);
                rdw.Show();
                rdw.BringToFront();
                f.Add(rdw);
            }
        }
    }
}
