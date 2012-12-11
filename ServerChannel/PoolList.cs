/*********************************************************
 ** Filename: PoolList.cs
 ** Description: 虚拟机池列表，常驻内存，包含所有该登陆用户有权限的池
 ** Version: 1.0
 ** Author: Michael Yan
 ** Date: 2012-12-10
 ** Copyright (C) 2012 云晫科技
 **********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerChannel
{
    public class PoolList
    {
        private ArrayList pools = null;

        public PoolList()
        {
            pools = new ArrayList();
        }

        public PoolList(ArrayList pools)
        {
            this.pools = pools;
        }

        /*
         * 根据pool id查找在poolList中的pool，如果没有返回null
         */
        public Pool findPoolById(string id)
        {
            Pool pool = null;
            for (int i = 0; i < pools.Count; i++)
            {
                Pool p = (Pool)pools[i];
                if (p.getId().Equals(id))
                    pool = p;
            }
            return pool;
        }

        /*
         * Usage: 更新内存中的虚拟机池列表，如果有变化的话
         * Param: newPools(ArrayList): 新的列表
         * Return: int: 0为无更新，1为有更新（pool数量增多或减少，或者其中某个、某些pool的状态有变化）
         */
        public int updatePools(ArrayList newPools)
        {
            if (pools.Count != newPools.Count)
            {
                pools = newPools;
                return 1;
            }
            for (int i = 0; i < newPools.Count; i++)
            {
                Pool pNew = (Pool)newPools[i];
                Pool p = findPoolById(pNew.getId());
                if (null == p || p.getStatus() != pNew.getStatus())
                {
                    pools = newPools;
                    return 1;
                }
            }
            return 0;
        }

        public ArrayList getPools()
        {
            return pools;
        }

        public void setPools(ArrayList pools)
        {
            this.pools = pools;
        }
    }
}
