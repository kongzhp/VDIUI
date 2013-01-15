using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Collections;
namespace VDI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public ArrayList domainList; // 全局的domainList, 以便页面跳转
        public string serverIP; // 全局服务器IP
    }
}
