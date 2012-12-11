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
        public string id;
        public string name;
        public bool ready;

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
    }
}
