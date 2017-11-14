using System.Collections.Generic;
using System.Linq;
using System.Collections;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2279
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using( ISession s = Sfi.OpenSession() )
			{
				s.Delete( "from A" );
				s.Flush();
			}
		}

		[Test]
		public void IdBagIndexTracking()
		{
			A a = new A("a");
			a.Name = "first generic type";
			a.Items.Add("a");
			a.Items.Add("b");
			a.Items.Add("c");

			ISession s = OpenSession();
			s.SaveOrUpdate(a);
			s.Flush();
			s.Close();

			s = OpenSession();
			a = (A)s.Load(typeof(A), a.Id);
			CollectionAssert.AreEquivalent(new[] {"a", "b", "c"}, a.Items);

			// Add and remove a "transient" item.
			a.Items.Add("d");
			a.Items.Remove("d");

			// Remove persisted items and then insert a transient item.
			a.Items.Remove("b");
			a.Items.Remove("a");
			a.Items.Insert(0, "e");

			// Add a couple transient items and insert another transient item between them.
			a.Items.Add("f");
			a.Items.Add("g");
			a.Items.Insert(3, "h");

			// Save and then see if we get what we expect.
			s.SaveOrUpdate(a);
			s.Flush();
			s.Close();

			s = OpenSession();
			a = (A)s.Load(typeof(A), a.Id);
			CollectionAssert.AreEquivalent(new [] {"c", "e", "f", "g", "h"}, a.Items);

			// Test changing a value by index directly.
			a.Items[2] = "i";

			string[] expected = a.Items.Cast<string>().ToArray();

			s.SaveOrUpdate(a);
			s.Flush();
			s.Close();

			s = OpenSession();
			a = (A)s.Load(typeof(A), a.Id);
			CollectionAssert.AreEquivalent(expected, a.Items);

			s.Flush();
			s.Close();
		}

		[Test]
		public void CartesianProduct()
		{
			A a1 = new A("a1");
			A a2 = new A("a2");
			B b1 = new B("b1");
			B b2 = new B("b2");
			B b3 = new B("b3");
			B b4 = new B("b4");
			C c1 = new C("c1");
			C c2 = new C("c2");
			C c3 = new C("c3");
			C c4 = new C("c4");
			C c5 = new C("c5");
			C c6 = new C("c6");

			a1.Bs.Add(b1);
			a2.Bs.Add(b2);
			a2.Bs.Add(b2);

			a1.Bs.Add(b3);
			a2.Bs.Add(b3);

			a1.Bs.Add(b4);
			a2.Bs.Add(b4);
			a1.Bs.Add(b4);
			a2.Bs.Add(b4);

			b1.Cs.Add(c1);
			b2.Cs.Add(c2);
			b2.Cs.Add(c2);
			b3.Cs.Add(c3);
			b3.Cs.Add(c3);
			b3.Cs.Add(c3);
			b4.Cs.Add(c4);
			b4.Cs.Add(c4);
			b4.Cs.Add(c4);
			b4.Cs.Add(c4);

			b1.Cs.Add(c5);
			b2.Cs.Add(c5);
			b3.Cs.Add(c5);
			b4.Cs.Add(c5);

			b1.Cs.Add(c6);
			b2.Cs.Add(c6);
			b3.Cs.Add(c6);
			b4.Cs.Add(c6);
			b1.Cs.Add(c6);
			b2.Cs.Add(c6);
			b3.Cs.Add(c6);
			b4.Cs.Add(c6);

			ISession s = OpenSession();
			s.Save(a1);
			s.Save(a2);
			s.Flush();
			s.Close();

			s = OpenSession();
			IList<A> results = s.CreateQuery("from A a join fetch a.Bs b join fetch b.Cs").List<A>().Distinct().ToList();

			Assert.That(results, Has.Count.EqualTo(2));
			A ta1 = results.Single(a => a.Name == "a1");
			A ta2 = results.Single(a => a.Name == "a2");

			Assert.That(ta1.Bs.Select(b => b.Name).ToArray(), Is.EquivalentTo(new[] { "b1", "b3", "b4", "b4" }));
			Assert.That(ta2.Bs.Select(b => b.Name).ToArray(), Is.EquivalentTo(new[] { "b2", "b2", "b3", "b4", "b4" }));
			B tb1 = ta1.Bs.First(b => b.Name == "b1");
			B tb2 = ta2.Bs.First(b => b.Name == "b2");
			B tb3 = ta1.Bs.First(b => b.Name == "b3");
			B tb4 = ta1.Bs.First(b => b.Name == "b4");

			Assert.That(tb1.Cs.Select(c => c.Name).ToArray(), Is.EquivalentTo(new[] { "c1", "c5", "c6", "c6" }));
			Assert.That(tb2.Cs.Select(c => c.Name).ToArray(), Is.EquivalentTo(new[] { "c2", "c2", "c5", "c6", "c6" }));
			Assert.That(tb3.Cs.Select(c => c.Name).ToArray(), Is.EquivalentTo(new[] { "c3", "c3", "c3", "c5", "c6", "c6" }));
			Assert.That(tb4.Cs.Select(c => c.Name).ToArray(), Is.EquivalentTo(new[] { "c4", "c4", "c4", "c4", "c5", "c6", "c6" }));
			s.Close();
		}
	}
}