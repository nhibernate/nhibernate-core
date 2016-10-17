using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3911
{
    public class Derived2 : Base
    {
        public virtual new Derived1 Ref
        {
            get { return (Derived1)base.Ref.Unwrap(); }
            set { base.Ref = value; }
        }
    }
}
