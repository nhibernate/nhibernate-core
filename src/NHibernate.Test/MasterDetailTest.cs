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
		[Ignore("Test not yet written")]
		public void MasterDetail() 
		{
		}

		[Test]
		[Ignore("FilterKeyFactory is null so code for test is not complete.  Test not yet written")]
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
		[Ignore("Test not yet written")]
		public void Serialization() 
		{
			
		}

		[Test]
		[Ignore("Test not yet written")]
		public void UpdateLazyCollections() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void MultiLevelCascade() 
		{
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
		[Ignore("Test not yet written")]
		public void CollectionReplace2() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CollectionReplace() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void Categories() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CollectionRefresh() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void CustomPersister() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void Interface() 
		{
		}

		[Test]
		[Ignore("Test not yet written")]
		public void NoUpdatedManyToOne() 
		{
		}
	}
}
