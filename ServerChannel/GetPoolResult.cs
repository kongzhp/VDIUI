using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerChannel
{
    public class GetPoolResult
    {
        private string userId = null;
        private PoolList poolList = null;

        public GetPoolResult()
        {
            userId = "";
            poolList = new PoolList();
        }

        public GetPoolResult(string userId, PoolList poolList)
        {
            this.userId = userId;
            this.poolList = poolList;
        }

        public string getUserId()
        {
            return userId;
        }

        public PoolList getPoolList()
        {
            return poolList;
        }

        public void setUserId(string userId)
        {
            this.userId = userId;
        }

        public void setPoolList(PoolList poolList)
        {
            this.poolList = poolList;
        }

    }
}
