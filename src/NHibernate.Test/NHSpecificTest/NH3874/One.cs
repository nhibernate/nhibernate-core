using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3874
{
    public class One
    {
        public virtual IntWrapper Id { get; set; }

        public virtual IList<Two> Twos { get; set; }
    }
}
