using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2065
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
        protected override void OnSetUp()
        {
            using (var s = OpenSession())
            using (var t = s.BeginTransaction())
            {
                var person = new Person
                {
                    Children = new HashSet<Person>()
                };
                s.Save(person);
                var child = new Person();
                s.Save(child);
                person.Children.Add(child);

                t.Commit();
            }
        }

        protected override void OnTearDown()
        {
            using (var s = OpenSession())
            using (var t = s.BeginTransaction())
            {
                s.Delete("from Person");
                t.Commit();
            }
        }

		[Test]
		public void GetGoodErrorForDirtyReassociatedCollection()
		{
			Person person;
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				person = s.Get<Person>(1);
				NHibernateUtil.Initialize(person.Children);
				t.Commit();
			}

			person.Children.Clear();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.That(
					() =>
					{
						s.Lock(person, LockMode.None);
					},
					Throws.TypeOf<HibernateException>()
					      .And.Message.EqualTo(
						      "reassociated object has dirty collection: NHibernate.Test.NHSpecificTest.NH2065.Person.Children"));
			}
		}
	}
}
