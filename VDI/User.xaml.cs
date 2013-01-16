using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections;
using ServerChannel;
using System.Threading;
using log4net;
namespace VDI
{
	public partial class User
	{
        private static readonly ILog logger = LogManager.GetLogger(typeof(YZ_Home));
        private ArrayList DomainList {get; set;} //domainList每个元素是Domains对象
       // public ArrayList DomainNameList { get; set; } //把DomainList的string[0]保存到此，绑定到combobox
        public PoolList Plist { get; set; } //桌面池
        public String UserID { get; set; }         //用户ID
        public String IP { get; set; }             //服务器IP
        private string userName;
        private string pwd;
        private string domainID;
        private ServerCommunicator serverChannel;
        private string domainName;
		public User()
		{
			this.InitializeComponent();
            serverChannel = new ServerCommunicator();
            //domainListBox.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
			// Insert code required on object creation below this point.
		}
        //当domain Box加载完时，把第一项设为默认
        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (domainListBox.ItemContainerGenerator.Status == 
                                          System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                
                domainListBox.SelectedIndex = 0;
            }
        }
        public User(ArrayList domains , String ip)
            : this()
        {
            
            DomainList = domains;
            IP = ip;
            domainListBox.ItemsSource = DomainList;
            
        }

        private void TextBoxItem_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox box = sender as TextBox;
            box.BorderBrush = Brushes.White;
            box.BorderThickness = new Thickness(1.0);
       
        }
        private void PasswordBoxItem_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;
            box.BorderBrush = Brushes.White;
            box.BorderThickness = new Thickness(1.0);

        }
        private void ComboBoxItem_LostFocus(object sender, RoutedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            box.BorderBrush = Brushes.White;
            box.BorderThickness = new Thickness(1.0);

        }
        private void connectServer()
        {
            try
            {
                this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    (UpdateTheUI)delegate()
                    {
                        contentPanel.IsEnabled = false;
                    });
                GetPoolResult res = serverChannel.getPoosWithAuth(IP, userName, pwd, domainID);
                this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    (UpdateTheUI)delegate()
                    {
                        if (res == null)
                        {
                            warningBlock.Text = "* 账号或密码错误。";
                            warningBlock.Style = (Style)this.Resources["warningBoxStyle"];
                        }
                        else
                        {

                            UserID = res.getUserId();
                            Plist = res.getPoolList();
                            DesktopPools dpools = new DesktopPools(IP, UserID, userName, pwd, Plist, domainName, domainID);
                            this.NavigationService.Navigate(dpools);
                        }
                  });
            }
            catch (Exception ex)
            {
                logger.Error("为用户" + userName + "从" + IP + "处获取桌面池信息时产生错误：" + ex.Message);
                String errorText = "登录超时或者产生错误，请确保网络连通，或联系网络管理员。";
                MessageBoxButton btn = MessageBoxButton.OK;
                MessageBoxImage img = MessageBoxImage.Error;
                MessageBox.Show(errorText, "网络异常", btn, img);
            }
            finally
            {
                this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    (UpdateTheUI)delegate()
                    {
                        contentPanel.IsEnabled = true;
                        this.Cursor = Cursors.Arrow;
                    });
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //改变光标为loading
            
            //获取用户名、密码、domain
            userName = username.Text;
            userName.Trim();
            pwd = password.Password;
            pwd.Trim();
            domainID = (String)domainListBox.SelectedValue;
            
            //检查用户名、密码、域名是否为空
            if (userName == "" || userName == null || userName == String.Empty)
            {
                warningBlock.Text = "* 用户名不能为空。";
                warningBlock.Style = (Style)this.Resources["warningBoxStyle"];
                username.Style = (Style)this.Resources["boxHightlight"];
            }
            else if (pwd == "" || pwd == null || pwd == String.Empty)
            {
                warningBlock.Text = "* 密码不能为空。";
                warningBlock.Style = (Style)this.Resources["warningBoxStyle"];
                password.Style = (Style)this.Resources["passwordboxHightlight"];
            }
            else if (domainListBox.SelectedItem == null)
            {
                warningBlock.Text = "* 请选择活动目录域。";
                warningBlock.Style = (Style)this.Resources["warningBoxStyle"];
                domainListBox.Style = (Style)this.Resources["comboboxHightlight"];
            }
            else
            {
                domainName = ((Domain)domainListBox.SelectedItem).Name;
                //向服务器请求pool列表
                Thread conServer = new Thread(connectServer);
                conServer.Start();
                this.Cursor = Cursors.Wait;
                Mouse.SetCursor(Cursors.Wait);
            }


        }




	}
}