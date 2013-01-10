using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerChannel
{
    public class Gateway
    {
        private string username;
        private string password;
        private string address;
        private string type;

        public Gateway(string u, string p, string a, string t)
        {
            username = u;
            password = p;
            address = a;
            type = t;
        }

        public void setUsername(string u)
        {
            username = u;
        }

        public string getUsername()
        {
            return username;
        }

        public void setPassword(string p)
        {
            password = p;
        }

        public string getPassword()
        {
            return password;
        }

        public void setAddress(string a)
        {
            address = a;
        }

        public string getAddress()
        {
            return address;
        }

        public void setType(string t)
        {
            type = t;
        }

        public string getType()
        {
            return type;
        }
    }
}
