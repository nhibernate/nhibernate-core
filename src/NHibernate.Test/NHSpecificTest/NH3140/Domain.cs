using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3140
{
    public class Foo
    {
        public virtual ICollection<Bar> Bars { get; set; }
    }

    public class Bar { }
}