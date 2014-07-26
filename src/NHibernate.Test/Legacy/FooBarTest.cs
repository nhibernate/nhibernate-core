using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Iesi.Collections.Generic;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.DomainModel;
using NHibernate.Criterion;
using NHibernate.Proxy;
using NHibernate.Test.NHSpecificTest.NH1914;
using NHibernate.Type;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.Legacy
{
	[TestFixture]
	public class FooBarTest : TestCase
	{
		// Equivalent of Java String.getBytes()
		private static byte[] GetBytes(string str)
		{
			return Encoding.Unicode.GetBytes(str);
		}

		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"FooBar.hbm.xml",
						"Baz.hbm.xml",
						"Qux.hbm.xml",
						"Glarch.hbm.xml",
						"Fum.hbm.xml",
						"Fumm.hbm.xml",
						"Fo.hbm.xml",
						"One.hbm.xml",
						"Many.hbm.xml",
						"Immutable.hbm.xml",
						"Fee.hbm.xml",
						"Vetoer.hbm.xml",
						"Holder.hbm.xml",
						"Location.hbm.xml",
						"Stuff.hbm.xml",
						"Container.hbm.xml",
						"Simple.hbm.xml",
						"XY.hbm.xml"
					};
			}
		}

		[Test]
		public void CollectionVersioning()
		{
			using (ISession s = OpenSession())
			{
				One one = new One();
				one.Manies = new HashSet<Many>();
				s.Save(one);
				s.Flush();

				Many many = new Many();
				many.One = one;
				one.Manies.Add(many);
				s.Save(many);
				s.Flush();

				// Versions are incremented compared to Hibernate because they start from 1
				// in NH.
				Assert.AreEqual(1, many.V);
				Assert.AreEqual(2, one.V);

				s.Delete(many);
				s.Delete(one);
				s.Flush();
			}
		}

		[Test]
		public void ForCertain()
		{
			Glarch g = new Glarch();
			Glarch g2 = new Glarch();
			IList<string> strings = new List<string>();
			strings.Add("foo");
			g2.Strings = strings;

			object gid, g2id;

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					gid = s.Save(g);
					g2id = s.Save(g2);
					t.Commit();

					// Versions are initialized to 1 in NH.
					Assert.AreEqual(1, g.Version);
					Assert.AreEqual(1, g2.Version);
				}
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					g = (Glarch) s.Get(typeof(Glarch), gid);
					g2 = (Glarch) s.Get(typeof(Glarch), g2id);
					Assert.AreEqual(1, g2.Strings.Count);

					s.Delete(g);
					s.Delete(g2);
					t.Commit();
				}
			}
		}

		[Test]
		public void BagMultipleElements()
		{
			string bazCode;

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					Baz baz = new Baz();
					baz.Bag = new List<string>();
					baz.ByteBag = new List<byte[]>();
					s.Save(baz);
					baz.Bag.Add("foo");
					baz.Bag.Add("bar");
					baz.ByteBag.Add(GetBytes("foo"));
					baz.ByteBag.Add(GetBytes("bar"));
					t.Commit();

					bazCode = baz.Code;
				}
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					//put in cache
					Baz baz = (Baz) s.Get(typeof(Baz), bazCode);
					Assert.AreEqual(2, baz.Bag.Count);
					Assert.AreEqual(2, baz.ByteBag.Count);
					t.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					Baz baz = (Baz) s.Get(typeof(Baz), bazCode);
					Assert.AreEqual(2, baz.Bag.Count);
					Assert.AreEqual(2, baz.ByteBag.Count);

					baz.Bag.Remove("bar");
					baz.Bag.Add("foo");
					baz.ByteBag.Add(GetBytes("bar"));
					t.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					Baz baz = (Baz) s.Get(typeof(Baz), bazCode);
					Assert.AreEqual(2, baz.Bag.Count);
					Assert.AreEqual(3, baz.ByteBag.Count);
					s.Delete(baz);
					t.Commit();
				}
			}
		}

		[Test]
		public void WierdSession()
		{
			object id;

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					id = s.Save(new Foo());
					t.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				s.FlushMode = FlushMode.Never;
				using (ITransaction t = s.BeginTransaction())
				{
					Foo foo = (Foo) s.Get(typeof(Foo), id);
					t.Commit();
				}

				s.Disconnect();
				s.Reconnect();

				using (ITransaction t = s.BeginTransaction())
				{
					s.Flush();
					t.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					Foo foo = (Foo) s.Get(typeof(Foo), id);
					s.Delete(foo);
					t.Commit();
				}
			}
		}

		[Test]
		public void DereferenceLazyCollection()
		{
			string fooKey;
			string bazCode;

			using (ISession s = OpenSession())
			{
				Baz baz = new Baz();
				baz.FooSet = new HashSet<FooProxy>();
				Foo foo = new Foo();
				baz.FooSet.Add(foo);
				s.Save(foo);
				s.Save(baz);
				foo.Bytes = GetBytes("foobar");
				s.Flush();

				fooKey = foo.Key;
				bazCode = baz.Code;
			}

			using (ISession s = OpenSession())
			{
				Foo foo = (Foo) s.Get(typeof(Foo), fooKey);
				Assert.IsTrue(NHibernateUtil.IsInitialized(foo.Bytes));

				// H2.1 has 6 here, but we are using Unicode
				Assert.AreEqual(12, foo.Bytes.Length);

				Baz baz = (Baz) s.Get(typeof(Baz), bazCode);
				Assert.AreEqual(1, baz.FooSet.Count);
				s.Flush();
			}

			sessions.EvictCollection("NHibernate.DomainModel.Baz.FooSet");

			using (ISession s = OpenSession())
			{
				Baz baz = (Baz) s.Get(typeof(Baz), bazCode);
				Assert.IsFalse(NHibernateUtil.IsInitialized(baz.FooSet));
				baz.FooSet = null;
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				Foo foo = (Foo) s.Get(typeof(Foo), fooKey);
				Assert.AreEqual(12, foo.Bytes.Length);
				Baz baz = (Baz) s.Get(typeof(Baz), bazCode);

				Assert.IsFalse(NHibernateUtil.IsInitialized(baz.FooSet));
				Assert.AreEqual(0, baz.FooSet.Count);
				s.Delete(baz);
				s.Delete(foo);
				s.Flush();
			}
		}

		[Test]
		public void MoveLazyCollection()
		{
			string fooKey, bazCode, baz2Code;

			using (ISession s = OpenSession())
			{
				Baz baz = new Baz();
				Baz baz2 = new Baz();
				baz.FooSet = new HashSet<FooProxy>();
				Foo foo = new Foo();
				baz.FooSet.Add(foo);
				s.Save(foo);
				s.Save(baz);
				s.Save(baz2);
				foo.Bytes = GetBytes("foobar");
				s.Flush();

				fooKey = foo.Key;
				bazCode = baz.Code;
				baz2Code = baz2.Code;
			}

			using (ISession s = OpenSession())
			{
				Foo foo = (Foo) s.Get(typeof(Foo), fooKey);
				Assert.IsTrue(NHibernateUtil.IsInitialized(foo.Bytes));
				Assert.AreEqual(12, foo.Bytes.Length);
				Baz baz = (Baz) s.Get(typeof(Baz), bazCode);
				Assert.AreEqual(1, baz.FooSet.Count);
				s.Flush();
			}

			sessions.EvictCollection("NHibernate.DomainModel.Baz.FooSet");

			using (ISession s = OpenSession())
			{
				Baz baz = (Baz) s.Get(typeof(Baz), bazCode);
				Assert.IsFalse(NHibernateUtil.IsInitialized(baz.FooSet));
				Baz baz2 = (Baz) s.Get(typeof(Baz), baz2Code);
				baz2.FooSet = baz.FooSet;
				baz.FooSet = null;
				Assert.IsFalse(NHibernateUtil.IsInitialized(baz2.FooSet));
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				Foo foo = (Foo) s.Get(typeof(Foo), fooKey);
				Assert.AreEqual(12, foo.Bytes.Length);
				Baz baz = (Baz) s.Get(typeof(Baz), bazCode);
				Baz baz2 = (Baz) s.Get(typeof(Baz), baz2Code);

				Assert.IsFalse(NHibernateUtil.IsInitialized(baz.FooSet));
				Assert.AreEqual(0, baz.FooSet.Count);

				Assert.IsTrue( NHibernateUtil.IsInitialized( baz2.FooSet ) ); //FooSet has batching enabled

				Assert.AreEqual(1, baz2.FooSet.Count);

				s.Delete(baz);
				s.Delete(baz2);
				s.Delete(foo);
				s.Flush();
			}
		}

		[Test]
		public void CriteriaCollection()
		{
			ISession s = OpenSession();
			Baz bb = (Baz) s.CreateCriteria(typeof(Baz)).UniqueResult();
			Baz baz = new Baz();
			s.Save(baz);
			s.Flush();
			s.Close();

			s = OpenSession();
			Baz b = (Baz) s.CreateCriteria(typeof(Baz)).UniqueResult();
			Assert.IsTrue(NHibernateUtil.IsInitialized(b.TopGlarchez));
			Assert.AreEqual(0, b.TopGlarchez.Count);
			s.Delete(b);
			s.Flush();

			s.CreateCriteria(typeof(Baz))
				.CreateCriteria("TopFoos")
				.Add(Expression.IsNotNull("id"))
				.List();

			s.CreateCriteria(typeof(Baz))
				.CreateCriteria("Foo")
				.CreateCriteria("Component.Glarch")
				.CreateCriteria("ProxySet")
				.Add(Expression.IsNotNull("id"))
				.List();

			s.Close();
		}

		private static bool IsEmpty(IEnumerable enumerable)
		{
			return !enumerable.GetEnumerator().MoveNext();
		}

		private static bool ContainsSingleObject(IEnumerable enumerable, object obj)
		{
			IEnumerator enumerator = enumerable.GetEnumerator();

			// Fail if no items
			if (!enumerator.MoveNext())
			{
				return false;
			}

			// Fail if item not equal
			if (!Equals(obj, enumerator.Current))
			{
				return false;
			}

			// Fail if more items
			if (enumerator.MoveNext())
			{
				return false;
			}

			// Succeed
			return true;
		}

		[Test]
		public void Query()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();
			Foo foo = new Foo();
			s.Save(foo);
			Foo foo2 = new Foo();
			s.Save(foo2);
			foo.TheFoo = foo2;

			IList list = s.CreateQuery("from Foo foo inner join fetch foo.TheFoo").List();
			Foo foof = (Foo) list[0];
			Assert.IsTrue(NHibernateUtil.IsInitialized(foof.TheFoo));

			list = s.CreateQuery("from Baz baz left outer join fetch baz.FooToGlarch").List();

			list = s.CreateQuery("select foo, bar from Foo foo left outer join foo.TheFoo bar where foo = ?")
				.SetEntity(0, foo).List();


			object[] row1 = (object[]) list[0];
			Assert.IsTrue(row1[0] == foo && row1[1] == foo2);

			s.CreateQuery("select foo.TheFoo.TheFoo.String from foo in class Foo where foo.TheFoo = 'bar'").List();
			s.CreateQuery("select foo.TheFoo.TheFoo.TheFoo.String from foo in class Foo where foo.TheFoo.TheFoo = 'bar'").List();
			s.CreateQuery("select foo.TheFoo.TheFoo.String from foo in class Foo where foo.TheFoo.TheFoo.TheFoo.String = 'bar'").
				List();
			//			if( !( dialect is Dialect.HSQLDialect ) ) 
			//			{
			s.CreateQuery("select foo.String from foo in class Foo where foo.TheFoo.TheFoo.TheFoo = foo.TheFoo.TheFoo").List();
			//			}
			s.CreateQuery(
				"select foo.String from foo in class Foo where foo.TheFoo.TheFoo = 'bar' and foo.TheFoo.TheFoo.TheFoo = 'baz'").List
				();
			s.CreateQuery(
				"select foo.String from foo in class Foo where foo.TheFoo.TheFoo.TheFoo.String = 'a' and foo.TheFoo.String = 'b'").
				List();

			s.CreateQuery("from bar in class Bar, foo in elements(bar.Baz.FooArray)").List();

			if (Dialect is DB2Dialect)
			{
				s.CreateQuery("from foo in class Foo where lower( foo.TheFoo.String ) = 'foo'").List();
				s.CreateQuery("from foo in class Foo where lower( (foo.TheFoo.String || 'foo') || 'bar' ) = 'foo'").List();
				s.CreateQuery("from foo in class Foo where repeat( (foo.TheFoo.STring || 'foo') || 'bar', 2 ) = 'foo'").List();
				s.CreateQuery(
					"From foo in class Bar where foo.TheFoo.Integer is not null and repeat( (foo.TheFoo.String || 'foo') || 'bar', (5+5)/2 ) = 'foo'")
					.List();
				s.CreateQuery(
					"From foo in class Bar where foo.TheFoo.Integer is not null or repeat( (foo.TheFoo.String || 'foo') || 'bar', (5+5)/2 ) = 'foo'")
					.List();
			}

			if (Dialect is MsSql2000Dialect)
			{
				s.CreateQuery("select baz from Baz as baz join baz.FooArray foo group by baz order by sum(foo.Float)").Enumerable();
			}

			s.CreateQuery("from Foo as foo where foo.Component.Glarch.Name is not null").List();
			s.CreateQuery("from Foo as foo left outer join foo.Component.Glarch as glarch where glarch.Name = 'foo'").List();

			list = s.CreateQuery("from Foo").List();
			Assert.AreEqual(2, list.Count);
			Assert.IsTrue(list[0] is FooProxy);
			list = s.CreateQuery("from Foo foo left outer join foo.TheFoo").List();
			Assert.AreEqual(2, list.Count);
			Assert.IsTrue(((object[]) list[0])[0] is FooProxy);

			s.CreateQuery("From Foo, Bar").List();
			s.CreateQuery("from Baz baz left join baz.FooToGlarch, Bar bar join bar.TheFoo").List();
			s.CreateQuery("from Baz baz left join baz.FooToGlarch join baz.FooSet").List();
			s.CreateQuery("from Baz baz left join baz.FooToGlarch join fetch baz.FooSet foo left join fetch foo.TheFoo").List();

			list =
				s.CreateQuery(
					"from foo in class NHibernate.DomainModel.Foo where foo.String='osama bin laden' and foo.Boolean = true order by foo.String asc, foo.Component.Count desc")
					.List();
			Assert.AreEqual(0, list.Count, "empty query");
			IEnumerable enumerable =
				s.CreateQuery(
					"from foo in class NHibernate.DomainModel.Foo where foo.String='osama bin laden' order by foo.String asc, foo.Component.Count desc")
					.Enumerable();
			Assert.IsTrue(IsEmpty(enumerable), "empty enumerator");

			list = s.CreateQuery("select foo.TheFoo from foo in class NHibernate.DomainModel.Foo").List();
			Assert.AreEqual(1, list.Count, "query");
			Assert.AreEqual(foo.TheFoo, list[0], "returned object");
			foo.TheFoo.TheFoo = foo;
			foo.String = "fizard";

			if (Dialect.SupportsSubSelects && TestDialect.SupportsOperatorSome)
			{
				if (!(Dialect is FirebirdDialect))
				{
					list = s.CreateQuery(
							"from foo in class NHibernate.DomainModel.Foo where ? = some elements(foo.Component.ImportantDates)").
							SetDateTime(0, DateTime.Today).List();
					
					Assert.AreEqual(2, list.Count, "component query");
				}

				list =
					s.CreateQuery("from foo in class NHibernate.DomainModel.Foo where size(foo.Component.ImportantDates) = 3").List();
				Assert.AreEqual(2, list.Count, "component query");
				list = s.CreateQuery("from foo in class Foo where 0 = size(foo.Component.ImportantDates)").List();
				Assert.AreEqual(0, list.Count, "component query");
				list = s.CreateQuery("from foo in class Foo where exists elements(foo.Component.ImportantDates)").List();
				Assert.AreEqual(2, list.Count, "component query");
				s.CreateQuery("from foo in class Foo where not exists (from bar in class Bar where bar.id = foo.id)").List();

				s.CreateQuery(
					"select foo.TheFoo from foo in class Foo where foo = some(select x from x in class Foo where x.Long > foo.TheFoo.Long)")
					.List();
				s.CreateQuery(
					"from foo in class Foo where foo = some(select x from x in class Foo where x.Long > foo.TheFoo.Long) and foo.TheFoo.String='baz'")
					.List();
				s.CreateQuery(
					"from foo in class Foo where foo.TheFoo.String='baz' and foo = some(select x from x in class Foo where x.Long>foo.TheFoo.Long)")
					.List();
				s.CreateQuery("from foo in class Foo where foo = some(select x from x in class Foo where x.Long > foo.TheFoo.Long)")
					.List();

				s.CreateQuery(
					"select foo.String, foo.Date, foo.TheFoo.String, foo.id from foo in class Foo, baz in class Baz where foo in elements(baz.FooArray) and foo.String like 'foo'")
					.Enumerable();
			}

			list = s.CreateQuery("from foo in class Foo where foo.Component.Count is null order by foo.Component.Count").List();
			Assert.AreEqual(0, list.Count, "component query");

			list = s.CreateQuery("from foo in class Foo where foo.Component.Name='foo'").List();
			Assert.AreEqual(2, list.Count, "component query");

			list =
				s.CreateQuery(
					"select distinct foo.Component.Name, foo.Component.Name from foo in class Foo where foo.Component.Name='foo'").List
					();
			Assert.AreEqual(1, list.Count, "component query");

			list =
				s.CreateQuery("select distinct foo.Component.Name, foo.id from foo in class Foo where foo.Component.Name='foo'").
					List();
			Assert.AreEqual(2, list.Count, "component query");

			list = s.CreateQuery("select foo.TheFoo from foo in class Foo").List();
			Assert.AreEqual(2, list.Count, "query");

			list = s.CreateQuery("from foo in class Foo where foo.id=?").SetString(0, foo.Key).List();
			Assert.AreEqual(1, list.Count, "id query");

			list = s.CreateQuery("from foo in class Foo where foo.Key=?").SetString(0, foo.Key).List();
			Assert.AreEqual(1, list.Count, "named id query");
			Assert.AreSame(foo, list[0], "id query");

			list = s.CreateQuery("select foo.TheFoo from foo in class Foo where foo.String='fizard'").List();
			Assert.AreEqual(1, list.Count, "query");
			Assert.AreSame(foo.TheFoo, list[0], "returned object");

			list = s.CreateQuery("from foo in class Foo where foo.Component.Subcomponent.Name='bar'").List();
			Assert.AreEqual(2, list.Count, "components of components");

			list = s.CreateQuery("select foo.TheFoo from foo in class Foo where foo.TheFoo.id=?")
				.SetString(0, foo.TheFoo.Key).List();
			Assert.AreEqual(1, list.Count, "by id query");
			Assert.AreSame(foo.TheFoo, list[0], "by id returned object");

			s.CreateQuery("from foo in class Foo where foo.TheFoo = ?").SetEntity(0, foo.TheFoo).List();

			Assert.IsTrue(
				IsEmpty(s.CreateQuery("from bar in class Bar where bar.String='a string' or bar.String='a string'").Enumerable()
					));

			enumerable = s.CreateQuery(
						"select foo.Component.Name, elements(foo.Component.ImportantDates) from foo in class Foo where foo.TheFoo.id=?").
						SetString(0, foo.TheFoo.Key).Enumerable();
			

			int i = 0;
			foreach (object[] row in enumerable)
			{
				i++;
				Assert.IsTrue(row[0] is String);
				Assert.IsTrue(row[1] == null || row[1] is DateTime);
			}
			Assert.AreEqual(3, i); //WAS: 4

			enumerable = s.CreateQuery("select max(elements(foo.Component.ImportantDates)) from foo in class Foo group by foo.id").
						Enumerable();
			
			IEnumerator enumerator = enumerable.GetEnumerator();

			Assert.IsTrue(enumerator.MoveNext());
			Assert.IsTrue(enumerator.Current is DateTime);

			list = s.CreateQuery(
				"select foo.TheFoo.TheFoo.TheFoo from foo in class Foo, foo2 in class Foo where"
				+ " foo = foo2.TheFoo and not not ( not foo.String='fizard' )"
				+ " and foo2.String between 'a' and (foo.TheFoo.String)"
				+ (Dialect is SQLiteDialect
				   	? " and ( foo2.String in ( 'fiz', 'blah') or 1=1 )"
				   	: " and ( foo2.String in ( 'fiz', 'blah', foo.TheFoo.String, foo.String, foo2.String ) )")
				).List();
			Assert.AreEqual(1, list.Count, "complex query");
			Assert.AreSame(foo, list[0], "returned object");

			foo.String = "from BoogieDown  -tinsel town  =!@#$^&*())";

			list = s.CreateQuery("from foo in class Foo where foo.String='from BoogieDown  -tinsel town  =!@#$^&*())'").List();
			Assert.AreEqual(1, list.Count, "single quotes");

			list = s.CreateQuery("from foo in class Foo where not foo.String='foo''bar'").List();
			Assert.AreEqual(2, list.Count, "single quotes");

			list = s.CreateQuery("from foo in class Foo where foo.Component.Glarch.Next is null").List();
			Assert.AreEqual(2, list.Count, "query association in component");

			Bar bar = new Bar();
			Baz baz = new Baz();
			baz.SetDefaults();
			bar.Baz = baz;
			baz.ManyToAny = new List<object>();
			baz.ManyToAny.Add(bar);
			baz.ManyToAny.Add(foo);
			s.Save(bar);
			s.Save(baz);
			list =
				s.CreateQuery(" from bar in class Bar where bar.Baz.Count=667 and bar.Baz.Count!=123 and not bar.Baz.Name='1-E-1'").
					List();
			Assert.AreEqual(1, list.Count, "query many-to-one");
			list = s.CreateQuery(" from i in class Bar where i.Baz.Name='Bazza'").List();
			Assert.AreEqual(1, list.Count, "query many-to-one");

			if (DialectSupportsCountDistinct)
			{
				enumerable = s.CreateQuery("select count(distinct foo.TheFoo) from foo in class Foo").Enumerable();
				Assert.IsTrue(ContainsSingleObject(enumerable, (long) 2), "count"); // changed to Int64 (HQLFunction H3.2)
			}

			enumerable = s.CreateQuery("select count(foo.TheFoo.Boolean) from foo in class Foo").Enumerable();
			Assert.IsTrue(ContainsSingleObject(enumerable, (long) 2), "count"); // changed to Int64 (HQLFunction H3.2)

			enumerable = s.CreateQuery("select count(*), foo.Int from foo in class Foo group by foo.Int").Enumerable();
			enumerator = enumerable.GetEnumerator();
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(3L, (long) ((object[]) enumerator.Current)[0]);
			Assert.IsFalse(enumerator.MoveNext());

			enumerable = s.CreateQuery("select sum(foo.TheFoo.Int) from foo in class Foo").Enumerable();
			Assert.IsTrue(ContainsSingleObject(enumerable, (long) 4), "sum"); // changed to Int64 (HQLFunction H3.2)

			enumerable = s.CreateQuery("select count(foo) from foo in class Foo where foo.id=?")
				.SetString(0, foo.Key).Enumerable();
			Assert.IsTrue(ContainsSingleObject(enumerable, (long) 1), "id query count");

			list = s.CreateQuery("from foo in class Foo where foo.Boolean = ?").SetBoolean(0, true).List();

			list = s.CreateQuery("select new Foo(fo.X) from Fo fo").List();
			list = s.CreateQuery("select new Foo(fo.Integer) from Foo fo").List();

			list = s.CreateQuery("select new Foo(fo.X) from Foo fo")
				.SetCacheable(true)
				.List();
			Assert.IsTrue(list.Count == 3);
			list = s.CreateQuery("select new Foo(fo.X) from Foo fo")
				.SetCacheable(true)
				.List();
			Assert.IsTrue(list.Count == 3);

			enumerable = s.CreateQuery("select new Foo(fo.X) from Foo fo").Enumerable();
			enumerator = enumerable.GetEnumerator();
			Assert.IsTrue(enumerator.MoveNext(), "projection iterate (results)");
			Assert.IsTrue(typeof(Foo).IsAssignableFrom(enumerator.Current.GetType()),
			              "projection iterate (return check)");

			// TODO: ScrollableResults not implemented
			//ScrollableResults sr = s.CreateQuery("select new Foo(fo.x) from Foo fo").Scroll();
			//Assert.IsTrue( "projection scroll (results)", sr.next() );
			//Assert.IsTrue( "projection scroll (return check)", typeof(Foo).isAssignableFrom( sr.get(0).getClass() ) );

			list = s.CreateQuery("select foo.Long, foo.Component.Name, foo, foo.TheFoo from foo in class Foo").List();
			Assert.IsTrue(list.Count > 0);
			foreach (object[] row in list)
			{
				Assert.IsTrue(row[0] is long);
				Assert.IsTrue(row[1] is string);
				Assert.IsTrue(row[2] is Foo);
				Assert.IsTrue(row[3] is Foo);
			}

			if (DialectSupportsCountDistinct)
			{
				list =
					s.CreateQuery("select avg(foo.Float), max(foo.Component.Name), count(distinct foo.id) from foo in class Foo").List();
				Assert.IsTrue(list.Count > 0);
				foreach (object[] row in list)
				{
					Assert.IsTrue(row[0] is double); // changed from float to double (HQLFunction H3.2) 
					Assert.IsTrue(row[1] is string);
					Assert.IsTrue(row[2] is long); // changed from int to long (HQLFunction H3.2)
				}
			}

			list = s.CreateQuery("select foo.Long, foo.Component, foo, foo.TheFoo from foo in class Foo").List();
			Assert.IsTrue(list.Count > 0);
			foreach (object[] row in list)
			{
				Assert.IsTrue(row[0] is long);
				Assert.IsTrue(row[1] is FooComponent);
				Assert.IsTrue(row[2] is Foo);
				Assert.IsTrue(row[3] is Foo);
			}

			s.Save(new Holder("ice T"));
			s.Save(new Holder("ice cube"));

			Assert.AreEqual(15, s.CreateQuery("from o in class System.Object").List().Count);
			Assert.AreEqual(7, s.CreateQuery("from n in class INamed").List().Count);
			Assert.IsTrue(s.CreateQuery("from n in class INamed where n.Name is not null").List().Count == 4);

			foreach (INamed named in s.CreateQuery("from n in class INamed").Enumerable())
			{
				Assert.IsNotNull(named);
			}

			s.Save(new Holder("bar"));
			enumerable = s.CreateQuery("from n0 in class INamed, n1 in class INamed where n0.Name = n1.Name").Enumerable();
			int cnt = 0;
			foreach (object[] row in enumerable)
			{
				if (row[0] != row[1])
				{
					cnt++;
				}
			}

			//if ( !(dialect is Dialect.HSQLDialect) )
			//{
			Assert.IsTrue(cnt == 2);
			Assert.IsTrue(s.CreateQuery("from n0 in class INamed, n1 in class INamed where n0.Name = n1.Name").List().Count == 7);
			//}

			IQuery qu = s.CreateQuery("from n in class INamed where n.Name = :name");
			object temp = qu.ReturnTypes;
			temp = qu.NamedParameters;

			int c = 0;

			foreach (object obj in s.CreateQuery("from o in class System.Object").Enumerable())
			{
				c++;
			}
			Assert.IsTrue(c == 16);

			s.CreateQuery("select baz.Code, min(baz.Count) from baz in class Baz group by baz.Code").Enumerable();

			Assert.IsTrue(
				IsEmpty(
					s.CreateQuery(
						"selecT baz from baz in class Baz where baz.StringDateMap['foo'] is not null or baz.StringDateMap['bar'] = ?")
						.SetDateTime(0, DateTime.Today).Enumerable()));

			list = s.CreateQuery("select baz from baz in class Baz where baz.StringDateMap['now'] is not null").List();
			Assert.AreEqual(1, list.Count);

			list =
				s.CreateQuery("select baz from baz in class Baz where baz.StringDateMap[:now] is not null").SetString("now", "now").
					List();
			Assert.AreEqual(1, list.Count);

			list =
				s.CreateQuery(
					"select baz from baz in class Baz where baz.StringDateMap['now'] is not null and baz.StringDateMap['big bang'] < baz.StringDateMap['now']")
					.List();
			Assert.AreEqual(1, list.Count);

			list = s.CreateQuery("select index(date) from Baz baz join baz.StringDateMap date").List();
			Console.WriteLine(list);
			Assert.AreEqual(3, list.Count);

			s.CreateQuery(
				"from foo in class Foo where foo.Integer not between 1 and 5 and foo.String not in ('cde', 'abc') and foo.String is not null and foo.Integer<=3")
				.List();

			s.CreateQuery("from Baz baz inner join baz.CollectionComponent.Nested.Foos foo where foo.String is null").List();
			if (Dialect.SupportsSubSelects)
			{
				s.CreateQuery("from Baz baz inner join baz.FooSet where '1' in (from baz.FooSet foo where foo.String is not null)").
					List();
				s.CreateQuery(
					"from Baz baz where 'a' in elements(baz.CollectionComponent.Nested.Foos) and 1.0 in elements(baz.CollectionComponent.Nested.Floats)")
					.List();
				s.CreateQuery(
					"from Baz baz where 'b' in elements(baz.CollectionComponent.Nested.Foos) and 1.0 in elements(baz.CollectionComponent.Nested.Floats)")
					.List();
			}

			s.CreateQuery("from Foo foo join foo.TheFoo where foo.TheFoo in ('1','2','3')").List();

			//if ( !(dialect is Dialect.HSQLDialect) )
			s.CreateQuery("from Foo foo left join foo.TheFoo where foo.TheFoo in ('1','2','3')").List();
			s.CreateQuery("select foo.TheFoo from Foo foo where foo.TheFoo in ('1','2','3')").List();
			s.CreateQuery("select foo.TheFoo.String from Foo foo where foo.TheFoo in ('1','2','3')").List();
			s.CreateQuery("select foo.TheFoo.String from Foo foo where foo.TheFoo.String in ('1','2','3')").List();
			s.CreateQuery("select foo.TheFoo.Long from Foo foo where foo.TheFoo.String in ('1','2','3')").List();
			s.CreateQuery("select count(*) from Foo foo where foo.TheFoo.String in ('1','2','3') or foo.TheFoo.Long in (1,2,3)").
				List();
			s.CreateQuery("select count(*) from Foo foo where foo.TheFoo.String in ('1','2','3') group by foo.TheFoo.Long").List();

			s.CreateQuery("from Foo foo1 left join foo1.TheFoo foo2 left join foo2.TheFoo where foo1.String is not null").List();
			s.CreateQuery("from Foo foo1 left join foo1.TheFoo.TheFoo where foo1.String is not null").List();
			s.CreateQuery(
				"from Foo foo1 left join foo1.TheFoo foo2 left join foo1.TheFoo.TheFoo foo3 where foo1.String is not null").List();

			s.CreateQuery("select foo.Formula from Foo foo where foo.Formula > 0").List();

			int len = s.CreateQuery("from Foo as foo join foo.TheFoo as foo2 where foo2.id >'a' or foo2.id <'a'").List().Count;
			Assert.IsTrue(len == 2);

			s.Delete("from Holder");
			txn.Commit();
			s.Close();

			s = OpenSession();
			txn = s.BeginTransaction();
			baz = (Baz) s.CreateQuery("from Baz baz left outer join fetch baz.ManyToAny").UniqueResult();
			Assert.IsTrue(NHibernateUtil.IsInitialized(baz.ManyToAny));
			Assert.IsTrue(baz.ManyToAny.Count == 2);
			BarProxy barp = (BarProxy) baz.ManyToAny[0];
			s.CreateQuery("from Baz baz join baz.ManyToAny").List();
			Assert.IsTrue(s.CreateQuery("select baz from Baz baz join baz.ManyToAny a where index(a) = 0").List().Count == 1);

			FooProxy foop = (FooProxy) s.Get(typeof(Foo), foo.Key);
			Assert.IsTrue(foop == baz.ManyToAny[1]);

			barp.Baz = baz;
			Assert.IsTrue(s.CreateQuery("select bar from Bar bar where bar.Baz.StringDateMap['now'] is not null").List().Count ==
			              1);
			Assert.IsTrue(
				s.CreateQuery(
					"select bar from Bar bar join bar.Baz b where b.StringDateMap['big bang'] < b.StringDateMap['now'] and b.StringDateMap['now'] is not null")
					.List().Count == 1);
			Assert.IsTrue(
				s.CreateQuery(
					"select bar from Bar bar where bar.Baz.StringDateMap['big bang'] < bar.Baz.StringDateMap['now'] and bar.Baz.StringDateMap['now'] is not null")
					.List().Count == 1);

			list = s.CreateQuery("select foo.String, foo.Component, foo.id from Bar foo").List();
			Assert.IsTrue(((FooComponent) ((object[]) list[0])[1]).Name == "foo");
			list = s.CreateQuery("select elements(baz.Components) from Baz baz").List();
			Assert.IsTrue(list.Count == 2);
			list = s.CreateQuery("select bc.Name from Baz baz join baz.Components bc").List();
			Assert.IsTrue(list.Count == 2);
			//list = s.CreateQuery("select bc from Baz baz join baz.components bc").List();

			s.CreateQuery("from Foo foo where foo.Integer < 10 order by foo.String").SetMaxResults(12).List();

			s.Delete(barp);
			s.Delete(baz);
			s.Delete(foop.TheFoo);
			s.Delete(foop);
			txn.Commit();
			s.Close();
		}

		[Test]
		public void CascadeDeleteDetached()
		{
			Baz baz;

			using (ISession s = OpenSession())
			{
				baz = new Baz();
				IList<Fee> list = new List<Fee>();
				list.Add(new Fee());
				baz.Fees = list;
				s.Save(baz);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				baz = (Baz) s.Get(typeof(Baz), baz.Code);
			}

			Assert.IsFalse(NHibernateUtil.IsInitialized(baz.Fees));

			using (ISession s = OpenSession())
			{
				s.Delete(baz);
				s.Flush();

				Assert.IsFalse(s.CreateQuery("from Fee").Enumerable().GetEnumerator().MoveNext());
			}

			using (ISession s = OpenSession())
			{
				baz = new Baz();
				IList<Fee> list = new List<Fee>();
				list.Add(new Fee());
				list.Add(new Fee());
				baz.Fees = list;
				s.Save(baz);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				baz = (Baz) s.Get(typeof(Baz), baz.Code);
				NHibernateUtil.Initialize(baz.Fees);
			}

			Assert.AreEqual(2, baz.Fees.Count);

			using (ISession s = OpenSession())
			{
				s.Delete(baz);
				s.Flush();
				Assert.IsTrue(IsEmpty(s.CreateQuery("from Fee").Enumerable()));
			}
		}

		[Test]
		public void ForeignKeys()
		{
			Baz baz;
			using (ISession s = OpenSession())
			{
				baz = new Baz();
				Foo foo = new Foo();
				IList<Foo> bag = new List<Foo>();
				bag.Add(foo);
				baz.IdFooBag = bag;
				baz.Foo = foo;
				s.Save(baz);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				baz = (Baz) s.Load(typeof(Baz), baz.Code);
				s.Delete(baz);
				s.Flush();
			}
		}

		[Test]
		public void NonlazyCollections()
		{
			object glarchId;

			using (ISession s = OpenSession())
			{
				Glarch glarch1 = new Glarch();
				glarch1.ProxySet = new LinkedHashSet<GlarchProxy>();

				Glarch glarch2 = new Glarch();
				glarch1.ProxySet.Add(glarch1);

				s.Save(glarch2);
				glarchId = s.Save(glarch1);
				s.Flush();
			}

			Glarch loadedGlarch;
			using (ISession s = OpenSession())
			{
				loadedGlarch = (Glarch) s.Get(typeof(Glarch), glarchId);
				Assert.IsTrue(NHibernateUtil.IsInitialized(loadedGlarch.ProxySet));
			}

			// ProxySet is a non-lazy collection, so this should work outside
			// a session.
			Assert.AreEqual(1, loadedGlarch.ProxySet.Count);

			using (ISession s = OpenSession())
			{
				s.Delete("from Glarch");
				s.Flush();
			}
		}


		[Test]
		public void ReuseDeletedCollection()
		{
			Baz baz, baz2;

			using (ISession s = OpenSession())
			{
				baz = new Baz();
				baz.SetDefaults();
				s.Save(baz);
				s.Flush();
				s.Delete(baz);

				baz2 = new Baz();
				baz2.StringArray = new string[] {"x-y-z"};
				s.Save(baz2);
				s.Flush();
			}

			baz2.StringSet = baz.StringSet;
			baz2.StringArray = baz.StringArray;
			baz2.FooArray = baz.FooArray;

			using (ISession s = OpenSession())
			{
				s.Update(baz2);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				baz2 = (Baz) s.Load(typeof(Baz), baz2.Code);
				Assert.AreEqual(3, baz2.StringArray.Length);
				Assert.AreEqual(3, baz2.StringSet.Count);
				s.Delete(baz2);
				s.Flush();
			}
		}

		[Test]
		public void PropertyRef()
		{
			object qid;
			object hid;

			using (ISession s = OpenSession())
			{
				Holder h = new Holder();
				h.Name = "foo";
				Holder h2 = new Holder();
				h2.Name = "bar";
				h.OtherHolder = h2;
				hid = s.Save(h);

				Qux q = new Qux();
				q.Holder = h2;
				qid = s.Save(q);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				Holder h = (Holder) s.Load(typeof(Holder), hid);
				Assert.AreEqual(h.Name, "foo");
				Assert.AreEqual(h.OtherHolder.Name, "bar");
				object[] res =
					(object[]) s.CreateQuery("from Holder h join h.OtherHolder oh where h.OtherHolder.Name = 'bar'").List()[0];
				Assert.AreSame(h, res[0]);

				Qux q = (Qux) s.Get(typeof(Qux), qid);
				Assert.AreSame(q.Holder, h.OtherHolder);
				s.Delete(h);
				s.Delete(q);
				s.Flush();
			}
		}

		[Test]
		public void QueryCollectionOfValues()
		{
			object gid;

			using (ISession s = OpenSession())
			{
				Baz baz = new Baz();
				baz.SetDefaults();
				s.Save(baz);
				Glarch g = new Glarch();
				gid = s.Save(g);

				if (Dialect.SupportsSubSelects)
				{
					s.CreateFilter(baz.FooArray, "where size(this.Bytes) > 0").List();
					s.CreateFilter(baz.FooArray, "where 0 in elements(this.Bytes)").List();
				}
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				//s.CreateQuery("from Baz baz where baz.FooSet.String = 'foo'").List();
				//s.CreateQuery("from Baz baz where baz.FooArray.String = 'foo'").List();
				//s.CreateQuery("from Baz baz where baz.FooSet.foo.String = 'foo'").List();
				//s.CreateQuery("from Baz baz join baz.FooSet.Foo foo where foo.String = 'foo'").List();
				s.CreateQuery("from Baz baz join baz.FooSet foo join foo.TheFoo.TheFoo foo2 where foo2.String = 'foo'").List();
				s.CreateQuery("from Baz baz join baz.FooArray foo join foo.TheFoo.TheFoo foo2 where foo2.String = 'foo'").List();
				s.CreateQuery("from Baz baz join baz.StringDateMap date where index(date) = 'foo'").List();
				s.CreateQuery("from Baz baz join baz.TopGlarchez g where index(g) = 'A'").List();
				s.CreateQuery("select index(g) from Baz baz join baz.TopGlarchez g").List();

				Assert.AreEqual(3, s.CreateQuery("from Baz baz left join baz.StringSet").List().Count);
				Baz baz = (Baz) s.CreateQuery("from Baz baz join baz.StringSet str where str='foo'").List()[0];
				Assert.IsFalse(NHibernateUtil.IsInitialized(baz.StringSet));
				baz = (Baz) s.CreateQuery("from Baz baz left join fetch baz.StringSet").List()[0];
				Assert.IsTrue(NHibernateUtil.IsInitialized(baz.StringSet));
				Assert.AreEqual(1, s.CreateQuery("from Baz baz join baz.StringSet string where string='foo'").List().Count);
				Assert.AreEqual(1, s.CreateQuery("from Baz baz inner join baz.Components comp where comp.Name='foo'").List().Count);
				//IList bss = s.CreateQuery("select baz, ss from Baz baz inner join baz.StringSet ss").List();
				s.CreateQuery("from Glarch g inner join g.FooComponents comp where comp.Fee is not null").List();
				s.CreateQuery("from Glarch g inner join g.FooComponents comp join comp.Fee fee where fee.Count > 0").List();
				s.CreateQuery("from Glarch g inner join g.FooComponents comp where comp.Fee.Count is not null").List();

				s.Delete(baz);
				//s.delete("from Glarch g");
				s.Delete(s.Get(typeof(Glarch), gid));
				s.Flush();
			}
		}

		[Test]
		public void BatchLoad()
		{
			Baz baz, baz2, baz3;

			using (ISession s = OpenSession())
			{
				baz = new Baz();
				var stringSet = new SortedSet<string> { "foo", "bar" };
				var fooSet = new HashSet<FooProxy>();

				for (int i = 0; i < 3; i++)
				{
					Foo foo = new Foo();
					s.Save(foo);
					fooSet.Add(foo);
				}

				baz.FooSet = fooSet;
				baz.StringSet = stringSet;
				s.Save(baz);

				baz2 = new Baz();
				fooSet = new HashSet<FooProxy>();
				for (int i = 0; i < 2; i++)
				{
					Foo foo = new Foo();
					s.Save(foo);
					fooSet.Add(foo);
				}
				baz2.FooSet = fooSet;
				s.Save(baz2);

				baz3 = new Baz();
				stringSet = new SortedSet<string>();
				stringSet.Add("foo");
				stringSet.Add("baz");
				baz3.StringSet = stringSet;
				s.Save(baz3);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				baz = (Baz) s.Load(typeof(Baz), baz.Code);
				baz2 = (Baz) s.Load(typeof(Baz), baz2.Code);
				baz3 = (Baz) s.Load(typeof(Baz), baz3.Code);

				Assert.IsFalse(NHibernateUtil.IsInitialized(baz.FooSet));
				Assert.IsFalse(NHibernateUtil.IsInitialized(baz2.FooSet));
				Assert.IsFalse(NHibernateUtil.IsInitialized(baz3.FooSet));

				Assert.IsFalse(NHibernateUtil.IsInitialized(baz.StringSet));
				Assert.IsFalse(NHibernateUtil.IsInitialized(baz2.StringSet));
				Assert.IsFalse(NHibernateUtil.IsInitialized(baz3.StringSet));

				Assert.AreEqual(3, baz.FooSet.Count);

				Assert.IsTrue(NHibernateUtil.IsInitialized(baz.FooSet));
				Assert.IsTrue(NHibernateUtil.IsInitialized(baz2.FooSet));
				Assert.IsTrue(NHibernateUtil.IsInitialized(baz3.FooSet));

				Assert.AreEqual(2, baz2.FooSet.Count);

				Assert.IsTrue(baz3.StringSet.Contains("baz"));

				Assert.IsTrue(NHibernateUtil.IsInitialized(baz.StringSet));
				Assert.IsTrue(NHibernateUtil.IsInitialized(baz2.StringSet));
				Assert.IsTrue(NHibernateUtil.IsInitialized(baz3.StringSet));

				Assert.AreEqual(2, baz.StringSet.Count);
				Assert.AreEqual(0, baz2.StringSet.Count);

				s.Delete(baz);
				s.Delete(baz2);
				s.Delete(baz3);

				IEnumerable en = new JoinedEnumerable(
					new IEnumerable[] {baz.FooSet, baz2.FooSet});

				foreach (object obj in en)
				{
					s.Delete(obj);
				}

				s.Flush();
			}
		}

		[Test]
		public void FetchInitializedCollection()
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			IList<Foo> fooBag = new List<Foo>();
			fooBag.Add(new Foo());
			fooBag.Add(new Foo());
			baz.FooBag = fooBag;
			s.Save(baz);
			s.Flush();
			fooBag = baz.FooBag;
			s.CreateQuery("from Baz baz left join fetch baz.FooBag").List();
			Assert.IsTrue(NHibernateUtil.IsInitialized(fooBag));
			s.Close();

			s = OpenSession();
			baz = (Baz) s.Load(typeof(Baz), baz.Code);
			Object bag = baz.FooBag;
			Assert.IsFalse(NHibernateUtil.IsInitialized(bag));
			s.CreateQuery("from Baz baz left join fetch baz.FooBag").List();
			Assert.IsTrue(bag == baz.FooBag);
			Assert.IsTrue(baz.FooBag.Count == 2);
			s.Delete(baz);
			s.Flush();

			s.Close();
		}


		[Test]
		public void LateCollectionAdd()
		{
			object id;

			using (ISession s = OpenSession())
			{
				Baz baz = new Baz();
				IList<string> l = new List<string>();
				baz.StringList = l;

				l.Add("foo");
				id = s.Save(baz);

				l.Add("bar");
				s.Flush();

				l.Add("baz");
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				Baz baz = (Baz) s.Load(typeof(Baz), id);
				Assert.AreEqual(3, baz.StringList.Count);
				Assert.IsTrue(baz.StringList.Contains("foo"));
				Assert.IsTrue(baz.StringList.Contains("bar"));
				Assert.IsTrue(baz.StringList.Contains("baz"));

				s.Delete(baz);
				s.Flush();
			}
		}

		[Test]
		public void Update()
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			s.Flush();
			s.Close();

			s = OpenSession();
			FooProxy foo2 = (FooProxy) s.Load(typeof(Foo), foo.Key);
			foo2.String = "dirty";
			foo2.Boolean = false;
			foo2.Bytes = new byte[] {1, 2, 3};
			foo2.Date = DateTime.Today;
			foo2.Short = 69;
			s.Flush();
			s.Close();

			s = OpenSession();
			Foo foo3 = new Foo();
			s.Load(foo3, foo.Key);
			Assert.IsTrue(foo2.EqualsFoo(foo3), "update");
			s.Delete(foo3);
			s.Flush();
			s.Close();
		}


		[Test]
		public void ListRemove()
		{
			using (ISession s = OpenSession())
			{
				Baz b = new Baz();
				IList<string> stringList = new List<string>();
				IList<Fee> feeList = new List<Fee>();
				b.Fees = feeList;
				b.StringList = stringList;
				feeList.Add(new Fee());
				feeList.Add(new Fee());
				feeList.Add(new Fee());
				feeList.Add(new Fee());
				stringList.Add("foo");
				stringList.Add("bar");
				stringList.Add("baz");
				stringList.Add("glarch");
				s.Save(b);
				s.Flush();

				stringList.RemoveAt(1);
				feeList.RemoveAt(1);
				s.Flush();

				s.Evict(b);
				s.Refresh(b);
				Assert.AreEqual(3, b.Fees.Count);
				stringList = b.StringList;
				Assert.AreEqual(3, stringList.Count);
				Assert.AreEqual("baz", stringList[1]);
				Assert.AreEqual("foo", stringList[0]);

				s.Delete(b);
				s.Delete("from Fee");
				s.Flush();
			}
		}

		[Test]
		public void FetchInitializedCollectionDupe()
		{
			string bazCode;

			using (ISession s = OpenSession())
			{
				Baz baz = new Baz();
				IList<Foo> fooBag = new List<Foo>();
				fooBag.Add(new Foo());
				fooBag.Add(new Foo());
				baz.FooBag = fooBag;
				s.Save(baz);
				s.Flush();
				fooBag = baz.FooBag;
				s.CreateQuery("from Baz baz left join fetch baz.FooBag").List();
				Assert.IsTrue(NHibernateUtil.IsInitialized(fooBag));
				Assert.AreSame(fooBag, baz.FooBag);
				Assert.AreEqual(2, baz.FooBag.Count);

				bazCode = baz.Code;
			}

			using (ISession s = OpenSession())
			{
				Baz baz = (Baz) s.Load(typeof(Baz), bazCode);
				object bag = baz.FooBag;
				Assert.IsFalse(NHibernateUtil.IsInitialized(bag));
				s.CreateQuery("from Baz baz left join fetch baz.FooBag").List();
				Assert.IsTrue(NHibernateUtil.IsInitialized(bag));
				Assert.AreSame(bag, baz.FooBag);
				Assert.AreEqual(2, baz.FooBag.Count);
				s.Delete(baz);
				s.Flush();
			}
		}

		[Test]
		public void Sortables()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Baz b = new Baz();
			var ss = new HashSet<Sortable>
				{
					new Sortable("foo"), new Sortable("bar"), new Sortable("baz")
				};
			b.Sortablez = ss;
			s.Save(b);
			s.Flush();
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IList result = s.CreateCriteria(typeof(Baz))
				.AddOrder(Order.Asc("Name"))
				.List();
			b = (Baz) result[0];
			Assert.IsTrue(b.Sortablez.Count == 3);

			// compare the first item in the "Set" sortablez - can't reference
			// the first item using b.sortablez[0] because it thinks 0 is the
			// DictionaryEntry key - not the index.
			foreach (Sortable sortable in b.Sortablez)
			{
				Assert.AreEqual(sortable.name, "bar");
				break;
			}

			s.Flush();
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			result = s.CreateQuery("from Baz baz left join fetch baz.Sortablez order by baz.Name asc")
				.List();
			b = (Baz) result[0];
			Assert.IsTrue(b.Sortablez.Count == 3);
			foreach (Sortable sortable in b.Sortablez)
			{
				Assert.AreEqual(sortable.name, "bar");
				break;
			}
			s.Flush();
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			result = s.CreateQuery("from Baz baz order by baz.Name asc")
				.List();
			b = (Baz) result[0];
			Assert.IsTrue(b.Sortablez.Count == 3);
			foreach (Sortable sortable in b.Sortablez)
			{
				Assert.AreEqual(sortable.name, "bar");
				break;
			}
			s.Delete(b);
			s.Flush();
			t.Commit();
			s.Close();
		}


		[Test]
		public void FetchList()
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			s.Save(baz);
			Foo foo = new Foo();
			s.Save(foo);
			Foo foo2 = new Foo();
			s.Save(foo2);
			s.Flush();
			IList<Fee> list = new List<Fee>();
			for (int i = 0; i < 5; i++)
			{
				Fee fee = new Fee();
				list.Add(fee);
			}
			baz.Fees = list;
			var result = s.CreateQuery("from Foo foo, Baz baz left join fetch baz.Fees").List();
			Assert.IsTrue(NHibernateUtil.IsInitialized(((Baz)((object[])result[0])[1]).Fees));
			s.Delete(foo);
			s.Delete(foo2);
			s.Delete(baz);
			s.Flush();
			s.Close();
		}


		[Test]
		public void BagOneToMany()
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			IList<Baz> list = new List<Baz>();
			baz.Bazez = list;
			list.Add(new Baz());
			s.Save(baz);
			s.Flush();
			list.Add(new Baz());
			s.Flush();
			list.Insert(0, new Baz());
			s.Flush();
			object toDelete = list[1];
			list.RemoveAt(1);
			s.Delete(toDelete);
			s.Flush();
			s.Delete(baz);
			s.Flush();
			s.Close();
		}


		[Test]
		public void QueryLockMode()
		{
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();

			Bar bar = new Bar();
			Assert.IsNull(bar.Bytes);
			s.Save(bar);
			Assert.IsNotNull(bar.Bytes);
			s.Flush();
			Assert.IsNotNull(bar.Bytes);

			bar.String = "changed";
			Baz baz = new Baz();
			baz.Foo = bar;
			s.Save(baz);
			Assert.IsNotNull(bar.Bytes);

			IQuery q = s.CreateQuery("from Foo foo, Bar bar");
			q.SetLockMode("bar", LockMode.Upgrade);
			object[] result = (object[]) q.List()[0];
			Assert.IsNotNull(bar.Bytes);

			object b = result[0];

			Assert.IsTrue(s.GetCurrentLockMode(b) == LockMode.Write && s.GetCurrentLockMode(result[1]) == LockMode.Write);
			tx.Commit();
			Assert.IsNotNull(bar.Bytes);
			s.Disconnect();

			s.Reconnect();
			tx = s.BeginTransaction();
			Assert.IsNotNull(bar.Bytes);

			Assert.AreEqual(LockMode.None, s.GetCurrentLockMode(b));
			Assert.IsNotNull(bar.Bytes);
			s.CreateQuery("from Foo foo").List();
			Assert.IsNotNull(bar.Bytes);
			Assert.AreEqual(LockMode.None, s.GetCurrentLockMode(b));
			q = s.CreateQuery("from Foo foo");
			q.SetLockMode("foo", LockMode.Read);
			q.List();

			Assert.AreEqual(LockMode.Read, s.GetCurrentLockMode(b));
			s.Evict(baz);

			tx.Commit();
			s.Disconnect();

			s.Reconnect();
			tx = s.BeginTransaction();

			Assert.AreEqual(LockMode.None, s.GetCurrentLockMode(b));
			s.Delete(s.Load(typeof(Baz), baz.Code));
			Assert.AreEqual(LockMode.None, s.GetCurrentLockMode(b));

			tx.Commit();
			s.Close();

			s = OpenSession();
			tx = s.BeginTransaction();
			q = s.CreateQuery("from Foo foo, Bar bar, Bar bar2");
			q.SetLockMode("bar", LockMode.Upgrade);
			q.SetLockMode("bar2", LockMode.Read);
			result = (object[]) q.List()[0];

			Assert.IsTrue(s.GetCurrentLockMode(result[0]) == LockMode.Upgrade &&
			              s.GetCurrentLockMode(result[1]) == LockMode.Upgrade);
			s.Delete(result[0]);
			tx.Commit();
			s.Close();
		}


		[Test]
		public void ManyToManyBag()
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			object id = s.Save(baz);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz) s.Load(typeof(Baz), id);
			baz.FooBag.Add(new Foo());
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz) s.Load(typeof(Baz), id);
			Assert.IsFalse(NHibernateUtil.IsInitialized(baz.FooBag));
			Assert.AreEqual(1, baz.FooBag.Count);

			Assert.IsTrue(NHibernateUtil.IsInitialized(baz.FooBag[0]));
			s.Delete(baz);
			s.Flush();
			s.Close();
		}


		[Test]
		public void IdBag()
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			s.Save(baz);

			IList<Foo> l = new List<Foo>();
			IList<byte[]> l2 = new List<byte[]>();

			baz.IdFooBag = l;
			baz.ByteBag = l2;

			l.Add(new Foo());
			l.Add(new Bar());

			byte[] bytes = GetBytes("ffo");
			l2.Add(bytes);
			l2.Add(GetBytes("foo"));
			s.Flush();

			l.Add(new Foo());
			l.Add(new Bar());
			l2.Add(GetBytes("bar"));
			s.Flush();

			object removedObject = l[3];
			l.RemoveAt(3);
			s.Delete(removedObject);

			bytes[1] = Convert.ToByte('o');
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz) s.Load(typeof(Baz), baz.Code);
			Assert.AreEqual(3, baz.IdFooBag.Count);
			Assert.AreEqual(3, baz.ByteBag.Count);
			bytes = GetBytes("foobar");

			foreach (object obj in baz.IdFooBag)
			{
				s.Delete(obj);
			}
			baz.IdFooBag = null;
			baz.ByteBag.Add(bytes);
			baz.ByteBag.Add(bytes);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz) s.Load(typeof(Baz), baz.Code);
			Assert.AreEqual(0, baz.IdFooBag.Count);
			Assert.AreEqual(5, baz.ByteBag.Count);
			s.Delete(baz);
			s.Flush();
			s.Close();
		}


		[Test]
		public void ForceOuterJoin()
		{
			if (sessions.Settings.IsOuterJoinFetchEnabled == false)
			{
				// don't bother to run the test if we can't test it
				return;
			}

			ISession s = OpenSession();
			Glarch g = new Glarch();
			FooComponent fc = new FooComponent();
			fc.Glarch = g;
			FooProxy f = new Foo();
			FooProxy f2 = new Foo();
			f.Component = fc;
			f.TheFoo = f2;
			s.Save(f2);
			object id = s.Save(f);
			object gid = s.GetIdentifier(f.Component.Glarch);
			s.Flush();
			s.Close();

			s = OpenSession();
			f = (FooProxy) s.Load(typeof(Foo), id);
			Assert.IsFalse(NHibernateUtil.IsInitialized(f));
			Assert.IsTrue(NHibernateUtil.IsInitialized(f.Component.Glarch)); //outer-join="true"
			Assert.IsFalse(NHibernateUtil.IsInitialized(f.TheFoo)); //outer-join="auto"
			Assert.AreEqual(gid, s.GetIdentifier(f.Component.Glarch));
			s.Delete(f);
			s.Delete(f.TheFoo);
			s.Delete(f.Component.Glarch);
			s.Flush();
			s.Close();
		}


		[Test]
		public void EmptyCollection()
		{
			ISession s = OpenSession();
			object id = s.Save(new Baz());
			s.Flush();
			s.Close();
			s = OpenSession();
			Baz baz = (Baz) s.Load(typeof(Baz), id);
			Assert.IsTrue(baz.FooSet.Count == 0);
			Foo foo = new Foo();
			baz.FooSet.Add(foo);
			s.Save(foo);
			s.Flush();
			s.Delete(foo);
			s.Delete(baz);
			s.Flush();
			s.Close();
		}


		[Test]
		public void OneToOneGenerator()
		{
			ISession s = OpenSession();
			X x = new X();
			Y y = new Y();
			x.Y = y;
			y.TheX = x;

			object yId = s.Save(y);
			object xId = s.Save(x);

			Assert.AreEqual(yId, xId);
			s.Flush();

			Assert.IsTrue(s.Contains(y) && s.Contains(x));
			s.Close();

			Assert.AreEqual(x.Id, y.Id);


			s = OpenSession();
			x = new X();
			y = new Y();

			x.Y = y;
			y.TheX = x;

			s.Save(y);
			s.Flush();

			Assert.IsTrue(s.Contains(y) && s.Contains(x));
			s.Close();

			Assert.AreEqual(x.Id, y.Id);


			s = OpenSession();
			x = new X();
			y = new Y();
			x.Y = y;
			y.TheX = x;
			xId = s.Save(x);

			Assert.AreEqual(xId, y.Id);
			Assert.AreEqual(xId, x.Id);
			s.Flush();

			Assert.IsTrue(s.Contains(y) && s.Contains(x));
			s.Delete("from X x");
			s.Flush();
			s.Close();

			// check to see if Y can exist without a X
			y = new Y();
			s = OpenSession();
			s.Save(y);
			s.Flush();
			s.Close();

			s = OpenSession();
			y = (Y) s.Load(typeof(Y), y.Id);
			Assert.IsNull(y.X, "y does not need an X");
			s.Delete(y);
			s.Flush();
			s.Close();
		}


		[Test]
		public void Limit()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();
			for (int i = 0; i < 10; i++)
			{
				s.Save(new Foo());
			}

			IEnumerable enumerable = s.CreateQuery("from Foo foo")
				.SetMaxResults(4)
				.SetFirstResult(2)
				.Enumerable();

			int count = 0;
			IEnumerator e = enumerable.GetEnumerator();
			while (e.MoveNext())
			{
				object temp = e.Current;
				count++;
			}

			Assert.AreEqual(4, count);
			Assert.AreEqual(10, s.Delete("from Foo foo"));
			txn.Commit();
			s.Close();
		}


		[Test]
		public void Custom()
		{
			GlarchProxy g = new Glarch();
			Multiplicity m = new Multiplicity();
			m.count = 12;
			m.glarch = (Glarch) g;
			g.Multiple = m;

			ISession s = OpenSession();
			object gid = s.Save(g);
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (Glarch) s.CreateQuery("from Glarch g where g.Multiple.glarch=g and g.Multiple.count=12").List()[0];
			Assert.IsNotNull(g.Multiple);
			Assert.AreEqual(12, g.Multiple.count);
			Assert.AreSame(g, g.Multiple.glarch);
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy) s.Load(typeof(Glarch), gid);
			Assert.IsNotNull(g.Multiple);
			Assert.AreEqual(12, g.Multiple.count);
			Assert.AreSame(g, g.Multiple.glarch);
			s.Delete(g);
			s.Flush();
			s.Close();
		}


		[Test]
		public void SaveAddDelete()
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			baz.CascadingBars = new HashSet<BarProxy>();
			s.Save(baz);
			s.Flush();

			baz.CascadingBars.Add(new Bar());
			s.Delete(baz);
			s.Flush();
			s.Close();
		}


		[Test]
		public void NamedParams()
		{
			Bar bar = new Bar();
			Bar bar2 = new Bar();
			bar.Name = "Bar";
			bar2.Name = "Bar Two";
			Baz baz = new Baz();
			baz.CascadingBars = new HashSet<BarProxy> {bar};
			bar.Baz = baz;

			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();
			s.Save(baz);
			s.Save(bar2);

			IList list =
				s.CreateQuery("from Bar bar left join bar.Baz baz left join baz.CascadingBars b where bar.Name like 'Bar %'").List();
			object row = list[0];
			Assert.IsTrue(row is object[] && ((object[]) row).Length == 3);

			IQuery q = s.CreateQuery(
				"select bar, b " +
				"from Bar bar " +
				"left join bar.Baz baz left join baz.CascadingBars b " +
				"where (bar.Name in (:nameList) or bar.Name in (:nameList)) and bar.String = :stringVal");

			var nameList = new List<string> {"bar", "Bar", "Bar Two"};
			q.SetParameterList("nameList", nameList);
			q.SetParameter("stringVal", "a string");
			list = q.List();
			// a check for SAPDialect here
			Assert.AreEqual(2, list.Count);

			q = s.CreateQuery(
				"select bar, b " +
				"from Bar bar " +
				"inner join bar.Baz baz inner join baz.CascadingBars b " +
				"where bar.Name like 'Bar%'");
			list = q.List();
			Assert.AreEqual(1, list.Count);

			q = s.CreateQuery(
				"select bar, b " +
				"from Bar bar " +
				"left join bar.Baz baz left join baz.CascadingBars b " +
				"where bar.Name like :name and b.Name like :name");

			// add a check for HSQLDialect
			q.SetString("name", "Bar%");
			list = q.List();
			Assert.AreEqual(1, list.Count);

			s.Delete(baz);
			s.Delete(bar2);
			txn.Commit();
			s.Close();
		}

		[Test]
		public void VerifyParameterNamedMissing()
		{
			using (ISession s = OpenSession())
			{
				IQuery q = s.CreateQuery("select bar from Bar as bar where bar.X > :myX");
				Assert.Throws<QueryException>(() =>q.List());
			}
		}

		[Test]
		public void VerifyParameterPositionalMissing()
		{
			using (ISession s = OpenSession())
			{
				IQuery q = s.CreateQuery("select bar from Bar as bar where bar.X > ?");
				Assert.Throws<QueryException>(() =>q.List());
			}
		}

		[Test]
		public void VerifyParameterPositionalInQuotes()
		{
			using (ISession s = OpenSession())
			{
				IQuery q = s.CreateQuery("select bar from Bar as bar where bar.X > ? or bar.Short = 1 or bar.String = 'ff ? bb'");
				q.SetInt32(0, 1);
				q.List();
			}
		}

		[Test]
		public void VerifyParameterPositionalInQuotes2()
		{
			using (ISession s = OpenSession())
			{
				IQuery q = s.CreateQuery("select bar from Bar as bar where bar.String = ' ? ' or bar.String = '?'");
				q.List();
			}
		}

		[Test]
		public void VerifyParameterPositionalMissing2()
		{
			using (ISession s = OpenSession())
			{
				IQuery q = s.CreateQuery("select bar from Bar as bar where bar.String = ? or bar.String = ? or bar.String = ?");
				q.SetParameter(0, "bull");
				q.SetParameter(2, "shit");
				Assert.Throws<QueryException>(() =>q.List());
			}
		}

		[Test]
		public void Dyna()
		{
			ISession s = OpenSession();
			GlarchProxy g = new Glarch();
			g.Name = "G";
			object id = s.Save(g);
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy) s.Load(typeof(Glarch), id);
			Assert.AreEqual("G", g.Name);
			Assert.AreEqual("foo", g.DynaBean["foo"]);
			Assert.AreEqual(66, g.DynaBean["bar"]);

			Assert.IsFalse(g is Glarch);
			g.DynaBean["foo"] = "bar";
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy) s.Load(typeof(Glarch), id);
			Assert.AreEqual("bar", g.DynaBean["foo"]);
			Assert.AreEqual(66, g.DynaBean["bar"]);
			g.DynaBean = null;
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy) s.Load(typeof(Glarch), id);
			Assert.IsNull(g.DynaBean);
			s.Delete(g);
			s.Flush();
			s.Close();
		}

		[Test]
		public void FindByCriteria()
		{
			ISession s = OpenSession();
			Foo f = new Foo();
			s.Save(f);
			s.Flush();

			IList list = s.CreateCriteria(typeof(Foo))
				.Add(Expression.Eq("Integer", f.Integer))
				.Add(Expression.EqProperty("Integer", "Integer"))
				.Add(Expression.Like("String", f.String))
				.Add(Expression.In("Boolean", new bool[] {f.Boolean, f.Boolean}))
				.SetFetchMode("TheFoo", FetchMode.Eager)
				.SetFetchMode("Baz", FetchMode.Lazy)
				.List();

			Assert.IsTrue(list.Count == 1 && list[0] == f);

			list = s.CreateCriteria(typeof(Foo)).Add(
				Expression.Disjunction()
					.Add(Expression.Eq("Integer", f.Integer))
					.Add(Expression.Like("String", f.String))
					.Add(Expression.Eq("Boolean", f.Boolean))
				)
				.Add(Expression.IsNotNull("Boolean"))
				.List();

			Assert.IsTrue(list.Count == 1 && list[0] == f);

			ICriterion andExpression;
			ICriterion orExpression;

			andExpression =
				Expression.And(Expression.Eq("Integer", f.Integer),
				                          Expression.Like("String", f.String));
			orExpression = Expression.Or(andExpression, Expression.Eq("Boolean", f.Boolean));

			list = s.CreateCriteria(typeof(Foo))
				.Add(orExpression)
				.List();

			Assert.IsTrue(list.Count == 1 && list[0] == f);


			list = s.CreateCriteria(typeof(Foo))
				.SetMaxResults(5)
				.AddOrder(Order.Asc("Date"))
				.List();

			Assert.IsTrue(list.Count == 1 && list[0] == f);

			list = s.CreateCriteria(typeof(Foo)).SetMaxResults(0).List();
			Assert.AreEqual(0, list.Count);

			list = s.CreateCriteria(typeof(Foo))
				.SetFirstResult(1)
				.AddOrder(Order.Asc("Date"))
				.AddOrder(Order.Desc("String"))
				.List();

			Assert.AreEqual(0, list.Count);

			f.TheFoo = new Foo();
			s.Save(f.TheFoo);
			s.Flush();
			s.Close();

			s = OpenSession();
			list = s.CreateCriteria(typeof(Foo))
				.Add(Expression.Eq("Integer", f.Integer))
				.Add(Expression.Like("String", f.String))
				.Add(Expression.In("Boolean", new bool[] {f.Boolean, f.Boolean}))
				.Add(Expression.IsNotNull("TheFoo"))
				.SetFetchMode("TheFoo", FetchMode.Eager)
				.SetFetchMode("Baz", FetchMode.Lazy)
				.SetFetchMode("Component.Glarch", FetchMode.Lazy)
				.SetFetchMode("TheFoo.Baz", FetchMode.Lazy)
				.SetFetchMode("TheFoo.Component.Glarch", FetchMode.Lazy)
				.List();

			f = (Foo) list[0];
			Assert.IsTrue(NHibernateUtil.IsInitialized(f.TheFoo));

			Assert.IsFalse(NHibernateUtil.IsInitialized(f.Component.Glarch));

			s.Delete(f.TheFoo);
			s.Delete(f);
			s.Flush();
			s.Close();
		}


		[Test]
		public void AfterDelete()
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			s.Flush();
			s.Delete(foo);
			s.Save(foo);
			s.Delete(foo);
			s.Flush();
			s.Close();
		}


		[Test]
		public void CollectionWhere()
		{
			Foo foo1 = new Foo();
			Foo foo2 = new Foo();
			Baz baz = new Baz();
			Foo[] arr = new Foo[10];
			arr[0] = foo1;
			arr[9] = foo2;

			ISession s = OpenSession();
			s.Save(foo1);
			s.Save(foo2);
			baz.FooArray = arr;
			s.Save(baz);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz) s.Load(typeof(Baz), baz.Code);
			Assert.AreEqual(1, baz.FooArray.Length);
			Assert.AreEqual(1, s.CreateQuery("from Baz baz join baz.FooArray foo").List().Count);
			Assert.AreEqual(2, s.CreateQuery("from Foo foo").List().Count);
			Assert.AreEqual(1, s.CreateFilter(baz.FooArray, "").List().Count);

			s.Delete("from Foo foo");
			s.Delete(baz);

			IDbCommand deleteCmd = s.Connection.CreateCommand();
			deleteCmd.CommandText = "delete from FooArray where id_='" + baz.Code + "' and i>=8";
			deleteCmd.CommandType = CommandType.Text;
			int rows = deleteCmd.ExecuteNonQuery();
			Assert.AreEqual(1, rows);

			s.Flush();
			s.Close();
		}


		[Test]
		public void ComponentParent()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			BarProxy bar = new Bar();
			bar.Component = new FooComponent();
			Baz baz = new Baz();
			baz.Components = new FooComponent[] {new FooComponent(), new FooComponent()};
			s.Save(bar);
			s.Save(baz);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			bar = (BarProxy) s.Load(typeof(Bar), bar.Key);
			s.Load(baz, baz.Code);

			Assert.AreEqual(bar, bar.BarComponent.Parent);
			Assert.IsTrue(baz.Components[0].Baz == baz && baz.Components[1].Baz == baz);

			s.Delete(baz);
			s.Delete(bar);

			t.Commit();
			s.Close();
		}


		[Test]
		public void CollectionCache()
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			baz.SetDefaults();
			s.Save(baz);
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Load(typeof(Baz), baz.Code);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz) s.Load(typeof(Baz), baz.Code);
			s.Delete(baz);
			s.Flush();
			s.Close();
		}


		[Test]
		//[Ignore("TimeZone Portions commented out - http://jira.nhibernate.org:8080/browse/NH-88")]
		public void AssociationId()
		{
			string id;
			Bar bar;
			MoreStuff more;

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					bar = new Bar();
					id = (string) s.Save(bar);
					more = new MoreStuff();
					more.Name = "More Stuff";
					more.IntId = 12;
					more.StringId = "id";
					Stuff stuf = new Stuff();
					stuf.MoreStuff = more;
					more.Stuffs = new List<Stuff> {stuf};
					stuf.Foo = bar;
					stuf.Id = 1234;

					s.Save(more);
					t.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					//The special property (lowercase) id may be used to reference the unique identifier of an object. (You may also use its property name.) 
					string hqlString =
						"from s in class Stuff where s.Foo.id = ? and s.id.Id = ? and s.MoreStuff.id.IntId = ? and s.MoreStuff.id.StringId = ?";
					object[] values = new object[] {bar, (long) 1234, 12, "id"};
					IType[] types = new IType[]
						{
							NHibernateUtil.Entity(typeof(Foo)),
							NHibernateUtil.Int64,
							NHibernateUtil.Int32,
							NHibernateUtil.String
						};


					//IList results = s.List( hqlString, values, types );
					IQuery q = s.CreateQuery(hqlString);
					for (int i = 0; i < values.Length; i++)
					{
						q.SetParameter(i, values[i], types[i]);
					}
					IList results = q.List();
					Assert.AreEqual(1, results.Count);

					hqlString = "from s in class Stuff where s.Foo.id = ? and s.id.Id = ? and s.MoreStuff.Name = ?";
					values = new object[] {bar, (long) 1234, "More Stuff"};
					types = new IType[]
						{
							NHibernateUtil.Entity(typeof(Foo)),
							NHibernateUtil.Int64,
							NHibernateUtil.String
						};

					q = s.CreateQuery(hqlString);
					for (int i = 0; i < values.Length; i++)
					{
						q.SetParameter(i, values[i], types[i]);
					}
					results = q.List();
					Assert.AreEqual(1, results.Count);


					hqlString = "from s in class Stuff where s.Foo.String is not null";
					s.CreateQuery(hqlString).List();

					hqlString = "from s in class Stuff where s.Foo > '0' order by s.Foo";
					results = s.CreateQuery(hqlString).List();
					Assert.AreEqual(1, results.Count);

					t.Commit();
				}
			}

			FooProxy foo;

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					foo = (FooProxy) s.Load(typeof(Foo), id);
					s.Load(more, more);
					t.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					Stuff stuff = new Stuff();
					stuff.Foo = foo;
					stuff.Id = 1234;
					stuff.MoreStuff = more;
					s.Load(stuff, stuff);

					Assert.AreEqual("More Stuff", stuff.MoreStuff.Name);
					s.Delete("from ms in class MoreStuff");
					s.Delete("from foo in class Foo");

					t.Commit();
				}
			}
		}


		[Test]
		public void CascadeSave()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Baz baz = new Baz();
			IList<Fee> list = new List<Fee>();
			list.Add(new Fee());
			list.Add(new Fee());
			baz.Fees = list;
			s.Save(baz);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			baz = (Baz) s.Load(typeof(Baz), baz.Code);
			Assert.AreEqual(2, baz.Fees.Count);
			s.Delete(baz);

			Assert.IsTrue(IsEmpty(s.CreateQuery("from fee in class Fee").Enumerable()));
			t.Commit();
			s.Close();
		}


		[Test]
		public void CompositeKeyPathExpressions()
		{
			ISession s = OpenSession();

			string hql = "select fum1.Fo from fum1 in class Fum where fum1.Fo.FumString is not null";
			s.CreateQuery(hql).List();

			hql = "from fum1 in class Fum where fum1.Fo.FumString is not null order by fum1.Fo.FumString";
			s.CreateQuery(hql).List();

			if (Dialect.SupportsSubSelects)
			{
				hql = "from fum1 in class Fum where size(fum1.Friends) = 0";
				s.CreateQuery(hql).List();

				hql = "from fum1 in class Fum where exists elements (fum1.Friends)";
				s.CreateQuery(hql).List();
			}

			hql = "select elements(fum1.Friends) from fum1 in class Fum";

			s.CreateQuery(hql).List();

			hql = "from fum1 in class Fum, fr in elements( fum1.Friends )";
			s.CreateQuery(hql).List();

			s.Close();
		}


		[Test]
		public void CollectionsInSelect()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Foo[] foos = new Foo[] {null, new Foo()};
			s.Save(foos[1]);
			Baz baz = new Baz();
			baz.SetDefaults();
			baz.FooArray = foos;
			s.Save(baz);
			Baz baz2 = new Baz();
			baz2.SetDefaults();
			s.Save(baz2);

			Bar bar = new Bar();
			bar.Baz = baz;
			s.Save(bar);

			IList list = s.CreateQuery("select new Result(foo.String, foo.Long, foo.Integer) from foo in class Foo").List();
			Assert.AreEqual(2, list.Count);
			Assert.IsTrue(list[0] is Result);
			Assert.IsTrue(list[1] is Result);

			list =
				s.CreateQuery(
					"select new Result( baz.Name, foo.Long, count(elements(baz.FooArray)) ) from Baz baz join baz.FooArray foo group by baz.Name, foo.Long")
					.List();
			Assert.AreEqual(1, list.Count);
			Assert.IsTrue(list[0] is Result);
			Result r = (Result) list[0];

			Assert.AreEqual(baz.Name, r.Name);
			Assert.AreEqual(1, r.Count);
			Assert.AreEqual(foos[1].Long, r.Amount);


			list =
				s.CreateQuery(
					"select new Result( baz.Name, max(foo.Long), count(foo) ) from Baz baz join baz.FooArray foo group by baz.Name").
					List();
			Assert.AreEqual(1, list.Count);
			Assert.IsTrue(list[0] is Result);
			r = (Result) list[0];
			Assert.AreEqual(baz.Name, r.Name);
			Assert.AreEqual(1, r.Count);

			s.CreateQuery("select max( elements(bar.Baz.FooArray) ) from Bar as bar").List();
			// the following test is disable for databases with no subselects... also for Interbase (not sure why) - comment from h2.0.3
			if (Dialect.SupportsSubSelects)
			{
				s.CreateQuery("select count(*) from Baz as baz where 1 in indices(baz.FooArray)").List();
				s.CreateQuery("select count(*) from Bar as bar where 'abc' in elements(bar.Baz.FooArray)").List();
				s.CreateQuery("select count(*) from Bar as bar where 1 in indices(bar.Baz.FooArray)").List();
				s.CreateQuery(
					"select count(*) from Bar as bar where '1' in (from bar.Component.Glarch.ProxyArray g where g.Name='foo')").List();
				
				// The nex query is wrong and is not present in H3.2:
				// The SQL result, from Classic parser, is the same of the previous query.
				// The AST parser has some problem to parse 'from g in bar.Component.Glarch.ProxyArray'
				// which should be parsed as 'from bar.Component.Glarch.ProxyArray g'
				//s.CreateQuery(
				//  "select count(*) from Bar as bar where '1' in (from g in bar.Component.Glarch.ProxyArray.elements where g.Name='foo')")
				//  .List();

				// TODO: figure out why this is throwing an ORA-1722 error
				// probably the conversion ProxyArray.id (to_number ensuring a not null value)
				if (!(Dialect is Oracle8iDialect))
				{
					s.CreateQuery(
						"select count(*) from Bar as bar join bar.Component.Glarch.ProxyArray as g where cast(g.id as Int32) in indices(bar.Baz.FooArray)").
						List();
					s.CreateQuery(
						"select max( elements(bar.Baz.FooArray) ) from Bar as bar join bar.Component.Glarch.ProxyArray as g where cast(g.id as Int32) in indices(bar.Baz.FooArray)")
						.List();
					s.CreateQuery(
						"select count(*) from Bar as bar left outer join bar.Component.Glarch.ProxyArray as pg where '1' in (from g in bar.Component.Glarch.ProxyArray)")
						.List();
				}
			}

			list =
				s.CreateQuery("from Baz baz left join baz.FooToGlarch join fetch baz.FooArray foo left join fetch foo.TheFoo").List();
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual(2, ((object[]) list[0]).Length);

			list =
				s.CreateQuery(
					"select baz.Name from Bar bar inner join bar.Baz baz inner join baz.FooSet foo where baz.Name = bar.String").List();
			s.CreateQuery(
				"SELECT baz.Name FROM Bar AS bar INNER JOIN bar.Baz AS baz INNER JOIN baz.FooSet AS foo WHERE baz.Name = bar.String")
				.List();

			s.CreateQuery(
				"select baz.Name from Bar bar join bar.Baz baz left outer join baz.FooSet foo where baz.Name = bar.String").List();

			s.CreateQuery("select baz.Name from Bar bar join bar.Baz baz join baz.FooSet foo where baz.Name = bar.String").List();
			s.CreateQuery("SELECT baz.Name FROM Bar AS bar join bar.Baz AS baz join baz.FooSet AS foo WHERE baz.Name = bar.String").List();

			s.CreateQuery(
				"select baz.Name from Bar bar left join bar.Baz baz left join baz.FooSet foo where baz.Name = bar.String").List();
			s.CreateQuery("select foo.String from Bar bar left join bar.Baz.FooSet foo where bar.String = foo.String").List();

			s.CreateQuery(
				"select baz.Name from Bar bar left join bar.Baz baz left join baz.FooArray foo where baz.Name = bar.String").List();
			s.CreateQuery("select foo.String from Bar bar left join bar.Baz.FooArray foo where bar.String = foo.String").List();

			s.CreateQuery(
				"select bar.String, foo.String from bar in class Bar inner join bar.Baz as baz inner join elements(baz.FooSet) as foo where baz.Name = 'name'")
				.List();
			s.CreateQuery("select foo from bar in class Bar inner join bar.Baz as baz inner join baz.FooSet as foo").List();
			s.CreateQuery("select foo from bar in class Bar inner join bar.Baz.FooSet as foo").List();

			s.CreateQuery(
				"select bar.String, foo.String from bar in class Bar join bar.Baz as baz, elements(baz.FooSet) as foo where baz.Name = 'name'")
				.List();
			s.CreateQuery("select foo from bar in class Bar join bar.Baz as baz join baz.FooSet as foo").List();
			s.CreateQuery("select foo from bar in class Bar join bar.Baz.FooSet as foo").List();

			Assert.AreEqual(1, s.CreateQuery("from Bar bar join bar.Baz.FooArray foo").List().Count);

			Assert.AreEqual(0, s.CreateQuery("from bar in class Bar, foo in elements(bar.Baz.FooSet)").List().Count);

			Assert.AreEqual(1, s.CreateQuery("from bar in class Bar, foo in elements( bar.Baz.FooArray )").List().Count);

			s.Delete(bar);

			s.Delete(baz);
			s.Delete(baz2);
			s.Delete(foos[1]);
			t.Commit();
			s.Close();
		}


		[Test]
		public void NewFlushing()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();
			Baz baz = new Baz();
			baz.SetDefaults();
			s.Save(baz);
			s.Flush();

			baz.StringArray[0] = "a new value";
			IEnumerator enumer = s.CreateQuery("from baz in class Baz").Enumerable().GetEnumerator(); // no flush
			Assert.IsTrue(enumer.MoveNext());
			Assert.AreSame(baz, enumer.Current);

			enumer = s.CreateQuery("select elements(baz.StringArray) from baz in class Baz").Enumerable().GetEnumerator();

			bool found = false;
			while (enumer.MoveNext())
			{
				if (enumer.Current.Equals("a new value"))
				{
					found = true;
				}
			}
			Assert.IsTrue(found);

			baz.StringArray = null;
			s.CreateQuery("from baz in class Baz").Enumerable(); // no flush

			enumer = s.CreateQuery("select elements(baz.StringArray) from baz in class Baz").Enumerable().GetEnumerator();

			Assert.IsFalse(enumer.MoveNext());

			baz.StringList.Add("1E1");
			enumer = s.CreateQuery("from foo in class Foo").Enumerable().GetEnumerator(); // no flush
			Assert.IsFalse(enumer.MoveNext());

			enumer = s.CreateQuery("select elements(baz.StringList) from baz in class Baz").Enumerable().GetEnumerator();

			found = false;
			while (enumer.MoveNext())
			{
				if (enumer.Current.Equals("1E1"))
				{
					found = true;
				}
			}
			Assert.IsTrue(found);

			baz.StringList.Remove("1E1");
			s.CreateQuery("select elements(baz.StringArray) from baz in class Baz").Enumerable(); //no flush

			enumer = s.CreateQuery("select elements(baz.StringList) from baz in class Baz").Enumerable().GetEnumerator();

			found = false;
			while (enumer.MoveNext())
			{
				if (enumer.Current.Equals("1E1"))
				{
					found = true;
				}
			}
			Assert.IsFalse(found);

			IList<string> newList = new List<string>();
			newList.Add("value");
			baz.StringList = newList;
			
			s.CreateQuery("from foo in class Foo").Enumerable().GetEnumerator(); //no flush
			
			baz.StringList = null;

			enumer = s.CreateQuery("select elements(baz.StringList) from baz in class Baz").Enumerable().GetEnumerator();

			Assert.IsFalse(enumer.MoveNext());

			s.Delete(baz);
			txn.Commit();
			s.Close();
		}


		[Test]
		public void PersistCollections()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();
			IEnumerator enumer = s.CreateQuery("select count(*) from b in class Bar").Enumerable().GetEnumerator();
			enumer.MoveNext();
			Assert.AreEqual(0L, enumer.Current);

			Baz baz = new Baz();
			s.Save(baz);
			baz.SetDefaults();
			baz.StringArray = new string[] {"stuff"};
			baz.CascadingBars = new HashSet<BarProxy> {new Bar()};
			IDictionary<string, Glarch> sgm = new Dictionary<string, Glarch>();
			sgm["a"] = new Glarch();
			sgm["b"] = new Glarch();
			baz.StringGlarchMap = sgm;
			txn.Commit();
			s.Close();

			s = OpenSession();
			txn = s.BeginTransaction();
			baz = (Baz) ((object[]) s.CreateQuery("select baz, baz from baz in class NHibernate.DomainModel.Baz").List()[0])[1];
			Assert.AreEqual(1, baz.CascadingBars.Count, "baz.CascadingBars.Count");
			Foo foo = new Foo();
			s.Save(foo);
			Foo foo2 = new Foo();
			s.Save(foo2);
			baz.FooArray = new Foo[] {foo, foo, null, foo2};
			baz.FooSet.Add(foo);
			baz.Customs.Add(new string[] {"new", "custom"});
			baz.StringArray = null;
			baz.StringList[0] = "new value";
			baz.StringSet = new HashSet<string>();

			// NOTE: We put two items in the map, but expect only one to come back, because
			// of where="..." specified in the mapping for StringGlarchMap
			Assert.AreEqual(1, baz.StringGlarchMap.Count, "baz.StringGlarchMap.Count");
			IList list;

			// disable this for dbs with no subselects
			if (Dialect.SupportsSubSelects && TestDialect.SupportsOperatorAll)
			{
				list = s.CreateQuery(
							"select foo from foo in class NHibernate.DomainModel.Foo, baz in class NHibernate.DomainModel.Baz where foo in elements(baz.FooArray) and 3 = some elements(baz.IntArray) and 4 > all indices(baz.IntArray)")
							.List();
				

				Assert.AreEqual(2, list.Count, "collection.elements find");
			}

			// sapdb doesn't like distinct with binary type
			//if( !(dialect is Dialect.SAPDBDialect) ) 
			//{
			list = s.CreateQuery("select distinct foo from baz in class NHibernate.DomainModel.Baz, foo in elements(baz.FooArray)").
						List();
			
			Assert.AreEqual(2, list.Count, "collection.elements find");
			//}

			list = s.CreateQuery("select foo from baz in class NHibernate.DomainModel.Baz, foo in elements(baz.FooSet)").List();

			Assert.AreEqual(1, list.Count, "association.elements find");

			txn.Commit();
			s.Close();

			s = OpenSession();
			txn = s.BeginTransaction();
			baz = (Baz)s.CreateQuery("select baz from baz in class NHibernate.DomainModel.Baz order by baz").List()[0];
			Assert.AreEqual(4, baz.Customs.Count, "collection of custom types - added element");
			Assert.IsNotNull(baz.Customs[0], "collection of custom types - added element");
			Assert.IsNotNull(baz.Components[1].Subcomponent, "component of component in collection");
			Assert.AreSame(baz, baz.Components[1].Baz);

			IEnumerator fooSetEnumer = baz.FooSet.GetEnumerator();
			fooSetEnumer.MoveNext();
			Assert.IsTrue(((FooProxy) fooSetEnumer.Current).Key.Equals(foo.Key), "set of objects");
			Assert.AreEqual(0, baz.StringArray.Length, "collection removed");
			Assert.AreEqual("new value", baz.StringList[0], "changed element");
			Assert.AreEqual(0, baz.StringSet.Count, "replaced set");

			baz.StringSet.Add("two");
			baz.StringSet.Add("one");
			baz.Bag.Add("three");
			txn.Commit();
			s.Close();

			s = OpenSession();
			txn = s.BeginTransaction();
			baz = (Baz)s.CreateQuery("select baz from baz in class NHibernate.DomainModel.Baz order by baz").List()[0];
			Assert.AreEqual(2, baz.StringSet.Count);
			int index = 0;
			foreach (string key in baz.StringSet)
			{
				// h2.0.3 doesn't have this because the Set has a first() and last() method
				index++;
				if (index == 1)
				{
					Assert.AreEqual("one", key);
				}
				if (index == 2)
				{
					Assert.AreEqual("two", key);
				}
				if (index > 2)
				{
					Assert.Fail("should not be more than 2 items in StringSet");
				}
			}
			Assert.AreEqual(5, baz.Bag.Count);
			baz.StringSet.Remove("two");
			baz.Bag.Remove("duplicate");
			txn.Commit();
			s.Close();

			s = OpenSession();
			txn = s.BeginTransaction();
			baz = (Baz)s.Load(typeof(Baz), baz.Code);
			Bar bar = new Bar();
			Bar bar2 = new Bar();
			s.Save(bar);
			s.Save(bar2);
			baz.TopFoos = new HashSet<Bar> { bar, bar2 };
			baz.TopGlarchez = new Dictionary<char, GlarchProxy>();
			GlarchProxy g = new Glarch();
			s.Save(g);
			baz.TopGlarchez['G'] = g;
			
			var map = new Dictionary<Foo, GlarchProxy>();
			map[bar] = g;
			map[bar2] = g;
			baz.FooToGlarch = map;
			
			var map2 = new Dictionary<FooComponent, Foo>();
			map2[new FooComponent("name", 123, null, null)] = bar;
			map2[new FooComponent("nameName", 12, null, null)] = bar;
			baz.FooComponentToFoo = map2;

			var map3 = new Dictionary<Foo, GlarchProxy>();
			map3[bar] = g;
			baz.GlarchToFoo = map3;
			txn.Commit();
			s.Close();

			using(s = OpenSession())
			using (txn = s.BeginTransaction())
			{
				baz = (Baz) s.CreateQuery("select baz from baz in class NHibernate.DomainModel.Baz order by baz").List()[0];
				ISession s2 = OpenSession();
				ITransaction txn2 = s2.BeginTransaction();
				baz = (Baz) s.CreateQuery("select baz from baz in class NHibernate.DomainModel.Baz order by baz").List()[0];
				object o = baz.FooComponentToFoo[new FooComponent("name", 123, null, null)];
				Assert.IsNotNull(o);
				Assert.AreEqual(o, baz.FooComponentToFoo[new FooComponent("nameName", 12, null, null)]);
				txn2.Commit();
				s2.Close();
				Assert.AreEqual(2, baz.TopFoos.Count);
				Assert.AreEqual(1, baz.TopGlarchez.Count);
				enumer = baz.TopFoos.GetEnumerator();
				Assert.IsTrue(enumer.MoveNext());
				Assert.IsNotNull(enumer.Current);
				Assert.AreEqual(1, baz.StringSet.Count);
				Assert.AreEqual(4, baz.Bag.Count);
				Assert.AreEqual(2, baz.FooToGlarch.Count);
				Assert.AreEqual(2, baz.FooComponentToFoo.Count);
				Assert.AreEqual(1, baz.GlarchToFoo.Count);

				enumer = baz.FooToGlarch.Keys.GetEnumerator();
				for (int i = 0; i < 2; i++)
				{
					enumer.MoveNext();
					Assert.IsTrue(enumer.Current is BarProxy);
				}
				enumer = baz.FooComponentToFoo.Keys.GetEnumerator();
				enumer.MoveNext();
				FooComponent fooComp = (FooComponent) enumer.Current;
				Assert.IsTrue((fooComp.Count == 123 && fooComp.Name.Equals("name"))
				              || (fooComp.Count == 12 && fooComp.Name.Equals("nameName")));
				Assert.IsTrue(baz.FooComponentToFoo[fooComp] is BarProxy);

				Glarch g2 = new Glarch();
				s.Save(g2);
				g = (GlarchProxy) baz.TopGlarchez['G'];
				baz.TopGlarchez['H'] = g;
				baz.TopGlarchez['G'] = g2;
				txn.Commit();
				s.Close();
			}

			s = OpenSession();
			txn = s.BeginTransaction();
			baz = (Baz)s.CreateQuery("select baz from baz in class NHibernate.DomainModel.Baz order by baz").List()[0];
			Assert.AreEqual(2, baz.TopGlarchez.Count);
			txn.Commit();
			s.Disconnect();

			// serialize and then deserialize the session.
			Stream stream = new MemoryStream();
			IFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, s);

			s.Close();

			stream.Position = 0;
			s = (ISession) formatter.Deserialize(stream);
			stream.Close();

			s.Reconnect();
			txn = s.BeginTransaction();
			baz = (Baz) s.Load(typeof(Baz), baz.Code);
			s.Delete(baz);
			s.Delete(baz.TopGlarchez['G']);
			s.Delete(baz.TopGlarchez['H']);

			IDbCommand cmd = s.Connection.CreateCommand();
			s.Transaction.Enlist(cmd);
			cmd.CommandText = "update " + Dialect.QuoteForTableName("glarchez") + " set baz_map_id=null where baz_map_index='a'";
			int rows = cmd.ExecuteNonQuery();
			Assert.AreEqual(1, rows);
			Assert.AreEqual(2, s.Delete("from bar in class NHibernate.DomainModel.Bar"));
			FooProxy[] arr = baz.FooArray;
			Assert.AreEqual(4, arr.Length);
			Assert.AreEqual(foo.Key, arr[1].Key);
			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i] != null)
				{
					s.Delete(arr[i]);
				}
			}

			try
			{
				s.Load(typeof(Qux), (long) 666); //nonexistent
			}
			catch (ObjectNotFoundException onfe)
			{
				Assert.IsNotNull(onfe, "should not find a Qux with id of 666 when Proxies are not implemented.");
			}

			Assert.AreEqual(1, s.Delete("from g in class Glarch"), "Delete('from g in class Glarch')");
			txn.Commit();
			s.Disconnect();

			// serialize and then deserialize the session.
			stream = new MemoryStream();
			formatter.Serialize(stream, s);

			s.Close();

			stream.Position = 0;
			s = (ISession) formatter.Deserialize(stream);
			stream.Close();

			Qux nonexistentQux = (Qux) s.Load(typeof(Qux), (long) 666); //nonexistent
			Assert.IsNotNull(nonexistentQux, "even though it doesn't exists should still get a proxy - no db hit.");

			s.Close();
		}


		[Test]
		public void SaveFlush()
		{
			ISession s = OpenSession();
			Fee fee = new Fee();
			s.Save(fee, "key");
			fee.Fi = "blah";
			s.Flush();
			s.Close();

			s = OpenSession();
			fee = (Fee) s.Load(typeof(Fee), fee.Key);
			Assert.AreEqual("blah", fee.Fi);
			Assert.AreEqual("key", fee.Key);
			s.Delete(fee);
			s.Flush();
			s.Close();
		}


		[Test]
		public void CreateUpdate()
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			foo.String = "dirty";
			s.Flush();
			s.Close();

			s = OpenSession();
			Foo foo2 = new Foo();
			s.Load(foo2, foo.Key);
			Assert.IsTrue(foo.EqualsFoo(foo2), "create-update");
			s.Delete(foo2);
			s.Flush();
			s.Close();

			s = OpenSession();
			foo = new Foo();
			s.Save(foo, "assignedid");
			foo.String = "dirty";
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Load(foo2, "assignedid");
			Assert.IsTrue(foo.EqualsFoo(foo2), "create-update");
			s.Delete(foo2);
			s.Flush();
			s.Close();
		}


		[Test]
		public void UpdateCollections()
		{
			ISession s = OpenSession();
			Holder baz = new Holder();
			baz.Name = "123";
			Foo f1 = new Foo();
			Foo f2 = new Foo();
			Foo f3 = new Foo();
			One o = new One();
			baz.Ones = new List<One>();
			baz.Ones.Add(o);
			Foo[] foos = new Foo[] {f1, null, f2};
			baz.FooArray = foos;
			// in h2.0.3 this is a Set
			baz.Foos = new HashSet<Foo> { f1 };
			s.Save(f1);
			s.Save(f2);
			s.Save(f3);
			s.Save(o);
			s.Save(baz);
			s.Flush();
			s.Close();

			baz.Ones[0] = null;
			baz.Ones.Add(o);
			baz.Foos.Add(f2);
			foos[0] = f3;
			foos[1] = f1;

			s = OpenSession();
			s.SaveOrUpdate(baz);
			s.Flush();
			s.Close();

			s = OpenSession();
			Holder h = (Holder) s.Load(typeof(Holder), baz.Id);
			Assert.IsNull(h.Ones[0]);
			Assert.IsNotNull(h.Ones[1]);
			Assert.IsNotNull(h.FooArray[0]);
			Assert.IsNotNull(h.FooArray[1]);
			Assert.IsNotNull(h.FooArray[2]);
			Assert.AreEqual(2, h.Foos.Count);

			// new to nh to make sure right items in right index
			Assert.AreEqual(f3.Key, h.FooArray[0].Key);
			Assert.AreEqual(o.Key, ((One) h.Ones[1]).Key);
			Assert.AreEqual(f1.Key, h.FooArray[1].Key);
			Assert.AreEqual(f2.Key, h.FooArray[2].Key);
			s.Close();

			baz.Foos.Remove(f1);
			baz.Foos.Remove(f2);
			baz.FooArray[0] = null;
			baz.FooArray[1] = null;
			baz.FooArray[2] = null;

			s = OpenSession();
			s.SaveOrUpdate(baz);
			s.Delete("from f in class NHibernate.DomainModel.Foo");
			baz.Ones.Remove(o);
			s.Delete("from o in class NHibernate.DomainModel.One");
			s.Delete(baz);
			s.Flush();
			s.Close();
		}


		[Test]
		public void Load()
		{
			ISession s = OpenSession();
			Qux q = new Qux();
			s.Save(q);
			BarProxy b = new Bar();
			s.Save(b);
			s.Flush();
			s.Close();

			s = OpenSession();
			q = (Qux) s.Load(typeof(Qux), q.Key);
			b = (BarProxy) s.Load(typeof(Foo), b.Key);
			string tempKey = b.Key;
			Assert.IsFalse(NHibernateUtil.IsInitialized(b), "b should have been an unitialized Proxy");
			string tempString = b.BarString;
			Assert.IsTrue(NHibernateUtil.IsInitialized(b), "b should have been an initialized Proxy");
			BarProxy b2 = (BarProxy) s.Load(typeof(Bar), tempKey);
			Qux q2 = (Qux) s.Load(typeof(Qux), q.Key);
			Assert.AreSame(q, q2, "loaded same Qux");
			Assert.AreSame(b, b2, "loaded same BarProxy");
			s.Delete(q2);
			s.Delete(b2);
			s.Flush();
			s.Close();
		}


		[Test]
		public void Create()
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			s.Flush();
			s.Close();

			s = OpenSession();
			Foo foo2 = new Foo();
			s.Load(foo2, foo.Key);

			Assert.IsTrue(foo.EqualsFoo(foo2), "create");
			s.Delete(foo2);
			s.Flush();
			s.Close();
		}


		[Test]
		public void Callback()
		{
			ISession s = OpenSession();
			Qux q = new Qux("0");
			s.Save(q);
			q.Child = (new Qux("1"));
			s.Save(q.Child);

			Qux q2 = new Qux("2");
			q2.Child = q.Child;

			Qux q3 = new Qux("3");
			q.Child.Child = q3;
			s.Save(q3);

			Qux q4 = new Qux("4");
			q4.Child = q3;

			s.Save(q4);
			s.Save(q2);
			s.Flush();
			s.Close();

			s = OpenSession();
			IList l = s.CreateQuery("from q in class NHibernate.DomainModel.Qux").List();
			Assert.AreEqual(5, l.Count);

			s.Delete(l[0]);
			s.Delete(l[1]);
			s.Delete(l[2]);
			s.Delete(l[3]);
			s.Delete(l[4]);
			s.Flush();
			s.Close();
		}


		[Test]
		public void Polymorphism()
		{
			ISession s = OpenSession();
			Bar bar = new Bar();
			s.Save(bar);
			bar.BarString = "bar bar";
			s.Flush();
			s.Close();

			s = OpenSession();
			FooProxy foo = (FooProxy) s.Load(typeof(Foo), bar.Key);
			Assert.IsTrue(foo is BarProxy, "polymorphic");
			Assert.IsTrue(((BarProxy) foo).BarString.Equals(bar.BarString), "subclass property");
			s.Delete(foo);
			s.Flush();
			s.Close();
		}


		[Test]
		public void RemoveContains()
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			baz.SetDefaults();
			s.Save(baz);
			s.Flush();

			Assert.IsTrue(s.Contains(baz));

			s.Evict(baz);
			Assert.IsFalse(s.Contains(baz), "baz should have been evicted");

			Baz baz2 = (Baz) s.Load(typeof(Baz), baz.Code);
			Assert.IsFalse(baz == baz2, "should be different objects because Baz not contained in Session");

			s.Delete(baz2);
			s.Flush();
			s.Close();
		}


		[Test]
		public void CollectionOfSelf()
		{
			ISession s = OpenSession();
			Bar bar = new Bar();
			s.Save(bar);
			// h2.0.3 was a set
			bar.Abstracts = new HashSet<object>();
			bar.Abstracts.Add(bar);
			Bar bar2 = new Bar();
			bar.Abstracts.Add(bar2);
			bar.TheFoo = bar;
			s.Save(bar2);
			s.Flush();
			s.Close();

			bar.Abstracts = null;
			s = OpenSession();
			s.Load(bar, bar.Key);

			Assert.AreEqual(2, bar.Abstracts.Count);
			Assert.IsTrue(bar.Abstracts.Contains(bar), "collection contains self");
			Assert.AreSame(bar, bar.TheFoo, "association to self");

			if (Dialect is MySQLDialect)
			{
				// Break the self-reference cycle to avoid error when deleting the row
				bar.TheFoo = null;
				s.Flush();
			}

			foreach (object obj in bar.Abstracts)
			{
				s.Delete(obj);
			}

			s.Flush();
			s.Close();
		}


		[Test]
		public void Find()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			// some code commented out in h2.0.3

			Bar bar = new Bar();
			s.Save(bar);
			bar.BarString = "bar bar";
			bar.String = "xxx";
			Foo foo = new Foo();
			s.Save(foo);
			foo.String = "foo bar";
			s.Save(new Foo());
			s.Save(new Bar());

			IList list1 =
				s.CreateQuery("select foo from foo in class NHibernate.DomainModel.Foo where foo.String='foo bar'").List();
			Assert.AreEqual(1, list1.Count, "find size");
			Assert.AreSame(foo, list1[0], "find ==");

			IList list2 = s.CreateQuery("from foo in class NHibernate.DomainModel.Foo order by foo.String, foo.Date").List();
			Assert.AreEqual(4, list2.Count, "find size");
			list1 = s.CreateQuery("from foo in class NHibernate.DomainModel.Foo where foo.class='B'").List();
			Assert.AreEqual(2, list1.Count, "class special property");
			list1 =
				s.CreateQuery("from foo in class NHibernate.DomainModel.Foo where foo.class=NHibernate.DomainModel.Bar").List();
			Assert.AreEqual(2, list1.Count, "class special property");
			list1 = s.CreateQuery("from foo in class NHibernate.DomainModel.Foo where foo.class=Bar").List();
			list2 =
				s.CreateQuery(
					"select bar from bar in class NHibernate.DomainModel.Bar, foo in class NHibernate.DomainModel.Foo where bar.String = foo.String and not bar=foo")
					.List();

			Assert.AreEqual(2, list1.Count, "class special property");
			Assert.AreEqual(1, list2.Count, "select from a subclass");

			Trivial t = new Trivial();
			s.Save(t);
			txn.Commit();
			s.Close();

			s = OpenSession();
			txn = s.BeginTransaction();
			list1 = s.CreateQuery("from foo in class NHibernate.DomainModel.Foo where foo.String='foo bar'").List();
			Assert.AreEqual(1, list1.Count, "find count");
			// There is an interbase bug that causes null integers to return as 0, also numeric precision is <=15 -h2.0.3 comment
			Assert.IsTrue(((Foo) list1[0]).EqualsFoo(foo), "find equals");
			list2 = s.CreateQuery("select foo from foo in class NHibernate.DomainModel.Foo").List();
			Assert.AreEqual(5, list2.Count, "find count");
			IList list3 = s.CreateQuery("from bar in class NHibernate.DomainModel.Bar where bar.BarString='bar bar'").List();
			Assert.AreEqual(1, list3.Count, "find count");
			Assert.IsTrue(list2.Contains(list1[0]) && list2.Contains(list2[0]), "find same instance");
			Assert.AreEqual(1, s.CreateQuery("from t in class NHibernate.DomainModel.Trivial").List().Count);
			s.Delete("from t in class NHibernate.DomainModel.Trivial");

			list2 =
				s.CreateQuery("from foo in class NHibernate.DomainModel.Foo where foo.Date = ?").SetDateTime(0,
				                                                                                             new DateTime(1970, 01,
				                                                                                                          01)).List();
			Assert.AreEqual(4, list2.Count, "find by date");
			IEnumerator enumer = list2.GetEnumerator();
			while (enumer.MoveNext())
			{
				s.Delete(enumer.Current);
			}

			list2 = s.CreateQuery("from foo in class NHibernate.DomainModel.Foo").List();
			Assert.AreEqual(0, list2.Count, "find deleted");
			txn.Commit();
			s.Close();
		}


		[Test]
		public void DeleteRecursive()
		{
			ISession s = OpenSession();
			Foo x = new Foo();
			Foo y = new Foo();
			x.TheFoo = y;
			y.TheFoo = x;
			s.Save(x);
			s.Save(y);
			s.Flush();
			s.Delete(y);
			s.Delete(x);
			s.Flush();
			s.Close();
		}


		[Test]
		public void Reachability()
		{
			// first for unkeyed collections
			ISession s = OpenSession();
			Baz baz1 = new Baz();
			s.Save(baz1);
			Baz baz2 = new Baz();
			s.Save(baz2);
			baz1.IntArray = new int[] {1, 2, 3, 4};
			baz1.FooSet = new HashSet<FooProxy>();
			Foo foo = new Foo();
			s.Save(foo);
			baz1.FooSet.Add(foo);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz2 = (Baz) s.Load(typeof(Baz), baz2.Code);
			baz1 = (Baz) s.Load(typeof(Baz), baz1.Code);
			baz2.FooSet = baz1.FooSet;
			baz1.FooSet = null;
			baz2.IntArray = baz1.IntArray;
			baz1.IntArray = null;
			s.Flush();
			s.Close();

			s = OpenSession();
			baz2 = (Baz) s.Load(typeof(Baz), baz2.Code);
			baz1 = (Baz) s.Load(typeof(Baz), baz1.Code);
			Assert.AreEqual(4, baz2.IntArray.Length, "unkeyed reachability - baz2.IntArray");
			Assert.AreEqual(1, baz2.FooSet.Count, "unkeyed reachability - baz2.FooSet");
			Assert.AreEqual(0, baz1.IntArray.Length, "unkeyed reachability - baz1.IntArray");
			Assert.AreEqual(0, baz1.FooSet.Count, "unkeyed reachability - baz1.FooSet");

			foreach (object obj in baz2.FooSet)
			{
				s.Delete((FooProxy) obj);
			}

			s.Delete(baz1);
			s.Delete(baz2);
			s.Flush();
			s.Close();

			// now for collections of collections
			s = OpenSession();
			baz1 = new Baz();
			s.Save(baz1);
			baz2 = new Baz();
			s.Save(baz2);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz2 = (Baz) s.Load(typeof(Baz), baz2.Code);
			baz1 = (Baz) s.Load(typeof(Baz), baz1.Code);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz2 = (Baz) s.Load(typeof(Baz), baz2.Code);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz2 = (Baz) s.Load(typeof(Baz), baz2.Code);
			baz1 = (Baz) s.Load(typeof(Baz), baz1.Code);
			s.Delete(baz1);
			s.Delete(baz2);
			s.Flush();
			s.Close();

			// now for keyed collections
			s = OpenSession();
			baz1 = new Baz();
			s.Save(baz1);
			baz2 = new Baz();
			s.Save(baz2);
			Foo foo1 = new Foo();
			Foo foo2 = new Foo();

			s.Save(foo1);
			s.Save(foo2);
			baz1.FooArray = new Foo[] {foo1, null, foo2};
			baz1.StringDateMap = new Dictionary<string, DateTime?>();
			baz1.StringDateMap["today"] = DateTime.Today;
			baz1.StringDateMap["foo"] = null;
			baz1.StringDateMap["tomm"] = new DateTime(DateTime.Today.Ticks + (new TimeSpan(1, 0, 0, 0, 0)).Ticks);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz2 = (Baz) s.Load(typeof(Baz), baz2.Code);
			baz1 = (Baz) s.Load(typeof(Baz), baz1.Code);
			baz2.FooArray = baz1.FooArray;
			baz1.FooArray = null;
			baz2.StringDateMap = baz1.StringDateMap;
			baz1.StringDateMap = null;
			s.Flush();
			s.Close();

			s = OpenSession();
			baz2 = (Baz) s.Load(typeof(Baz), baz2.Code);
			baz1 = (Baz) s.Load(typeof(Baz), baz1.Code);
			Assert.AreEqual(3, baz2.StringDateMap.Count, "baz2.StringDateMap count - reachability");
			Assert.AreEqual(3, baz2.FooArray.Length, "baz2.FooArray length - reachability");
			Assert.AreEqual(0, baz1.StringDateMap.Count, "baz1.StringDateMap count - reachability");
			Assert.AreEqual(0, baz1.FooArray.Length, "baz1.FooArray length - reachability");

			Assert.IsNull(baz2.FooArray[1], "null element");
			Assert.IsNotNull(baz2.StringDateMap["today"], "today non-null element");
			Assert.IsNotNull(baz2.StringDateMap["tomm"], "tomm non-null element");
			Assert.IsNull(baz2.StringDateMap["foo"], "foo is null element");

			s.Delete(baz2.FooArray[0]);
			s.Delete(baz2.FooArray[2]);
			s.Delete(baz1);
			s.Delete(baz2);
			s.Flush();
			s.Close();
		}


		[Test]
		public void PersistentLifecycle()
		{
			ISession s = OpenSession();
			Qux q = new Qux();
			s.Save(q);
			q.Stuff = "foo bar baz qux";
			s.Flush();
			s.Close();

			s = OpenSession();
			q = (Qux) s.Load(typeof(Qux), q.Key);
			Assert.IsTrue(q.Created, "lifecycle create");
			Assert.IsTrue(q.Loaded, "lifecycle load");
			Assert.IsNotNull(q.Foo, "lifecycle subobject");
			s.Delete(q);
			Assert.IsTrue(q.Deleted, "lifecyle delete");
			s.Flush();
			s.Close();

			s = OpenSession();
			Assert.AreEqual(0, s.CreateQuery("from foo in class NHibernate.DomainModel.Foo").List().Count, "subdeletion");
			s.Flush();
			s.Close();
		}


		[Test]
		public void Enumerable()
		{
			// this test used to be called Iterators()

			ISession s = OpenSession();
			for (int i = 0; i < 10; i++)
			{
				Qux q = new Qux();
				object qid = s.Save(q);
				Assert.IsNotNull(q, "q is not null");
				Assert.IsNotNull(qid, "qid is not null");
			}
			s.Flush();
			s.Close();

			s = OpenSession();
			IEnumerator enumer =
				s.CreateQuery("from q in class NHibernate.DomainModel.Qux where q.Stuff is null").Enumerable().GetEnumerator();
			int count = 0;
			while (enumer.MoveNext())
			{
				Qux q = (Qux) enumer.Current;
				q.Stuff = "foo";
				// can't remove item from IEnumerator in .net 
				if (count == 0 || count == 5)
				{
					s.Delete(q);
				}
				count++;
			}

			Assert.AreEqual(10, count, "found 10 items");
			s.Flush();
			s.Close();

			s = OpenSession();

			Assert.AreEqual(8,
			                s.Delete("from q in class NHibernate.DomainModel.Qux where q.Stuff=?", "foo", NHibernateUtil.String),
			                "delete by query");

			s.Flush();
			s.Close();

			s = OpenSession();
			enumer = s.CreateQuery("from q in class NHibernate.DomainModel.Qux").Enumerable().GetEnumerator();
			Assert.IsFalse(enumer.MoveNext(), "no items in enumerator");
			s.Flush();
			s.Close();
		}

		/// <summary>
		/// Adding a test to verify that a database action can occur in the
		/// middle of an Enumeration.  Under certain conditions an open 
		/// DataReader can be kept open and cause any other action to fail. 
		/// </summary>
		[Test]
		public void EnumerableDisposable()
		{
			// this test used to be called Iterators()

			ISession s = OpenSession();
			for (long i = 0L; i < 10L; i++)
			{
				Simple simple = new Simple();
				simple.Count = (int) i;
				s.Save(simple, i);
				Assert.IsNotNull(simple, "simple is not null");
			}
			s.Flush();
			s.Close();

			s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Simple simp = (Simple) s.Load(typeof(Simple), 8L);

			// the reader under the enum has to still be a SqlDataReader (subst db name here) and 
			// can't be a NDataReader - the best way to get this result is to query on just a property
			// of an object.  If the query is "from Simple as s" then it will be converted to a NDataReader
			// on the MoveNext so it can get the object from the id - thus needing another open DataReader so
			// it must convert to an NDataReader.
			IEnumerable enumer = s.CreateQuery("select s.Count from Simple as s").Enumerable();
			//int count = 0;
			foreach (object obj in enumer)
			{
				if ((int) obj == 7)
				{
					break;
				}
			}

			// if Enumerable doesn't implement Dispose() then the test fails on this line
			t.Commit();
			s.Close();

			s = OpenSession();
			Assert.AreEqual(10,
			                s.Delete("from Simple"),
			                "delete by query");

			s.Flush();
			s.Close();

			s = OpenSession();
			enumer = s.CreateQuery("from Simple").Enumerable();
			Assert.IsFalse(enumer.GetEnumerator().MoveNext(), "no items in enumerator");
			s.Flush();
			s.Close();
		}

		[Test]
		public void Versioning()
		{
			GlarchProxy g = new Glarch();
			GlarchProxy g2 = new Glarch();

			object gid, g2id;

			using (ISession s = OpenSession())
			using (ITransaction txn = s.BeginTransaction())
			{
				s.Save(g);
				s.Save(g2);
				gid = s.GetIdentifier(g);
				g2id = s.GetIdentifier(g2);
				g.Name = "glarch";
				txn.Commit();
			}

			sessions.Evict(typeof(Glarch));

			using (ISession s = OpenSession())
			using (ITransaction txn = s.BeginTransaction())
			{
				g = (GlarchProxy) s.Load(typeof(Glarch), gid);
				s.Lock(g, LockMode.Upgrade);
				g2 = (GlarchProxy) s.Load(typeof(Glarch), g2id);

				// Versions are initialized to 1 in NH (not to 0 like in Hibernate)
				Assert.AreEqual(2, g.Version, "version");
				Assert.AreEqual(2, g.DerivedVersion, "version");
				Assert.AreEqual(1, g2.Version, "version");
				g.Name = "foo";
				Assert.IsTrue(
					s.CreateQuery("from g in class Glarch where g.Version=3").List().Count == 1,
					"find by version"
					);
				g.Name = "bar";
				txn.Commit();
			}

			sessions.Evict(typeof(Glarch));

			using (ISession s = OpenSession())
			using (ITransaction txn = s.BeginTransaction())
			{
				g = (GlarchProxy) s.Load(typeof(Glarch), gid);
				g2 = (GlarchProxy) s.Load(typeof(Glarch), g2id);
				Assert.AreEqual(4, g.Version, "version");
				Assert.AreEqual(4, g.DerivedVersion, "version");
				Assert.AreEqual(1, g2.Version, "version");
				g.Next = null;
				g2.Next = g;
				s.Delete(g2);
				s.Delete(g);
				txn.Commit();
			}
		}

		// The test below is commented out, it fails because Glarch is mapped with optimistic-lock="dirty"
		// which means that the version column is not used for optimistic locking.
		/*
		[Test]
		public void Versioning() 
		{
			object gid, g2id;

			using( ISession s = OpenSession() )
			{
				GlarchProxy g = new Glarch();
				s.Save(g);

				GlarchProxy g2 = new Glarch();
				s.Save(g2);

				gid = s.GetIdentifier(g);
				g2id = s.GetIdentifier(g2);
				g.Name = "glarch";
				s.Flush();
			}

			GlarchProxy gOld;

			// grab a version of g that is old and hold onto it until later
			// for a StaleObjectException check.
			using( ISession sOld = OpenSession() )
			{
				gOld = (GlarchProxy)sOld.Get( typeof(Glarch), gid );

				// want gOld to be initialized so later I can change a property
				Assert.IsTrue( NHibernateUtil.IsInitialized( gOld ), "should be initialized" );
			}

			using( ISession s = OpenSession() )
			{
				GlarchProxy g = (GlarchProxy)s.Load( typeof(Glarch), gid );
				s.Lock(g, LockMode.Upgrade);
				GlarchProxy g2 = (GlarchProxy)s.Load( typeof(Glarch), g2id );
				Assert.AreEqual(1, g.Version, "g's version");
				Assert.AreEqual(1, g.DerivedVersion, "g's derived version");
				Assert.AreEqual(0, g2.Version, "g2's version");
				g.Name = "foo";
				Assert.AreEqual(1, s.CreateQuery("from g in class NHibernate.DomainModel.Glarch where g.Version=2").List().Count, "find by version");
				g.Name = "bar";
				s.Flush();
			}

			// now that g has been changed verify that we can't go back and update 
			// it with an old version of g
			bool isStale = false;

			using( ISession sOld = OpenSession() )
			{
				gOld.Name = "should not update";
				try 
				{
					sOld.Update( gOld, gid );
					sOld.Flush();
					//sOld.Close();
					sOld.Dispose();
				}
				catch(Exception e) 
				{
					Exception exc = e;
					while( exc!=null ) 
					{
						if( exc is StaleObjectStateException ) 
						{
							isStale = true;
							break;
						}
						exc = exc.InnerException;
					}
				}
			}

			Assert.IsTrue( isStale, "Did not catch a stale object exception when updating an old GlarchProxy." );

			using( ISession s = OpenSession() )
			{
				GlarchProxy g = (GlarchProxy)s.Load( typeof(Glarch), gid );
				GlarchProxy g2 = (GlarchProxy)s.Load( typeof(Glarch), g2id );

				Assert.AreEqual(3, g.Version, "g's version");
				Assert.AreEqual(3, g.DerivedVersion, "g's derived version");
				Assert.AreEqual(0, g2.Version, "g2's version");

				g.Next = null;
				g2.Next = g;
				s.Delete(g2);
				s.Delete(g);
				s.Flush();
				//s.Close();
			}
		}
		*/

		[Test]
		public void VersionedCollections()
		{
			ISession s = OpenSession();
			GlarchProxy g = new Glarch();
			s.Save(g);
			g.ProxyArray = new GlarchProxy[] {g};
			string gid = (string) s.GetIdentifier(g);
			IList<string> list = new List<string>();
			list.Add("foo");
			g.Strings = list;
			// <sets> in h2.0.3
			g.ProxySet = new HashSet<GlarchProxy> { g };
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy) s.Load(typeof(Glarch), gid);
			Assert.AreEqual(1, g.Strings.Count);
			Assert.AreEqual(1, g.ProxyArray.Length);
			Assert.AreEqual(1, g.ProxySet.Count);
			Assert.AreEqual(2, g.Version, "version collection before");
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy) s.Load(typeof(Glarch), gid);
			Assert.AreEqual("foo", g.Strings[0]);
			Assert.AreSame(g, g.ProxyArray[0]);
			IEnumerator enumer = g.ProxySet.GetEnumerator();
			enumer.MoveNext();
			Assert.AreSame(g, enumer.Current);
			Assert.AreEqual(2, g.Version, "versioned collection before");
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy) s.Load(typeof(Glarch), gid);
			Assert.AreEqual(2, g.Version, "versioned collection before");
			g.Strings.Add("bar");
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy) s.Load(typeof(Glarch), gid);
			Assert.AreEqual(3, g.Version, "versioned collection after");
			Assert.AreEqual(2, g.Strings.Count, "versioned collection after");
			g.ProxyArray = null;
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy) s.Load(typeof(Glarch), gid);
			Assert.AreEqual(4, g.Version, "versioned collection after");
			Assert.AreEqual(0, g.ProxyArray.Length, "version collection after");
			g.FooComponents = new List<FooComponent>();
			g.ProxyArray = null;
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy) s.Load(typeof(Glarch), gid);
			Assert.AreEqual(5, g.Version, "versioned collection after");
			s.Delete(g);
			s.Flush();
			s.Close();
		}


		[Test]
		public void RecursiveLoad()
		{
			// Non polymorphisc class (there is an implementation optimization
			// being tested here) - from h2.0.3 - what does that mean?
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();
			GlarchProxy last = new Glarch();
			s.Save(last);
			last.Order = 0;
			for (int i = 0; i < 5; i++)
			{
				GlarchProxy next = new Glarch();
				s.Save(next);
				last.Next = next;
				last = next;
				last.Order = (short) (i + 1);
			}

			IEnumerator enumer = s.CreateQuery("from g in class NHibernate.DomainModel.Glarch").Enumerable().GetEnumerator();
			while (enumer.MoveNext())
			{
				object objTemp = enumer.Current;
			}

			IList list = s.CreateQuery("from g in class NHibernate.DomainModel.Glarch").List();
			Assert.AreEqual(6, list.Count, "recursive find");
			txn.Commit();
			s.Close();

			s = OpenSession();
			txn = s.BeginTransaction();
			list = s.CreateQuery("from g in class NHibernate.DomainModel.Glarch").List();
			Assert.AreEqual(6, list.Count, "recursive iter");
			list = s.CreateQuery("from g in class NHibernate.DomainModel.Glarch where g.Next is not null").List();
			Assert.AreEqual(5, list.Count, "exclude the null next");
			txn.Commit();
			s.Close();

			s = OpenSession();
			txn = s.BeginTransaction();
			enumer =
				s.CreateQuery("from g in class NHibernate.DomainModel.Glarch order by g.Order asc").Enumerable().GetEnumerator();
			while (enumer.MoveNext())
			{
				GlarchProxy g = (GlarchProxy) enumer.Current;
				Assert.IsNotNull(g, "not null");
				// no equiv in .net - so ran a delete query
				// iter.remove();
			}

			s.Delete("from NHibernate.DomainModel.Glarch as g");
			txn.Commit();
			s.Close();

			// same thing bug using polymorphic class (no optimization possible)
			s = OpenSession();
			txn = s.BeginTransaction();
			FooProxy flast = new Bar();
			s.Save(flast);
			for (int i = 0; i < 5; i++)
			{
				FooProxy foo = new Bar();
				s.Save(foo);
				flast.TheFoo = foo;
				flast = flast.TheFoo;
				flast.String = "foo" + (i + 1);
			}

			enumer = s.CreateQuery("from foo in class NHibernate.DomainModel.Foo").Enumerable().GetEnumerator();
			while (enumer.MoveNext())
			{
				object objTemp = enumer.Current;
			}

			list = s.CreateQuery("from foo in class NHibernate.DomainModel.Foo").List();
			Assert.AreEqual(6, list.Count, "recursive find");
			txn.Commit();
			s.Close();

			s = OpenSession();
			txn = s.BeginTransaction();
			list = s.CreateQuery("from foo in class NHibernate.DomainModel.Foo").List();
			Assert.AreEqual(6, list.Count, "recursive iter");
			enumer = list.GetEnumerator();
			while (enumer.MoveNext())
			{
				Assert.IsTrue(enumer.Current is BarProxy, "polymorphic recursive load");
			}
			txn.Commit();
			s.Close();

			s = OpenSession();
			txn = s.BeginTransaction();
			enumer =
				s.CreateQuery("from foo in class NHibernate.DomainModel.Foo order by foo.String asc").Enumerable().GetEnumerator();
			string currentString = String.Empty;

			while (enumer.MoveNext())
			{
				BarProxy bar = (BarProxy) enumer.Current;
				string theString = bar.String;
				Assert.IsNotNull(bar, "not null");
				if (currentString != String.Empty)
				{
					Assert.IsTrue(theString.CompareTo(currentString) >= 0, "not in asc order");
				}
				currentString = theString;
				// no equiv in .net - so made a hql delete
				// iter.remove();
			}

			s.Delete("from NHibernate.DomainModel.Foo as foo");
			txn.Commit();
			s.Close();
		}

		// Not ported - testScrollableIterator - ScrollableResults are not supported by NH,
		// since they rely on the underlying ResultSet to support scrolling, and ADO.NET
		// IDataReaders do not support it.

		private bool DialectSupportsCountDistinct
		{
			get { return !(Dialect is SQLiteDialect); }
		}

		[Test]
		public void MultiColumnQueries()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();
			Foo foo = new Foo();
			s.Save(foo);
			Foo foo1 = new Foo();
			s.Save(foo1);
			foo.TheFoo = foo1;
			IList l =
				s.CreateQuery(
					"select parent, child from parent in class NHibernate.DomainModel.Foo, child in class NHibernate.DomainModel.Foo where parent.TheFoo = child")
					.List();
			Assert.AreEqual(1, l.Count, "multi-column find");

			IEnumerator rs;
			object[] row;

			if (DialectSupportsCountDistinct)
			{
				rs =
					s.CreateQuery(
						"select count(distinct child.id), count(distinct parent.id) from parent in class NHibernate.DomainModel.Foo, child in class NHibernate.DomainModel.Foo where parent.TheFoo = child")
						.Enumerable().GetEnumerator();
				Assert.IsTrue(rs.MoveNext());
				row = (object[]) rs.Current;
				Assert.AreEqual(1, row[0], "multi-column count");
				Assert.AreEqual(1, row[1], "multi-column count");
				Assert.IsFalse(rs.MoveNext());
			}

			rs =
				s.CreateQuery(
					"select child.id, parent.id, child.Long from parent in class NHibernate.DomainModel.Foo, child in class NHibernate.DomainModel.Foo where parent.TheFoo = child")
					.Enumerable().GetEnumerator();
			Assert.IsTrue(rs.MoveNext());
			row = (object[]) rs.Current;
			Assert.AreEqual(foo.TheFoo.Key, row[0], "multi-column id");
			Assert.AreEqual(foo.Key, row[1], "multi-column id");
			Assert.AreEqual(foo.TheFoo.Long, row[2], "multi-column property");
			Assert.IsFalse(rs.MoveNext());

			rs =
				s.CreateQuery(
					"select child.id, parent.id, child.Long, child, parent.TheFoo from parent in class NHibernate.DomainModel.Foo, child in class NHibernate.DomainModel.Foo where parent.TheFoo = child")
					.Enumerable().GetEnumerator();
			Assert.IsTrue(rs.MoveNext());
			row = (object[]) rs.Current;
			Assert.AreEqual(foo.TheFoo.Key, row[0], "multi-column id");
			Assert.AreEqual(foo.Key, row[1], "multi-column id");
			Assert.AreEqual(foo.TheFoo.Long, row[2], "multi-column property");
			Assert.AreSame(foo.TheFoo, row[3], "multi-column object");
			Assert.AreSame(row[3], row[4], "multi-column same object");
			Assert.IsFalse(rs.MoveNext());

			row = (object[]) l[0];
			Assert.AreSame(foo, row[0], "multi-column find");
			Assert.AreSame(foo.TheFoo, row[1], "multi-column find");
			txn.Commit();
			s.Close();

			s = OpenSession();
			txn = s.BeginTransaction();
			IEnumerator enumer =
				s.CreateQuery(
					"select parent, child from parent in class NHibernate.DomainModel.Foo, child in class NHibernate.DomainModel.Foo where parent.TheFoo = child and parent.String='a string'")
					.Enumerable().GetEnumerator();
			int deletions = 0;
			while (enumer.MoveNext())
			{
				object[] pnc = (object[]) enumer.Current;
				s.Delete(pnc[0]);
				s.Delete(pnc[1]);
				deletions++;
			}
			Assert.AreEqual(1, deletions, "multi-column enumerate");
			txn.Commit();
			s.Close();
		}


		[Test]
		public void DeleteTransient()
		{
			Fee fee = new Fee();
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			s.Save(fee);
			s.Flush();
			fee.Count = 123;
			tx.Commit();
			s.Close();

			s = OpenSession();
			tx = s.BeginTransaction();
			s.Delete(fee);
			tx.Commit();
			s.Close();

			s = OpenSession();
			tx = s.BeginTransaction();
			Assert.AreEqual(0, s.CreateQuery("from fee in class Fee").List().Count);
			tx.Commit();
			s.Close();
		}

		[Test]
		public void DeleteUpdatedTransient()
		{
			Fee fee = new Fee();
			Fee fee2 = new Fee();
			fee2.AnotherFee = fee;

			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Save(fee);
					s.Save(fee2);
					s.Flush();
					fee.Count = 123;
					tx.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Update(fee);
					//fee2.AnotherFee = null;
					s.Update(fee2);
					s.Delete(fee);
					s.Delete(fee2);
					tx.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Assert.AreEqual(0, s.CreateQuery("from fee in class Fee").List().Count);
					tx.Commit();
				}
			}
		}

		[Test]
		public void UpdateOrder()
		{
			Fee fee1, fee2, fee3;

			using (ISession s = OpenSession())
			{
				fee1 = new Fee();
				s.Save(fee1);

				fee2 = new Fee();
				fee1.TheFee = fee2;
				fee2.TheFee = fee1;
				fee2.Fees = new HashSet<string>();

				fee3 = new Fee();
				fee3.TheFee = fee1;
				fee3.AnotherFee = fee2;
				fee2.AnotherFee = fee3;
				s.Save(fee3);
				s.Save(fee2);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				fee1.Count = 10;
				fee2.Count = 20;
				fee3.Count = 30;
				s.Update(fee1);
				s.Update(fee2);
				s.Update(fee3);
				s.Flush();
				s.Delete(fee1);
				s.Delete(fee2);
				s.Delete(fee3);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Assert.AreEqual(0, s.CreateQuery("from fee in class Fee").List().Count);
					tx.Commit();
				}
			}
		}

		[Test]
		public void UpdateFromTransient()
		{
			ISession s = OpenSession();
			Fee fee1 = new Fee();
			s.Save(fee1);
			Fee fee2 = new Fee();
			fee1.TheFee = fee2;
			fee2.TheFee = fee1;
			fee2.Fees = new HashSet<string>();
			Fee fee3 = new Fee();
			fee3.TheFee = fee1;
			fee3.AnotherFee = fee2;
			fee2.AnotherFee = fee3;
			s.Save(fee3);
			s.Save(fee2);
			s.Flush();
			s.Close();

			fee1.Fi = "changed";
			s = OpenSession();
			s.SaveOrUpdate(fee1);
			s.Flush();
			s.Close();

			Qux q = new Qux("quxxy");
			fee1.Qux = q;
			s = OpenSession();
			s.SaveOrUpdate(fee1);
			s.Flush();
			s.Close();

			s = OpenSession();
			fee1 = (Fee) s.Load(typeof(Fee), fee1.Key);
			Assert.AreEqual("changed", fee1.Fi, "updated from transient");
			Assert.IsNotNull(fee1.Qux, "unsaved-value");
			s.Delete(fee1.Qux);
			fee1.Qux = null;
			s.Flush();
			s.Close();

			fee2.Fi = "CHANGED";
			fee2.Fees.Add("an element");
			fee1.Fi = "changed again";
			s = OpenSession();
			s.SaveOrUpdate(fee2);
			s.Update(fee1, fee1.Key);
			s.Flush();
			s.Close();

			s = OpenSession();
			Fee fee = new Fee();
			s.Load(fee, fee2.Key);
			fee1 = (Fee) s.Load(typeof(Fee), fee1.Key);
			Assert.AreEqual("changed again", fee1.Fi, "updated from transient");
			Assert.AreEqual("CHANGED", fee.Fi, "updated from transient");
			Assert.IsTrue(fee.Fees.Contains("an element"), "updated collection");
			s.Flush();
			s.Close();

			fee.Fees.Clear();
			fee.Fees.Add("new element");
			fee1.TheFee = null;
			s = OpenSession();
			s.SaveOrUpdate(fee);
			s.SaveOrUpdate(fee1);
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Load(fee, fee.Key);
			Assert.IsNotNull(fee.AnotherFee, "update");
			Assert.IsNotNull(fee.TheFee, "update");
			Assert.AreSame(fee.AnotherFee.TheFee, fee.TheFee, "update");
			Assert.IsTrue(fee.Fees.Contains("new element"), "updated collection");
			Assert.IsFalse(fee.Fees.Contains("an element"), "updated collection");
			s.Flush();
			s.Close();

			fee.Qux = new Qux("quxy");
			s = OpenSession();
			s.SaveOrUpdate(fee);
			s.Flush();
			s.Close();

			fee.Qux.Stuff = "xxx";
			s = OpenSession();
			s.SaveOrUpdate(fee);
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Load(fee, fee.Key);
			Assert.IsNotNull(fee.Qux, "cascade update");
			Assert.AreEqual("xxx", fee.Qux.Stuff, "cascade update");
			Assert.IsNotNull(fee.AnotherFee, "update");
			Assert.IsNotNull(fee.TheFee, "update");
			Assert.AreSame(fee.AnotherFee.TheFee, fee.TheFee, "update");
			fee.AnotherFee.AnotherFee = null;
			s.Delete(fee);
			s.Delete("from fee in class NHibernate.DomainModel.Fee");
			s.Flush();
			s.Close();
		}


		[Test]
		public void ArraysOfTimes()
		{
			Baz baz;

			using (ISession s = OpenSession())
			{
				baz = new Baz();
				s.Save(baz);
				baz.SetDefaults();
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				baz.TimeArray[2] = new DateTime(123); // H2.1: new Date(123)
				baz.TimeArray[3] = new DateTime(1234); // H2.1: new java.sql.Time(1234)
				s.Update(baz); // missing in H2.1
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				baz = (Baz) s.Load(typeof(Baz), baz.Code);
				s.Delete(baz);
				s.Flush();
			}
		}

		[Test]
		public void Components()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();
			Foo foo = new Foo();
			foo.Component = new FooComponent("foo", 69, null, new FooComponent("bar", 96, null, null));
			s.Save(foo);
			foo.Component.Name = "IFA";
			txn.Commit();
			s.Close();

			foo.Component = null;
			s = OpenSession();
			txn = s.BeginTransaction();
			s.Load(foo, foo.Key);

			Assert.AreEqual("IFA", foo.Component.Name, "save components");
			Assert.AreEqual("bar", foo.Component.Subcomponent.Name, "save subcomponent");
			Assert.IsNotNull(foo.Component.Glarch, "cascades save via component");
			foo.Component.Subcomponent.Name = "baz";
			txn.Commit();
			s.Close();

			foo.Component = null;
			s = OpenSession();
			txn = s.BeginTransaction();
			s.Load(foo, foo.Key);
			Assert.AreEqual("IFA", foo.Component.Name, "update components");
			Assert.AreEqual("baz", foo.Component.Subcomponent.Name, "update subcomponent");
			s.Delete(foo);
			txn.Commit();
			s.Close();

			s = OpenSession();
			txn = s.BeginTransaction();
			foo = new Foo();
			s.Save(foo);
			foo.Custom = new string[] {"one", "two"};

			// Custom.s1 uses the first column under the <property name="Custom"...>
			// which is first_name
			Assert.AreSame(foo, s.CreateQuery("from Foo foo where foo.Custom.s1 = 'one'").List()[0]);
			s.Delete(foo);
			txn.Commit();
			s.Close();
		}


		[Test]
		public void Enum()
		{
			ISession s = OpenSession();
			FooProxy foo = new Foo();
			object id = s.Save(foo);
			foo.Status = FooStatus.ON;
			s.Flush();
			s.Close();

			// verify an enum can be in the ctor
			s = OpenSession();
			IList list =
				s.CreateQuery("select new Result(foo.String, foo.Long, foo.Integer, foo.Status) from foo in class Foo").List();
			Assert.AreEqual(1, list.Count, "Should have found foo");
			Assert.AreEqual(FooStatus.ON, ((Result) list[0]).Status, "verifying enum set in ctor - should have been ON");
			s.Close();

			s = OpenSession();
			foo = (FooProxy) s.Load(typeof(Foo), id);
			Assert.AreEqual(FooStatus.ON, foo.Status);
			foo.Status = FooStatus.OFF;
			s.Flush();
			s.Close();

			s = OpenSession();
			foo = (FooProxy) s.Load(typeof(Foo), id);
			Assert.AreEqual(FooStatus.OFF, foo.Status);
			s.Close();

			// verify that SetEnum with named params works correctly
			s = OpenSession();
			IQuery q = s.CreateQuery("from Foo as f where f.Status = :status");
			q.SetEnum("status", FooStatus.OFF);
			IList results = q.List();
			Assert.AreEqual(1, results.Count, "should have found 1");
			foo = (Foo) results[0];

			q = s.CreateQuery("from Foo as f where f.Status = :status");
			q.SetEnum("status", FooStatus.ON);
			results = q.List();
			Assert.AreEqual(0, results.Count, "no foo with status of ON");

			// try to have the Query guess the enum type
			q = s.CreateQuery("from Foo as f where f.Status = :status");
			q.SetParameter("status", FooStatus.OFF);
			results = q.List();
			Assert.AreEqual(1, results.Count, "found the 1 result");

			// have the query guess the enum type in a ParameterList.
			q = s.CreateQuery("from Foo as f where f.Status in (:status)");
			q.SetParameterList("status", new FooStatus[] {FooStatus.OFF, FooStatus.ON});
			results = q.List();
			Assert.AreEqual(1, results.Count, "should have found the 1 foo");

			q = s.CreateQuery("from Foo as f where f.Status = FooStatus.OFF");
			Assert.AreEqual(1, q.List().Count, "Enum in string - should have found OFF");

			q = s.CreateQuery("from Foo as f where f.Status = FooStatus.ON");
			Assert.AreEqual(0, q.List().Count, "Enum in string - should not have found ON");

			s.Delete(foo);
			s.Flush();
			s.Close();
		}


		[Test]
		public void NoForeignKeyViolations()
		{
			ISession s = OpenSession();
			Glarch g1 = new Glarch();
			Glarch g2 = new Glarch();
			g1.Next = g2;
			g2.Next = g1;
			s.Save(g1);
			s.Save(g2);
			s.Flush();
			s.Close();

			s = OpenSession();
			IList l = s.CreateQuery("from g in class NHibernate.DomainModel.Glarch where g.Next is not null").List();
			s.Delete(l[0]);
			s.Delete(l[1]);
			s.Flush();
			s.Close();
		}

		[Test]
		public void LazyCollections()
		{
			ISession s = OpenSession();
			Qux q = new Qux();
			s.Save(q);
			s.Flush();
			s.Close();

			s = OpenSession();
			q = (Qux) s.Load(typeof(Qux), q.Key);
			s.Flush();
			s.Close();

			// two exceptions are supposed to occur:")
			bool ok = false;
			try
			{
				int countMoreFums = q.MoreFums.Count;
			}
			catch (LazyInitializationException lie)
			{
				Debug.WriteLine("caught expected " + lie.ToString());
				ok = true;
			}
			Assert.IsTrue(ok, "lazy collection with one-to-many");

			ok = false;
			try
			{
				int countFums = q.Fums.Count;
			}
			catch (LazyInitializationException lie)
			{
				Debug.WriteLine("caught expected " + lie.ToString());
				ok = true;
			}

			Assert.IsTrue(ok, "lazy collection with many-to-many");

			s = OpenSession();
			q = (Qux) s.Load(typeof(Qux), q.Key);
			s.Delete(q);
			s.Flush();
			s.Close();
		}

		[Test]
		public void NewSessionLifecycle()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			object fid = null;
			try
			{
				Foo f = new Foo();
				s.Save(f);
				fid = s.GetIdentifier(f);
				//s.Flush();
				t.Commit();
			}
			catch (Exception)
			{
				t.Rollback();
				throw;
			}
			finally
			{
				s.Close();
			}

			s = OpenSession();
			t = s.BeginTransaction();
			try
			{
				Foo f = new Foo();
				s.Delete(f);
				//s.Flush();
				t.Commit();
			}
			catch (Exception e)
			{
				Assert.IsNotNull(e); //getting ride of 'e' is never used compile warning
				t.Rollback();
			}
			finally
			{
				s.Close();
			}

			s = OpenSession();
			t = s.BeginTransaction();
			try
			{
				Foo f = (Foo) s.Load(typeof(Foo), fid, LockMode.Upgrade);
				s.Delete(f);
				// s.Flush();
				t.Commit();
			}
			catch (Exception)
			{
				t.Rollback();
				throw;
			}
			finally
			{
				Assert.IsNull(s.Close());
			}
		}

		[Test]
		public void Disconnect()
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			Foo foo2 = new Foo();
			s.Save(foo);
			s.Save(foo2);
			foo2.TheFoo = foo;
			s.Flush();
			s.Disconnect();

			s.Reconnect();
			s.Delete(foo);
			foo2.TheFoo = null;
			s.Flush();
			s.Disconnect();

			s.Reconnect();
			s.Delete(foo2);
			s.Flush();
			s.Close();
		}

		[Test]
		public void OrderBy()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Foo foo = new Foo();
			s.Save(foo);
			IList list =
				s.CreateQuery(
					"select foo from foo in class Foo, fee in class Fee where foo.Dependent = fee order by foo.String desc, foo.Component.Count asc, fee.id")
					.List();
			Assert.AreEqual(1, list.Count, "order by");
			Foo foo2 = new Foo();
			s.Save(foo2);
			foo.TheFoo = foo2;
			list =
				s.CreateQuery(
					"select foo.TheFoo, foo.Dependent from foo in class Foo order by foo.TheFoo.String desc, foo.Component.Count asc, foo.Dependent.id")
					.List();
			Assert.AreEqual(1, list.Count, "order by");
			list =
				s.CreateQuery("select foo from foo in class NHibernate.DomainModel.Foo order by foo.Dependent.id, foo.Dependent.Fi")
					.List();
			Assert.AreEqual(2, list.Count, "order by");
			s.Delete(foo);
			s.Delete(foo2);
			t.Commit();
			s.Close();

			s = OpenSession();
			Many manyB = new Many();
			s.Save(manyB);
			One oneB = new One();
			s.Save(oneB);
			oneB.Value = "b";
			manyB.One = oneB;
			Many manyA = new Many();
			s.Save(manyA);
			One oneA = new One();
			s.Save(oneA);
			oneA.Value = "a";
			manyA.One = oneA;
			s.Flush();
			s.Close();

			s = OpenSession();
			IEnumerable enumerable =
				s.CreateQuery("SELECT one FROM one IN CLASS " + typeof(One).Name + " ORDER BY one.Value ASC").Enumerable();
			int count = 0;
			foreach (One one in enumerable)
			{
				switch (count)
				{
					case 0:
						Assert.AreEqual("a", one.Value, "a - ordering failed");
						break;
					case 1:
						Assert.AreEqual("b", one.Value, "b - ordering failed");
						break;
					default:
						Assert.Fail("more than two elements");
						break;
				}
				count++;
			}

			s.Flush();
			s.Close();

			s = OpenSession();
			enumerable =
				s.CreateQuery("SELECT many.One FROM many IN CLASS " + typeof(Many).Name +
				              " ORDER BY many.One.Value ASC, many.One.id").Enumerable();
			count = 0;
			foreach (One one in enumerable)
			{
				switch (count)
				{
					case 0:
						Assert.AreEqual("a", one.Value, "'a' should be first element");
						break;
					case 1:
						Assert.AreEqual("b", one.Value, "'b' should be second element");
						break;
					default:
						Assert.Fail("more than 2 elements");
						break;
				}
				count++;
			}
			s.Flush();
			s.Close();

			s = OpenSession();
			oneA = (One) s.Load(typeof(One), oneA.Key);
			manyA = (Many) s.Load(typeof(Many), manyA.Key);
			oneB = (One) s.Load(typeof(One), oneB.Key);
			manyB = (Many) s.Load(typeof(Many), manyB.Key);
			s.Delete(manyA);
			s.Delete(oneA);
			s.Delete(manyB);
			s.Delete(oneB);
			s.Flush();
			s.Close();
		}

		[Test]
		public void ManyToOne()
		{
			ISession s = OpenSession();
			One one = new One();
			s.Save(one);
			one.Value = "yada";
			Many many = new Many();
			many.One = one;
			s.Save(many);
			s.Flush();
			s.Close();

			s = OpenSession();
			one = (One) s.Load(typeof(One), one.Key);
			int countManies = one.Manies.Count;
			s.Close();

			s = OpenSession();
			many = (Many) s.Load(typeof(Many), many.Key);
			Assert.IsNotNull(many.One, "many-to-one assoc");
			s.Delete(many.One);
			s.Delete(many);
			s.Flush();
			s.Close();
		}

		[Test]
		public void SaveDelete()
		{
			ISession s = OpenSession();
			Foo f = new Foo();
			s.Save(f);
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Delete(s.Load(typeof(Foo), f.Key));
			s.Flush();
			s.Close();
		}

		[Test]
		public void ProxyArray()
		{
			ISession s = OpenSession();
			GlarchProxy g = new Glarch();
			Glarch g1 = new Glarch();
			Glarch g2 = new Glarch();
			g.ProxyArray = new GlarchProxy[] {g1, g2};
			Glarch g3 = new Glarch();
			s.Save(g3);
			g2.ProxyArray = new GlarchProxy[] {null, g3, g};

			g.ProxySet = new HashSet<GlarchProxy> { g1, g2 };
			s.Save(g);
			s.Save(g1);
			s.Save(g2);
			object id = s.GetIdentifier(g);
			s.Flush();
			s.Close();

			s = OpenSession();
			g = (GlarchProxy) s.Load(typeof(Glarch), id);
			Assert.AreEqual(2, g.ProxyArray.Length, "array of proxies");
			Assert.IsNotNull(g.ProxyArray[0], "array of proxies");
			Assert.IsNull(g.ProxyArray[1].ProxyArray[0], "deferred load test");
			Assert.AreEqual(g, g.ProxyArray[1].ProxyArray[2], "deferred load test");
			Assert.AreEqual(2, g.ProxySet.Count, "set of proxies");

			IEnumerator enumer = s.CreateQuery("from g in class NHibernate.DomainModel.Glarch").Enumerable().GetEnumerator();
			while (enumer.MoveNext())
			{
				s.Delete(enumer.Current);
			}

			s.Flush();
			s.Disconnect();

			// serialize the session.
			Stream stream = new MemoryStream();
			IFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, s);

			// close the original session
			s.Close();

			// deserialize the session
			stream.Position = 0;
			s = (ISession) formatter.Deserialize(stream);
			stream.Close();

			s.Close();
		}

		[Test]
		public void Cache()
		{
			NHibernate.DomainModel.Immutable im = new NHibernate.DomainModel.Immutable();

			using (ISession s = OpenSession())
			{
				s.Save(im);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				s.Load(im, im.Id);
			}

			using (ISession s = OpenSession())
			{
				s.Load(im, im.Id);

				NHibernate.DomainModel.Immutable imFromFind =
					(NHibernate.DomainModel.Immutable) s.CreateQuery("from im in class Immutable where im = ?").SetEntity(0, im).List()[0];
				NHibernate.DomainModel.Immutable imFromLoad = (NHibernate.DomainModel.Immutable) s.Load(typeof(NHibernate.DomainModel.Immutable), im.Id);

				Assert.IsTrue(im == imFromFind, "cached object identity from Find ");
				Assert.IsTrue(im == imFromLoad, "cached object identity from Load ");
			}

			// Clean up the immutable. Need to do this using direct SQL, since ISession
			// refuses to delete immutable objects.
			using (ISession s = OpenSession())
			{
				IDbConnection connection = s.Connection;
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = "delete from immut";
					command.ExecuteNonQuery();
				}
			}
		}

		[Test]
		public void FindLoad()
		{
			ISession s = OpenSession();
			FooProxy foo = new Foo();
			s.Save(foo);
			s.Flush();
			s.Close();

			s = OpenSession();
			foo = (FooProxy) s.CreateQuery("from foo in class NHibernate.DomainModel.Foo").List()[0];
			FooProxy foo2 = (FooProxy) s.Load(typeof(Foo), foo.Key);
			Assert.AreSame(foo, foo2, "find returns same object as load");
			s.Flush();
			s.Close();

			s = OpenSession();
			foo2 = (FooProxy) s.Load(typeof(Foo), foo.Key);
			foo = (FooProxy) s.CreateQuery("from foo in class NHibernate.DomainModel.Foo").List()[0];
			Assert.AreSame(foo2, foo, "find returns same object as load");
			s.Delete("from foo in class NHibernate.DomainModel.Foo");
			s.Flush();
			s.Close();
		}

		[Test]
		public void Refresh()
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			s.Flush();

			IDbCommand cmd = s.Connection.CreateCommand();
			cmd.CommandText = "update " + Dialect.QuoteForTableName("foos") + " set long_ = -3";
			cmd.ExecuteNonQuery();

			s.Refresh(foo);
			Assert.AreEqual((long) -3, foo.Long);
			Assert.AreEqual(LockMode.Read, s.GetCurrentLockMode(foo));
			s.Refresh(foo, LockMode.Upgrade);
			Assert.AreEqual(LockMode.Upgrade, s.GetCurrentLockMode(foo));
			s.Delete(foo);
			s.Flush();
			s.Close();
		}

		[Test]
		public void RefreshTransient()
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			s.Save(foo);
			s.Flush();
			/* 
			Commented to have same behavior of H3.2 (test named FooBarTest.testRefresh())
			s.Close(); 
			s = OpenSession();
			btw using close and open a new session more than Transient the entity will be detached.
			*/
			IDbCommand cmd = s.Connection.CreateCommand();
			cmd.CommandText = "update " + Dialect.QuoteForTableName("foos") + " set long_ = -3";
			cmd.ExecuteNonQuery();
			s.Refresh(foo);
			Assert.AreEqual(-3L, foo.Long);
			s.Delete(foo);
			s.Flush();
			s.Close();
		}

		[Test]
		public void AutoFlush()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();
			FooProxy foo = new Foo();
			s.Save(foo);
			Assert.AreEqual(1, s.CreateQuery("from foo in class NHibernate.DomainModel.Foo").List().Count,
			                "autoflush inserted row");
			foo.Char = 'X';
			Assert.AreEqual(1, s.CreateQuery("from foo in class NHibernate.DomainModel.Foo where foo.Char='X'").List().Count,
			                "autflush updated row");
			txn.Commit();
			s.Close();

			s = OpenSession();
			txn = s.BeginTransaction();
			foo = (FooProxy) s.Load(typeof(Foo), foo.Key);

			if (Dialect.SupportsSubSelects)
			{
				foo.Bytes = GetBytes("osama");
				Assert.AreEqual(1,
				                s.CreateQuery("from foo in class NHibernate.DomainModel.Foo where 111 in elements(foo.Bytes)")
				                 .List().Count, "autoflush collection update");
				foo.Bytes[0] = 69;
				Assert.AreEqual(1,
				                s.CreateQuery("from foo in class NHibernate.DomainModel.Foo where 69 in elements(foo.Bytes)")
				                 .List().Count, "autoflush collection update");
			}

			s.Delete(foo);
			Assert.AreEqual(0, s.CreateQuery("from foo in class NHibernate.DomainModel.Foo").List().Count, "autoflush delete");
			txn.Commit();
			s.Close();
		}

		[Test]
		public void Veto()
		{
			ISession s = OpenSession();
			Vetoer v = new Vetoer();
			s.Save(v);
			object id = s.Save(v);
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Update(v, id);
			s.Update(v, id);
			s.Delete(v);
			s.Delete(v);
			s.Flush();
			s.Close();
		}

		[Test]
		public void SerializableType()
		{
			ISession s = OpenSession();
			Vetoer v = new Vetoer();
			v.Strings = new string[] {"foo", "bar", "baz"};
			s.Save(v);
			object id = s.Save(v);
			v.Strings[1] = "osama";
			s.Flush();
			s.Close();

			s = OpenSession();
			v = (Vetoer) s.Load(typeof(Vetoer), id);
			Assert.AreEqual("osama", v.Strings[1], "serializable type");
			s.Delete(v);
			s.Delete(v);
			s.Flush();
			s.Close();
		}

		[Test]
		public void AutoFlushCollections()
		{
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			Baz baz = new Baz();
			baz.SetDefaults();
			s.Save(baz);
			tx.Commit();
			s.Close();

			s = OpenSession();
			tx = s.BeginTransaction();
			baz = (Baz) s.Load(typeof(Baz), baz.Code);
			baz.StringArray[0] = "bark";

			IEnumerator e;

			e = s.CreateQuery("select elements(baz.StringArray) from baz in class NHibernate.DomainModel.Baz").Enumerable().
						GetEnumerator();
			

			bool found = false;
			while (e.MoveNext())
			{
				if ("bark".Equals(e.Current))
				{
					found = true;
				}
			}
			Assert.IsTrue(found);
			baz.StringArray = null;

			e = s.CreateQuery("select distinct elements(baz.StringArray) from baz in class NHibernate.DomainModel.Baz").Enumerable()
						.GetEnumerator();
			
			Assert.IsFalse(e.MoveNext());
			baz.StringArray = new string[] {"foo", "bar"};

			e = s.CreateQuery("select elements(baz.StringArray) from baz in class NHibernate.DomainModel.Baz").Enumerable().
						GetEnumerator();
			
			Assert.IsTrue(e.MoveNext());

			Foo foo = new Foo();
			s.Save(foo);
			s.Flush();
			baz.FooArray = new Foo[] {foo};

			e = s.CreateQuery("select foo from baz in class NHibernate.DomainModel.Baz, foo in elements(baz.FooArray)").Enumerable()
						.GetEnumerator();
			
			found = false;
			while (e.MoveNext())
			{
				if (foo == e.Current)
				{
					found = true;
				}
			}
			Assert.IsTrue(found);

			baz.FooArray[0] = null;

			e = s.CreateQuery("select foo from baz in class NHibernate.DomainModel.Baz, foo in elements(baz.FooArray)").Enumerable()
						.GetEnumerator();
			
			Assert.IsFalse(e.MoveNext());
			baz.FooArray[0] = foo;

			e = s.CreateQuery("select elements(baz.FooArray) from baz in class NHibernate.DomainModel.Baz").Enumerable().
						GetEnumerator();
			
			Assert.IsTrue(e.MoveNext());

			if (Dialect.SupportsSubSelects && !(Dialect is FirebirdDialect))
			{
				baz.FooArray[0] = null;
				e = s.CreateQuery("from baz in class NHibernate.DomainModel.Baz where ? in elements(baz.FooArray)").SetEntity(0, foo).
							Enumerable().GetEnumerator();

				Assert.IsFalse(e.MoveNext());
				baz.FooArray[0] = foo;
				e = s.CreateQuery("select foo from foo in class NHibernate.DomainModel.Foo where foo in "
													+ "(select elt from baz in class NHibernate.DomainModel.Baz, elt in elements(baz.FooArray))").
							Enumerable().GetEnumerator();
				
				Assert.IsTrue(e.MoveNext());
			}
			s.Delete(foo);
			s.Delete(baz);
			tx.Commit();
			s.Close();
		}

		[Test]
		public void UserProvidedConnection()
		{
			IConnectionProvider prov = ConnectionProviderFactory.NewConnectionProvider(cfg.Properties);
			ISession s = sessions.OpenSession(prov.GetConnection());
			ITransaction tx = s.BeginTransaction();
			s.CreateQuery("from foo in class NHibernate.DomainModel.Fo").List();
			tx.Commit();

			IDbConnection c = s.Disconnect();
			Assert.IsNotNull(c);

			s.Reconnect(c);
			tx = s.BeginTransaction();
			s.CreateQuery("from foo in class NHibernate.DomainModel.Fo").List();
			tx.Commit();
			Assert.AreSame(c, s.Close());
			c.Close();
		}

		[Test]
		public void CachedCollection()
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			baz.SetDefaults();
			s.Save(baz);
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz) s.Load(typeof(Baz), baz.Code);
			((FooComponent) baz.TopComponents[0]).Count = 99;
			s.Flush();
			s.Close();

			s = OpenSession();
			baz = (Baz) s.Load(typeof(Baz), baz.Code);
			Assert.AreEqual(99, ((FooComponent) baz.TopComponents[0]).Count);
			s.Delete(baz);
			s.Flush();
			s.Close();
		}

		[Test]
		public void ComplicatedQuery()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();
			Foo foo = new Foo();
			object id = s.Save(foo);
			Assert.IsNotNull(id);
			Qux q = new Qux("q");
			foo.Dependent.Qux = q;
			s.Save(q);
			q.Foo.String = "foo2";

			IEnumerator enumer =
				s.CreateQuery("from foo in class Foo where foo.Dependent.Qux.Foo.String = 'foo2'").Enumerable().GetEnumerator();
			Assert.IsTrue(enumer.MoveNext());
			s.Delete(foo);
			txn.Commit();
			s.Close();
		}

		[Test]
		public void LoadAfterDelete()
		{
			ISession s = OpenSession();
			Foo foo = new Foo();
			object id = s.Save(foo);
			s.Flush();
			s.Delete(foo);

			bool err = false;
			try
			{
				s.Load(typeof(Foo), id);
			}
			//catch (ObjectDeletedException ode) Changed to have same behavior of H3.2
			catch (ObjectNotFoundException ode)
			{
				Assert.IsNotNull(ode); //getting ride of 'ode' is never used compile warning
				err = true;
			}
			Assert.IsTrue(err);
			s.Flush();
			err = false;

			try
			{
				bool proxyBoolean = ((FooProxy) s.Load(typeof(Foo), id)).Boolean;
			}
			catch (ObjectNotFoundException lie)
			{
				// Proxy initialization which failed because the object was not found
				// now throws ONFE instead of LazyInitializationException
				Assert.IsNotNull(lie); //getting ride of 'lie' is never used compile warning
				err = true;
			}
			Assert.IsTrue(err);

			Fo fo = Fo.NewFo();
			id = FumTest.FumKey("abc"); //yuck!
			s.Save(fo, id);
			s.Flush();
			s.Delete(fo);
			err = false;

			try
			{
				s.Load(typeof(Fo), id);
			}
			//catch (ObjectDeletedException ode) Changed to have same behavior of H3.2
			catch (ObjectNotFoundException ode)
			{
				Assert.IsNotNull(ode); //getting ride of 'ode' is never used compile warning
				err = true;
			}

			Assert.IsTrue(err);
			s.Flush();
			s.Close();
		}


		[Test]
		public void ObjectType()
		{
			object gid;

			using (ISession s = OpenSession())
			{
				GlarchProxy g = new Glarch();
				Foo foo = new Foo();
				g.Any = foo;
				gid = s.Save(g);
				s.Save(foo);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				GlarchProxy g = (GlarchProxy) s.Load(typeof(Glarch), gid);
				Assert.IsNotNull(g.Any);
				Assert.IsTrue(g.Any is FooProxy);
				s.Delete(g.Any);
				s.Delete(g);
				s.Flush();
			}
		}


		[Test]
		public void Any()
		{
			ISession s = OpenSession();
			One one = new One();
			BarProxy foo = new Bar();
			foo.Object = one;
			object fid = s.Save(foo);
			object oid = one.Key;
			s.Flush();
			s.Close();

			s = OpenSession();
			IList list = s.CreateQuery("from Bar bar where bar.Object.id = ? and bar.Object.class = ?")
				.SetParameter(0, oid, NHibernateUtil.Int64).SetParameter(1, typeof(One).FullName, NHibernateUtil.ClassMetaType).List();
			Assert.AreEqual(1, list.Count);

			// this is a little different from h2.0.3 because the full type is stored, not
			// just the class name.
			list =
				s.CreateQuery(
					"select one from One one, Bar bar where bar.Object.id = one.id and bar.Object.class LIKE 'NHibernate.DomainModel.One%'")
					.List();
			Assert.AreEqual(1, list.Count);
			s.Flush();
			s.Close();

			s = OpenSession();
			foo = (BarProxy) s.Load(typeof(Foo), fid);
			Assert.IsNotNull(foo);
			Assert.IsTrue(foo.Object is One);
			Assert.AreEqual(oid, s.GetIdentifier(foo.Object));
			s.Delete(foo);
			s.Delete(foo.Object);
			s.Flush();
			s.Close();
		}

		[Test]
		public void EmbeddedCompositeID()
		{
			ISession s = OpenSession();
			Location l = new Location();
			l.CountryCode = "AU";
			l.Description = "foo bar";
			l.Locale = CultureInfo.CreateSpecificCulture("en-AU");
			l.StreetName = "Brunswick Rd";
			l.StreetNumber = 300;
			l.City = "Melbourne";
			s.Save(l);
			s.Flush();
			s.Close();

			s = OpenSession();
			s.FlushMode = FlushMode.Never;
			l =
				(Location)
				s.CreateQuery("from l in class Location where l.CountryCode = 'AU' and l.Description='foo bar'").List()[0];
			Assert.AreEqual("AU", l.CountryCode);
			Assert.AreEqual("Melbourne", l.City);
			Assert.AreEqual(CultureInfo.CreateSpecificCulture("en-AU"), l.Locale);
			s.Close();

			s = OpenSession();
			l.Description = "sick're";
			s.Update(l);
			s.Flush();
			s.Close();

			s = OpenSession();
			l = new Location();
			l.CountryCode = "AU";
			l.Description = "foo bar";
			l.Locale = CultureInfo.CreateSpecificCulture("en-US");
			l.StreetName = "Brunswick Rd";
			l.StreetNumber = 300;
			l.City = "Melbourne";
			Assert.AreSame(l, s.Load(typeof(Location), l));
			Assert.AreEqual(CultureInfo.CreateSpecificCulture("en-AU"), l.Locale);
			s.Delete(l);
			s.Flush();
			s.Close();
		}

		[Test]
		public void AutosaveChildren()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Baz baz = new Baz();
			baz.CascadingBars = new HashSet<BarProxy>();
			s.Save(baz);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			baz = (Baz) s.Load(typeof(Baz), baz.Code);
			baz.CascadingBars.Add(new Bar());
			baz.CascadingBars.Add(new Bar());
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			baz = (Baz) s.Load(typeof(Baz), baz.Code);
			Assert.AreEqual(2, baz.CascadingBars.Count);
			IEnumerator enumer = baz.CascadingBars.GetEnumerator();
			Assert.IsTrue(enumer.MoveNext());
			Assert.IsNotNull(enumer.Current);
			baz.CascadingBars.Clear(); // test all-delete-orphan
			s.Flush();

			Assert.AreEqual(0, s.CreateQuery("from Bar bar").List().Count);
			s.Delete(baz);
			t.Commit();
			s.Close();
		}

		[Test]
		public void OrphanDelete()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Baz baz = new Baz();
			IDictionary bars = new Hashtable();
			bars.Add(new Bar(), new object());
			bars.Add(new Bar(), new object());
			bars.Add(new Bar(), new object());
			s.Save(baz);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			baz = (Baz) s.Load(typeof(Baz), baz.Code);
			IEnumerator enumer = bars.GetEnumerator();
			enumer.MoveNext();
			bars.Remove(enumer.Current);
			s.Delete(baz);
			enumer.MoveNext();
			bars.Remove(enumer.Current);
			s.Flush();

			Assert.AreEqual(0, s.CreateQuery("from Bar bar").List().Count);
			t.Commit();
			s.Close();
		}

		[Test]
		public void TransientOrphanDelete()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Baz baz = new Baz();
			var bars = new HashSet<BarProxy> { new Bar(), new Bar(), new Bar() };
			baz.CascadingBars = bars;
			IList<Foo> foos = new List<Foo>();
			foos.Add(new Foo());
			foos.Add(new Foo());
			baz.FooBag = foos;
			s.Save(baz);

			IEnumerator enumer = new JoinedEnumerable(new IEnumerable[] {foos, bars}).GetEnumerator();
			while (enumer.MoveNext())
			{
				FooComponent cmp = ((Foo) enumer.Current).Component;
				s.Delete(cmp.Glarch);
				cmp.Glarch = null;
			}

			t.Commit();
			s.Close();

			var enumerBar = bars.GetEnumerator();
			enumerBar.MoveNext();
			bars.Remove(enumerBar.Current);
			foos.RemoveAt(1);
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(baz);
			Assert.AreEqual(2, s.CreateQuery("from Bar bar").List().Count);
			Assert.AreEqual(3, s.CreateQuery("from Foo foo").List().Count);
			t.Commit();
			s.Close();

			foos.RemoveAt(0);
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(baz);
			enumerBar = bars.GetEnumerator();
			enumerBar.MoveNext();
			bars.Remove(enumerBar.Current);
			s.Delete(baz);
			s.Flush();
			Assert.AreEqual(0, s.CreateQuery("from Foo foo").List().Count);
			t.Commit();
			s.Close();
		}

		[Test]
		public void ProxiesInCollections()
		{
			ISession s = OpenSession();
			Baz baz = new Baz();
			Bar bar = new Bar();
			Bar bar2 = new Bar();
			s.Save(bar);
			object bar2id = s.Save(bar2);
			baz.FooArray = new Foo[] {bar, bar2};

			bar = new Bar();
			s.Save(bar);
			baz.FooSet = new HashSet<FooProxy> { bar };
			baz.CascadingBars = new HashSet<BarProxy> { new Bar(), new Bar() };
			var list = new List<Foo>();
			list.Add(new Foo());
			baz.FooBag = list;
			object id = s.Save(baz);
			IEnumerator enumer = baz.CascadingBars.GetEnumerator();
			enumer.MoveNext();
			object bid = ((Bar) enumer.Current).Key;
			s.Flush();
			s.Close();

			s = OpenSession();
			BarProxy barprox = (BarProxy) s.Load(typeof(Bar), bid);
			BarProxy bar2prox = (BarProxy) s.Load(typeof(Bar), bar2id);
			Assert.IsTrue(bar2prox is INHibernateProxy);
			Assert.IsTrue(barprox is INHibernateProxy);
			baz = (Baz) s.Load(typeof(Baz), id);
			enumer = baz.CascadingBars.GetEnumerator();
			enumer.MoveNext();
			BarProxy b1 = (BarProxy) enumer.Current;
			enumer.MoveNext();
			BarProxy b2 = (BarProxy) enumer.Current;
			Assert.IsTrue(
				(b1 == barprox && !(b2 is INHibernateProxy))
				|| (b2 == barprox && !(b1 is INHibernateProxy))); //one-to-many
			Assert.IsTrue(baz.FooArray[0] is INHibernateProxy); //many-to-many
			Assert.AreEqual(bar2prox, baz.FooArray[1]);
			if (sessions.Settings.IsOuterJoinFetchEnabled)
			{
				enumer = baz.FooBag.GetEnumerator();
				enumer.MoveNext();
				Assert.IsFalse(enumer.Current is INHibernateProxy); // many-to-many outer-join="true"
			}

			enumer = baz.FooSet.GetEnumerator();
			enumer.MoveNext();
			Assert.IsFalse(enumer.Current is INHibernateProxy); //one-to-many
			s.Delete("from o in class Baz");
			s.Delete("from o in class Foo");
			s.Flush();
			s.Close();
		}

		// Not ported - testService() - not applicable to NHibernate

		[Test]
		public void PSCache()
		{
			using (ISession s = OpenSession())
			using(ITransaction txn = s.BeginTransaction())
			{
				for (int i = 0; i < 10; i++)
				{
					s.Save(new Foo());
				}

				IQuery q = s.CreateQuery("from f in class Foo");
				q.SetMaxResults(2);
				q.SetFirstResult(5);

				Assert.AreEqual(2, q.List().Count);

				q = s.CreateQuery("from f in class Foo");

				Assert.AreEqual(10, q.List().Count);
				Assert.AreEqual(10, q.List().Count);

				q.SetMaxResults(3);
				q.SetFirstResult(3);

				Assert.AreEqual(3, q.List().Count);

				q = s.CreateQuery("from f in class Foo");
				Assert.AreEqual(10, q.List().Count);
				txn.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction txn = s.BeginTransaction())
			{
				IQuery q = s.CreateQuery("from f in class Foo");
				Assert.AreEqual(10, q.List().Count);

				q.SetMaxResults(5);
				Assert.AreEqual(5, q.List().Count);

				s.Delete("from f in class Foo");
				txn.Commit();
			}
		}

		#region NHibernate specific tests

		[Test]
		public void Formula()
		{
			Foo foo = new Foo();
			ISession s = OpenSession();
			object id = s.Save(foo);
			s.Flush();
			s.Close();

			s = OpenSession();
			foo = (Foo) s.CreateQuery("from Foo as f where f.id = ?").SetParameter(0, id, NHibernateUtil.String).List()[0];
			Assert.AreEqual(4, foo.Formula, "should be 2x 'Int' property that is defaulted to 2");

			s.Delete(foo);
			s.Flush();
			s.Close();
		}

		/// <summary>
		/// This test verifies that the AddAll() method works
		/// correctly for a persistent Set.
		/// </summary>
		[Test]
		public void AddAll()
		{
			using (ISession s = OpenSession())
			{
				Foo foo1 = new Foo();
				s.Save(foo1);
				Foo foo2 = new Foo();
				s.Save(foo2);
				Foo foo3 = new Foo();
				s.Save(foo3);
				Baz baz = new Baz();
				baz.FooSet = new HashSet<FooProxy> { foo1 };
				s.Save(baz);
				Assert.AreEqual(1, baz.FooSet.Count);

				var foos = new List<FooProxy> { foo2, foo3 };
				baz.FooSet.UnionWith(foos);
				Assert.AreEqual(3, baz.FooSet.Count);

				s.Flush();

				// Clean up
				foreach (Foo foo in baz.FooSet)
				{
					s.Delete(foo);
				}

				s.Delete(baz);
				s.Flush();
			}
		}

		[Test]
		public void Copy()
		{
			Baz baz = new Baz();
			baz.SetDefaults();

			using (ISession s = OpenSession())
			{
				Baz persistentBaz = new Baz();
				s.Save(persistentBaz);
				s.Flush();

				baz.Code = persistentBaz.Code;
			}

			using (ISession s = OpenSession())
			{
				Baz persistentBaz = s.Get(typeof(Baz), baz.Code) as Baz;
				Baz copiedBaz = s.Merge(baz);
				Assert.AreSame(persistentBaz, copiedBaz);

				s.Delete(persistentBaz);
				s.Flush();
			}
		}

		[Test]
		public void ParameterInHavingClause()
		{
			using (ISession s = OpenSession())
			{
				s.CreateQuery("select f.id from Foo f group by f.id having count(f.id) >= ?")
					.SetInt32(0, 0)
					.List();
			}
		}

		// It's possible that this test only works on MS SQL Server. If somebody complains about
		// the test not working on their DB, I'll put an if around the code to only run on MS SQL.
		[Test]
		public void ParameterInOrderByClause()
		{
			using (ISession s = OpenSession())
			{
				s.CreateQuery("from Foo as foo order by case ? when 0 then foo.id else foo.id end")
					.SetInt32(0, 0)
					.List();
			}
		}

		#endregion
	}
}
