using System;
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
                //把光标样式改为pen
                Mouse.SetCursor(Cursors.Wait);
                
                //this.Visibility = System.Windows.Visibility.Hidden;
                //this.Visibility = System.Windows.Visibility.Visible;
                ServerCommunicator serverChannel = new ServerCommunicator();
                try
                {
                    ArrayList ipOfServers = serverChannel.getServersOfCluster(ip); //获取在同一集群上的所有vdi的IP
                    //随机从ipOfServers中选取一个IP
                    Random random = new Random();
                    int randomIndex = random.Next(ipOfServers.Count);
                    String IPsel = (String)ipOfServers[randomIndex];
                    ArrayList domains = serverChannel.getDomains(IPsel);
                    //把光标样式改回arrow
                    // this.Cursor = Cursors.Arrow;
                    User userPage = new User(domains, IPsel);
                    this.NavigationService.Navigate(userPage);
                }
                catch (Exception webex)
                {
                    String errorText = "连接超时，请确保服务器IP正确，或联系网络管理员";
                    MessageBoxButton btn = MessageBoxButton.OK;
                    MessageBoxImage img = MessageBoxImage.Error;
                    MessageBox.Show(errorText, "网络异常", btn, img);
                }


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
