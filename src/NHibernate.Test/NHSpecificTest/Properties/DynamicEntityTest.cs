using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Properties
{
	[TestFixture]
	public class DynamicEntityTest : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					var props = new Dictionary<string, object>();
					props["Foo"] = "Sweden";
					props["Bar"] = "IsCold";
					s.Save("DynamicEntity", new Dictionary<string, object>
					                      	{
					                      		{"SomeProps", props},
					                      	});
					tx.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Delete("from DynamicEntity");
					tx.Commit();
				}
			}
		}

		[Test]
		public void CanFetchByProperty()
		{
			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					var l = s.CreateQuery("from DynamicEntity de where de.SomeProps.Foo=:fooParam")
						.SetString("fooParam", "Sweden").List();
					Assert.AreEqual(1, l.Count);
					var props = ((IDictionary)l[0])["SomeProps"];
					Assert.AreEqual("IsCold", ((IDictionary)props)["Bar"]);
				}
			}
		}
	}
}
