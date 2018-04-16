using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1159
{
	[TestFixture]
	public class Fixture:BugTestCase
	{

		protected override void  OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction tran = session.BeginTransaction())
			{
				Contact c=new Contact{Id=1,Forename ="David",Surname="Bates",PreferredName="Davey"};
				session.Save(c);
				tran.Commit();
			}
			HibernateInterceptor.CallCount = 0;

        }

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction tran = session.BeginTransaction())
			{
				session.Delete("from Contact");
				tran.Commit();
			}
		}

		[Test]
		public void DoesNotFlushWithCriteriaWithCommit()
		{
			using (ISession session = OpenSession(new HibernateInterceptor()))
			using (ITransaction tran = session.BeginTransaction())
			{
				session.FlushMode = FlushMode.Commit;
				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(0));
				Contact contact = session.Get<Contact>((Int64)1);
				contact.PreferredName = "Updated preferred name";
				session.Flush();
				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(1));

				contact.Forename = "Updated forename";

				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(1));

				ICriteria query = session.CreateCriteria(typeof(ContactTitle));
				query.Add(Expression.Eq("Id", (Int64)1));
				query.UniqueResult<ContactTitle>();

				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(1));

				contact.Surname = "Updated surname";
				session.Flush();
				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(2));

				session.Merge(contact);
			}
		}
		[Test]
		public void DoesNotFlushWithCriteriaWithNever()
		{
			using (ISession session = OpenSession(new HibernateInterceptor()))
			using (ITransaction tran = session.BeginTransaction())
			{
				session.FlushMode = FlushMode.Manual;
				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(0));
				Contact contact = session.Get<Contact>((Int64)1);
				contact.PreferredName = "Updated preferred name";
				session.Flush();
				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(1));

				contact.Forename = "Updated forename";

				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(1));

				ICriteria query = session.CreateCriteria(typeof(ContactTitle));
				query.Add(Expression.Eq("Id", (Int64)1));
				query.UniqueResult<ContactTitle>();

				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(1));

				contact.Surname = "Updated surname";
				session.Flush();
				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(2));

				session.Merge(contact);
			}
		}
		[Test]
		public void DoesNotFlushWithCriteriaWithAuto()
		{
			using (ISession session = OpenSession(new HibernateInterceptor()))
			using (ITransaction tran = session.BeginTransaction())
			{
				session.FlushMode = FlushMode.Auto;
				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(0));

				Contact contact = session.Get<Contact>((Int64)1);
				contact.PreferredName = "Updated preferred name";
				session.Flush();
				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(1));

				contact.Forename = "Updated forename";

				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(1));

				ICriteria query = session.CreateCriteria(typeof(ContactTitle));
				query.Add(Expression.Eq("Id", (Int64)1));
				query.UniqueResult<ContactTitle>();

				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(2));

				contact.Surname = "Updated surname";
				session.Flush();
				Assert.That(HibernateInterceptor.CallCount, Is.EqualTo(3));

				session.Merge(contact);
			}
		}
	}
}
