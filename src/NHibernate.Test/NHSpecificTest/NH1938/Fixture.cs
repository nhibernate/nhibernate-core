using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1938
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			// Database needs to be case-sensitive
			return (dialect is NHibernate.Dialect.Oracle10gDialect);
		}

		[Test]
		public void Can_Query_By_Example_Case_Insensitive()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "John Smith" });

				Person examplePerson = new Person() { Name = "oHn" };
				IList<Person> matchingPeople;

				matchingPeople =
					s.CreateCriteria<Person>()
						.Add(Example
							.Create(examplePerson)
							.EnableLike(MatchMode.Anywhere)
							.IgnoreCase())
						.List<Person>();

				Assert.That(matchingPeople.Count, Is.EqualTo(1));

				matchingPeople =
					s.CreateCriteria<Person>()
						.Add(Example
							.Create(examplePerson)
							.EnableLike(MatchMode.Anywhere))
						.List<Person>();

				Assert.That(matchingPeople.Count, Is.EqualTo(0));

				t.Rollback();
			}
		}
	}
}
