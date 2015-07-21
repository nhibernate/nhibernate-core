using System.Collections.Generic;
using NUnit.Framework;
using System;

namespace NHibernate.Test.NHSpecificTest.NH2985
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from WebImage");
				s.Delete("from ClassA");
				tx.Commit();
			}
		}

		[Test]
		public void Test()
		{
			Guid a_id;
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				// Create an A and save it
				ClassA a = new ClassA();
				a.Name = "a1";
				s.Save(a);
				a_id = a.Id;
				a.Childs = new List<WebImage>();

				a.Childs.Add(new WebImage() { ImageUrl = "http://blabla/bla1.jpg", ImageData = new byte[] { 11 } });
				a.Childs.Add(new WebImage() { ImageUrl = "http://blabla/bla2.jpg", ImageData = new byte[] { 13 } });

				tx.Commit();
			}

			// Clear the cache
			sessions.Evict(typeof (ClassA));
			sessions.Evict(typeof (WebImage));

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				// Load a so we can use it to load b
				ClassA a = s.Get<ClassA>(a_id);

				Assert.That(a.Childs, Has.Count.EqualTo(2));
				var firstElement = a.Childs[0];

				//I'm expect an object to be equal to itself
				Assert.That(firstElement.Equals(firstElement));

				//expect a list to contain the first element
				Assert.That(a.Childs.Contains(a.Childs[0]));

				tx.Commit();
			}
		}
	}
}
