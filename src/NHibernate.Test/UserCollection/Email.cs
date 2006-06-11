using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.UserCollection
{
    public class Email
    {
        int id;
        string address;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public Email(string address)
        {
            this.address = address;
        }

        public Email()
        {

        }

        public override bool Equals(object obj)
        {
            if(! (obj is Email) )
                return false;
            return ((Email) obj).address.Equals(address);
        }

        public override int GetHashCode()
        {
            return address.GetHashCode();
        }
    }
}
