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
										  //"Custom.hbm.xml",
										  "Category.hbm.xml",
										  "INameable.hbm.xml",
										  "SingleSeveral.hbm.xml",
										  "WZ.hbm.xml"
									  }, true);
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
		[Ignore("HQL bugs - http://jira.nhibernate.org:8080/browse/NH-79, http://jira.nhibernate.org:8080/browse/NH-80")]
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
			IEnumerator enumer = master.Details.Keys.GetEnumerator();
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
			Assert.IsTrue( NHibernate.IsInitialized( m.Details ), "joined fetch should initialize collection" );
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
			Assert.IsTrue( NHibernate.IsInitialized( ((Master)list[0]).Details ) );
			dt = (Detail)s.Load( typeof(Detail), dtid );
			Assert.IsTrue( ((Master)list[0]).Details.Contains(dt) );
			t.Commit();
			s.Close();

			//http://jira.nhibernate.org:8080/browse/NH-79
			/*
			s = sessions.OpenSession();
			t = s.BeginTransaction();
			list = s.Find("select m, m1.Name from Master m1, Master m left join fetch m.Details where m.Name=m1.Name");
			Master masterFromHql = (Master)((object[])list[0])[0];
			Assert.IsTrue( NHibernate.IsInitialized( masterFromHql.Details ) );
			dt = (Detail)s.Load( typeof(Detail), dtid );
			Assert.IsTrue( masterFromHql.Details.Contains(dt) );
			list = s.Find("select m.id from Master m inner join fetch m.Details");
			t.Commit();
			s.Close();
			*/


			// rest of the test depends on ISession.Filter() working

		}

		[Test]
		[Ignore("FilterKeyFactory is null so code for test is not complete.  http://jira.nhibernate.org:8080/browse/NH-80")]
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

			//TODO: Filter's are not working because FilterKeyFactory is null.
			Assert.AreEqual( 2, s.Filter(master1.Incoming, "where this.id > 0 and this.Name is not null").Count );
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
			m.MoreDetails.Add( d1, new object() );
			m.MoreDetails.Add( d2, new object() );
			object mid = s.Save(m);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			m = (Master)s.Load( typeof(Master), mid );
			Assert.AreEqual( 2, m.MoreDetails.Count, "cascade save" );
			IEnumerator enumer = m.MoreDetails.Keys.GetEnumerator();
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

			s = (ISession)f.Deserialize(stream);
			stream.Close();

			s.Reconnect();
			Master m2 = (Master) s.Load(typeof(Master), mid);
			Assert.IsTrue( m2.Details.Count==2, "serialized state" );
			foreach(Detail d in m2.Details.Keys)
			{
				Assert.IsTrue( d.Master==m2, "deserialization" );
				try 
				{
					s.GetIdentifier(d);
					s.Delete(d);
				}
				catch (Exception e) 
				{
					Assert.IsTrue( e is Exception, "just getting rid of a compiler warning." );
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
			IEnumerator enumer = m.Details.Keys.GetEnumerator();
			int i = 0;
			while( enumer.MoveNext() ) 
			{
				Assert.IsNotNull( enumer.Current );
				i++;
			}
			Assert.AreEqual( 2, i );

			enumer = m.Details.Keys.GetEnumerator();
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
			m.MoreDetails.Add( detail, new object() );
			detail.SubDetails = new Hashtable();
			detail.SubDetails.Add( subdetail, new object() );
			object mid = s.Save(m);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			m = (Master)s.Load( typeof(Master), mid);
			IEnumerator enumer = m.MoreDetails.Keys.GetEnumerator();
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

			Assert.IsFalse( NHibernate.IsInitialized( c.Subcategories ) );

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
		[Ignore("HQL can't reference static property for const.  http://jira.nhibernate.org:8080/browse/NH-78")]
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
			s.Delete(c);
			s.Flush();
			s.Close();
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void CustomPersister() 
		{
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
	}
}
