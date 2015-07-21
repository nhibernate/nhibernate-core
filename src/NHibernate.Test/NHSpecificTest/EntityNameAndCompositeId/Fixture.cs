using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.EntityNameAndCompositeId
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.CreateSQLQuery("delete from Person").ExecuteUpdate();
					tx.Commit();
				}
			}
		}

		[Test]
		public void CanPersistAndRead()
		{
			object id;
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					id = s.Save("Person", new Dictionary<string, object>
					                      	{
					                      		{"OuterId", new Dictionary<string, int> {{"InnerId", 1}}},
					                      		{"Data", "hello"}
					                      	});
					tx.Commit();
				}
			}
			using (ISession s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					var p = (IDictionary) s.Get("Person", id);
					Assert.AreEqual("hello", p["Data"]);
				}
			}
		}
	}
}