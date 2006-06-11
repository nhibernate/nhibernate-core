using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections;

namespace NHibernate.Test.UserCollection
{
    public class User
    {
        string userName;
        IList emailAddresses = new MyList();
        ISet sessionData = new HashedSet();

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public IList EmailAddresses
        {
            get { return emailAddresses; }
            set { emailAddresses = value; }
        }

        public ISet SessionData
        {
            get { return sessionData; }
            set { sessionData = value; }
        }

        public User()
        {
        }

        public User(string userName)
        {
            this.userName = userName;
        }
    }
}
