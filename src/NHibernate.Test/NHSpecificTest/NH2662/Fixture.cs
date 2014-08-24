using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2662
{
    public class Fixture : BugTestCase
    {
        [Test]
        public void WhenCastAliasInQueryOverThenDoNotThrow()
        {
            using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
            {
                var customer = new Customer
                {
                    Order = new PizzaOrder { OrderDate = DateTime.Now, PizzaName = "Margarita" }
                };

                var customer2 = new Customer
                {
                    Order = new Order { OrderDate = DateTime.Now.AddDays(1) }
                };

                session.Save(customer);
                session.Save(customer2);
                session.Flush();

                Executing.This(
                    () =>
                    {
                        var temp = session.Query<Customer>().Select(
                            c => new {c.Id, c.Order.OrderDate, ((PizzaOrder)c.Order).PizzaName })
                            .ToArray();

                        foreach (var item in temp) { Trace.WriteLine(item.PizzaName);}
                    })
                    .Should().NotThrow();

                Executing.This(
                    () =>
                        {
                            Order orderAlias = null;

                            var results = 
                            session.QueryOver<Customer>()
                                .Left.JoinAlias(o => o.Order, () => orderAlias)
								.OrderBy(() => orderAlias.OrderDate).Asc
                                .SelectList(list =>
                                            list
                                                .Select(o => o.Id)
                                                .Select(() => orderAlias.OrderDate)
                                                .Select(() => ((PizzaOrder) orderAlias).PizzaName))
                                .List<object[]>();

							Assert.That(results.Count, Is.EqualTo(2));
							Assert.That(results[0][2], Is.EqualTo("Margarita"));

                        }).Should().NotThrow();
            }
        }
    }
}