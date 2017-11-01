using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Linq;
using NHibernate.Linq.Visitors;
using NHibernate.Param;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.Linq.Visitors
{
	[TestFixture]
	public class ExpressionKeyVisitorTests
	{
		[Test]
		public void ExpressionsWithSameQueryShouldRenderEqualKeys()
		{
			var parameterMap = Substitute.For<IDictionary<ConstantExpression, NamedParameter>>();

			NamedParameter parameter;
			parameterMap
				.TryGetValue(Arg.Is<ConstantExpression>(x=>x.Type==typeof(int)), out parameter)
				.Returns(x => {
					x[1] = new NamedParameter("p1",((ConstantExpression) x[0]).Value,NHibernateUtil.Int32);
					return true;
				});

			var expression1 = new List<Order>().AsQueryable().Where(x => x.OrderId > 4).Expression;
			var expression2 = new List<Order>().AsQueryable().Where(x => x.OrderId > 5).SetOptions(x => x.SetCacheable(true)).Expression;

			
			var key1 = ExpressionKeyVisitor.Visit(expression1, parameterMap);
			var key2 = ExpressionKeyVisitor.Visit(expression2, parameterMap);

			Assert.That(key1,Is.EqualTo(key2));
		}
	}
}
