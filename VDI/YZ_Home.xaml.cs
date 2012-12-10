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
using ServerChannel;
using System.Collections;
namespace VDI
{
    /// <summary>
    /// YZ_Home.xaml 的交互逻辑
    /// </summary>
    public partial class YZ_Home : Page
    {
        public YZ_Home()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String ip = ipTextBox.Text;
            ip = ip.Trim(); //去除字符串前的空格
            //假如ip为空或格式不正确，指出错误
            if (ip == "" || ip == null || ip == String.Empty)
            {
                warningBlock.Text = "* IP地址不能为空！";
                warningBlock.Style = (Style)this.Resources["warningBoxStyle"];

            }
            else if (!IpRule.isValidIP(ip))                //格式不正确
            {
                warningBlock.Text = "* IP格式不正确！";
                warningBlock.Style = (Style)this.Resources["warningBoxStyle"];
            }
            else
            {
                ServerCommunicator serverChannel = new ServerCommunicator();
                ArrayList ipOfServers = serverChannel.getServersOfCluster(ip); //获取在同一集群上的所有vdi的IP
                //随机从ipOfServers中选取一个IP
                Random random = new Random();
                int randomIndex = random.Next(ipOfServers.Count);
                String IPsel = (String)ipOfServers[randomIndex];
                ArrayList domains = serverChannel.getDomains(IPsel);

                User userPage = new User(domains);
                this.NavigationService.Navigate(userPage);
            }
        }
    }
}
