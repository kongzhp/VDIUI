/*********************************************************
 ** Filename: ServerCommunicator.cs
 ** Description: 与vdi后台交互，通过http请求获取所需信息
 ** Version: 1.0
 ** Author: Michael Yan
 ** Date: 2012-12-07
 ** Copyright (C) 2012 云晫科技
 **********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace ServerChannel
{
    /*
     * ClassName: ServerCommunicator
     * Description: 发送请求、接收结果并包装返回。有5个请求函数，分别获取5种信息：servers, domains, pools, desktop, status
     */
    class ServerCommunicator
    {
        private HttpWebRequest generateRequest(string url, string method = "GET", bool allowAutoRedirect = false)
        {
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            catch (Exception)
            {
                throw;
            }
            request.Method = method;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.AllowAutoRedirect = allowAutoRedirect;
            return request;
        }

        /*
         * Description: 返回给定ip的server所在的cluster中的其他server ip
         * Param: ip(string): server ip
         * Retern: ArrayList: 其他servers ip组成的arraylist，如{"192.168.0.1", "192.168.0.2"}
         *         如果为空数组，则表示返回信息为空或格式不对或没有server ip返回
         * Throw: 可能有以下几种类型的异常产生，此处不作处理直接抛出
         *   IOException, OutOfMemoryException: IO异常，产生自ReadToEnd
         *   WebExcption: 处理请求超时或发生错误，产生自GetResponse
         *   OtherException: 其他一些异常，产生自create request, StreamReader
         */
        public ArrayList getServersOfCluster(string ip)
        {
            if (null == ip || ip.Equals(""))
                return new ArrayList();

            string url = "http://" + ip + "/dp/rpc/dc/getservers";
            string s = "";

            try
            {
                HttpWebRequest request = generateRequest(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                s = sr.ReadToEnd().Trim();

                response.Close();
                sr.Close();
            }
            catch (Exception)
            {
                throw;
            }

            ArrayList result = new ArrayList();
            string[] ss = s.Split('\n');
            if (ss.Length < 2)
                return result;

            for (int i = 1; i < ss.Length; i++)
            {
                string[] sss = ss[i].Split('=');
                if (sss.Length < 2)
                    continue;
                result.Add(sss[1]);
            }

            return result;
        }

        /*
         * Description: 返回给定ip的server vdi虚拟机中所有的domain
         * Param: ip(string): server ip
         * Retern: ArrayList: 元素为domain信息，大小为2的string数组，string[0]为domain name，string[1]为domain id
         *         如果为空，则表示返回信息为空或格式不对或没有domain返回
         * Throw: 可能有以下几种类型的异常产生，此处不作处理直接抛出
         *   IOException, OutOfMemoryException: IO异常，产生自ReadToEnd
         *   WebExcption: 处理请求超时或发生错误，产生自GetResponse
         *   OtherException: 其他一些异常，产生自create request, StreamReader
         */
        public ArrayList getDomains(string ip)
        {
            if (null == ip || ip.Equals(""))
                return new ArrayList();

            string url = "http://" + ip + "/dp/rpc/dc/getdomains";
            string s = "";

            try
            {
                HttpWebRequest request = generateRequest(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                s = sr.ReadToEnd().Trim();

                response.Close();
                sr.Close();
            }
            catch (Exception)
            {
                throw;
            }

            ArrayList result = new ArrayList();
            string[] ss = s.Split('\n');
            if (ss.Length < 3)
                return result;

            int i = 1;
            while (i < ss.Length)
            {
                string[] domain = new string[2];

                // 获取domain name
                string[] sss = ss[i].Split('=');
                if (sss.Length < 2)
                {
                    i = i + 2;
                    continue;
                }
                domain[0] = sss[1];

                i = i + 1;
                if (i >= ss.Length)
                    return result;

                // 获取domain id
                sss = ss[i].Split('=');
                if (sss.Length < 2)
                {
                    i = i + 1;
                    continue;
                }
                domain[1] = sss[1];               

                result.Add(domain);
                i = i + 1;
            }

            return result;
        }

        /*
         * Description: 返回给定ip的server所在的cluster中的其他server ip
         * Param: ip(string): server ip
         *        username(string): username
         *        password(string): password
         *        domain(string): domain id
         * Retern: ArrayList: 用户有权限的桌面池列表，每个元素为一个大小为3的字符串数组，string[0]=poolname, string[1]=poolid, string[2]=poolready 
         *         如果为空数组，则表示返回信息为空或格式不对或没有pool返回
         * Throw: 可能有以下几种类型的异常产生，此处不作处理直接抛出
         *   IOException, OutOfMemoryException: IO异常，产生自ReadToEnd
         *   WebExcption: 处理请求超时或发生错误，产生自GetResponse
         *   OtherException: 其他一些异常，产生自create request, StreamReader
         */
        /*public ArrayList getPoosWithAuth(string ip, string username, string password, string domain)
        {
            if (null == ip || ip.Equals(""))
                return new ArrayList();

            string url = "http://" + ip + "/dp/rpc/dc/login";
            string postcontent = "user=" + username + "&password=" + password + "&ldap=" + domain;
            string s = "";

            try
            {
                HttpWebRequest request = generateRequest(url, "POST", true);
                byte[] Postbyte = Encoding.ASCII.GetBytes(postcontent);
                request.ContentLength = Postbyte.Length;
                Stream newStream = request.GetRequestStream();
                newStream.Write(Postbyte, 0, Postbyte.Length);//把参数用流对象写入request对象中   
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                s = sr.ReadToEnd().Trim();

                response.Close();
                sr.Close();
            }
            catch (Exception)
            {
                throw;
            }

            ArrayList result = new ArrayList();
            string[] ss = s.Split('\n');
            if (ss.Length < 2)
                return result;

            for (int i = 1; i < ss.Length; i++)
            {
                string[] sss = ss[i].Split('=');
                if (sss.Length < 2)
                    continue;
                result.Add(sss[1]);
            }

            return result;
        }*/
    }
}
