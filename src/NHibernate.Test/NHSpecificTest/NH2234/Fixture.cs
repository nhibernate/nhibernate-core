using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2234
{
  public class SomethingLinq
	{
		public virtual string Name { get; set; }
		public virtual MyUsertype Relation { get; set; }
	}

	[TestFixture]
	public class Fixture: BugTestCase
	{
	  [Test]
	  public void CanQueryViaLinq()
	  {
	    using (var s = OpenSession())
	    {
        var qry = from item in s.Query<SomethingLinq>() where item.Relation == MyUserTypes.Value1 select item;

	    	qry.ToList();
	    	qry.Executing(q => q.ToList()).NotThrows();
	    }
	  }
	}
}
