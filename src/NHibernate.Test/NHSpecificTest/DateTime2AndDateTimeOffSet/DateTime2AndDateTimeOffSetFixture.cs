using System;
using System.Collections;
using NHibernate.Dialect;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.DateTime2AndDateTimeOffSet
{
    [TestFixture]
    public class DateTime2AndDateTimeOffSetFixture : TestCase
    {
        protected override IList Mappings
        {
            get { return new[] { "NHSpecificTest.DateTime2AndDateTimeOffSet.Mappings.hbm.xml" }; }
        }

        protected override string MappingsAssembly
        {
            get { return "NHibernate.Test"; }
        }

        protected override bool AppliesTo(Dialect.Dialect dialect)
        {
            return dialect is MsSql2008Dialect;
        }
        

        [Test]
        public void Dates01()
        {

            var Now = DateTime.Now;
            var NowOS = DateTimeOffset.Now;
            var dates = new AllDates
                            {
                                Sql_datetime = Now,
                                Sql_datetime2 = DateTime.MinValue,
                                Sql_datetimeoffset = NowOS,
                            };

            using(ISession s = OpenSession())
            using(ITransaction tx = s.BeginTransaction())
            {
                s.Save(dates);
                tx.Commit();
            }

            using(ISession s = OpenSession())
            using(ITransaction tx = s.BeginTransaction())
            {
                var datesRecovered = s.CreateQuery("from AllDates").UniqueResult<AllDates>();
                
                DateTimeAssert.AreEqual(datesRecovered.Sql_datetime,Now);
                DateTimeAssert.AreEqual(datesRecovered.Sql_datetime2, DateTime.MinValue);
                DateTimeAssert.AreEqual(datesRecovered.Sql_datetimeoffset, NowOS);
            }

             using(ISession s = OpenSession())
             using (ITransaction tx = s.BeginTransaction())
             {
                 var datesRecovered = s.CreateQuery("from AllDates").UniqueResult<AllDates>();
                 s.Delete(datesRecovered);
                 tx.Commit();
             }
        }

        public class DateTimeAssert
        {
            public static void AreEqual(DateTime dt1,DateTime dt2)
            {
                bool areEqual = new DateTimeType().IsEqual(dt1,dt2);

                if(!areEqual)
                    Assert.Fail("Expected {0} but was {1}");
            }

            public static void AreEqual(DateTimeOffset dt1, DateTimeOffset dt2)
            {
                bool areEqual = new DateTimeOffsetType().IsEqual(dt1, dt2);

                if (!areEqual)
                    Assert.Fail("Expected {0} but was {1}");
            }
        }
    }
}