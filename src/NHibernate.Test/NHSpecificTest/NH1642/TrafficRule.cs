using System;

namespace NHibernate.Test.NHSpecificTest.NH1642
{
    public class TrafficRule : AbstractRule
    {
    	public TrafficRule()
    	{
				effectiveStartDate = DateTime.Today;
				effectiveEndDate = DateTime.Today;
    	}
    	public virtual DateTime effectiveStartDate { get; set; }
        public virtual DateTime effectiveEndDate { get; set; }

        public virtual TrafficRuleSet ruleSet { get; set; }
    }
}