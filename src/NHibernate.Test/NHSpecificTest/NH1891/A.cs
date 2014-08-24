using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1891
{
    public class A
    {
        public virtual Guid Id { get; set; }
        public virtual int FormulaCount { get; set; }
        public virtual string FormulaConstraint { get; set; }
    }
}
