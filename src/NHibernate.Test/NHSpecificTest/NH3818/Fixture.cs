using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3818
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
        [Test]
        public void SelectConditionalValuesTest()
        {
            var now = RoundForDialect(DateTime.Now);
            using (var spy = new SqlLogSpy())
            using (var session = OpenSession())
            using (session.BeginTransaction())
            {
                var days = 33;

                var cat = new MyLovelyCat
                {
                    GUID = Guid.NewGuid(),
                    Birthdate = now.AddDays(-days),
                    Color = "Black",
                    Name = "Kitty",
                    Price = 0
                };
                session.Save(cat);
                
                session.Flush();

                var catInfo =
                    session.Query<MyLovelyCat>()
                        .Select(o => new
                        {
                            o.Color,
                            AliveDays = (now - o.Birthdate).TotalDays,
                            o.Name,
                            o.Price,
                        })
                        .Single();

                //Console.WriteLine(spy.ToString());
                // We need a tolerance: we are diffing dates as a timespan instead of counting days boundaries,
                // yielding a float. TimeSpan.Days yould always truncate a resulting partial day, so do not use it.
                Assert.That(catInfo.AliveDays, Is.EqualTo(days).Within(0.1));

                var catInfo2 =
                    session.Query<MyLovelyCat>()
                        .Select(o => new
                        {
                            o.Color,
                            AliveDays = o.Price > 0 ? (now - o.Birthdate).TotalDays : 0,
                            o.Name,
                            o.Price,
                        })
                        .Single();

                //Console.WriteLine(spy.ToString());
                Assert.That(catInfo2.AliveDays, Is.EqualTo(0));

            }
        }

    }
}
