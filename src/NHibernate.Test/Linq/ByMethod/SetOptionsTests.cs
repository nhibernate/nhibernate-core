using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Cfg;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Engine;
using NHibernate.Linq;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
    public class SetOptionsTests
    {
		

		[Test]
		public void AppliesOptionsToQuery()
		{
			var session = Substitute.For<ISessionImplementor>();
			session.Factory.Settings.Returns(new Settings());

			var query = Substitute.For<IQuery>();

			query.List().Returns(new List<Order>());

			session.CreateQuery(Arg.Any<IQueryExpression>()).Returns(query);

			var queryable = new NhQueryable<Order>(session).SetOptions(x=>
				x
					.SetCacheable(true)
					.SetCacheMode(CacheMode.Normal)
					.SetCacheRegion("testregion")
					.SetTimeout(10)
					
			);

			var t = queryable.ToList();

			query.Received(1).SetCacheable(true);
			query.Received(1).SetCacheMode(CacheMode.Normal);
			query.Received(1).SetCacheRegion("testregion");
			query.Received(1).SetTimeout(10);

		}
	}
}
