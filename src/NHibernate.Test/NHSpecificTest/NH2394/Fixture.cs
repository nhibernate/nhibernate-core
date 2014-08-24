using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;
using NHibernate.Linq;
using System.Linq;
using NHibernate.Linq.Functions;

namespace NHibernate.Test.NHSpecificTest.NH2394
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession s = sessions.OpenSession())
			{
				s.Delete("from A");
				s.Flush();
			}
		}

		[Test]
		public void LinqUserTypeEquality()
		{
			ISession s = OpenSession();
			try
			{
				s.Save(new A { Type = TypeOfA.Awesome, Phone = new PhoneNumber(1, "555-1111") });
				s.Save(new A { Type = TypeOfA.Boring, NullableType = TypeOfA.Awesome, Phone = new PhoneNumber(1, "555-2222") });
				s.Save(new A { Type = TypeOfA.Cool, Phone = new PhoneNumber(1, "555-3333") });
				s.Flush();
			}
			finally
			{
				s.Close();
			}

			s = OpenSession();
			try
			{
				A item;

				Assert.AreEqual(3, s.CreateQuery("from A a where a.IsNice = ?").SetParameter(0, false).List().Count);
				Assert.AreEqual(3, s.Query<A>().Count(a => a.IsNice == false));

				item = s.CreateQuery("from A a where a.Type = ?").SetParameter(0, TypeOfA.Awesome).UniqueResult<A>();
				Assert.AreEqual(TypeOfA.Awesome, item.Type);
				Assert.AreEqual("555-1111", item.Phone.Number);

				item = s.Query<A>().Where(a => a.Type == TypeOfA.Awesome).Single();
				Assert.AreEqual(TypeOfA.Awesome, item.Type);
				Assert.AreEqual("555-1111", item.Phone.Number);

				item = s.Query<A>().Where(a => TypeOfA.Awesome == a.Type).Single();
				Assert.AreEqual(TypeOfA.Awesome, item.Type);
				Assert.AreEqual("555-1111", item.Phone.Number);

				IA interfaceItem = s.Query<IA>().Where(a => a.Type == TypeOfA.Awesome).Single();
				Assert.AreEqual(TypeOfA.Awesome, interfaceItem.Type);
				Assert.AreEqual("555-1111", interfaceItem.Phone.Number);

				item = s.CreateQuery("from A a where a.NullableType = ?").SetParameter(0, TypeOfA.Awesome).UniqueResult<A>();
				Assert.AreEqual(TypeOfA.Boring, item.Type);
				Assert.AreEqual("555-2222", item.Phone.Number);
				Assert.AreEqual(TypeOfA.Awesome, item.NullableType);

				item = s.Query<A>().Where(a => a.NullableType == TypeOfA.Awesome).Single();
				Assert.AreEqual(TypeOfA.Boring, item.Type);
				Assert.AreEqual("555-2222", item.Phone.Number);
				Assert.AreEqual(TypeOfA.Awesome, item.NullableType);

				Assert.AreEqual(2, s.Query<A>().Count(a => a.NullableType == null));

				item = s.CreateQuery("from A a where a.Phone = ?").SetParameter(0, new PhoneNumber(1, "555-2222")).UniqueResult<A>();
				Assert.AreEqual(TypeOfA.Boring, item.Type);
				Assert.AreEqual("555-2222", item.Phone.Number);

				item = s.Query<A>().Where(a => a.Phone == new PhoneNumber(1, "555-2222")).Single();
				Assert.AreEqual(TypeOfA.Boring, item.Type);
				Assert.AreEqual("555-2222", item.Phone.Number);
			}
			finally
			{
				s.Close();
			}
		}
	}
}
