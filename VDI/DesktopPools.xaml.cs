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
using MSTSCLib;
using log4net;
namespace VDI
{
    /// <summary>
    /// Interaction logic for DesktopPools.xaml
    /// </summary>
     delegate void UpdateTheUI();
    public partial class DesktopPools : Page
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DesktopPools));
        public ArrayList PList { get; set; }
        public String UserID { get; set; }
        public String UserName { get; set; }
        public String ServerIP { get; set; }
        public String DomainName { get; set; }
        public String Password { get; set; }
        private string domainID;
        public delegate void desktopStatDelegate();
        private ServerCommunicator serverChannel;
        private ArrayList rdpWinList = new ArrayList();

        public DesktopPools()
        {
            
            InitializeComponent();
            serverChannel = new ServerCommunicator();
            poolListBox.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
        }

        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (poolListBox.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                foreach (Pool pitem in poolListBox.Items)
                {
                    if (!pitem.Ready)
                    {
                        ListBoxItem lbi = poolListBox.ItemContainerGenerator.ContainerFromItem(pitem) as ListBoxItem;
                        lbi.IsEnabled = false;
                    }
                }
            }
        }

        public DesktopPools(String ip, String userID, String userName , String pwd, PoolList pList, String domainName, string domainid)
            : this()
        {
            UserID = userID;
            UserName = userName;
            PList = pList.getPools();
            userLabel.Content = userName;
            DomainName = domainName;
            poolListBox.ItemsSource = PList;
            domainID = domainid;
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
        private void updateStatus(ListBoxItem poolSelItem, string statusString, bool loading)
        {
                this.Dispatcher.BeginInvoke(
                   

                     System.Windows.Threading.DispatcherPriority.Normal,
                            (UpdateTheUI)delegate()
                            {
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
        public void checkDesktopStatus(object param)
        {
            ArrayList p = (ArrayList)param;
            Pool poolSel = (Pool)p[0];
            Gateway gw = (Gateway)p[1];
            string pixel = (string)p[2];
            ListBoxItem poolSelItem = (ListBoxItem)p[3];
            string RequestID = (string)p[4];
            bool ready = false; //标记桌面是否已准备好
            while (!ready)
            {
                string status = "";
                try
                {
                    status = serverChannel.getDesktopStatus(ServerIP, RequestID);
                }
                catch (Exception e)
                {
                    logger.Error("为用户" + UserName + "请求桌面状态时产生错误：" + e.Message);
                    updateStatus(poolSelItem, "无法获取桌面状态", false);
                    return;
                }
                if (status == null)
                {
                    //desktopStatus.Text = "没有空闲桌面";
                    logger.Error("没有空闲桌面可分配给用户" + UserName);
                    updateStatus(poolSelItem, "没有空闲桌面", false);
                    return;
                }
                else if (status.Equals("not ready"))
                {
                    updateStatus(poolSelItem, "桌面正在启动", true);
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
                                        string formTitle = string.Format(formTitlePattern, UserName, poolSel.Name);
                                        Thread.Sleep(10000);
                                        RemoteDesktopWindow rdw = new RemoteDesktopWindow(formTitle, ip, port, UserName, Password, width, height, fullScreen);
                                        if (null != gw)
                                            rdw.setRDGW(gw.getAddress(), gw.getUsername(), gw.getPassword());
                                        rdw.connect();
                                        rdw.Show();
                                        rdw.BringToFront();
                                        rdpWinList.Add(rdw);
                                        ready = true;
                                        updateStatus(poolSelItem, "准备", false);
                                     });
                 
                    // 桌面已准备好，开始连接
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string displayMode = (string)((ComboBoxItem)displayComboBox.SelectedValue).Content;
            Pool poolSel = (Pool)poolListBox.SelectedItem;
            string poolname = poolSel.Name;
            ArrayList gateways = poolSel.getGateways();
            Gateway gw = null;
            ComboBoxItem pixelItem = (ComboBoxItem)displayComboBox.SelectedItem;
            string pixel = (string)pixelItem.Content;
            ListBoxItem poolSelItem = (ListBoxItem)(poolListBox.ItemContainerGenerator.ContainerFromItem(poolListBox.SelectedItem));
            if (null != gateways && gateways.Count > 0)
            {
                Random random = new Random();
                int randomIndex = random.Next(gateways.Count);
                gw = (Gateway)gateways[randomIndex];
            }
            
            string formTitlePattern = "云晫 - {0}@{1}";
            string formTitle = string.Format(formTitlePattern, UserName, poolname, poolSelItem);
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
                Thread conThread = new Thread(new ParameterizedThreadStart(connectPool));
                ArrayList param = new ArrayList() { poolSel, gw, pixel, poolSelItem };
                conThread.Start(param);              
            }
        }

        private void connectPool(Object param)
        {
            ArrayList p = (ArrayList)param;
            Pool poolSel = (Pool)p[0];
            ListBoxItem poolSelItem = (ListBoxItem)p[3];
            string RequestID = "";
            updateStatus(poolSelItem, "正在请求桌面", true);
            //向服务器请求桌面
            try
            {
                RequestID = serverChannel.requestDesktop(ServerIP, UserID, DomainName, poolSel.Id);
            }
            catch (Exception ex)
            {
                logger.Error("为用户" + UserName + "请求桌面池" + poolSel.Name + "中的桌面时产生错误：" + ex.Message);
                updateStatus(poolSelItem, "请求桌面失败", false);
                return;
            }
            p.Add(RequestID);
            Thread checkThread = new Thread(new ParameterizedThreadStart(checkDesktopStatus));
            checkThread.Start(p);
        }

        private void poolListBox_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Pool pitem in poolListBox.Items)
            {
                if (!pitem.Ready)
                {
                    ListBoxItem lbi = poolListBox.ItemContainerGenerator.ContainerFromItem(pitem) as ListBoxItem;
                    lbi.IsEnabled = false;
                }
            }
        }
        private void refreshPools()
        {
            try {
                this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    (UpdateTheUI)delegate()
                    {
                        contentPanel.IsEnabled = false;
                    });
                GetPoolResult res = serverChannel.getPoosWithAuth(ServerIP, UserName, Password, domainID);
                this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    (UpdateTheUI)delegate()
                    {
                            if (res == null)
                            {
                                logger.Error("刷新桌面池时验证失败，即用户名或密码不正确");
                                String errorText = "验证失败，请检查用户是否已经被撤销，或者密码有无被更改，并尝试重新登录！";
                                MessageBoxButton btn = MessageBoxButton.OK;
                                MessageBoxImage img = MessageBoxImage.Error;
                                System.Windows.MessageBox.Show(errorText, "验证失败", btn, img);
                            }
                            else
                            {
                                poolListBox.ItemsSource = res.getPoolList().getPools();
                            }     
                    });
            }
            catch (Exception ex)
            {
                logger.Error("更新桌面池信息时产生错误：" + ex.Message);
                String errorText = "更新桌面池超时或者产生错误，请确保网络连通，或联系网络管理员。";
                MessageBoxButton btn = MessageBoxButton.OK;
                MessageBoxImage img = MessageBoxImage.Error;
                System.Windows.MessageBox.Show(errorText, "网络异常", btn, img);
            }
            finally
            {
                this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    (UpdateTheUI)delegate()
                    {
                        contentPanel.IsEnabled = true;
                        this.Cursor = System.Windows.Input.Cursors.Arrow;
                    });
            }
        }
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            //改变光标为loading
            Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
            Thread refThread = new Thread(refreshPools);
            refThread.Start();
            this.Cursor = System.Windows.Input.Cursors.Wait;
            Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
        }
        //退出登录
        private void userLabel_Click(object sender, RoutedEventArgs e)
        {
            User userPage = new User((System.Windows.Application.Current as App).domainList,
                                       (System.Windows.Application.Current as App).serverIP);
            this.NavigationService.Navigate(userPage);
            
        }
    }
    
}
