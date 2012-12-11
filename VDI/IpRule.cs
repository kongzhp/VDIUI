using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Text.RegularExpressions;
namespace VDI
{
    public class IpRule : ValidationRule
    {
        public String IP { get; set; }  //臆造的属性，只是用于绑定，无实际意义

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (isValidIP(value.ToString()))
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                
                return new ValidationResult(false, "IP地址格式:x.x.x.x 如：192.168.0.1");
            }
        }
        public static bool isValidIP(string ip)
        {
            //判断是否为IP
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
    }
}
