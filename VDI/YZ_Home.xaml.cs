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
using System.Net;
using System.Threading;
namespace VDI
{
    /// <summary>
    /// YZ_Home.xaml 的交互逻辑
    /// </summary>
    public partial class YZ_Home : Page
    {
        private ServerCommunicator serverChannel;
        private string ip;
        public YZ_Home()
        {
            InitializeComponent();
            serverChannel = new ServerCommunicator();
        }
        private void connectServer()
        {
            try
            {
                this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    (UpdateTheUI)delegate()
                    {
                        forwardButton.IsEnabled = false;
                        ipTextBox.IsEnabled = false;
                    });
            
                (Application.Current as App).domainList = serverChannel.getDomains(ip);
                //把光标样式改回arrow
                // this.Cursor = Cursors.Arrow;
                this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    (UpdateTheUI)delegate()
                {
                    User userPage = new User((Application.Current as App).domainList, ip);
                    this.NavigationService.Navigate(userPage);
                });
            }                
                catch (Exception webex)
                {
                    String errorText = "连接超时，请确保服务器IP正确，或联系网络管理员";
                    MessageBoxButton btn = MessageBoxButton.OK;
                    MessageBoxImage img = MessageBoxImage.Error;
                    MessageBox.Show(errorText, "网络异常", btn, img);
                    this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    (UpdateTheUI)delegate()
                    {
                        this.Cursor = Cursors.Arrow;
                        forwardButton.IsEnabled = true;
                        ipTextBox.IsEnabled = true;
                    });
                    
                }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ip = ipTextBox.Text;
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
                //把光标样式改为pen
                
                
                //this.Visibility = System.Windows.Visibility.Hidden;
                //this.Visibility = System.Windows.Visibility.Visible;
               
     
                    /*ArrayList ipOfServers = serverChannel.getServersOfCluster(ip); //获取在同一集群上的所有vdi的IP
                    //随机从ipOfServers中选取一个IP
                    Random random = new Random();
                    int randomIndex = random.Next(ipOfServers.Count);
                    String IPsel = (String)ipOfServers[randomIndex];*/
                   // ArrayList domains = serverChannel.getDomains(ip);
                    //把光标样式改回arrow
                   // this.Cursor = Cursors.Arrow;
                   // User userPage = new User(domains, ip);
                    //this.NavigationService.Navigate(userPage);
                    (Application.Current as App).serverIP = ip;
                    Thread connThread = new Thread(connectServer);
                    connThread.Start();
                    
                    this.Cursor = Cursors.Wait;
                    Mouse.SetCursor(Cursors.Wait);

            }
        }

        private void ipTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Button_Click(sender, e);
            }
        }
    }
}
