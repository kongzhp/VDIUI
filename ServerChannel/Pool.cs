/*********************************************************
 ** Filename: Pool.cs
 ** Description: 表示一个虚拟机池
 ** Version: 1.0
 ** Author: Michael Yan
 ** Date: 2012-12-10
 ** Copyright (C) 2012 云晫科技
 **********************************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace ServerChannel
{
    public class Pool
    {
        private string id;
        private string name;
        private bool ready;
        private ArrayList gateways; // 网关信息，如果为null则没有配置网关

        public String Name { 
            get
            {
                return name; 
            }
            set
            {
                name = value;
            }
        }
        public String Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        public Boolean Ready
        {
            get
            {
                return ready;
            }
            set
            {
                ready = value;
            }
        }

        public Pool()
        {
            id = name = "";
            ready = false;
            gateways = null;
        }

        public Pool(string id, string name, bool ready, ArrayList gateways)
        {
            this.id = id;
            this.name = name;
            this.ready = ready;
            this.gateways = gateways;
        }

        public string getId()
        {
            return id;
        }

        public string getName()
        {
            return name;
        }

        public bool getStatus()
        {
            return ready;
        }

        public ArrayList getGateways()
        {
            return gateways;
        }

        public void setId(string id)
        {
            this.id = id;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public void setStatus(bool ready)
        {
            this.ready = ready;
        }

        public void setGateways(ArrayList gateways)
        {
            this.gateways = gateways;
        }
    }
}
