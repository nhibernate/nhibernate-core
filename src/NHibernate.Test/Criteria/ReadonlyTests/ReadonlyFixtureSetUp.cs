using NUnit.Framework;

namespace NHibernate.Test.Criteria.ReadonlyTests
{
	/// <summary>
	/// Single one-time fixture set up for all test fixtures in NHibernate.Test.Criteria.ReadonlyTests namespace  
	/// </summary>
	[SetUpFixture]
	public class ReadonlyFixtureSetUp : NHibernate.Test.Linq.LinqReadonlyTestsContext
	{
	}
}
