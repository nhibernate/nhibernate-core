using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.PropertyRef
{
	[TestFixture]
	public class KeyEntityPropertyRefFixture : TestCase
	{
		protected override string[] Mappings => new string[] { "PropertyRef.KeyEntityPropertyRef.hbm.xml" };

		protected override string MappingsAssembly => "NHibernate.Test";

		private int _aWithItemsId;

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var c1 = new C { Name = "Third" };
				s.Save(c1);
				var c2 = new C { Name = "ThirdBis" };
				s.Save(c2);

				var b1 = new B { Id = 1, Name = "SecondC1" };
				var b2 = new B { Id = 2, Name = "SecondC2" };
				s.Save(b1);
				s.Save(b2);
				var a = new A { Id = 3, Name = "First", C = c1 };
				a.CItems.Add(b1);
				a.CItems.Add(b2);
				_aWithItemsId = (int) s.Save(a);

				s.Save(new A { Id = 4, Name = "FirstBis", C = c2 });

				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from B").ExecuteUpdate();
				s.CreateQuery("delete from A").ExecuteUpdate();
				s.CreateQuery("delete from C").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void PropertyRefLazyLoad()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var a = s.Get<A>(_aWithItemsId);

				Assert.That(NHibernateUtil.IsInitialized(a.CItems), Is.False, "a with items");
				Assert.That(a.CItems, Has.Count.EqualTo(2), "a with items");
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var entity =
					s
						.Query<A>()
						.Single(a => a.Id != _aWithItemsId);

				Assert.That(NHibernateUtil.IsInitialized(entity.CItems), Is.False, "a without items");
				Assert.That(entity.CItems, Has.Count.EqualTo(0), "a without items");
				t.Commit();
			}
		}

		[Test]
		public void PropertyRefEagerLoad()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var entity =
					s
						.Query<A>()
						.Where(a => a.Id == _aWithItemsId)
						.FetchMany(a => a.CItems)
						.Single();

				Assert.That(NHibernateUtil.IsInitialized(entity.CItems), Is.True, "a with items");
				Assert.That(entity.CItems, Has.Count.EqualTo(2), "a with items");
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var entity =
					s
						.Query<A>()
						.Where(a => a.Id != _aWithItemsId)
						.FetchMany(a => a.CItems)
						.Single();

				Assert.That(NHibernateUtil.IsInitialized(entity.CItems), Is.True, "a without items");
				Assert.That(entity.CItems, Has.Count.EqualTo(0), "a without items");
				t.Commit();
			}
		}
	}
}
