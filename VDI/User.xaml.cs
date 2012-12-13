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
namespace VDI
{
	public partial class User
	{
        private ArrayList DomainList {get; set;} //domainList每个元素是Domains对象
       // public ArrayList DomainNameList { get; set; } //把DomainList的string[0]保存到此，绑定到combobox
        public PoolList Plist { get; set; } //桌面池
        public String UserID { get; set; }         //用户ID
        public String IP { get; set; }             //服务器IP
		public User()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.
		}
        public User(ArrayList domains , String ip)
            : this()
        {
            
            DomainList = domains;
            IP = ip;
            
        }
        private void domainListBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            box.ItemsSource = DomainList;   
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //改变光标为loading
            Mouse.SetCursor(Cursors.Wait);
            //获取用户名、密码、domain
            String userName = username.Text;
            String pwd = password.Password;

            String domainID = (String)domainListBox.SelectedValue;
            String domainName = ((Domain)domainListBox.SelectedItem).Name;
            ServerCommunicator serverChannel = new ServerCommunicator();
            //向服务器请求pool列表
            GetPoolResult res = serverChannel.getPoosWithAuth(IP, userName, pwd, domainID);
            UserID = res.getUserId();
            Plist = res.getPoolList();
            DesktopPools dpools = new DesktopPools(IP, UserID, userName, Plist, domainName);
            this.NavigationService.Navigate(dpools);
        }
	}
}