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
using log4net;

namespace ServerChannel
{
    /*
     * ClassName: ServerCommunicator
     * Description: 发送请求、接收结果并包装返回。有5个请求函数，分别获取5种信息：servers, domains, pools, desktop, status
     */
    public class ServerCommunicator
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ServerCommunicator));

        private HttpWebRequest generateRequest(string url, string method = "GET", bool allowAutoRedirect = false)
        {
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            catch (Exception e)
            {
                logger.Error("创建Http请求时（url为" + url + "）产生错误，错误信息：" + e.Message);
                throw;
            }
            request.Method = method;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.AllowAutoRedirect = allowAutoRedirect;
            return request;
        }

        /*public ArrayList getServersOfCluster(string ip)
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
        }*/

        /*
         * Usage: 返回给定ip的server vdi虚拟机中所有的domain
         * Param: ip(string): server ip
         * Retern: ArrayList: 元素为Domain类对象
         * Throw: 可能有以下几种类型的异常产生，此处不作处理直接抛出
         *   IOException, OutOfMemoryException: IO异常，产生自ReadToEnd
         *   WebExcption: 处理请求超时或发生错误，产生自GetResponse
         *   OtherException: 其他一些异常，产生自create request, StreamReader等
         */
        public ArrayList getDomains(string ip)
        {
            if (null == ip || ip.Equals(""))
            {
                logger.Error("getDomains--ip为空");
                throw new Exception("ip不能为空");
            }

            string url = "http://" + ip + "/dp/rpc/dc/getdomains";
            string s = "";
            ArrayList result = new ArrayList();

            try
            {
                HttpWebRequest request = generateRequest(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                s = sr.ReadToEnd().Trim();

                response.Close();
                sr.Close();          
               
                string[] ss = s.Split('\n');
                if (ss.Length < 3)
                {
                    logger.Error("getDomains--域信息(" + s + ")格式有误");
                    throw new Exception("返回的域信息格式有误");
                }

                string[] sss = ss[0].Split('=');
                int num = Int32.Parse(sss[1]); // domain.num = num
                if (num <= 0)
                {
                    logger.Error("getDomains--域个数(" + sss[1] + ")应大于0");
                    throw new Exception("域个数为0");
                }
                int i = 1;
                while (i <= 2*num)
                {
                    string[] sss1 = ss[i++].Split('=');   // domain name
                    string[] sss2 = ss[i++].Split('=');   // domain id
                    Domain domain = new Domain(sss2[1], sss1[1]);
                    result.Add(domain);
                }
            }
            catch (Exception e)
            {
                logger.Error("从" + ip + "获取域信息（" + s + "）时产生错误，错误信息为：" + e.Message);
                throw;
            }

            return result;
        }

        /*
         * Usage: 返回用户所在的pool列表以及用户id，需要验证信息
         * Param: ip(string): 服务器ip
         *        username(string): 用户名
         *        password(string): 密码
         *        domain(string): domain id
         * Retern: GetPoolResult 
         *         如果为null，则表示返回信息为空或格式不对
         * Throw: 可能有以下几种类型的异常产生，此处不作处理直接抛出
         *   IOException, OutOfMemoryException: IO异常，产生自ReadToEnd
         *   WebExcption: 处理请求超时或发生错误，产生自GetResponse
         *   OtherException: 其他一些异常，产生自create request, StreamReader
         */
        public GetPoolResult getPoosWithAuth(string ip, string username, string password, string domain)
        {
            if (null == ip || ip.Equals(""))
            {
                logger.Error("getPools--ip为空");
                throw new Exception("ip不能为空");
            }

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
            catch (Exception e)
            {
                logger.Error("为用户" + username + "从" + ip + "处获取桌面池信息时产生错误，错误信息为：" + e.Message);
                throw;
            }       
            
            string[] ss = s.Split('\n');
            if (ss[0].Equals("error=No desktop is assigned to the user."))
            {
                logger.Error("getPools--没有任何桌面池被分配给用户" + username);
                return new GetPoolResult();
            }
            else if (ss[0].Equals("error=An incorrect user name or password was entered."))
            {
                logger.Error("getPools--用户名或密码有误");
                return null;
            }

            string userId = "";
            ArrayList pools = new ArrayList();
            try
            {
                userId = ss[0].Split('=')[1];
                string[] temp = ss[3].Split('=');
                int numPool = Int32.Parse(temp[1]);

                int index = 4;
                for (int i = 0; i < numPool; i++)
                {
                    string[] sss1 = ss[index++].Split('=');   // pool name
                    string[] sss2 = ss[index++].Split('=');   // pool id
                    string[] sss3 = ss[index++].Split('=');   // pool status
                    string pool_name = sss1[1];
                    string pool_id = sss2[1];
                    bool pool_ready = sss3[1].Equals("1") ? true : false;

                    ArrayList gateways = null;
                    if (index != ss.Length)
                    {
                        string[] a = ss[index].Split('=');
                        string[] aa = a[0].Split('.');
                        if (a.Length == 2 && aa.Length == 4 && aa[2].Equals("gw"))
                        {
                            gateways = new ArrayList();
                            int numGw = Int32.Parse(a[1]);
                            for (int j = 0; j < numGw; j++)
                            {
                                string gwType = ss[++index].Split('=')[1];
                                string gwAddr = ss[++index].Split('=')[1];
                                string gwUser = ss[++index].Split('=')[1];
                                string gwPass = ss[++index].Split('=')[1];
                                Gateway gw = new Gateway(gwUser, gwPass, gwAddr, gwType);
                                gateways.Add(gw);
                            }
                            index++;
                        }
                    }

                    Pool pool = new Pool(pool_id, pool_name, pool_ready, gateways);
                    pools.Add(pool);
                }
            }
            catch (Exception ee)
            {
                logger.Error("为用户" + username + "从" + ip + "处获取桌面池信息时产生错误，错误信息为：" + ee.Message);
                throw;
            }

            PoolList poolList = new PoolList(pools);
            GetPoolResult result = new GetPoolResult(userId, poolList);
            return result;
        }

        /*
         * Usage: 返回用户所在的pool列表以及用户id，需要验证信息
         * Param: ip(string): 服务器ip
         *        user_id(string): 用户id
         *        domain_name(string): domain name
         *        pool_id(string): pool id
         * Retern: string: request id 
         * Throw: 可能有以下几种类型的异常产生，此处不作处理直接抛出
         *   IOException, OutOfMemoryException: IO异常，产生自ReadToEnd
         *   WebExcption: 处理请求超时或发生错误，产生自GetResponse
         *   OtherException: 其他一些异常，产生自create request, StreamReader
         */
        public string requestDesktop(string ip, string user_id, string domain_name, string pool_id)
        {
            if (null == ip || ip.Equals(""))
            {
                logger.Error("requestDesktop--ip为空");
                throw new Exception("ip不能为空");
            }

            string url = "http://" + ip + "/dp/rpc/dc/request?user=" + user_id + "&domain=" + domain_name + "&pool=" + pool_id;
            string s = "";
            string result = "";

            try
            {
                HttpWebRequest request = generateRequest(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                s = sr.ReadToEnd().Trim();

                response.Close();
                sr.Close();

                string[] ss = s.Split('\n');
                string[] sss = ss[0].Split('=');
                result = sss[1];
            }
            catch (Exception e)
            {
                logger.Error("请求桌面（" + s + "）时发生错误，错误信息为：" + e.Message);
                throw;
            }
            return result;
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
            {
                logger.Error("getDesktopStatus--ip为空");
                throw new Exception("ip不能为空");
            }

            string url = "http://" + ip + "/dp/rpc/dc/connect?request=" + request_id;
            string s = "";
            string result = "";

            try
            {
                HttpWebRequest request = generateRequest(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                s = sr.ReadToEnd().Trim();

                response.Close();
                sr.Close();           

                if (s.Equals(""))
                    return "not ready";
                else if (s.Contains("error"))
                {
                    logger.Error("错误：池中当前没有可用的桌面分配给用户");
                    return null;
                }

                string[] ss = s.Split('\n');
                string[] sss1 = ss[0].Split('=');
                string[] sss2 = ss[1].Split('=');
                result = sss1[1] + ":" + sss2[1];
             }
            catch (Exception e)
            {
                logger.Error("请求桌面状态（" + s + "）时产生错误，错误信息为：" + e.Message);
                throw;
            }

            return result;
        }
    }
}
