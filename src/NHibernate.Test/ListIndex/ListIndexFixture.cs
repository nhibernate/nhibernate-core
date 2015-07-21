using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.ListIndex
{
	[TestFixture]
	public class ListIndexFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] { "ListIndex.ListIndex.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from B");
				s.Flush();
				s.Delete("from A");
				s.Flush();
			}
		}

		[Test]
		public void ListIndexBaseIsUsed()
		{
			const int TheId = 2000;

			A a = new A();
			a.Name = "First";
			a.Id = TheId;

			B b = new B();
			b.AId = TheId;
			b.Name = "First B";
			a.Items.Add(b);

			B b2 = new B();
			b2.AId = TheId;
			b2.Name = "Second B";
			a.Items.Add(b2);

			ISession s = OpenSession();
			s.Save(a);
			s.Flush();
			s.Close();

			s = OpenSession();
			A newA = s.Get<A>(TheId);

			Assert.AreEqual(2, newA.Items.Count);
			int counter = 1;
			foreach (B item in newA.Items)
			{
				Assert.AreEqual(counter, item.ListIndex);
				counter++;
			}
			s.Close();
		}
	}
}