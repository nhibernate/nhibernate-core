using System;
using NHibernate;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH958
{
	[TestFixture]
    public class NH958Fixture : BugTestCase
    {
		[Test]
		public void MergeWithAny1()
		{
			Person person;

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				person = new Male("Test");

				for (int i = 0; i < 10; i++)
				{
					person.AddHobby(new Hobby("Hobby_" + i.ToString()));
				}

				session.SaveOrUpdate(person);
				transaction.Commit();
			}

			person.Hobbies.Clear();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				person = session.Merge(person);
				transaction.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete(person);
				transaction.Commit();
			}
		}

		[Test]
		public void MergeWithAny2()
		{
			Person person;

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				person = new Male("Test");
				session.Save(person);
				transaction.Commit();
			}

			person.AddHobby(new Hobby("Hobby_1"));
			person.AddHobby(new Hobby("Hobby_2"));

			using (ISession session = OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                // the transient hobby "test" is inserted and updated
                person = session.Merge(person);
                transaction.Commit();
            }

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete(person);
				transaction.Commit();
			}
        }
    }
}
