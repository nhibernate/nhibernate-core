using System;
using System.Collections;
using System.Threading;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.Stateless
{
	[TestFixture]
	public class StatelessSessionFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "Stateless.Document.hbm.xml" }; }
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from Document");
				s.Delete("from Paper");
			}
		}

		[Test]
		public void CreateUpdateReadDelete()
		{
			Document doc;
			DateTime? initVersion;

			using (IStatelessSession ss = sessions.OpenStatelessSession())
			{
				ITransaction tx;
				using (tx = ss.BeginTransaction())
				{
					doc = new Document("blah blah blah", "Blahs");
					ss.Insert(doc);
					Assert.IsNotNull(doc.LastModified);
					initVersion = doc.LastModified;
					Assert.IsTrue(initVersion.HasValue);
					tx.Commit();
				}
				Thread.Sleep(100); // Only to be secure that next modification have a different version
				using (tx = ss.BeginTransaction())
				{
					doc.Text = "blah blah blah .... blah";
					ss.Update(doc);
					Assert.IsTrue(doc.LastModified.HasValue);
					Assert.AreNotEqual(initVersion, doc.LastModified); 
					tx.Commit();
				}
				using (tx = ss.BeginTransaction())
				{
					doc.Text = "blah blah blah .... blah blay";
					ss.Update(doc);
					tx.Commit();
				}
				Document doc2 = (Document)ss.Get<Document>("Blahs");
				Assert.AreEqual("Blahs", doc2.Name);
				Assert.AreEqual(doc.Text, doc2.Text);

				doc2 = (Document)ss.CreateQuery("from Document where text is not null").UniqueResult();
				Assert.AreEqual("Blahs", doc2.Name);
				Assert.AreEqual(doc.Text, doc2.Text);

				doc2 = (Document)ss.CreateSQLQuery("select * from Document").AddEntity(typeof(Document)).UniqueResult();
				Assert.AreEqual("Blahs", doc2.Name);
				Assert.AreEqual(doc.Text, doc2.Text);

				doc2 = (Document)ss.CreateCriteria<Document>().UniqueResult();
				Assert.AreEqual("Blahs", doc2.Name);
				Assert.AreEqual(doc.Text, doc2.Text);

				using (tx = ss.BeginTransaction())
				{
					ss.Delete(doc);
					tx.Commit();
				}
			}
		}

		[Test, Ignore("Not supported yet")]
		public void HqlBulk()
		{
			IStatelessSession ss = sessions.OpenStatelessSession();
			ITransaction tx = ss.BeginTransaction();
			Document doc = new Document("blah blah blah", "Blahs");
			ss.Insert(doc);
			Paper paper = new Paper();
			paper.Color = "White";
			ss.Insert(paper);
			tx.Commit();

			tx = ss.BeginTransaction();
			int count = ss.CreateQuery("update Document set name = :newName where name = :oldName")
					.SetString("newName", "Foos")
					.SetString("oldName", "Blahs")
					.ExecuteUpdate();
			Assert.AreEqual(1, count, "hql-delete on stateless session");
			count = ss.CreateQuery("update Paper set color = :newColor")
					.SetString("newColor", "Goldenrod")
					.ExecuteUpdate();
			Assert.AreEqual(1, count, "hql-delete on stateless session");
			tx.Commit();

			tx = ss.BeginTransaction();
			count = ss.CreateQuery("delete Document").ExecuteUpdate();
			Assert.AreEqual(1, count, "hql-delete on stateless session");
			count = ss.CreateQuery("delete Paper").ExecuteUpdate();
			Assert.AreEqual(1, count, "hql-delete on stateless session");
			tx.Commit();
			ss.Close();
		}

		[Test]
		public void InitId()
		{
			Paper paper;

			using (IStatelessSession ss = sessions.OpenStatelessSession())
			{
				ITransaction tx;
				using (tx = ss.BeginTransaction())
				{
					paper = new Paper();
					paper.Color = "White";
					ss.Insert(paper);
					Assert.IsTrue(paper.Id != 0);
					tx.Commit();
				}
				using (tx = ss.BeginTransaction())
				{
					ss.Delete(ss.Get<Paper>(paper.Id));
					tx.Commit();
				}
			}
		}

		[Test]
		public void Refresh()
		{
			Paper paper;

			using (IStatelessSession ss = sessions.OpenStatelessSession())
			using (ITransaction tx = ss.BeginTransaction())
			{
				paper = new Paper();
				paper.Color = "whtie";
				ss.Insert(paper);
				tx.Commit();
			}
			using (IStatelessSession ss = sessions.OpenStatelessSession())
			using (ITransaction tx = ss.BeginTransaction())
			{
				Paper p2 = (Paper)ss.Get<Paper>(paper.Id);
				p2.Color = "White";
				ss.Update(p2);
				tx.Commit();
			}
			using (IStatelessSession ss = sessions.OpenStatelessSession())
			using (ITransaction tx = ss.BeginTransaction())
			{
				Assert.AreEqual("whtie", paper.Color);
				ss.Refresh(paper);
				Assert.AreEqual("White", paper.Color);
				ss.Delete(paper);
				tx.Commit();
			}
		}
	}
}
