using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Linq.Test
{
	[TestFixture]
	public class ExpressionEqualityCheckerTests
	{
		[Test]
		public void SimpleExpressionsProducesCorrectResult()
		{
			Assert.IsTrue(ExpressionEqualityChecker.ExpressionEquals(Expression.Constant(4), Expression.Constant(4)));
			Assert.IsFalse(ExpressionEqualityChecker.ExpressionEquals(Expression.Constant(4), Expression.Constant(3)));
			Assert.IsTrue(ExpressionEqualityChecker.ExpressionEquals(Expression.Not(Expression.Constant(4)), Expression.Not(Expression.Constant(4))));
			Assert.IsFalse(ExpressionEqualityChecker.ExpressionEquals(Expression.Not(Expression.Constant(4)), Expression.Not(Expression.Constant(2))));
		}
	}
}
