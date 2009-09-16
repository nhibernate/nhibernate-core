using System;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1922
{
    public class Customer 
    {
        public virtual int ID { get; private set; }
        public virtual DateTime ValidUntil { get; set; }
    }
}
