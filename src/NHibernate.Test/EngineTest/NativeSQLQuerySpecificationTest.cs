using NHibernate.Engine.Query.Sql;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.EngineTest
{
	[TestFixture]
	public class NativeSQLQuerySpecificationTest
	{
		[Test]
		public void Equality()
		{
			var sr1 = new NativeSQLQuerySpecification("SELECT * FROM SOMETHING", new INativeSQLQueryReturn[] { new NativeSQLQueryScalarReturn("myAlias", NHibernateUtil.Int32), new NativeSQLQueryScalarReturn("otherAlias", NHibernateUtil.Int32) }, new List<string> { "SOMETHING" });
			var sr2 = new NativeSQLQuerySpecification("SELECT * FROM SOMETHING", new INativeSQLQueryReturn[] { new NativeSQLQueryScalarReturn("myAlias", NHibernateUtil.Int32), new NativeSQLQueryScalarReturn("otherAlias", NHibernateUtil.Int32) }, new List<string> { "SOMETHING" });
			Assert.AreEqual(sr1, sr2);
		}

		[Test]
		public void HashCode()
		{
			var sr1 = new NativeSQLQuerySpecification("SELECT * FROM SOMETHING", new INativeSQLQueryReturn[] { new NativeSQLQueryScalarReturn("myAlias", NHibernateUtil.Int32), new NativeSQLQueryScalarReturn("otherAlias", NHibernateUtil.Int32) }, new List<string> { "SOMETHING" });
			var sr2 = new NativeSQLQuerySpecification("SELECT * FROM SOMETHING", new INativeSQLQueryReturn[] { new NativeSQLQueryScalarReturn("myAlias", NHibernateUtil.Int32), new NativeSQLQueryScalarReturn("otherAlias", NHibernateUtil.Int32) }, new List<string> { "SOMETHING" });
			Assert.AreEqual(sr1.GetHashCode(), sr2.GetHashCode());
		}

		[Test]
		public void WhenChangeReturns_NotEqual()
		{
			var sr1 = new NativeSQLQuerySpecification("SELECT * FROM SOMETHING", new INativeSQLQueryReturn[] { new NativeSQLQueryScalarReturn("myAlias", NHibernateUtil.Int32), new NativeSQLQueryScalarReturn("otherAlias", NHibernateUtil.Int32) }, new List<string> { "SOMETHING" });
			var sr2 = new NativeSQLQuerySpecification("SELECT * FROM SOMETHING", new INativeSQLQueryReturn[] { new NativeSQLQueryScalarReturn("myAliasChanged", NHibernateUtil.Int32), new NativeSQLQueryScalarReturn("otherAlias", NHibernateUtil.Int32) }, new List<string> { "SOMETHING" });
			Assert.AreNotEqual(sr1, sr2);
		}

		[Test]
		public void WhenChangeReturns_NotEqualHashCode()
		{
			var sr1 = new NativeSQLQuerySpecification("SELECT * FROM SOMETHING", new INativeSQLQueryReturn[] { new NativeSQLQueryScalarReturn("myAlias", NHibernateUtil.Int32), new NativeSQLQueryScalarReturn("otherAlias", NHibernateUtil.Int32) }, new List<string> { "SOMETHING" });
			var sr2 = new NativeSQLQuerySpecification("SELECT * FROM SOMETHING", new INativeSQLQueryReturn[] { new NativeSQLQueryScalarReturn("myAliasChanged", NHibernateUtil.Int32), new NativeSQLQueryScalarReturn("otherAlias", NHibernateUtil.Int32) }, new List<string> { "SOMETHING" });
			Assert.AreNotEqual(sr1.GetHashCode(), sr2.GetHashCode());
		}

		[Test]
		public void WhenChangeSpace_NotEqual()
		{
			var sr1 = new NativeSQLQuerySpecification("SELECT * FROM SOMETHING", new INativeSQLQueryReturn[] { new NativeSQLQueryScalarReturn("myAlias", NHibernateUtil.Int32), new NativeSQLQueryScalarReturn("otherAlias", NHibernateUtil.Int32) }, new List<string> { "SOMETHING" });
			var sr2 = new NativeSQLQuerySpecification("SELECT * FROM SOMETHING", new INativeSQLQueryReturn[] { new NativeSQLQueryScalarReturn("myAlias", NHibernateUtil.Int32), new NativeSQLQueryScalarReturn("otherAlias", NHibernateUtil.Int32) }, new List<string> { "ANOTHER" });
			Assert.AreNotEqual(sr1, sr2);
		}

		[Test]
		public void WhenChangeSpace_NotEqualHashCode()
		{
			var sr1 = new NativeSQLQuerySpecification("SELECT * FROM SOMETHING", new INativeSQLQueryReturn[] { new NativeSQLQueryScalarReturn("myAlias", NHibernateUtil.Int32), new NativeSQLQueryScalarReturn("otherAlias", NHibernateUtil.Int32) }, new List<string> { "SOMETHING" });
			var sr2 = new NativeSQLQuerySpecification("SELECT * FROM SOMETHING", new INativeSQLQueryReturn[] { new NativeSQLQueryScalarReturn("myAlias", NHibernateUtil.Int32), new NativeSQLQueryScalarReturn("otherAlias", NHibernateUtil.Int32) }, new List<string> { "ANOTHER" });
			Assert.AreNotEqual(sr1.GetHashCode(), sr2.GetHashCode());
		}

	}
}
