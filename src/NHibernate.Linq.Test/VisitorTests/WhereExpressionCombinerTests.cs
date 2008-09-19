using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Linq.Test.Model;
using NHibernate.Linq.Visitors;
using NUnit.Framework;
namespace NHibernate.Linq.Test.VisitorTests
{
	[TestFixture]
	public class WhereExpressionCombinerTests:BaseTest
	{
		[Test]
		public void CanReduceNestedWhere()
		{
			var query =
				session.Linq<Animal>()
					.Where(a => a.Mother != null)
					.Where(b => b.Father != null)
					.Where(c=>c.Offspring.Where(d=>c.Offspring.Count()>20).Where(e=>e.SerialNumber=="100001").Count()>0);
			var query2 = session.Linq<Animal>()
				.Where(p_0 =>
				       p_0.Mother != null &&
				       p_0.Father != null &&
				       p_0.Offspring.Where(p_1 => p_0.Offspring.Count() > 20 && p_1.SerialNumber == "100001").Count() > 0);
			WhereExpressionCombiner reducer = new WhereExpressionCombiner();
			Expression actual = reducer.Visit(query.Expression);
			Expression expected = query2.Expression;
			Console.WriteLine("Expected");
			Console.WriteLine(expected);
			Console.WriteLine();
			Console.WriteLine("Actual");
			Console.WriteLine(actual);
			Assert.That(actual.ExpressionEquals(expected));
			
		}

	}
}
