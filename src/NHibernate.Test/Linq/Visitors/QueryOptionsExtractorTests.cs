using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Linq;
using NHibernate.Linq.Visitors;
using NSubstitute;
using NUnit.Framework;
using Remotion.Linq;

namespace NHibernate.Test.Linq.Visitors
{
	[TestFixture]
	public class QueryOptionsExtractorTests
    {
		[Test]
	    public void NoOptionsShouldBeFound()
	    {
		    var orders = new List<Order>().AsQueryable();

		    var queryModel = CreateQueryModel(orders);

			var operators = QueryOptionsExtractor.ExtractOptions(queryModel);

		    Assert.That(operators.Count, Is.EqualTo(0));
		}

	    [Test]
	    public void OneOptionShouldBeFound()
	    {
		    var orders = new List<Order>().AsQueryable();

		    orders = orders.SetOptions(x => x.SetCacheable(true));

		    var queryModel = CreateQueryModel(orders);

			var operators = QueryOptionsExtractor.ExtractOptions(queryModel);

		    Assert.That(operators.Count, Is.EqualTo(1));
		}

	    [Test]
	    public void FindsAllOptionsOnRootQuery()
	    {
			var orders = new List<Order>().AsQueryable();
			var orders2 = new List<Order>().AsQueryable();
		    var orderslines = new List<OrderLine>().AsQueryable();

		    orderslines = orderslines.SetOptions(x => x.SetCacheRegion("Ignored"));

		    var result = orders
			    .SetOptions(x => x.SetCacheable(false))
			    .Join(orders2.SetOptions(x => x.SetCacheable(false)), x => x.OrderId, x => x.OrderId, (x, y) => x)
			    .SetOptions(x => x.SetCacheable(false))
			    .Where(o => orderslines.Any(s => s.Order == o))
			    .SetOptions(x => x.SetCacheable(false))
			    .SelectMany(x => x.OrderLines)
			    .SetOptions(x => x.SetCacheable(false))
			    .Select(x => new {Id = x.Id})
			    .SetOptions(x => x.SetCacheable(true));
				

			var queryModel = CreateQueryModel(result);



			var operators = QueryOptionsExtractor.ExtractOptions(queryModel);


			Assert.That(operators.Count, Is.EqualTo(5));

		    var options = Substitute.For<IQueryableOptions>();

			operators.Last().Invoke(options);

		    options.Received(1).SetCacheable(true);
	    }

	    private QueryModel CreateQueryModel<T>(IQueryable<T> queryable)
	    {
		    return NhRelinqQueryParser.Parse(NhRelinqQueryParser.PreTransform(queryable.Expression));
		}
    }
}
