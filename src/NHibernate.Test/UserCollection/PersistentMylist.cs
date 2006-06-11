using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Test.UserCollection
{
    public class PersistentMylist : PersistentList
    {
        public PersistentMylist(ISessionImplementor session, IList list) : base(session, list)
        {
        }

        public PersistentMylist(ISessionImplementor session) : base(session)
        {
        }
    }
}
