using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using NHibernate.Dialect;
using NHibernate.Impl;
using NUnit.Framework;
using NHibernate.Criterion;

namespace NHibernate.Test.NHSpecificTest.NH2113
{
	[TestFixture]
	public class Fixture : BugTestCase
	{

        [Test]
        public void ShouldNotEagerLoadKeyManyToOneWhenOverridingGetHashCode()
        {
            using (var s = OpenSession())
            using(var tx = s.BeginTransaction())
            {
                var grp = new Group();
                s.Save(grp);

                var broker = new Broker{Key = new Key{BankId = 1, Id = -1}};
                s.Save(broker);

                var load = new Loan {Broker = broker, Group = grp, Name = "money!!!"};
                s.Save(load);

                tx.Commit();
            }

            bool isInitialized;
            using (var s = OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var loan = s.CreateCriteria<Loan>()
                    .UniqueResult<Loan>();

                isInitialized = NHibernateUtil.IsInitialized(loan.Broker);

                tx.Commit();
            }


            using (var s = OpenSession())
            using (var tx = s.BeginTransaction())
            {
                s.Delete("from System.Object");

                tx.Commit();
            }

            Assert.False(isInitialized);
        }
	}
}
