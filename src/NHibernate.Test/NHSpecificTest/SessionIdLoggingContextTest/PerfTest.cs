using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.Transform;
using NUnit.Framework;
using NHibernate.Transaction;

namespace NHibernate.Test.NHSpecificTest.SessionIdLoggingContextTest
{
    [TestFixture, Explicit("This is a performance test and take a while.")]
    public class PerfTest : BugTestCase
    {
        const int noOfParents = 1000;
        const int noOfChildrenForEachParent = 20;

        [Test]
        public void Benchmark()
        {
            using(var s= OpenSession())
            {
                var ticksAtStart = DateTime.Now.Ticks;
                var res = s.CreateCriteria<ClassA>()
                    .Fetch("Children")
                    .SetResultTransformer(Transformers.DistinctRootEntity)
                    .Add(Restrictions.Eq("Name", "Parent"))
                    .List<ClassA>();
                Console.WriteLine(TimeSpan.FromTicks(DateTime.Now.Ticks-ticksAtStart));
                Assert.AreEqual(noOfParents, res.Count);
                Assert.AreEqual(noOfChildrenForEachParent, res[0].Children.Count);
            }
        }

        protected override void Configure(Cfg.Configuration configuration)
        {
            //get rid of the overhead supporting distr trans
            configuration.SetProperty(Cfg.Environment.TransactionStrategy, typeof(AdoNetTransactionFactory).FullName);
        }

        protected override void OnSetUp()
        {
            using (var s = OpenSession())
            {
                using (var tx = s.BeginTransaction())
                {
                    for (var i = 0; i < noOfParents; i++)
                    {
                        var parent = createEntity("Parent");
                        for (var j = 0; j < noOfChildrenForEachParent; j++)
                        {
                            var child = createEntity("Child");
                            parent.Children.Add(child);
                        }
                        s.Save(parent);
                    }
                    tx.Commit();
                }
            }
        }

        protected override void OnTearDown()
        {
            using(var s = OpenSession())
            {
                using(var tx = s.BeginTransaction())
                {
                    s.CreateQuery("delete from ClassA").ExecuteUpdate();
                    tx.Commit();
                }
            }
        }

        private static ClassA createEntity(string name)
        {
            var obj = new ClassA
            {
                Children = new List<ClassA>(),
                Name = name
            };
            return obj;
        }
    }
}
