using System;
using System.Collections.Generic;

using NHibernate.Criterion;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1499
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			Person john = new Person();
			john.Name = "John";

			Document doc1 = new Document();
			doc1.Person = john;
			doc1.Title = "John's Doc";
			Document doc2 = new Document();
			doc2.Title = "Spec";

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Save(john);
				session.Save(doc1);
				session.Save(doc2);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from Person");
				session.Delete("from Document");
				tx.Commit();
			}
		}
		[Test]
		public void CheckIfDetachedCriteriaCanBeUsedOnPropertyRestriction()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				DetachedCriteria detached = DetachedCriteria.For(typeof(Person))
				.Add(Property.ForName("Name").Eq("John"));
				ICriteria criteria = session.CreateCriteria(typeof(Document))
					.Add(Restrictions.Or(
							Property.ForName("Title").Eq("Spec"),
							Property.ForName("Person").Eq(detached)
							));
				Assert.Throws<QueryException>(() => criteria.List<Document>());
			}
		}


	}
}