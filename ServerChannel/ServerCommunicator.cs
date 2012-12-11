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
    public class ServerCommunicator
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
         * Usage: 返回给定ip的server所在的cluster中的其他server ip
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
         * Usage: 返回给定ip的server vdi虚拟机中所有的domain
         * Param: ip(string): server ip
         * Retern: ArrayList: 元素为domain信息，大小为2的string数组，string[0]为domain name，string[1]为domain id，
         *         如{{domain_name1, domain_id1}, {domain_name2, domain_id2}, ...}
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

            for (int i = 0; i < (ss.Length - 1) / 2; i++)
            {
                string[] sss1 = ss[1 + i * 2].Split('=');
                string[] sss2 = ss[2 + i * 2].Split('=');
                if (sss1.Length < 2 || sss2.Length < 2)
                    continue;
                string[] domain = new string[2];
                domain[0] = sss1[1];
                domain[1] = sss2[1];
                result.Add(domain);
            }         

            return result;
        }

        /*
         * Usage: 返回用户所在的pool列表以及用户id，需要验证信息
         * Param: ip(string): 服务器ip
         *        username(string): 用户名
         *        password(string): 密码
         *        domain(string): domain id
         * Retern: ArrayList: 用户id和PoolList，ArrayList[0]=string pool_id, ArrayList[1]=PoolList 
         *         如果为空数组，则表示返回信息为空或格式不对
         * Throw: 可能有以下几种类型的异常产生，此处不作处理直接抛出
         *   IOException, OutOfMemoryException: IO异常，产生自ReadToEnd
         *   WebExcption: 处理请求超时或发生错误，产生自GetResponse
         *   OtherException: 其他一些异常，产生自create request, StreamReader
         */
        public ArrayList getPoosWithAuth(string ip, string username, string password, string domain)
        {
            if (null == ip || ip.Equals(""))
                return new ArrayList();

            string url = "http://" + ip + "/dp/rpc/dc/login?user=" + username + "&ldap=" + domain + "&pass=" + password;
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
            if (ss.Length < 1)
                return result;
            else
            {
                string[] sss = ss[0].Split('=');
                if (sss.Length < 2)
                    return result;
                result.Add(sss[1]);
            }

            ArrayList pools = new ArrayList();
            for (int i = 0; i < (ss.Length-4)/3; i++)
            {
                string[] sss1 = ss[4 + i * 3].Split('=');
                string[] sss2 = ss[5 + i * 3].Split('=');
                string[] sss3 = ss[6 + i * 3].Split('=');
                if (sss1.Length < 2 || sss2.Length < 2 || sss3.Length < 2)
                    continue;
                string pool_name = sss1[1];
                string pool_id = sss2[1];
                bool pool_ready = sss3[1].Equals("1") ? true : false;
                Pool pool = new Pool(pool_id, pool_name, pool_ready);
                pools.Add(pool);
            }
            PoolList poolList = new PoolList(pools);
            result.Add(poolList);
            return result;
        }

        /*
         * Usage: 返回用户所在的pool列表以及用户id，需要验证信息
         * Param: ip(string): 服务器ip
         *        user_id(string): 用户id
         *        domain_name(string): domain name
         *        pool_id(string): pool id
         * Retern: string: request id 
         *         如果为null，则表示返回信息为空或格式不对
         * Throw: 可能有以下几种类型的异常产生，此处不作处理直接抛出
         *   IOException, OutOfMemoryException: IO异常，产生自ReadToEnd
         *   WebExcption: 处理请求超时或发生错误，产生自GetResponse
         *   OtherException: 其他一些异常，产生自create request, StreamReader
         */
        public string requestDesktop(string ip, string user_id, string domain_name, string pool_id)
        {
            if (null == ip || ip.Equals(""))
                return null;

            string url = "http://" + ip + "/dp/rpc/dc/request?user=" + user_id + "&domain=" + domain_name + "&pool=" + pool_id;
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

            string[] ss = s.Split('\n');
            if (ss.Length < 1)
                return null;

            string[] sss = ss[0].Split('=');
            if (sss.Length < 2)
                return null;
            return sss[1];
        }

        /*
         * Usage: 获取已经分配给用户的桌面状态：ready还是非ready
         * Param: ip(string): 服务器ip
         *        request_id: 前一步request desktop产生的request id
         * Retern: string: 桌面ip:port，如192.168.0.56:3389 
         *         如果为null，则表示没有空闲的桌面
         *         如果为"not ready"，则表示还没准备好桌面，但是已经在等待桌面启动
         *         如果为空字符串，则表示返回的桌面信息异常
         * Throw: 可能有以下几种类型的异常产生，此处不作处理直接抛出
         *   IOException, OutOfMemoryException: IO异常，产生自ReadToEnd
         *   WebExcption: 处理请求超时或发生错误，产生自GetResponse
         *   OtherException: 其他一些异常，产生自create request, StreamReader
         */
        public string getDesktopStatus(string ip, string request_id)
        {
            if (null == ip || ip.Equals(""))
                return null;

            string url = "http://" + ip + "/dp/rpc/dc/connect?request=" + request_id;
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

            string[] ss = s.Split('\n');
            if (ss.Length == 0)
                return "not ready";
            else if (ss.Length == 1)
                return null;

            string[] sss1 = ss[0].Split('=');
            string[] sss2 = ss[1].Split('=');
            if (sss1.Length < 2 || sss2.Length < 2)
                return "";
            return sss1[1] + ":" + sss2[1];
        }
    }
}
