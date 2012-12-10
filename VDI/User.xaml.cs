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
        public ArrayList domainList {get; set;}
		public User()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.
		}
        public User(ArrayList domains)
            : this()
        {
            domainList = domains;
            
        }
        private void comboBoxSizeType_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            box.ItemsSource = domainList.ToArray();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DesktopPools dpools = new DesktopPools();
            this.NavigationService.Navigate(dpools);
        }
	}
}