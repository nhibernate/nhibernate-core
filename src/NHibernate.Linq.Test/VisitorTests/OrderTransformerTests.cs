using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Test.Model;
using NHibernate.Linq.Visitors;
using NUnit.Framework;

namespace NHibernate.Linq.Test.VisitorTests
{
	[TestFixture]
	public class OrderTransformerTests:BaseTest
	{
		[Test,Ignore]
		public void CanBindOrderBys()
		{
			var query = from a in session.Linq<Animal>()
						orderby a.BodyWeight descending,a.BodyWeight 
						select a;
			OrderByTransformer transformer = new OrderByTransformer();
			Expression expr=transformer.Visit(query.Expression);
			Console.WriteLine(expr);
		}
	}
}
