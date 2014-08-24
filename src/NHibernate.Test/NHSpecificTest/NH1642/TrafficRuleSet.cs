using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1642
{
    public class TrafficRuleSet
    {
        public virtual int id { get; set; }
        public virtual string name { get; set; }
        public virtual string description { get; set; }
        public virtual IList<TrafficRule> rules { get; set; }
    }
}