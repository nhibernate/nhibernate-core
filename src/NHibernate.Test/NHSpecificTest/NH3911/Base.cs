using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3911
{
    public class Base
    {
        public virtual int Id { get; set; }

        protected Base _ref;

        public virtual Base Ref
        {
            get { return _ref.Unwrap(); }
            set { _ref = value; }
        }

        public virtual Base Unwrap() { return this; }
    }
}
