using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Dialect;
using NHibernate.DomainModel;
using NHibernate.Engine;
using NHibernate.Criterion;
using NHibernate.Mapping;
using NUnit.Framework;
using Single=NHibernate.DomainModel.Single;

namespace NHibernate.Test.Legacy
{
	/// <summary>
	/// Summary description for MasterDetailTest.
	/// </summary>
	[TestFixture]
	public class MasterDetailTest : TestCase
	{
		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"MasterDetail.hbm.xml",
						"Custom.hbm.xml",
						"Category.hbm.xml",
						"INameable.hbm.xml",
						"SingleSeveral.hbm.xml",
						"WZ.hbm.xml",
						"UpDown.hbm.xml",
						"Eye.hbm.xml",
						"MN.hbm.xml"
					};
			}
		}

		[Test]
		public void ParentChildren()
		{
			ISession session = OpenSession();

			M parent = new M();
			N n = new N();
			n.String = "0";
			parent.AddChildren(n);
			session.SaveOrUpdate(parent);
			parent.RemoveChildren(n);

			session.Flush();
			session.Close();

			session = OpenSession();
			Assert.AreEqual(0, session.CreateQuery("from N").List().Count);
			session.Delete("from M");
			session.Flush();
			session.Close();
		}

		[Test]
		public void OuterJoin()
		{
			ISession s = OpenSession();
			Eye e = new Eye();
			e.Name = "Eye Eye";
			Jay jay = new Jay(e);
			e.Jay = jay;
			s.SaveOrUpdate(e);
			s.Flush();
			s.Close();

			s = OpenSession();
			e = (Eye) s.CreateCriteria(typeof(Eye)).UniqueResult();
			Assert.IsTrue(NHibernateUtil.IsInitialized(e.Jay));
			Assert.IsTrue(NHibernateUtil.IsInitialized(e.Jays));
			s.Close();

			s = OpenSession();
			jay = (Jay) s.CreateQuery("select new Jay(eye) from Eye eye").UniqueResult();
			Assert.AreEqual("Eye Eye", jay.Eye.Name);
			s.Delete(jay.Eye);
			s.Flush();
			s.Close();
		}

		[Test]
		public void Meta()
		{
			PersistentClass clazz = cfg.GetClassMapping(typeof(Master));
			MetaAttribute meta = clazz.GetMetaAttribute("foo");
			Assert.AreEqual("foo", meta.Value);
			meta = clazz.GetProperty("Name").GetMetaAttribute("bar");
			Assert.IsTrue(meta.IsMultiValued);
		}

		[Test]
		public void Copy()
		{
			Category catWA = new Category();
			catWA.Name = "HSQL workaround";
			Category cat = new Category();
			cat.Name = "foo";
			Category subCatBar = new Category();
			subCatBar.Name = "bar";
			Category subCatBaz = new Category();
			subCatBaz.Name = "baz";
			cat.Subcategories.Add(subCatBar);
			cat.Subcategories.Add(subCatBaz);

			using (ISession s = OpenSession())
			{
				s.Save(catWA);
				s.Save(cat);
				s.Flush();
			}

			cat.Name = "new foo";
			subCatBar.Name = "new bar";
			cat.Subcategories.Remove(subCatBaz);
			Category newCat = new Category();
			newCat.Name = "new";
			cat.Subcategories.Add(newCat);
			Category newSubCat = new Category();
			newSubCat.Name = "new sub";
			newCat.Subcategories.Add(newSubCat);

			Category copiedCat;
			using (ISession s = OpenSession())
			{
				copiedCat = s.Merge(cat);
				s.Flush();
			}
			Assert.IsFalse(copiedCat == cat);
			Assert.IsTrue(cat.Subcategories.Contains(newCat));

			using (ISession s = OpenSession())
			{
				cat = (Category) s.CreateQuery("from Category cat where cat.Name='new foo'").UniqueResult();
				newSubCat = (Category)s.CreateQuery("from Category cat left join fetch cat.Subcategories where cat.Name='new sub'").UniqueResult();
				Assert.AreEqual("new sub", newSubCat.Name);
			}
			newSubCat.Subcategories.Add(cat);
			cat.Name="new new foo";

			using (ISession s = OpenSession())
			{
				newSubCat = s.Merge(newSubCat);
				Assert.IsTrue(newSubCat.Name.Equals("new sub"));
				Assert.AreEqual(1, newSubCat.Subcategories.Count);
				cat	= (Category)newSubCat.Subcategories[0];
				Assert.AreEqual("new new foo", cat.Name);
				newSubCat.Subcategories.Remove(cat);
				s.Delete(cat);
				s.Delete(subCatBaz);
				s.Delete(catWA);
				s.Flush();
			}
		}

		[Test]
		public void CopyCascade()
		{
			ISession s = OpenSession();
			Category child = new Category();
			child.Name = "child";
			s.Save(child);
			s.Flush();
			s.Close();

			s = OpenSession();
			Category parent = new Category();
			parent.Name = "parent";
			parent.Subcategories.Add(child);
			child.Name = "child2";

			// Save parent and cascade update detached child
			Category persistentParent = s.Merge(parent);
			Assert.IsTrue(persistentParent.Subcategories.Count == 1);
			Assert.AreEqual(((Category) persistentParent.Subcategories[0]).Name, "child2");
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Delete(persistentParent);
			s.Flush();
			s.Close();
		}

		[Test]
		public void NotNullDiscriminator()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Up up = new Up();
			up.Id1 = "foo";
			up.Id2 = 1231;
			Down down = new Down();
			down.Id1 = "foo";
			down.Id2 = 3211;
			down.Value = 123123121;
			s.Save(up);
			s.Save(down);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IList list = s.CreateQuery("from Up up order by up.Id2 asc").List();
			Assert.IsTrue(list.Count == 2);
			Assert.IsFalse(list[0] is Down);
			Assert.IsTrue(list[1] is Down);
			list = s.CreateQuery("from Down down").List();
			Assert.IsTrue(list.Count == 1);
			Assert.IsTrue(list[0] is Down);
			s.Delete("from Up up");
			t.Commit();
			s.Close();
		}

		[Test]
		public void SelfManyToOne()
		{
			// add a check to not run if HSQLDialect

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Master m = new Master();
			m.OtherMaster = m;
			s.Save(m);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IEnumerator enumer = s.CreateQuery("from m in class Master").Enumerable().GetEnumerator();
			enumer.MoveNext();
			m = (Master) enumer.Current;
			Assert.AreSame(m, m.OtherMaster);

			if (Dialect is MySQLDialect)
			{
				m.OtherMaster = null;
				s.Flush();
			}

			s.Delete(m);
			t.Commit();
			s.Close();
		}

		[Test]
		public void ExampleTest()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Master m = new Master();
			m.Name = "name";
			m.X = 5;
			m.OtherMaster = m;
			s.Save(m);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			Master m1 = (Master) s.CreateCriteria(typeof(Master))
			                     	.Add(Example.Create(m).EnableLike().IgnoreCase())
			                     	.UniqueResult();
			Assert.AreSame(m1.OtherMaster, m1);

			m1 = (Master) s.CreateCriteria(typeof(Master))
			              	.Add(Expression.Eq("Name", "foobar"))
			              	.UniqueResult();
			Assert.IsNull(m1);

			m1 = (Master) s.CreateCriteria(typeof(Master))
			              	.Add(Example.Create(m))
			              	.CreateCriteria("OtherMaster")
			              	.Add(Example.Create(m).ExcludeZeroes())
			              	.UniqueResult();
			Assert.AreSame(m1.OtherMaster, m1);
			Master m2 = (Master) s.CreateCriteria(typeof(Master))
			                     	.Add(Example.Create(m).ExcludeNone())
			                     	.UniqueResult();
			Assert.AreSame(m1, m2);

			m.Name = null;
			m2 = (Master) s.CreateCriteria(typeof(Master))
			              	.Add(Example.Create(m).ExcludeNone())
			              	.UniqueResult();
			Assert.IsNull(m2);

			if (Dialect is MySQLDialect)
			{
				m1.OtherMaster = null;
				s.Flush();
			}

			s.Delete(m1);
			t.Commit();
			s.Close();
		}

		[Test]
		public void NonLazyBidirectional()
		{
			Single sin = new Single();
			sin.Id = "asfdfds";
			sin.String = "adsa asdfasd";
			Several sev = new Several();
			sev.Id = "asdfasdfasd";
			sev.String = "asd ddd";
			sin.Several.Add(sev);
			sev.Single = sin;

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(sin);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				sin = (Single) s.Load(typeof(Single), sin);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				sev = (Several) s.Load(typeof(Several), sev);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("from s in class Several").List();
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("from s in class Single").List();
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from Single");
				t.Commit();
			}
		}

		[Test]
		public void CollectionQuery()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			const string path = "m.Details";

			if (Dialect.SupportsSubSelects)
			{
				s.CreateQuery("FROM m IN CLASS Master WHERE NOT EXISTS ( FROM " + path + " d WHERE NOT d.I=5 )").Enumerable();
				s.CreateQuery("FROM m IN CLASS Master WHERE NOT 5 IN ( SELECT d.I FROM d IN  " + path + " )").Enumerable();
			}

			s.CreateQuery("SELECT m FROM m in CLASS NHibernate.DomainModel.Master join m.Details d WHERE d.I=5").Enumerable();
			s.CreateQuery("SELECT m FROM m in CLASS NHibernate.DomainModel.Master join m.Details d WHERE d.I=5").List();
			s.CreateQuery("SELECT m.id FROM m IN CLASS NHibernate.DomainModel.Master join m.Details d WHERE d.I=5").List();
			t.Commit();
			s.Close();
		}

		[Test]
		public void MasterDetail()
		{
			//if( dialect is Dialect.HSQLDialect ) return;

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Master master = new Master();
			Assert.IsNotNull(s.Save(master), "save returned native id");
			object mid = s.GetIdentifier(master);
			Detail d1 = new Detail();
			d1.Master = master;
			object did = s.Save(d1);
			Detail d2 = new Detail();
			d2.I = 12;
			d2.Master = master;
			Assert.IsNotNull(s.Save(d2), "generated id returned");
			master.AddDetail(d1);
			master.AddDetail(d2);

			if (Dialect.SupportsSubSelects)
			{
				string hql = "from d in class NHibernate.DomainModel.Detail, m in class NHibernate.DomainModel.Master " +
				             "where m = d.Master and m.Outgoing.size = 0 and m.Incoming.size = 0";

				Assert.AreEqual(2, s.CreateQuery(hql).List().Count, "query");
			}
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			master = (Master) s.Load(typeof(Master), mid);
			IEnumerator enumer = master.Details.GetEnumerator();
			int i = 0;
			while (enumer.MoveNext())
			{
				Detail d = (Detail) enumer.Current;
				Assert.AreSame(master, d.Master, "master-detail");
				i++;
			}
			Assert.AreEqual(2, i, "master-detail count");
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			Assert.AreEqual(2, s.CreateQuery("select elements(master.Details) from Master master").List().Count);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IList list = s.CreateQuery("from Master m left join fetch m.Details").List();
			Master m = (Master) list[0];
			Assert.IsTrue(NHibernateUtil.IsInitialized(m.Details), "joined fetch should initialize collection");
			Assert.AreEqual(2, m.Details.Count);
			list = s.CreateQuery("from Detail d inner join fetch d.Master").List();
			Detail dt = (Detail) list[0];
			object dtid = s.GetIdentifier(dt);
			Assert.AreSame(m, dt.Master);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			list = s.CreateQuery("select m from Master m1, Master m left join fetch m.Details where m.Name=m1.Name").List();
			Assert.IsTrue(NHibernateUtil.IsInitialized(((Master) list[0]).Details));
			dt = (Detail) s.Load(typeof(Detail), dtid);
			Assert.IsTrue(((Master) list[0]).Details.Contains(dt));
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			list =
				s.CreateQuery("select m, m1.Name from Master m1, Master m left join fetch m.Details where m.Name=m1.Name").List();
			Master masterFromHql = (Master) ((object[]) list[0])[0];
			Assert.IsTrue(NHibernateUtil.IsInitialized(masterFromHql.Details));
			dt = (Detail) s.Load(typeof(Detail), dtid);
			Assert.IsTrue(masterFromHql.Details.Contains(dt));

			// This line is commentend in H3.2 tests because it work in the classic parser
			// even if it as no sense ('join fetch' where the 'select' is a scalar)
			// The AST check the case with an Exception
			//list = s.CreateQuery("select m.id from Master m inner join fetch m.Details").List();

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			Detail dd = (Detail) s.Load(typeof(Detail), did);
			master = dd.Master;
			Assert.IsTrue(master.Details.Contains(dd), "detail-master");
			Assert.AreEqual(2, s.CreateFilter(master.Details, "order by this.I desc").List().Count);
			Assert.AreEqual(2, s.CreateFilter(master.Details, "select this where this.id > -1").List().Count);

			IQuery q = s.CreateFilter(master.Details, "where this.id > :id");
			q.SetInt32("id", -1);
			Assert.AreEqual(2, q.List().Count);

			q = s.CreateFilter(master.Details, "where this.id > :id1 and this.id < :id2");
			q.SetInt32("id1", -1);
			q.SetInt32("id2", 99999999);
			Assert.AreEqual(2, q.List().Count);
			q.SetInt32("id2", -1);
			Assert.AreEqual(0, q.List().Count);

			q = s.CreateFilter(master.Details, "where this.id in (:ids)");
			q.SetParameterList("ids", new[] {did, (long) -1});

			Assert.AreEqual(1, q.List().Count);
			Assert.IsTrue(q.Enumerable().GetEnumerator().MoveNext());

			Assert.AreEqual(2, s.CreateFilter(master.Details, "where this.id > -1").List().Count);
			Assert.AreEqual(2, s.CreateFilter(master.Details, "select this.Master where this.id > -1").List().Count);
			Assert.AreEqual(2,
			                s.CreateFilter(master.Details, "select m from m in class Master where this.id > -1 and this.Master=m")
			                	.List().Count);
			Assert.AreEqual(0, s.CreateFilter(master.Incoming, "where this.id > -1 and this.Name is not null").List().Count);

			IQuery filter = s.CreateFilter(master.Details, "select max(this.I)");
			enumer = filter.Enumerable().GetEnumerator();
			Assert.IsTrue(enumer.MoveNext());
			Assert.IsTrue(enumer.Current is Int32);

			filter = s.CreateFilter(master.Details, "select max(this.I) group by this.id");
			enumer = filter.Enumerable().GetEnumerator();
			Assert.IsTrue(enumer.MoveNext());
			Assert.IsTrue(enumer.Current is Int32);

			filter = s.CreateFilter(master.Details, "select count(*)");
			enumer = filter.Enumerable().GetEnumerator();
			Assert.IsTrue(enumer.MoveNext());
			Assert.IsTrue(enumer.Current is Int64);

			Assert.AreEqual(2, s.CreateFilter(master.Details, "select this.Master").List().Count);

			IQuery f = s.CreateFilter(master.Details, "select max(this.I) where this.I < :top and this.I>=:bottom");
			f.SetInt32("top", 100);
			f.SetInt32("bottom", 0);

			enumer = f.Enumerable().GetEnumerator();
			Assert.IsTrue(enumer.MoveNext());
			Assert.AreEqual(12, enumer.Current);

			f.SetInt32("top", 2);
			enumer = f.Enumerable().GetEnumerator();
			Assert.IsTrue(enumer.MoveNext());
			Assert.AreEqual(0, enumer.Current);

			f = s.CreateFilter(master.Details, "select max(this.I) where this.I not in (:list)");
			f.SetParameterList("list", new List<int> {-666, 22, 0});
			enumer = f.Enumerable().GetEnumerator();
			Assert.IsTrue(enumer.MoveNext());
			Assert.AreEqual(12, enumer.Current);

			i = 0;
			foreach (Detail d in master.Details)
			{
				Assert.AreSame(master, d.Master, "master-detail");
				s.Delete(d);
				i++;
			}

			Assert.AreEqual(2, i, "master-detail");
			s.Delete(master);
			t.Commit();
			s.Close();
		}

		[Test]
		public void IncomingOutgoing()
		{
			//if HSQLDialect skip test

			ISession s = OpenSession();
			Master master1 = new Master();
			Master master2 = new Master();
			Master master3 = new Master();
			s.Save(master1);
			s.Save(master2);
			s.Save(master3);
			master1.AddIncoming(master2);
			master2.AddOutgoing(master1);
			master1.AddIncoming(master3);
			master3.AddOutgoing(master1);
			object m1id = s.GetIdentifier(master1);

			Assert.AreEqual(2, s.CreateFilter(master1.Incoming, "where this.id > 0 and this.Name is not null").List().Count);
			s.Flush();
			s.Close();

			s = OpenSession();
			master1 = (Master) s.Load(typeof(Master), m1id);
			int i = 0;
			foreach (Master m in master1.Incoming)
			{
				Assert.AreEqual(1, m.Outgoing.Count, "outgoing");
				Assert.IsTrue(m.Outgoing.Contains(master1), "outgoing");
				s.Delete(m);
				i++;
			}

			Assert.AreEqual(2, i, "incoming-outgoing");
			s.Delete(master1);
			s.Flush();
			s.Close();
		}

		[Test]
		public void Cascading()
		{
			//HSQLDialect return;

			ISession s = OpenSession();
			Detail d1 = new Detail();
			Detail d2 = new Detail();
			d2.I = 22;
			Master m = new Master();
			Master m0 = new Master();
			object m0id = s.Save(m0);
			m0.AddDetail(d1);
			m0.AddDetail(d2);
			d1.Master = m0;
			d2.Master = m0;
			m.MoreDetails.Add(d1);
			m.MoreDetails.Add(d2);
			object mid = s.Save(m);
			s.Flush();
			s.Close();

			s = OpenSession();
			m = (Master) s.Load(typeof(Master), mid);
			Assert.AreEqual(2, m.MoreDetails.Count, "cascade save");
			IEnumerator enumer = m.MoreDetails.GetEnumerator();
			enumer.MoveNext();
			Assert.AreEqual(2, ((Detail) enumer.Current).Master.Details.Count, "cascade save");

			s.Delete(m);
			s.Delete(s.Load(typeof(Master), m0id));
			s.Flush();
			s.Close();
		}

		[Test]
		public void NamedQuery()
		{
			ISession s = OpenSession();
			IQuery q = s.GetNamedQuery("all_details");
			q.List();
			s.Close();
		}

		[Test]
		public void Serialization()
		{
			ISession s = OpenSession();
			Master m = new Master();
			Detail d1 = new Detail();
			Detail d2 = new Detail();
			object mid = s.Save(m);
			d1.Master = (m);
			d2.Master = (m);
			m.AddDetail(d1);
			m.AddDetail(d2);
			if (Dialect is MsSql2000Dialect)
			{
				s.Save(d1);
			}
			else
			{
				s.Save(d1, 666L);
			}
			s.Flush();
			s.Disconnect();
			MemoryStream stream = new MemoryStream();
			BinaryFormatter f = new BinaryFormatter();
			f.Serialize(stream, s);
			s.Close();
			stream.Position = 0;
			Console.WriteLine(stream.Length);

			s = (ISession) f.Deserialize(stream);
			stream.Close();

			s.Reconnect();
			Master m2 = (Master) s.Load(typeof(Master), mid);
			Assert.IsTrue(m2.Details.Count == 2, "serialized state");
			foreach (Detail d in m2.Details)
			{
				Assert.IsTrue(d.Master == m2, "deserialization");
				try
				{
					s.GetIdentifier(d);
					s.Delete(d);
				}
				catch (Exception)
				{
					// Why are we ignoring exceptions here? /Oskar 2014-08-24
				}
			}
			s.Delete(m2);
			s.Flush();
			s.Close();

			s = OpenSession();
			mid = s.Save(new Master());
			object mid2 = s.Save(new Master());
			s.Flush();
			s.Disconnect();
			stream = new MemoryStream();
			f.Serialize(stream, s);
			s.Close();
			stream.Position = 0;

			s = (ISession) f.Deserialize(stream);
			stream.Close();

			s.Reconnect();
			s.Delete(s.Load(typeof(Master), mid));
			s.Delete(s.Load(typeof(Master), mid2));
			s.Flush();
			s.Close();

			s = OpenSession();
			string db = s.Connection.Database; //force session to grab a connection
			try
			{
				stream = new MemoryStream();
				f.Serialize(stream, s);
			}
			catch (Exception e)
			{
				Assert.IsTrue(e is InvalidOperationException, "illegal state");
				s.Close();
				return;
			}
			finally
			{
				stream.Close();
			}
			Assert.IsTrue(false, "serialization should have failed");
		}

		[Test]
		public void UpdateLazyCollections()
		{
			// if (dialect is HSQLDialect) return;
			ISession s = OpenSession();
			Master m = new Master();
			Detail d1 = new Detail();
			Detail d2 = new Detail();
			d2.X = 14;
			object mid = s.Save(m);
			// s.Flush();  commented out in h2.0.3 also
			d1.Master = m;
			d2.Master = m;
			m.AddDetail(d1);
			m.AddDetail(d2);
			if (Dialect is MsSql2000Dialect)
			{
				s.Save(d1);
				s.Save(d2);
			}
			else
			{
				s.Save(d1, 666L);
				s.Save(d2, 667L);
			}
			s.Flush();
			s.Close();

			s = OpenSession();
			m = (Master) s.Load(typeof(Master), mid);
			s.Close();
			m.Name = "New Name";
			s = OpenSession();
			s.Update(m, mid);
			IEnumerator enumer = m.Details.GetEnumerator();
			int i = 0;
			while (enumer.MoveNext())
			{
				Assert.IsNotNull(enumer.Current);
				i++;
			}
			Assert.AreEqual(2, i);

			enumer = m.Details.GetEnumerator();
			while (enumer.MoveNext())
			{
				s.Delete(enumer.Current);
			}
			s.Delete(m);
			s.Flush();
			s.Close();
		}

		[Test]
		public void MultiLevelCascade()
		{
			//if( dialect is Dialect.HSQLDialect ) return;

			object mid;
			object m0id;

			using (ISession s = OpenSession())
			{
				Detail detail = new Detail();
				SubDetail subdetail = new SubDetail();
				Master m = new Master();
				Master m0 = new Master();
				m0id = s.Save(m0);
				m0.AddDetail(detail);
				detail.Master = m0;
				m.MoreDetails.Add(detail);
				detail.SubDetails = new HashSet<SubDetail> { subdetail };
				mid = s.Save(m);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				Master m = (Master) s.Load(typeof(Master), mid);
				IEnumerator enumer = m.MoreDetails.GetEnumerator();
				enumer.MoveNext();
				Assert.IsTrue(((Detail) enumer.Current).SubDetails.Count != 0);
				s.Delete(m);
				s.Delete(s.Load(typeof(Master), m0id));
				s.Flush();
			}
		}

		[Test]
		public void MixNativeAssigned()
		{
			// if HSQLDialect then skip test
			ISession s = OpenSession();
			Category c = new Category();
			c.Name = "NAME";
			Assignable assn = new Assignable();
			assn.Id = "i.d.";
			assn.Categories = new List<Category> {c};
			c.Assignable = assn;
			s.Save(assn);
			s.Flush();
			s.Close();

			s = OpenSession();
			s.Delete(assn);
			s.Flush();
			s.Close();
		}

		[Test]
		public void CollectionReplaceOnUpdate()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Category c = new Category();
			IList<Category> list = new List<Category>();
			c.Subcategories = list;
			list.Add(new Category());
			s.Save(c);
			t.Commit();
			s.Close();
			c.Subcategories = list;

			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(c);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			c = (Category) s.Load(typeof(Category), c.Id, LockMode.Upgrade);
			IList<Category> list2 = c.Subcategories;
			t.Commit();
			s.Close();

			Assert.IsFalse(NHibernateUtil.IsInitialized(c.Subcategories));

			c.Subcategories = list2;
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(c);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			c = (Category) s.Load(typeof(Category), c.Id, LockMode.Upgrade);
			Assert.AreEqual(1, c.Subcategories.Count);
			s.Delete(c);
			t.Commit();
			s.Close();
		}

		[Test]
		public void CollectionReplace2()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Category c = new Category();
			IList<Category> list = new List<Category>();
			c.Subcategories = list;
			list.Add(new Category());
			Category c2 = new Category();
			s.Save(c2);
			s.Save(c);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			c = (Category) s.Load(typeof(Category), c.Id, LockMode.Upgrade);
			IList<Category> list2 = c.Subcategories;
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			c2 = (Category) s.Load(typeof(Category), c2.Id, LockMode.Upgrade);
			c2.Subcategories = list2;
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			c2 = (Category) s.Load(typeof(Category), c2.Id, LockMode.Upgrade);
			Assert.AreEqual(1, c2.Subcategories.Count);
			s.Delete(c2);
			s.Delete(s.Load(typeof(Category), c.Id));
			t.Commit();
			s.Close();
		}

		[Test]
		public void CollectionReplace()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Category c = new Category();
			IList<Category> list = new List<Category>();
			c.Subcategories = list;
			list.Add(new Category());
			s.Save(c);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			c = (Category) s.Load(typeof(Category), c.Id, LockMode.Upgrade);
			c.Subcategories = list;
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			c = (Category) s.Load(typeof(Category), c.Id, LockMode.Upgrade);
			IList<Category> list2 = c.Subcategories;
			t.Commit();
			s.Close();

			Assert.IsFalse(NHibernateUtil.IsInitialized(c.Subcategories));

			s = OpenSession();
			t = s.BeginTransaction();
			c = (Category) s.Load(typeof(Category), c.Id, LockMode.Upgrade);
			c.Subcategories = list2;
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			c = (Category) s.Load(typeof(Category), c.Id, LockMode.Upgrade);
			Assert.AreEqual(1, c.Subcategories.Count);
			s.Delete(c);
			t.Commit();
			s.Close();
		}

		[Test]
		public void Categories()
		{
			Category c = new Category();
			c.Name = Category.RootCategory;
			Category c1 = new Category();
			Category c2 = new Category();
			Category c3 = new Category();
			c.Subcategories.Add(c1);
			c.Subcategories.Add(c2);
			c2.Subcategories.Add(null);
			c2.Subcategories.Add(c3);

			using (ISession s = OpenSession())
			{
				s.Save(c);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				c = (Category) s.Load(typeof(Category), c.Id);
				Assert.IsNotNull(c.Subcategories[0]);
				Assert.IsNotNull(c.Subcategories[1]);
				IList<Category> list = ((Category) c.Subcategories[1]).Subcategories;
				Assert.IsNull(list[0]);
				Assert.IsNotNull(list[1]);

				IEnumerator enumer =
					s.CreateQuery("from c in class Category where c.Name = NHibernate.DomainModel.Category.RootCategory").Enumerable().
						GetEnumerator();
				Assert.IsTrue(enumer.MoveNext());
			}

			using (ISession s = OpenSession())
			{
				s.Delete("from Category");
				s.Flush();
			}
		}

		[Test]
		public void CollectionRefresh()
		{
			ISession s = OpenSession();
			Category c = new Category();
			IList<Category> list = new List<Category>();
			c.Subcategories = list;
			list.Add(new Category());
			c.Name = "root";
			object id = s.Save(c);
			s.Flush();
			s.Close();

			s = OpenSession();
			c = (Category) s.Load(typeof(Category), id);
			s.Refresh(c);
			s.Flush();

			Assert.AreEqual(1, c.Subcategories.Count);
			s.Flush();
			s.Close();

			s = OpenSession();
			c = (Category) s.Load(typeof(Category), id);
			Assert.AreEqual(1, c.Subcategories.Count);

			// modify the collection in another session
			ISession s2 = OpenSession();
			Category c2 = (Category) s2.Load(typeof(Category), id);
			c2.Subcategories.Add(new Category());
			s2.Flush();
			s2.Close();

			// now lets refresh the collection and see if it picks up 
			// the new objects
			s.Refresh(c);
			Assert.AreEqual(2, c.Subcategories.Count, "should have gotten the addition from s2");

			s.Delete(c);
			s.Flush();
			s.Close();
		}

		[Test]
		public void CachedCollectionRefresh()
		{
			ISession s = OpenSession();
			Category c = new Category();
			IList<Category> list = new List<Category>();
			c.Subcategories = list;
			list.Add(new Category());
			c.Name = "root";
			object id = s.Save(c);
			s.Flush();
			s.Close();

			s = OpenSession();
			c = (Category) s.Load(typeof(Category), id);
			NHibernateUtil.Initialize(c.Subcategories);

			ISession ss = OpenSession();
			Category c2 = (Category) ss.Load(typeof(Category), id);
			ss.Delete(c2.Subcategories[0]);
			c2.Subcategories.Clear();
			ss.Flush();
			ss.Close();

			s.Refresh(c);
			Assert.AreEqual(0, c.Subcategories.Count);

			ss = OpenSession();
			c2 = (Category) ss.Load(typeof(Category), id);
			c2.Subcategories.Add(new Category());
			c2.Subcategories.Add(new Category());
			ss.Flush();
			ss.Close();

			s.Refresh(c);
			Assert.AreEqual(2, c.Subcategories.Count);

			s.Flush();
			s.Close();

			s = OpenSession();
			c = (Category) s.Load(typeof(Category), id);
			Assert.AreEqual(2, c.Subcategories.Count);
			s.Delete(c);
			s.Flush();
			s.Close();
		}

		[Test]
		public void CustomPersister()
		{
			ISession s = OpenSession();
			Custom c = new Custom();
			c.Name = "foo";
			c.Id = "100";
			string id = (string)s.Save(c);
			Assert.AreSame(c, s.Load(typeof(Custom), id));
			s.Flush();
			s.Close();

			s = OpenSession();
			c = (Custom) s.Load(typeof(Custom), id);
			Assert.AreEqual("foo", c.Name);
			c.Name = "bar";
			s.Flush();
			s.Close();

			s = OpenSession();
			c = (Custom) s.Load(typeof(Custom), id);
			Assert.AreEqual("bar", c.Name);
			s.Delete(c);
			s.Flush();
			s.Close();

			s = OpenSession();
			bool none = false;
			try
			{
				s.Load(typeof(Custom), id);
			}
			catch (ObjectNotFoundException onfe)
			{
				Assert.IsNotNull(onfe); //getting ride of 'onfe' is never used compile warning
				none = true;
			}

			Assert.IsTrue(none);
			s.Close();
		}

		[Test]
		public void Interface()
		{
			ISession s = OpenSession();
			object id = s.Save(new BasicNameable());
			s.Flush();
			s.Close();

			s = OpenSession();
			INameable n = (INameable) s.Load(typeof(INameable), id);
			s.Delete(n);
			s.Flush();
			s.Close();
		}

		[Test]
		public void NoUpdatedManyToOne()
		{
			W w1 = new W();
			W w2 = new W();
			Z z = new Z();
			z.W = w1;

			using (ISession s = OpenSession())
			{
				s.Save(z);
				s.Flush();
				z.W = w2;
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				s.Update(z);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				s.Delete(z);
				s.Delete(w1);
				s.Delete(w2);
				s.Flush();
			}
		}

		[Test]
		public void QueuedBagAdds()
		{
			ISession s = OpenSession();
			Assignable a = new Assignable();
			a.Id = "foo";
			a.Categories = new List<Category>();
			Category c = new Category();
			c.Assignable = a;
			a.Categories.Add(c);
			s.Save(a);
			s.Flush();
			s.Close();

			Sfi.EvictCollection("NHibernate.DomainModel.Assignable.Categories");

			s = OpenSession();
			a = (Assignable) s.Get(typeof(Assignable), "foo");
			c = new Category();
			c.Assignable = a;
			a.Categories.Add(c);
			Assert.IsFalse(NHibernateUtil.IsInitialized(a.Categories));
			Assert.AreEqual(2, a.Categories.Count);
			s.Flush();
			s.Close();

			Sfi.EvictCollection("NHibernate.DomainModel.Assignable.Categories");

			s = OpenSession();
			a = (Assignable) s.Get(typeof(Assignable), "foo");
			c = new Category();
			c.Assignable = a;
			a.Categories.Add(c);
			Assert.IsFalse(NHibernateUtil.IsInitialized(a.Categories));
			s.Flush();
			Assert.IsFalse(NHibernateUtil.IsInitialized(a.Categories));
			Assert.AreEqual(3, a.Categories.Count);
			s.Close();

			Sfi.EvictCollection("NHibernate.DomainModel.Assignable.Categories");

			s = OpenSession();
			a = (Assignable) s.Get(typeof(Assignable), "foo");
			Assert.AreEqual(3, a.Categories.Count);
			s.Delete(a);
			s.Flush();
			s.Close();
		}

		[Test]
		public void PolymorphicCriteria()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();
			Category f = new Category();
			Single b = new Single();
			b.Id = "asdfa";
			b.String = "asdfasdf";
			s.Save(f);
			s.Save(b);
			IList list = s.CreateCriteria(typeof(object)).List();
			Assert.AreEqual(2, list.Count);
			Assert.IsTrue(list.Contains(f));
			Assert.IsTrue(list.Contains(b));
			s.Delete(f);
			s.Delete(b);
			txn.Commit();
			s.Close();
		}

		[Test]
		public void ToStringWithNoIdentifier()
		{
			NHibernateUtil.Entity(typeof(Master)).ToLoggableString(new Master(),
			                                                       (ISessionFactoryImplementor) Sfi);
		}

		[Test]
		public void NH741()
		{
			using (ISession s = OpenSession())
			{
				s.CreateQuery(
					"select m from Master m order by m.id + :param")
					.SetParameter("param", 10, NHibernateUtil.Int32)
					.List();
			}
		}
	}
}
