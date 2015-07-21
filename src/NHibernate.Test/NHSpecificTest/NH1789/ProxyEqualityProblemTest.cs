using System.Collections;
using System.Collections.Generic;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1789
{
	[TestFixture]
	public class ProxyEqualityProblemTest : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			{
				ICat cat1 = new Cat("Marcel", 1);
				ICat cat2 = new Cat("Maurice", 2);

				session.Save(cat1);
				session.Save(cat2);
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				string hql = "from System.Object";
				session.Delete(hql);
				session.Flush();
			}
		}

		/// <summary>
		/// This test fails: when comparing a proxy with a non-proxy, I want the proxy to use the Equals() method on DomainObject to check for equality.
		/// It doesn't do it, so the equality fails.
		/// </summary>
		[Test]
		public void TestProxyEqualityProblem()
		{
			using (ISession session = OpenSession())
			{
				//We load a proxy version of Maurice
				var mauriceProxy = session.Load<ICat>((long) 2);

				Assert.IsTrue(mauriceProxy is INHibernateProxy, "The proxy should be of type INHibernateProxy");

				//From it's proxy, we get a non-proxied (concrete?) version
				var mauriceNonProxy = DomainObject.UnProxy<ICat>(mauriceProxy);

				Assert.IsTrue(!(mauriceNonProxy is INHibernateProxy), "The non-proxy shouldn't be of type INHibernateProxy");

				//We check if the name and ID matches (as they should be because they are the same entity!)
				Assert.AreEqual(mauriceProxy.Name, mauriceNonProxy.Name, "The two objects should have the same name");
				Assert.AreEqual(mauriceProxy.ID, mauriceNonProxy.ID, "The two objects should have the same ID");

				//Now here's the problem:
				//When calling Equals() on the non-proxy, everything works (calling the overriden Equals() method on DomainObject as it should be)
				Assert.IsTrue(mauriceNonProxy.Equals(mauriceProxy), "The two instances should be declared equal");

				//But when calling it on the proxy, it doesn't, and they return a false for the equality, and that's a bug IMHO
				bool condition = mauriceProxy.Equals(mauriceNonProxy);
				Assert.IsTrue(condition, "The two instances should be declared equal");
			}
		}

		/// <summary>
		/// Here, instead of querying an ICat, we query a Cat directly: everything works, and DomainObject.Equals() is properly called on the proxy.
		/// </summary>
		[Test]
		public void TestProxyEqualityWhereItDoesWork()
		{
			using (ISession session = OpenSession())
			{
				//We load a proxy version of Maurice
				var mauriceProxy = session.Load<Cat>((long) 2);

				Assert.IsTrue(mauriceProxy is INHibernateProxy, "The proxy should be of type INHibernateProxy");

				//From it's proxy, we get a non-proxied (concrete?) version
				var mauriceNonProxy = DomainObject.UnProxy<Cat>(mauriceProxy);

				Assert.IsTrue(!(mauriceNonProxy is INHibernateProxy), "The non-proxy shouldn't be of type INHibernateProxy");

				//We check if the name and ID matches (as they should be because they are the same entity!)
				Assert.AreEqual(mauriceProxy.Name, mauriceNonProxy.Name, "The two objects should have the same name");
				Assert.AreEqual(mauriceProxy.ID, mauriceNonProxy.ID, "The two objects should have the same ID");

				//Because we queried a concrete class (Cat instead of ICat), here it works both ways:
				Assert.IsTrue(mauriceNonProxy.Equals(mauriceProxy), "The two instances should be declared equal");
				Assert.IsTrue(mauriceProxy.Equals(mauriceNonProxy), "The two instances should be declared equal");
			}
		}

		/// <summary>
		/// That's how I discovered something was wrong: here my object is not found in the collection, even if it's there.
		/// </summary>
		[Test]//, Ignore("To investigate. When run with the whole tests suit it fail...probably something related with the ProxyCache.")]
		public void TestTheProblemWithCollection()
		{
			using (ISession session = OpenSession())
			{
				//As before, we load a proxy, a non-proxy of the same entity, and checks everything is correct:
				var mauriceProxy = session.Load<ICat>((long) 2);
				Assert.IsTrue(mauriceProxy is INHibernateProxy, "The proxy should be of type INHibernateProxy");
				var mauriceNonProxy = DomainObject.UnProxy<ICat>(mauriceProxy);
				Assert.IsTrue(!(mauriceNonProxy is INHibernateProxy), "The non-proxy shouldn't be of type INHibernateProxy");
				Assert.AreEqual(mauriceProxy.Name, mauriceNonProxy.Name, "The two objects should have the same name");
				Assert.AreEqual(mauriceProxy.ID, mauriceNonProxy.ID, "The two objects should have the same ID");

				//Ok now we add the proxy version into a collection:
				var collection = new List<ICat> {mauriceProxy};

				//The proxy should be able to find itself:
				Assert.IsTrue(collection.Contains(mauriceProxy), "The proxy should be present in the collection");

				//Now, the non-proxy should also be able to find itself in the collection, using the Equals() on DomainObject...
				Assert.IsTrue(collection.Contains(mauriceNonProxy), "The proxy should be present in the collection");
			}
		}
	}
}