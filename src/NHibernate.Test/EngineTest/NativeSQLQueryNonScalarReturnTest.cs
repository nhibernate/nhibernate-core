using NHibernate.Engine.Query.Sql;
using NUnit.Framework;

namespace NHibernate.Test.EngineTest
{
	[TestFixture]
	public class NativeSQLQueryNonScalarReturnTest
	{
		[Test]
		public void NativeSQLQueryCollectionReturnEqualsByAlias()
		{
			var sr1 = new NativeSQLQueryCollectionReturn("myAlias", "owner", "ownerProp", null,LockMode.None);
			var sr2 = new NativeSQLQueryCollectionReturn("myAlias", "owner", "ownerProp", null, LockMode.None);
			Assert.AreEqual(sr1, sr2);
		}

		[Test]
		public void NativeSQLQueryCollectionReturnHashCodeByAlias()
		{
			var sr1 = new NativeSQLQueryCollectionReturn("myAlias", "owner", "ownerProp", null,LockMode.None);
			var sr2 = new NativeSQLQueryCollectionReturn("myAlias", "owner", "ownerProp", null, LockMode.None);
			Assert.AreEqual(sr1.GetHashCode(), sr2.GetHashCode());
		}

		[Test]
		public void NativeSQLQueryJoinReturnEqualsByAlias()
		{
			var sr1 = new NativeSQLQueryJoinReturn("myAlias", "owner", "ownerProp", null, LockMode.None);
			var sr2 = new NativeSQLQueryJoinReturn("myAlias", "owner", "ownerProp", null, LockMode.None);
			Assert.AreEqual(sr1, sr2);
		}

		[Test]
		public void NativeSQLQueryJoinReturnHashCodeByAlias()
		{
			var sr1 = new NativeSQLQueryJoinReturn("myAlias", "owner", "ownerProp", null, LockMode.None);
			var sr2 = new NativeSQLQueryJoinReturn("myAlias", "owner", "ownerProp", null, LockMode.None);
			Assert.AreEqual(sr1.GetHashCode(), sr2.GetHashCode());
		}

		[Test]
		public void NativeSQLQueryRootReturnEqualsByAlias()
		{
			var sr1 = new NativeSQLQueryRootReturn("myAlias", "entity", null, LockMode.None);
			var sr2 = new NativeSQLQueryRootReturn("myAlias", "entity", null, LockMode.None);
			Assert.AreEqual(sr1, sr2);
		}

		[Test]
		public void NativeSQLQueryRootReturnHashCodeByAlias()
		{
			var sr1 = new NativeSQLQueryRootReturn("myAlias", "entity", null, LockMode.None);
			var sr2 = new NativeSQLQueryRootReturn("myAlias", "entity", null, LockMode.None);
			Assert.AreEqual(sr1.GetHashCode(), sr2.GetHashCode());
		}
	}
}
