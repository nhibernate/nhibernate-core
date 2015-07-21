using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH623
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH623"; }
		}

		private ISession session;
		private ITransaction tran;

		protected override void OnSetUp()
		{
			session = OpenSession();
			tran = session.BeginTransaction();

			// save some data
			Document doc = new Document(1, "test doc");
			Image img = new Image(1, doc, "c:\a.jpg");
			Paragraph para = new Paragraph(2, doc, "Arial");
			Page p1 = new Page(1, doc);
			p1.IsActive = true;
			Page p2 = new Page(2, doc);
			p2.IsActive = false;
			Review r = new Review(10, doc, "this is a good document"); // this id is 10 on purpose (to be != docId)

			session.Save(doc);
			session.Save(img);
			session.Save(para);
			session.Save(p1);
			session.Save(p2);
			session.Save(r);

			session.Flush();
			session.Clear();
		}

		protected override void OnTearDown()
		{
			if (session != null)
			{
				tran.Rollback();
				session.Dispose();
			}
		}

		[Test]
		public void WhereAttributesOnBags()
		{
			IList result;
			Document d;

			result = session.CreateCriteria(typeof(Document)).List();
			d = result[0] as Document;

			// collection is lazy loaded an so it is also filtered so we will get here one element
			Assert.AreEqual(1, d.Pages.Count);

			session.Clear();

			result = session.CreateCriteria(typeof(Document)).SetFetchMode("Pages", FetchMode.Join).List();
			d = result[0] as Document;

			// this assertion fails because if the collection is eager fetched it will contain all elements and will ignore the where clause.
			Assert.AreEqual(1, d.Pages.Count);
		}
	}
}