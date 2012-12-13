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
using System.Linq;
using System.Text;

namespace ServerChannel
{
    public class Pool
    {
        private string id;
        private string name;
        private bool ready;

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
        }

        public Pool(string id, string name, bool ready)
        {
            this.id = id;
            this.name = name;
            this.ready = ready;
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
    }
}
