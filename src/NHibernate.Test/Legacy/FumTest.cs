using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.DomainModel;
using NHibernate.Criterion;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.Legacy
{
	/// <summary>
	/// FumTest handles testing Composite Ids.
	/// </summary>
	[TestFixture]
	public class FumTest : TestCase
	{
		protected static short fumKeyShort = 1;

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
						"Middle.hbm.xml"
					};
			}
		}

		[Test]
		public void CriteriaCollection()
		{
			//if( dialect is Dialect.HSQLDialect ) return;

			using (ISession s = OpenSession())
			{
				Fum fum = new Fum(FumKey("fum"));
				fum.FumString = "a value";
				fum.MapComponent.Fummap["self"] = fum;
				fum.MapComponent.Stringmap["string"] = "a staring";
				fum.MapComponent.Stringmap["string2"] = "a notha staring";
				fum.MapComponent.Count = 1;
				s.Save(fum);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				Fum b = (Fum) s.CreateCriteria(typeof(Fum))
								.Add(Expression.In(
										"FumString", new string[] {"a value", "no value"}))
								.UniqueResult();
				//Assert.IsTrue( NHibernateUtil.IsInitialized( b.MapComponent.Fummap ) );
				Assert.IsTrue(NHibernateUtil.IsInitialized(b.MapComponent.Stringmap));
				Assert.IsTrue(b.MapComponent.Fummap.Count == 1);
				Assert.IsTrue(b.MapComponent.Stringmap.Count == 2);

				int none = s.CreateCriteria(typeof(Fum))
					.Add(Expression.In("FumString", new string[0]))
					.List().Count;
				Assert.AreEqual(0, none);

				s.Delete(b);
				s.Flush();
			}
		}

		[Test]
		public void Criteria()
		{
			using (ISession s = OpenSession())
			using (ITransaction txn = s.BeginTransaction())
			{
				Fum fum = new Fum(FumKey("fum"));
				fum.Fo = new Fum(FumKey("fo"));
				fum.FumString = "fo fee fi";
				fum.Fo.FumString = "stuff";
				Fum fr = new Fum(FumKey("fr"));
				fr.FumString = "goo";
				Fum fr2 = new Fum(FumKey("fr2"));
				fr2.FumString = "soo";
				fum.Friends = new HashSet<Fum> { fr, fr2 };

				s.Save(fr);
				s.Save(fr2);
				s.Save(fum.Fo);
				s.Save(fum);

				ICriteria baseCriteria = s.CreateCriteria(typeof(Fum))
					.Add(Expression.Like("FumString", "f", MatchMode.Start));
				baseCriteria.CreateCriteria("Fo")
					.Add(Expression.IsNotNull("FumString"));
				baseCriteria.CreateCriteria("Friends")
					.Add(Expression.Like("FumString", "g%"));
				IList list = baseCriteria.List();

				Assert.AreEqual(1, list.Count);
				Assert.AreSame(fum, list[0]);

				baseCriteria = s.CreateCriteria(typeof(Fum))
					.Add(Expression.Like("FumString", "f%"))
					.SetResultTransformer(CriteriaSpecification.AliasToEntityMap);
				baseCriteria.CreateCriteria("Fo", "fo")
					.Add(Expression.IsNotNull("FumString"));
				baseCriteria.CreateCriteria("Friends", "fum")
					.Add(Expression.Like("FumString", "g", MatchMode.Start));
				IDictionary map = (IDictionary) baseCriteria.UniqueResult();

				Assert.AreSame(fum, map["this"]);
				Assert.AreSame(fum.Fo, map["fo"]);
				Assert.IsTrue(fum.Friends.Contains((Fum)map["fum"]));
				Assert.AreEqual(3, map.Count);

				baseCriteria = s.CreateCriteria(typeof(Fum))
					.Add(Expression.Like("FumString", "f%"))
					.SetResultTransformer(CriteriaSpecification.AliasToEntityMap)
					.SetFetchMode("Friends", FetchMode.Eager);
				baseCriteria.CreateCriteria("Fo", "fo")
					.Add(Expression.Eq("FumString", fum.Fo.FumString));
				map = (IDictionary) baseCriteria.List()[0];

				Assert.AreSame(fum, map["this"]);
				Assert.AreSame(fum.Fo, map["fo"]);
				Assert.AreEqual(2, map.Count);

				list = s.CreateCriteria(typeof(Fum))
					.CreateAlias("Friends", "fr")
					.CreateAlias("Fo", "fo")
					.Add(Expression.Like("FumString", "f%"))
					.Add(Expression.IsNotNull("Fo"))
					.Add(Expression.IsNotNull("fo.FumString"))
					.Add(Expression.Like("fr.FumString", "g%"))
					.Add(Expression.EqProperty("fr.id.Short", "id.Short"))
					.List();
				Assert.AreEqual(1, list.Count);
				Assert.AreSame(fum, list[0]);
				txn.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction txn = s.BeginTransaction())
			{
				ICriteria baseCriteria = s.CreateCriteria(typeof(Fum))
					.Add(Expression.Like("FumString", "f%"));
				baseCriteria.CreateCriteria("Fo")
					.Add(Expression.IsNotNull("FumString"));
				baseCriteria.CreateCriteria("Friends")
					.Add(Expression.Like("FumString", "g%"));
				Fum fum = (Fum) baseCriteria.List()[0];
				Assert.AreEqual(2, fum.Friends.Count);
				s.Delete(fum);
				s.Delete(fum.Fo);

				foreach (object friend in fum.Friends)
				{
					s.Delete(friend);
				}
				txn.Commit();
			}
		}

		[Test]
		public void ListIdentifiers()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();
			Fum fum = new Fum(FumKey("fum"));
			fum.FumString = "fo fee fi";
			s.Save(fum);

			fum = new Fum(FumKey("fi"));
			fum.FumString = "fee fi fo";
			s.Save(fum);

			// not doing a flush because the Find will do an auto flush unless we tell the session a 
			// different FlushMode
			IList list =
				s.CreateQuery("select fum.Id from fum in class NHibernate.DomainModel.Fum where not fum.FumString = 'FRIEND'").List();

			Assert.AreEqual(2, list.Count, "List Identifiers");

			IEnumerator enumerator =
				s.CreateQuery("select fum.Id from fum in class NHibernate.DomainModel.Fum where not fum.FumString='FRIEND'").
					Enumerable().GetEnumerator();
			int i = 0;
			while (enumerator.MoveNext())
			{
				Assert.IsTrue(enumerator.Current is FumCompositeID, "Iterating Identifiers");
				i++;
			}

			Assert.AreEqual(2, i, "Number of Ids found.");

			// clean up by deleting the 2 Fum objects that were added.
			s.Delete(s.Load(typeof(Fum), list[0]));
			s.Delete(s.Load(typeof(Fum), list[1]));
			txn.Commit();
			s.Close();
		}

		public static FumCompositeID FumKey(String str)
		{
			return FumKey(str, false);
		}

		public static FumCompositeID FumKey(String str, bool aCompositeQueryTest)
		{
			FumCompositeID id = new FumCompositeID();
			//			if( dialect is Dialect.MckoiDialect ) 
			//												{
			//													  GregorianCalendar now = new GregorianCalendar();
			//													  GregorianCalendar cal = new GregorianCalendar( 
			//														  now.get(java.util.Calendar.YEAR),
			//														  now.get(java.util.Calendar.MONTH),
			//														  now.get(java.util.Calendar.DATE) 
			//														  );
			//													  id.setDate( cal.getTime() );
			//												  }
			//			else 
			//			{
			id.Date = new DateTime(2004, 4, 29, 9, 0, 0, 0);
			//				 }
			id.String = str;

			if (aCompositeQueryTest)
			{
				id.Short = fumKeyShort++;
			}
			else
			{
				id.Short = (short) 12;
			}

			return id;
		}

		[Test]
		public void CompositeID()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Fum fum = new Fum(FumKey("fum"));
			fum.FumString = "fee fi fo";
			s.Save(fum);

			Assert.AreSame(fum, s.Load(typeof(Fum), FumKey("fum"), LockMode.Upgrade));
			//s.Flush();
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			fum = (Fum) s.Load(typeof(Fum), FumKey("fum"), LockMode.Upgrade);
			Assert.IsNotNull(fum, "Load by composite key");

			Fum fum2 = new Fum(FumKey("fi"));
			fum2.FumString = "fee fo fi";
			fum.Fo = fum2;
			s.Save(fum2);

			IList list = s.CreateQuery("from fum in class NHibernate.DomainModel.Fum where not fum.FumString='FRIEND'").List();
			Assert.AreEqual(2, list.Count, "Find a List of Composite Keyed objects");

			IList list2 =
				s.CreateQuery("select fum from fum in class NHibernate.DomainModel.Fum where fum.FumString='fee fi fo'").List();
			Assert.AreEqual(fum, (Fum) list2[0], "Find one Composite Keyed object");

			fum.Fo = null;
			//s.Flush();
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IEnumerator enumerator =
				s.CreateQuery("from fum in class NHibernate.DomainModel.Fum where not fum.FumString='FRIEND'").Enumerable().
					GetEnumerator();
			int i = 0;
			while (enumerator.MoveNext())
			{
				fum = (Fum) enumerator.Current;
				s.Delete(fum);
				i++;
			}

			Assert.AreEqual(2, i, "Iterate on Composite Key");
			//s.Flush();
			t.Commit();
			s.Close();
		}

		[Test]
		public void CompositeIDOneToOne()
		{
			ISession s = OpenSession();
			Fum fum = new Fum(FumKey("fum"));
			fum.FumString = "fee fi fo";
			//s.Save(fum); commented out in h2.0.3
			Fumm fumm = new Fumm();
			fumm.Fum = fum;
			s.Save(fumm);
			s.Flush();
			s.Close();

			s = OpenSession();
			fumm = (Fumm) s.Load(typeof(Fumm), FumKey("fum"));
			//s.delete(fumm.Fum); commented out in h2.0.3
			s.Delete(fumm);
			s.Flush();
			s.Close();
		}

		[Test]
		public void CompositeIDQuery()
		{
			ISession s = OpenSession();
			Fum fee = new Fum(FumKey("fee", true));
			fee.FumString = "fee";
			s.Save(fee);
			Fum fi = new Fum(FumKey("fi", true));
			fi.FumString = "fi";
			short fiShort = fi.Id.Short;
			s.Save(fi);
			Fum fo = new Fum(FumKey("fo", true));
			fo.FumString = "fo";
			s.Save(fo);
			Fum fum = new Fum(FumKey("fum", true));
			fum.FumString = "fum";
			s.Save(fum);
			s.Flush();
			s.Close();

			s = OpenSession();
			// Try to find the Fum object "fo" that we inserted searching by the string in the id
			IList vList = s.CreateQuery("from fum in class NHibernate.DomainModel.Fum where fum.Id.String='fo'").List();
			Assert.AreEqual(1, vList.Count, "find by composite key query (find fo object)");
			fum = (Fum) vList[0];
			Assert.AreEqual("fo", fum.Id.String, "find by composite key query (check fo object)");

			// Try to fnd the Fum object "fi" that we inserted by searching the date in the id
			vList =
				s.CreateQuery("from fum in class NHibernate.DomainModel.Fum where fum.Id.Short = ?").SetInt16(0, fiShort).List();
			Assert.AreEqual(1, vList.Count, "find by composite key query (find fi object)");
			fi = (Fum) vList[0];
			Assert.AreEqual("fi", fi.Id.String, "find by composite key query (check fi object)");

			// make sure we can return all of the objects by searching by the date id
			vList =
				s.CreateQuery("from fum in class NHibernate.DomainModel.Fum where fum.Id.Date <= ? and not fum.FumString='FRIEND'").
					SetDateTime(0, DateTime.Now).List();
			Assert.AreEqual(4, vList.Count, "find by composite key query with arguments");
			s.Flush();
			s.Close();

			s = OpenSession();
			Assert.IsTrue(
				s.CreateQuery("select fum.Id.Short, fum.Id.Date, fum.Id.String from fum in class NHibernate.DomainModel.Fum").
					Enumerable().GetEnumerator().MoveNext());
			Assert.IsTrue(
				s.CreateQuery("select fum.Id from fum in class NHibernate.DomainModel.Fum").Enumerable().GetEnumerator().MoveNext());

			IQuery qu =
				s.CreateQuery("select fum.FumString, fum, fum.FumString, fum.Id.Date from fum in class NHibernate.DomainModel.Fum");
			IType[] types = qu.ReturnTypes;
			Assert.AreEqual(4, types.Length);
			for (int k = 0; k < types.Length; k++)
			{
				Assert.IsNotNull(types[k]);
			}
			Assert.IsTrue(types[0] is StringType);
			Assert.IsTrue(types[1] is EntityType);
			Assert.IsTrue(types[2] is StringType);
			Assert.IsTrue(types[3] is DateTimeType);
			IEnumerator enumer = qu.Enumerable().GetEnumerator();
			int j = 0;
			while (enumer.MoveNext())
			{
				j++;
				Assert.IsTrue(((object[]) enumer.Current)[1] is Fum);
			}
			Assert.AreEqual(8, j, "iterate on composite key");

			fum = (Fum) s.Load(typeof(Fum), fum.Id);
			s.CreateFilter(fum.QuxArray, "where this.Foo is null").List();
			s.CreateFilter(fum.QuxArray, "where this.Foo.id = ?").SetString(0, "fooid");
			IQuery f = s.CreateFilter(fum.QuxArray, "where this.Foo.id = :fooId");
			f.SetString("fooId", "abc");
			Assert.IsFalse(f.Enumerable().GetEnumerator().MoveNext());

			enumer =
				s.CreateQuery("from fum in class NHibernate.DomainModel.Fum where not fum.FumString='FRIEND'").Enumerable().
					GetEnumerator();
			int i = 0;
			while (enumer.MoveNext())
			{
				fum = (Fum) enumer.Current;
				s.Delete(fum);
				i++;
			}
			Assert.AreEqual(4, i, "iterate on composite key");
			s.Flush();

			s.CreateQuery(
				"from fu in class Fum, fo in class Fum where fu.Fo.Id.String = fo.Id.String and fo.FumString is not null").
				Enumerable();
			s.CreateQuery("from Fumm f1 inner join f1.Fum f2").List();
			s.Close();
		}

		[Test]
		public void CompositeIDCollections()
		{
			ISession s = OpenSession();
			Fum fum1 = new Fum(FumKey("fum1"));
			Fum fum2 = new Fum(FumKey("fum2"));
			fum1.FumString = "fee fo fi";
			fum2.FumString = "fee fo fi";
			s.Save(fum1);
			s.Save(fum2);
			Qux q = new Qux();
			s.Save(q);
			q.Fums = new HashSet<Fum> {fum1, fum2};
			q.MoreFums = new List<Fum> {fum1};
			fum1.QuxArray = new Qux[] {q};
			s.Flush();
			s.Close();

			s = OpenSession();
			q = (Qux) s.Load(typeof(Qux), q.Key);
			Assert.AreEqual(2, q.Fums.Count, "collection of fums");
			Assert.AreEqual(1, q.MoreFums.Count, "collection of fums");
			Assert.AreSame(q, ((Fum) q.MoreFums[0]).QuxArray[0], "unkeyed composite id collection");
			IEnumerator enumer = q.Fums.GetEnumerator();
			enumer.MoveNext();
			s.Delete((Fum) enumer.Current);
			enumer.MoveNext();
			s.Delete((Fum) enumer.Current);
			s.Delete(q);
			s.Flush();
			s.Close();
		}

		[Test]
		public void DeleteOwner()
		{
			ISession s = OpenSession();
			Qux q = new Qux();
			s.Save(q);
			Fum f1 = new Fum(FumKey("f1"));
			Fum f2 = new Fum(FumKey("f2"));
			f1.FumString = "f1";
			f2.FumString = "f2";
			q.Fums = new HashSet<Fum> {f1, f2};
			q.MoreFums = new List<Fum> {f1, f2};
			s.Save(f1);
			s.Save(f2);
			s.Flush();
			s.Close();

			s = OpenSession();
			ITransaction t = s.BeginTransaction();
			q = (Qux) s.Load(typeof(Qux), q.Key, LockMode.Upgrade);
			s.Lock(q, LockMode.Upgrade);
			s.Delete(q);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			var list = s.CreateQuery("from fum in class NHibernate.DomainModel.Fum where not fum.FumString='FRIEND'").List();
			Assert.AreEqual(2, list.Count, "deleted owner");
			s.Lock(list[0], LockMode.Upgrade);
			s.Lock(list[1], LockMode.Upgrade);
			foreach (object obj in list)
			{
				s.Delete(obj);
			}
			t.Commit();
			s.Close();
		}

		[Test]
		public void CompositeIDs()
		{
			ISession s = OpenSession();
			Fo fo = Fo.NewFo();
			s.Save(fo, FumKey("an instance of fo"));
			s.Flush();
			s.Close();

			s = OpenSession();
			fo = (Fo) s.Load(typeof(Fo), FumKey("an instance of fo"));
			fo.X = 5;
			s.Flush();
			s.Close();

			s = OpenSession();
			fo = (Fo) s.Load(typeof(Fo), FumKey("an instance of fo"));
			Assert.AreEqual(5, fo.X);
			IEnumerator enumer =
				s.CreateQuery("from fo in class NHibernate.DomainModel.Fo where fo.id.String like 'an instance of fo'").Enumerable()
					.GetEnumerator();
			Assert.IsTrue(enumer.MoveNext());
			Assert.AreSame(fo, enumer.Current);
			s.Delete(fo);
			s.Flush();
			try
			{
				s.Save(Fo.NewFo());
				Assert.Fail("should not get here");
			}
			catch (Exception e)
			{
				Assert.IsNotNull(e);
			}
			s.Close();
		}


		[Test]
		public void KeyManyToOne()
		{
			ISession s = OpenSession();
			Inner sup = new Inner();
			InnerKey sid = new InnerKey();
			sup.Dudu = "dudu";
			sid.AKey = "a";
			sid.BKey = "b";
			sup.Id = sid;
			Middle m = new Middle();
			MiddleKey mid = new MiddleKey();
			mid.One = "one";
			mid.Two = "two";
			mid.Sup = sup;
			m.Id = mid;
			m.Bla = "bla";
			Outer d = new Outer();
			OuterKey did = new OuterKey();
			did.Master = m;
			did.DetailId = "detail";
			d.Id = did;
			d.Bubu = "bubu";
			s.Save(sup);
			s.Save(m);
			s.Save(d);
			s.Flush();
			s.Close();

			s = OpenSession();
			d = (Outer) s.Load(typeof(Outer), did);
			Assert.AreEqual("dudu", d.Id.Master.Id.Sup.Dudu);
			s.Delete(d);
			s.Delete(d.Id.Master);
			s.Save(d.Id.Master);
			s.Save(d);
			s.Flush();
			s.Close();

			s = OpenSession();
			d = (Outer) s.CreateQuery("from Outer o where o.id.DetailId=?").SetString(0, d.Id.DetailId).List()[0];
			s.CreateQuery("from Outer o where o.Id.Master.Id.Sup.Dudu is not null").List();
			s.CreateQuery("from Outer o where o.Id.Master.Bla = ''").List();
			s.CreateQuery("from Outer o where o.Id.Master.Id.One = ''").List();
			s.Delete(d);
			s.Delete(d.Id.Master);
			s.Delete(d.Id.Master.Id.Sup);
			s.Flush();
			s.Close();
		}


		[Test]
		public void CompositeKeyPathExpressions()
		{
			using (ISession s = OpenSession())
			{
				s.CreateQuery("select fum1.Fo from fum1 in class Fum where fum1.Fo.FumString is not null").List();
				s.CreateQuery("from fum1 in class Fum where fum1.Fo.FumString is not null order by fum1.Fo.FumString").List();
				if (Dialect.SupportsSubSelects)
				{
					s.CreateQuery("from fum1 in class Fum where exists elements(fum1.Friends)").List();
					s.CreateQuery("from fum1 in class Fum where size(fum1.Friends) = 0").List();
				}
				s.CreateQuery("select elements(fum1.Friends) from fum1 in class Fum").List();
				s.CreateQuery("from fum1 in class Fum, fr in elements( fum1.Friends )").List();
			}
		}

		[Test]
		public void UnflushedSessionSerialization()
		{
			///////////////////////////////////////////////////////////////////////////
			// Test insertions across serializations

			ISession s2;

			// NOTE: H2.1 has getSessions().openSession() here (and below),
			// instead of just the usual openSession()
			using (ISession s = sessions.OpenSession())
			{
				s.FlushMode = FlushMode.Manual;

				Simple simple = new Simple();
				simple.Address = "123 Main St. Anytown USA";
				simple.Count = 1;
				simple.Date = new DateTime(2005, 1, 1);
				simple.Name = "My UnflushedSessionSerialization Simple";
				simple.Pay = 5000.0f;

				s.Save(simple, 10L);

				// Now, try to serialize session without flushing...
				s.Disconnect();

				s2 = SpoofSerialization(s);
			}

			Simple check, other;

			using (ISession s = s2)
			{
				s.Reconnect();

				Simple simple = (Simple) s.Load(typeof(Simple), 10L);
				other = new Simple();
				other.Init();
				s.Save(other, 11L);

				simple.Other = other;
				s.Flush();

				check = simple;
			}

			///////////////////////////////////////////////////////////////////////////
			// Test updates across serializations

			using (ISession s = sessions.OpenSession())
			{
				s.FlushMode = FlushMode.Manual;
				Simple simple = (Simple) s.Get(typeof(Simple), 10L);
				Assert.AreEqual(check.Name, simple.Name, "Not same parent instances");
				Assert.AreEqual(check.Other.Name, other.Name, "Not same child instances");

				simple.Name = "My updated name";

				s.Disconnect();
				s2 = SpoofSerialization(s);

				check = simple;
			}

			using (ISession s = s2)
			{
				s.Reconnect();
				s.Flush();
			}

			///////////////////////////////////////////////////////////////////////////
			// Test deletions across serializations
			using (ISession s = sessions.OpenSession())
			{
				s.FlushMode = FlushMode.Manual;
				Simple simple = (Simple) s.Get(typeof(Simple), 10L);
				Assert.AreEqual(check.Name, simple.Name, "Not same parent instances");
				Assert.AreEqual(check.Other.Name, other.Name, "Not same child instances");

				// Now, lets delete across serialization...
				s.Delete(simple);

				s.Disconnect();
				s2 = SpoofSerialization(s);
			}

			using (ISession s = s2)
			{
				s.Reconnect();
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				s.Delete("from Simple");
				s.Flush();
			}
		}

		private ISession SpoofSerialization(ISession session)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, session);

			stream.Position = 0;

			return (ISession) formatter.Deserialize(stream);
		}
	}
}