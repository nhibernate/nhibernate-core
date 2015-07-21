
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.EntityNameAndInheritance
{
	public class Fixture : BugTestCase
	{
		private int id;
		private const string entityName = "SuperClass";

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					id = (int)s.Save(entityName, new Hashtable());
					tx.Commit();
				}
			}
		}

		[Test]
		public void DoesNotCrash()
		{
			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					Assert.IsNotNull(s.Get(entityName, id));
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.CreateSQLQuery("delete from " + entityName).ExecuteUpdate();
					tx.Commit();
				}
			}
		}
	}
}
