using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.GenericTest.OrderedSetGeneric
{
	[TestFixture]
	public class OrderedSetFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] {"GenericTest.OrderedSetGeneric.OrderedSetFixture.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Delete("from B");
					s.Delete("from A");
					tx.Commit();
				}
			}
		}

		[Test]
		public void OrderedSetIsInOrder()
		{
			var names = new[] {"First B", "Second B"};
			const int TheId = 100;

			var a = new A {Name = "First", Id = TheId};

			var b = new B {Name = names[1], OrderBy = 3, AId = TheId};
			a.Items.Add(b);

			var b2 = new B {Name = names[0], OrderBy = 1, AId = TheId};
			a.Items.Add(b2);

			ISession s = OpenSession();
			s.Save(a);
			s.Flush();
			s.Close();

			s = OpenSession();
			var newA = s.Get<A>(a.Id);

			Assert.AreEqual(2, newA.Items.Count);
			int counter = 0;
			foreach (B item in newA.Items)
			{
				Assert.AreEqual(names[counter], item.Name);
				counter++;
			}
			s.Close();
		}
	}
}