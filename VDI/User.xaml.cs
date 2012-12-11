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
namespace VDI
{
	public partial class User
	{
        private ArrayList DomainList {get; set;} //domainList每个元素是个string[2]数组，string[0]为domain name ,string[1]为domain id
        public ArrayList DomainNameList { get; set; } //把DomainList的string[0]保存到此，绑定到combobox

		public User()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.
		}
        public User(ArrayList domains)
            : this()
        {
            DomainNameList = new ArrayList();
            DomainList = domains;
            foreach (String[] domain in DomainList)
            {
                DomainNameList.Add(domain[0]);
            }
            
        }
        private void domainListBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            box.ItemsSource = DomainNameList;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            DesktopPools dpools = new DesktopPools();
            this.NavigationService.Navigate(dpools);
        }
	}
}