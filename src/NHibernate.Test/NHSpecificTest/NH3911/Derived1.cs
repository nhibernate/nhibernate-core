using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3911
{
    public class Derived1 : Base
    {
        public virtual new Derived2 Ref
        {
            get { return (Derived2)base.Ref.Unwrap(); }
            set { base.Ref = value; }
        }
    }
}
