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
using System.Windows.Forms;
using ServerChannel;
using RDPConnecter;
using System.Windows.Threading;
using System.Threading;
namespace VDI
{
    /// <summary>
    /// Interaction logic for DesktopPools.xaml
    /// </summary>
     delegate void UpdateTheUI();
    public partial class DesktopPools : Page
    {
        public ArrayList PList { get; set; }
        public String UserID { get; set; }
        public String UserName { get; set; }
        public String ServerIP { get; set; }
        public String DomainName { get; set; }
        public String  RequestID { get; set; }
        public String Password { get; set; }
        private ComboBoxItem pixelItem;
        private string pixel;
        private string poolname;
        public delegate void desktopStatDelegate();
        private ServerCommunicator serverChannel;
        private ArrayList rdpWinList = new ArrayList();
        public DesktopPools()
        {
            
            InitializeComponent();
            serverChannel = new ServerCommunicator();
        }

        public DesktopPools(String ip, String userID, String userName , String pwd, PoolList pList, String domainName)
            : this()
        {
            UserID = userID;
            UserName = userName;
            PList = pList.getPools();
            userLabel.Content = userName;
            DomainName = domainName;
            poolListBox.ItemsSource = PList;
            ServerIP = ip;
            Password = pwd;
        }
        //用于查找datatemplate里的命名元素
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        //用于更新被选中的pool的UI
        private void updateStatus(string statusString , bool loading)
        {
                this.Dispatcher.BeginInvoke(
                   

                     System.Windows.Threading.DispatcherPriority.Normal,
                            (UpdateTheUI)delegate()
                            {
                                ListBoxItem poolSelItem = 
                                    (ListBoxItem)(poolListBox.ItemContainerGenerator.ContainerFromItem(poolListBox.SelectedItem));
                                ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(poolSelItem);
                                DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
                                System.Windows.Controls.Label statLabel = 
                                    (System.Windows.Controls.Label)myDataTemplate.FindName("desktopStatus", myContentPresenter);
                                statLabel.Content = statusString;
                                Image imageICON = (Image)myDataTemplate.FindName("loadingGIF", myContentPresenter);
                                if (loading)
                                    imageICON.Visibility = Visibility.Visible;
                                else
                                    imageICON.Visibility = Visibility.Hidden;
                            });
        }
        public void checkDesktopStatus()
        {
            bool ready = false; //标记桌面是否已准备好
            while (!ready)
            {         
                string status = serverChannel.getDesktopStatus(ServerIP, RequestID);
                if (status == null)
                {
                    //desktopStatus.Text = "没有空闲桌面";
                    updateStatus("没有空闲桌面", false);
                    ready = true;
                }
                else if (status.Equals("not ready"))
                {
                    updateStatus("桌面正在启动", true);
                    Thread.Sleep(2000);
                }
                else
                {
                    string[] desktop = status.Split(':');
                    string ip = desktop[0];
                    int  port = Int32.Parse(desktop[1]);
                    bool fullScreen = false;
                    int width = 800;
                    int height = 600;
                    
                   
                    if (pixel.Equals("全屏"))
                    {
                        fullScreen = true;
                    }
                    else
                    {
                        string[] pixelstring = pixel.Split('*');
                        width = Int32.Parse(pixelstring[0]);
                        height = Int32.Parse(pixelstring[1]);
                    }
                    this.Dispatcher.BeginInvoke(
                             System.Windows.Threading.DispatcherPriority.Normal,
                                (UpdateTheUI)delegate()
                                     {
                                        //statusBlock.Text = "桌面正在启动...";
                                         //updateStatus("桌面正在启动");
                                        string formTitlePattern = "云晫 - {0}@{1}";
                                        string formTitle = string.Format(formTitlePattern, UserName, poolname);                                      
                                        RemoteDesktopWindow rdw = new RemoteDesktopWindow(formTitle, ip, port, UserName, Password, width, height, fullScreen);
                                        rdw.Show();
                                        rdw.BringToFront();
                                        rdpWinList.Add(rdw);
                                        ready = true;
                                        updateStatus("准备", false);
                                     });
                 
                    // 桌面已准备好，开始连接
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
      
            String displayMode = (String)((ComboBoxItem)displayComboBox.SelectedValue).Content;
            Pool poolSel = (Pool)poolListBox.SelectedItem;
            poolname = poolSel.Name;
            Mouse.SetCursor(System.Windows.Input.Cursors.Wait); //把鼠标设置为等待状态
            RequestID = null;

            string formTitlePattern = "云晫 - {0}@{1}";
            string formTitle = string.Format(formTitlePattern, UserName, poolname);
            bool canCreateNewForm = true;
            // 检查是否已经开启该pool的桌面
            foreach (Form f in rdpWinList)
            {
                if (f != null && f.Text == formTitle)
                {
                    f.Activate();
                    canCreateNewForm = false;
                    break;
                }
            }
            if (canCreateNewForm)
            {
                try
                {
                    //向服务器请求桌面
                    RequestID = serverChannel.requestDesktop(ServerIP, UserID, DomainName, poolSel.Id);
                    Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
                    if (RequestID == null)
                    {
                        //statusBlock.Text = "请重试";
                    }
                    else
                    {
                        // statusBlock.Dispatcher.BeginInvoke(DispatcherPriority.Normal, 
                        //                                 new desktopStatDelegate(checkDesktopStatus));
                        pixelItem = (ComboBoxItem)displayComboBox.SelectedItem;
                        pixel = (string)pixelItem.Content;
                        Thread checkThread = new Thread(checkDesktopStatus);
                        checkThread.Start();
                    }

                }
                catch (Exception ex) { }
            }

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
