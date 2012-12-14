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
using RDPConnecter;
using System.Windows.Threading;
using System.Threading;
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
        public String ServerIP { get; set; }
        public String DomainName { get; set; }
        public String  RequestID { get; set; }
        public delegate void desktopStatDelegate();
        private ServerCommunicator serverChannel;
        public DesktopPools()
        {
            
            InitializeComponent();
            serverChannel = new ServerCommunicator();
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
            ServerIP = ip;

        }
        public void checkDesktopStatus()
        {
            bool ready = false; //标记桌面是否已准备好
            while (!ready)
            {
                Thread.Sleep(5000);
                string status = serverChannel.getDesktopStatus(ServerIP, RequestID);
                if (status == null)
                {
                    statusBlock.Text = "没有空闲桌面";
                    ready = true;
                }
                else if (status.Equals("not ready"))
                {
                    statusBlock.Text = "桌面正在准备中";                 
                }
                else
                {
                    statusBlock.Text = status;
                    ready = true;
                    //桌面已准备好，开始连接
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
      
            String displayMode = (String)((ComboBoxItem)displayComboBox.SelectedValue).Content;
            Pool poolSel = (Pool)poolListBox.SelectedItem;
            Mouse.SetCursor(Cursors.Wait); //把鼠标设置为等待状态
            RequestID = null;
            try
            {
                //向服务器请求桌面
                RequestID = serverChannel.requestDesktop(ServerIP, UserID, DomainName, poolSel.Id);
                Mouse.SetCursor(Cursors.Arrow);
                if (RequestID == null)
                {
                    statusBlock.Text = "请重试";
                }
                else
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, 
                                                    new desktopStatDelegate(checkDesktopStatus));
                }

            }
            catch (Exception ex) { }

            //String server = "222.200.185.55";
            //int port = 8007;
            //String username = "kongzhp";
            //String password = "admin123";
            //int width = 1024;
            //int height = 800;
            //bool fullScreen = true;
            //RemoteDesktopWindow rdw = new RemoteDesktopWindow(server, port, username, password, width, height, fullScreen);
            //rdw.Show();
            //rdw.BringToFront();


        }
    }
    
}
