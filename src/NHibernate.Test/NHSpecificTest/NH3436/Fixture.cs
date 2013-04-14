using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Linq;
using NHibernate.Test.Linq;
using NUnit.Framework;
using Environment = System.Environment;

namespace NHibernate.Test.NHSpecificTest.NH3436
{
    [TestFixture]
    public class Fixture : LinqTestCase
    {
        [Test]
        public void Few_same_queries_with_closure_in_predicate_should_not_throw_error_when_run_in_parallel()
        {
            var errors = new ConcurrentBag<Exception>();
            Parallel.Invoke(Enumerable.Range(0, 64).Select(i =>
                {
                    return (System.Action)( () =>
                        {
                            try
                            {
                                var names = new[] {"ayende", "xxx"};
                                var session = this.Sfi.OpenSession();
                                try
                                {
                                    var result = session.Query<User>().Where(x => names.Contains(x.Name)).ToList();
                                }
                                finally
                                {
                                    session.Close();
                                    session.Dispose();
                                }
                            }
                            catch (Exception ex)
                            {
                                errors.Add(ex);
                            }
                        });
                }).ToArray());

            if (errors.Any())
            {
                var message = string.Join(Environment.NewLine + "*******************************************************************" + Environment.NewLine,
                                          errors.Select(x => x.ToString()));
                Assert.Fail(message);
            }
        }

        private LinqReadonlyTestsContext _fixtureContext;

        protected override void OnSetUp()
        {
            _fixtureContext = new LinqReadonlyTestsContext();
            _fixtureContext.CreateNorthwindDb();

            base.OnSetUp();
        }

        protected override void OnTearDown()
        {
            if(_fixtureContext != null)
                _fixtureContext.DestroyNorthwindDb();

            base.OnTearDown();
        }
    }
}