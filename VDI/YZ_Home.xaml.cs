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
            User userPage = new User();
            this.NavigationService.Navigate(userPage);
        }
    }
}
