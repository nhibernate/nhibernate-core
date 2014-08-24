using NHibernate.Engine.Query.Sql;
using NUnit.Framework;

namespace NHibernate.Test.EngineTest
{
	[TestFixture]
	public class NativeSQLQueryScalarReturnTest
	{
		[Test]
		public void EqualsByAlias()
		{
			var sr1 = new NativeSQLQueryScalarReturn("myAlias", NHibernateUtil.Int32);
			var sr2 = new NativeSQLQueryScalarReturn("myAlias", NHibernateUtil.Int32);
			Assert.AreEqual(sr1, sr2);
		}

		[Test]
		public void HashCodeByAlias()
		{
			var sr1 = new NativeSQLQueryScalarReturn("myAlias", NHibernateUtil.Int32);
			var sr2 = new NativeSQLQueryScalarReturn("myAlias", NHibernateUtil.Int32);
			Assert.AreEqual(sr1.GetHashCode(), sr2.GetHashCode());
		}
	}
}
