﻿using System;
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
using System.Collections;
using ServerChannel;
using RDPConnecter;
namespace VDI
{
    /// <summary>
    /// Interaction logic for DesktopPools.xaml
    /// </summary>
    public partial class DesktopPools : Page
    {
        public ArrayList PList { get; set; }
        public String UserID { get; set; }
        public String UserName { get; set; }
        public String IP { get; set; }
        public String DomainName { get; set; }
        public DesktopPools()
        {
            
            InitializeComponent();
        }
        public DesktopPools(String ip, String userID, String userName , PoolList pList, String domainName)
            : this()
        {
            UserID = userID;
            UserName = userName;
            PList = pList.getPools();
            userLabel.Content = userName;
            DomainName = domainName;
            poolListBox.ItemsSource = PList;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ServerCommunicator serverChannel = new ServerCommunicator();
            String displayMode = (String)((ComboBoxItem)displayComboBox.SelectedValue).Content;
            Pool poolSel = (Pool)poolListBox.SelectedItem;
            String server = "222.200.185.55";
            int port = 8007;
            String username = "kongzhp";
            String password = "admin123";
            int width = 1024;
            int height = 800;
            bool fullScreen = true;
            RemoteDesktopWindow rdw = new RemoteDesktopWindow(server, port, username, password, width, height, fullScreen);
            rdw.Show();
            rdw.BringToFront();


        }
    }
    
}
