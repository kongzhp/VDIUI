using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerChannel
{
    public class Domain
    {
        private string id = null;
        private string name = null;
        public String Id { 
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
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
        public Domain()
        {
            id = name = "";
        }

        public Domain(string id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public string getDomainId()
        {
            return id;
        }

        public string getDomainName()
        {
            return name;
        }

        public void setDomainId(string id)
        {
            this.id = id;
        }

        public void setDomainName(string name)
        {
            this.name = name;
        }
    }
}
