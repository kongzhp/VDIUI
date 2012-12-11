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
        public DesktopPools()
        {
            
            InitializeComponent();
        }
        public DesktopPools(String userID, String userName , PoolList pList)
            : this()
        {
            UserID = userID;
            UserName = userName;
            PList = pList.getPools();
        }
    }
    
}
