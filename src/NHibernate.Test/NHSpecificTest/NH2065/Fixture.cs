using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using NHibernate.Impl;
using NUnit.Framework;
using NHibernate.Criterion;

namespace NHibernate.Test.NHSpecificTest.NH2065
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
        protected override void OnSetUp()
        {
            using (var s = OpenSession())
            using (s.BeginTransaction())
            {
                var person = new Person
                {
                    Children = new HashSet<Person>()
                };
                s.Save(person);
                var child = new Person();
                s.Save(child);
                person.Children.Add(child);

                s.Transaction.Commit();
            }
        }

        protected override void OnTearDown()
        {
            using (var s = OpenSession())
            using (s.BeginTransaction())
            {
                s.Delete("from Person");
                s.Transaction.Commit();
            }
        }

		[Test]
        [ExpectedException(
            ExpectedException=typeof(HibernateException), 
            ExpectedMessage="reassociated object has dirty collection: NHibernate.Test.NHSpecificTest.NH2065.Person.Children")]
		public void GetGoodErrorForDirtyReassociatedCollection()
		{
            Person person;
            using (var s = OpenSession())
            using (s.BeginTransaction())
            {
                person = s.Get<Person>(1);
                NHibernateUtil.Initialize(person.Children);
                s.Transaction.Commit();
            }

            person.Children.Clear();

            using (var s = OpenSession())
            using (s.BeginTransaction())
            {
                s.Lock(person, LockMode.None);
            }
		} 

	}
}
