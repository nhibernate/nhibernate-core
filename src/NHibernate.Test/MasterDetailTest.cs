using System;
using System.Collections;

using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test
{
	/// <summary>
	/// Summary description for MasterDetailTest.
	/// </summary>
	[TestFixture]
	public class MasterDetailTest : TestCase
	{
		[SetUp]
		public void SetUp()
		{
		 	ExportSchema(new string[] {  
							"MasterDetail.hbm.xml",
							"Custom.hbm.xml",
							"Category.hbm.xml",
							"INameable.hbm.xml",
							"SingleSeveral.hbm.xml",
							"WZ.hbm.xml",
							"UpDown.hbm.xml",
							"Eye.hbm.xml",
							"MN.hbm.xml"
									  }, true);
		}

		[Test]
		[Ignore( "Test not written" )]
		public void ParentChildren()
		{
			ISession s = sessions.OpenSession();

			//M parent = new M
		}

		[Test]
		[Ignore( "Test not written" )]
		public void OuterJoin()
		{
		}

		[Test]
		[Ignore( "Test not written" )]
		public void Meta()
		{
		}

		[Test]
		[Ignore( "Test not written" )]
		public void Copy()
		{
		}

		[Test]
		[Ignore( "Test not written" )]
		public void CopyCascade()
		{
		}

		[Test]
		[Ignore( "Test not written" )]
		public void NotNullDiscriminator()
		{
		}

		[Test]
		public void SelfManyToOne() 
		{
			// add a check to not run if HSQLDialect

			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Master m = new Master();
			m.OtherMaster = m;
			s.Save(m);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			IEnumerator enumer = s.Enumerable("from m in class Master").GetEnumerator();
			enumer.MoveNext();
			m = (Master)enumer.Current;
			Assert.AreSame(m, m.OtherMaster);
			s.Delete(m);
			t.Commit();
			s.Close();
		}

		[Test]
		[Ignore( "Test not written" )]
		public void Example()
		{
		}

		[Test]
		public void NonLazyBidrectional() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			DomainModel.Single sin = new DomainModel.Single();
			sin.Id = "asfdfds";
			sin.String = "adsa asdfasd";
			Several sev = new Several();
			sev.Id = "asdfasdfasd";
			sev.String = "asd ddd";
			sin.Several.Add(sev);
			sev.Single = sin;
			s.Save(sin);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			sin = (DomainModel.Single)s.Load( typeof(DomainModel.Single), sin );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			sev = (Several)s.Load( typeof(Several), sev );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			s.Find("from s in class Several");
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			s.Find("from s in class Single");
			t.Commit();
			s.Close();
		}

		[Test]
		public void CollectionQuery() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();

			// add checks for SAPDBDialect & MckoiDialect
			if( !(dialect is Dialect.MySQLDialect) ) 
			{
				s.Enumerable("FROM m IN CLASS Master WHERE NOT EXISTS ( FROM d in m.Details.elements WHERE NOT d.I=5 )");
				s.Enumerable("FROM m IN CLASS Master WHERE NOT 5 IN ( SELECT d.I FROM d IN m.Details.elements )");
			}

			s.Enumerable("SELECT m FROM m in CLASS NHibernate.DomainModel.Master, d IN m.Details.elements WHERE d.I=5");
			s.Find("SELECT m FROM m in CLASS NHibernate.DomainModel.Master, d IN m.Details.elements WHERE d.I=5");
			s.Find("SELECT m.id FROM m IN CLASS NHibernate.DomainModel.Master, d IN m.Details.elements WHERE d.I=5");
			t.Commit();
			s.Close();
		}

		[Test]
		public void MasterDetail() 
		{
			//if( dialect is Dialect.HSQLDialect ) return;

			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Master master = new Master();
			Assert.IsNotNull( s.Save(master), "save returned native id" );
			object mid = s.GetIdentifier(master);
			Detail d1 = new Detail();
			d1.Master = master;
			object did = s.Save(d1);
			Detail d2 = new Detail();
			d2.I = 12;
			d2.Master = master;
			Assert.IsNotNull( s.Save(d2), "generated id returned" );
			master.AddDetail(d1);
			master.AddDetail(d2);

			// add checks for SAPDBDialect and MckoiDialect
			if( !(dialect is Dialect.MySQLDialect) ) 
			{
				string hql = "from d in class NHibernate.DomainModel.Detail, m in class NHibernate.DomainModel.Master " + 
					"where m = d.Master and m.Outgoing.size = 0 and m.Incoming.size = 0";
				
				Assert.AreEqual( 2, s.Find(hql).Count, "query" );

			}
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			master = (Master)s.Load( typeof(Master), mid );
			IEnumerator enumer = master.Details.GetEnumerator();
			int i = 0;
			while( enumer.MoveNext() ) 
			{
				Detail d = (Detail)enumer.Current;
				Assert.AreSame( master, d.Master, "master-detail" );
				i++;
			}
			Assert.AreEqual( 2, i, "master-detail count" );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			Assert.AreEqual( 2, s.Find("select elements(master.Details) from Master master").Count );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			IList list = s.Find("from Master m left join fetch m.Details");
			Master m = (Master)list[0];
			Assert.IsTrue( NHibernateUtil.IsInitialized( m.Details ), "joined fetch should initialize collection" );
			Assert.AreEqual( 2, m.Details.Count );
			list = s.Find("from Detail d inner join fetch d.Master");
			Detail dt = (Detail)list[0];
			object dtid = s.GetIdentifier(dt);
			Assert.AreSame( m, dt.Master );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			list = s.Find("select m from Master m1, Master m left join fetch m.Details where m.Name=m1.Name");
			Assert.IsTrue( NHibernateUtil.IsInitialized( ((Master)list[0]).Details ) );
			dt = (Detail)s.Load( typeof(Detail), dtid );
			Assert.IsTrue( ((Master)list[0]).Details.Contains(dt) );
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			list = s.Find("select m, m1.Name from Master m1, Master m left join fetch m.Details where m.Name=m1.Name");
			Master masterFromHql = (Master)((object[])list[0])[0];
			Assert.IsTrue( NHibernateUtil.IsInitialized( masterFromHql.Details ) );
			dt = (Detail)s.Load( typeof(Detail), dtid );
			Assert.IsTrue( masterFromHql.Details.Contains(dt) );
			list = s.Find("select m.id from Master m inner join fetch m.Details");
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			Detail dd = (Detail)s.Load( typeof(Detail), did );
			master = dd.Master;
			Assert.IsTrue( master.Details.Contains(dd), "detail-master" );
			Assert.AreEqual( 2, s.Filter( master.Details, "order by this.I desc").Count );
			Assert.AreEqual( 2, s.Filter( master.Details, "select this where this.id > -1").Count );
			
			IQuery q = s.CreateFilter( master.Details, "where this.id > :id" );
			q.SetInt32("id", -1);
			Assert.AreEqual( 2, q.List().Count );

			q = s.CreateFilter( master.Details, "where this.id > :id1 and this.id < :id2" );
			q.SetInt32("id1", -1);
			q.SetInt32("id2", 99999999);
			Assert.AreEqual( 2, q.List().Count );
			q.SetInt32("id2", -1);
			Assert.AreEqual( 0, q.List().Count );

			q = s.CreateFilter( master.Details, "where this.id in (:ids)" );
			list = new ArrayList();
			list.Add(did);
			list.Add( (long)-1 );
			q.SetParameterList("ids", list);
			
			Assert.AreEqual( 1, q.List().Count );
			Assert.IsTrue( q.Enumerable().GetEnumerator().MoveNext() );
			
			Assert.AreEqual( 2, s.Filter( master.Details, "where this.id > -1").Count );
			Assert.AreEqual( 2, s.Filter( master.Details, "select this.Master where this.id > -1").Count );
			Assert.AreEqual( 2, s.Filter( master.Details, "select m from m in class Master where this.id > -1 and this.Master=m").Count );
			Assert.AreEqual( 0, s.Filter( master.Incoming, "where this.id > -1 and this.Name is not null").Count );

			IQuery filter = s.CreateFilter( master.Details, "select max(this.I)");
			enumer = filter.Enumerable().GetEnumerator();
			Assert.IsTrue( enumer.MoveNext() );
			Assert.IsTrue( enumer.Current is Int32 );

			filter = s.CreateFilter( master.Details, "select max(this.I) group by this.id" );
			enumer = filter.Enumerable().GetEnumerator();
			Assert.IsTrue( enumer.MoveNext() );
			Assert.IsTrue( enumer.Current is Int32 );

			filter = s.CreateFilter( master.Details, "select count(*)" );
			enumer = filter.Enumerable().GetEnumerator();
			Assert.IsTrue( enumer.MoveNext() );
			Assert.IsTrue( enumer.Current is Int32 );

			Assert.AreEqual( 2, s.CreateFilter( master.Details, "select this.Master" ).List().Count );

			IQuery f = s.CreateFilter( master.Details, "select max(this.I) where this.I < :top and this.I>=:bottom" );
			f.SetInt32("top", 100);
			f.SetInt32("bottom", 0);

			enumer = f.Enumerable().GetEnumerator();
			Assert.IsTrue( enumer.MoveNext() );
			Assert.AreEqual( 12, enumer.Current );
			
			f.SetInt32("top", 2);
			enumer = f.Enumerable().GetEnumerator();
			Assert.IsTrue( enumer.MoveNext() );
			Assert.AreEqual( 0, enumer.Current );

			f = s.CreateFilter( master.Details, "select max(this.I) where this.I not in (:list)" );
			IList coll = new ArrayList();
			coll.Add(-666);
			coll.Add(22);
			coll.Add(0);

			f.SetParameterList("list", coll);
			enumer = f.Enumerable().GetEnumerator();
			Assert.IsTrue( enumer.MoveNext() );
			Assert.AreEqual( 12, enumer.Current );

			i = 0;
			foreach( Detail d in master.Details  ) 
			{
				Assert.AreSame( master, d.Master, "master-detail" );
				s.Delete(d);
				i++;
			}

			Assert.AreEqual( 2, i, "master-detail" );
			s.Delete(master);
			t.Commit();
			s.Close();
		}

		[Test]
		public void IncomingOutgoing() 
		{
			//if HSQLDialect skip test

			ISession s = sessions.OpenSession();
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

			Assert.AreEqual( 2, s.Filter(master1.Incoming, "where this.id > 0 and this.Name is not null").Count );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			master1 = (Master)s.Load( typeof(Master), m1id );
			int i = 0;
			foreach( Master m in master1.Incoming  ) 
			{
				Assert.AreEqual( 1, m.Outgoing.Count, "outgoing" );
				Assert.IsTrue( m.Outgoing.Contains(master1), "outgoing" );
				s.Delete(m);
				i++;
			}

			Assert.AreEqual( 2, i, "incoming-outgoing" );
			s.Delete(master1);
			s.Flush();
			s.Close();
		}

		[Test]
		public void Cascading() 
		{
			//HSQLDialect return;

			ISession s = sessions.OpenSession();
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
			m.MoreDetails.Add( d1 );
			m.MoreDetails.Add( d2 );
			object mid = s.Save(m);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			m = (Master)s.Load( typeof(Master), mid );
			Assert.AreEqual( 2, m.MoreDetails.Count, "cascade save" );
			IEnumerator enumer = m.MoreDetails.GetEnumerator();
			enumer.MoveNext();
			Assert.AreEqual( 2, ((Detail)enumer.Current).Master.Details.Count , "cascade save" );

			s.Delete(m);
			s.Delete( s.Load( typeof(Master), m0id ) );
			s.Flush();
			s.Close();

		}

		[Test]
		public void NamedQuery() 
		{
			ISession s = sessions.OpenSession();
			IQuery q = s.GetNamedQuery("all_details");
			q.List();
			s.Close();
		}

		[Test]
		public void Serialization() 
		{
			ISession s = sessions.OpenSession();
			Master m = new Master();
			Detail d1 = new Detail();
			Detail d2 = new Detail();
			object mid = s.Save(m);
			d1.Master=(m);
			d2.Master=(m);
			m.AddDetail(d1);
			m.AddDetail(d2);
			if ((dialect is Dialect.SybaseDialect) || (dialect is Dialect.MsSql2000Dialect))
			{
				s.Save(d1);
			}
			else 
			{
				s.Save( d1, 666L );
			}
			s.Flush();
			s.Disconnect();
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			f.Serialize(stream, s);
			stream.Position = 0;
			Console.WriteLine(stream.Length);

			s = (ISession)f.Deserialize(stream);
			stream.Close();

			s.Reconnect();
			Master m2 = (Master) s.Load(typeof(Master), mid);
			Assert.IsTrue( m2.Details.Count==2, "serialized state" );
			foreach(Detail d in m2.Details)
			{
				Assert.IsTrue( d.Master==m2, "deserialization" );
				try 
				{
					s.GetIdentifier(d);
					s.Delete(d);
				}
				catch (Exception e) 
				{
					Assert.IsNotNull(e); //getting ride of 'e' is never used compile warning
				}
			}
			s.Delete(m2);
			s.Flush();
			s.Close();
		
			s = sessions.OpenSession();
			mid = s.Save( new Master() );
			object mid2 = s.Save( new Master() );
			s.Flush();
			s.Disconnect();
			stream = new System.IO.MemoryStream();
			f.Serialize(stream, s);
			stream.Position = 0;

			s = (ISession)f.Deserialize(stream);
			stream.Close();

			s.Reconnect();
			s.Delete( s.Load(typeof(Master), mid) );
			s.Delete( s.Load(typeof(Master), mid2) );
			s.Flush();
			s.Close();
		
			s = sessions.OpenSession();
			string db = s.Connection.Database; //force session to grab a connection
			try 
			{
				stream = new System.IO.MemoryStream();
				f.Serialize(stream, s);
			}
			catch (Exception e) 
			{
				Assert.IsTrue(e is InvalidOperationException, "illegal state" );
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
			ISession s = sessions.OpenSession();
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
			if( (dialect is Dialect.SybaseDialect) || (dialect is Dialect.MsSql2000Dialect) ) 
			{
				s.Save(d1);
				s.Save(d2);
			}
			else 
			{
				s.Save( d1, 666L );
				s.Save( d2, 667L );
			}
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			m = (Master)s.Load( typeof(Master), mid );
			s.Close();
			m.Name = "New Name";
			s = sessions.OpenSession();
			s.Update(m, mid);
			IEnumerator enumer = m.Details.GetEnumerator();
			int i = 0;
			while( enumer.MoveNext() ) 
			{
				Assert.IsNotNull( enumer.Current );
				i++;
			}
			Assert.AreEqual( 2, i );

			enumer = m.Details.GetEnumerator();
			while( enumer.MoveNext() ) 
			{
				s.Delete( enumer.Current );
			}
			s.Delete(m);
			s.Flush();
			s.Close();
		}

		[Test]
		public void MultiLevelCascade() 
		{
			//if( dialect is Dialect.HSQLDialect ) return;

			ISession s = sessions.OpenSession();
			Detail detail = new Detail();
			SubDetail subdetail = new SubDetail();
			Master m = new Master();
			Master m0 = new Master();
			object m0id = s.Save(m0);
			m0.AddDetail(detail);
			detail.Master = m0;
			m.MoreDetails.Add( detail );
			detail.SubDetails = new Iesi.Collections.HashedSet();
			detail.SubDetails.Add( subdetail );
			object mid = s.Save(m);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			m = (Master)s.Load( typeof(Master), mid);
			IEnumerator enumer = m.MoreDetails.GetEnumerator();
			enumer.MoveNext();
			Assert.IsTrue( ((Detail)enumer.Current).SubDetails.Count!=0 );
			s.Delete(m);
			s.Flush();
			s.Close();
		}

		[Test]
		public void MixNativeAssigned() 
		{
			// if HSQLDialect then skip test
			ISession s = sessions.OpenSession();
			Category c = new Category();
			c.Name = "NAME";
			Assignable assn = new Assignable();
			assn.Id = "i.d.";
			IList l = new ArrayList();
			l.Add(c);
			assn.Categories = l;
			c.Assignable = assn;
			s.Save(assn);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			s.Delete(assn);
			s.Flush();
			s.Close();
		}

		[Test]
		[Ignore( "Test not written" )]
		public void CollectionReplaceOnUpdate()
		{
		}

		[Test]
		public void CollectionReplace2() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Category c = new Category();
			IList list = new ArrayList();
			c.Subcategories = list;
			list.Add( new Category() );
			Category c2 = new Category();
			s.Save(c2);
			s.Save(c);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Category)s.Load( typeof(Category), c.Id, LockMode.Upgrade );
			IList list2 = c.Subcategories;
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c2 = (Category)s.Load( typeof(Category), c2.Id, LockMode.Upgrade );
			c2.Subcategories = list2;
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c2 = (Category)s.Load( typeof(Category), c2.Id, LockMode.Upgrade );
			Assert.AreEqual( 1, c2.Subcategories.Count );
			s.Delete(c2);
			s.Delete( s.Load( typeof(Category), c.Id ) );
			t.Commit();
			s.Close();
		}

		[Test]
		public void CollectionReplace() 
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			Category c = new Category();
			IList list = new ArrayList();
			c.Subcategories = list;
			list.Add( new Category() );
			s.Save(c);
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Category)s.Load( typeof(Category), c.Id, LockMode.Upgrade );
			c.Subcategories = list;
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Category)s.Load( typeof(Category), c.Id, LockMode.Upgrade );
			IList list2 = c.Subcategories;
			t.Commit();
			s.Close();

			Assert.IsFalse( NHibernateUtil.IsInitialized( c.Subcategories ) );

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Category)s.Load( typeof(Category), c.Id, LockMode.Upgrade );
			c.Subcategories = list2;
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();
			c = (Category)s.Load( typeof(Category), c.Id, LockMode.Upgrade );
			Assert.AreEqual( 1, c.Subcategories.Count );
			s.Delete(c);
			t.Commit();
			s.Close();
		}

		[Test]
		public void Categories() 
		{
			ISession s = sessions.OpenSession();
			Category c = new Category();
			c.Name = Category.RootCategory;
			Category c1 = new Category();
			Category c2 = new Category();
			Category c3 = new Category();
			c.Subcategories.Add(c1);
			c.Subcategories.Add(c2);
			c2.Subcategories.Add(null);
			c2.Subcategories.Add(c3);
			s.Save(c);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			c = (Category)s.Load( typeof(Category), c.Id );
			Assert.IsNotNull( c.Subcategories[0] );
			Assert.IsNotNull( c.Subcategories[1] );
			IList list = ((Category)c.Subcategories[1]).Subcategories;
			Assert.IsNull(list[0]);
			Assert.IsNotNull(list[1]);

			IEnumerator enumer = s.Enumerable("from c in class Category where c.Name = NHibernate.DomainModel.Category.RootCategory").GetEnumerator();
			Assert.IsTrue( enumer.MoveNext() );
			s.Close();
		}

		[Test]
		public void CollectionRefresh() 
		{
			ISession s = sessions.OpenSession();
			Category c = new Category();
			IList list = new ArrayList();
			c.Subcategories = list;
			list.Add( new Category() );
			c.Name = "root";
			object id = s.Save(c);
			s.Flush();
			
			s = sessions.OpenSession();
			c = (Category)s.Load( typeof(Category), id );
			s.Refresh(c);
			s.Flush();

			Assert.AreEqual( 1, c.Subcategories.Count );
			s.Flush();
			s = sessions.OpenSession();
			c = (Category) s.Load( typeof(Category), id );
			Assert.AreEqual( 1, c.Subcategories.Count );
			
			// modify the collection in another session
			ISession s2 = sessions.OpenSession();
			Category c2 = (Category)s2.Load( typeof(Category), id );
			c2.Subcategories.Add( new Category() );
			s2.Flush();
			s2.Close();

			// now lets refresh the collection and see if it picks up 
			// the new objects
			s.Refresh( c );
			Assert.AreEqual( 2, c.Subcategories.Count, "should have gotten the addition from s2" );
			
			s.Delete(c);
			s.Flush();
			s.Close();
		}

		[Test]
		[Ignore( "Test not written" )]
		public void CachedCollectionRefresh()
		{
		}

		[Test]
		public void CustomPersister() 
		{
			ISession s = sessions.OpenSession();
			Custom c = new Custom();
			c.Name = "foo";
			c.Id = 100;
			long id = (long)s.Save(c);
			Assert.AreSame( c, s.Load( typeof(Custom), id ) );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			c = (Custom)s.Load( typeof(Custom), id );
			Assert.AreEqual( "foo", c.Name );
			c.Name = "bar";
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			c = (Custom)s.Load( typeof(Custom), id );
			Assert.AreEqual( "bar", c.Name );
			s.Delete(c);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			bool none = false;
			try 
			{
				s.Load( typeof(Custom), id );
			}
			catch(ObjectNotFoundException onfe) 
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
			ISession s = sessions.OpenSession();
			object id = s.Save( new BasicNameable() );
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			INameable n = (INameable)s.Load( typeof(INameable), id );
			s.Delete(n);
			s.Flush();
			s.Close();
		}

		[Test]
		public void NoUpdatedManyToOne() 
		{
			ISession s = sessions.OpenSession();
			W w1 = new W();
			W w2 = new W();
			Z z = new Z();
			z.W = w1;
			s.Save(z);
			s.Flush();
			z.W = w2;
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			s.Update(z);
			s.Flush();
			s.Close();
		}

		[Test]
		[Ignore( "Test not written" )]
		public void QueuedBagAdds()
		{
		}

		[Test]
		[Ignore( "Test not written" )]
		public void PolymorphicCriteria()
		{
		}
	}
}
