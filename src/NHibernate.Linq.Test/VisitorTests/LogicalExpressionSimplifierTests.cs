using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Criterion;
using NHibernate.Linq.Visitors;
using NUnit.Framework;
using Expression=System.Linq.Expressions.Expression;

namespace NHibernate.Linq.Test.VisitorTests
{
	[TestFixture]
	public class LogicalExpressionSimplifierTests
	{
		[Test]
		public void CanReduceAndAlsoWithFalse()
		{
			var left = Expression.Constant(false);
			var right = Expression.GreaterThan(Expression.Constant(3), Expression.Constant(1));
			var ex1 = Expression.AndAlso(left, right);
			var ex2 = Expression.AndAlso(right, left);
			var reduced1 = Reduce(ex1) as ConstantExpression;
			var reduced2 = Reduce(ex2) as ConstantExpression;
			Assert.IsNotNull(reduced1);
			Assert.IsNotNull(reduced2);
			Assert.IsFalse((bool)reduced1.Value);
			Assert.IsFalse((bool)reduced2.Value);

			Console.WriteLine("Original expressions\n");
			Console.WriteLine(ex1.ToString());
			Console.WriteLine(ex2.ToString());
			Console.WriteLine("");
			Console.WriteLine("Reduced expressions\n");
			Console.WriteLine(reduced1.ToString());
			Console.WriteLine(reduced2.ToString());
		}

		[Test]
		public void CanReduceOrWithTrue()
		{
			var left = Expression.Constant(true);
			var right = Expression.GreaterThan(Expression.Constant(3), Expression.Constant(1));
			var ex1 = Expression.OrElse(left, right);
			var ex2 = Expression.OrElse(right, left);
			var reduced1 = Reduce(ex1) as ConstantExpression;
			var reduced2 = Reduce(ex2) as ConstantExpression;
			Assert.IsNotNull(reduced1);
			Assert.IsNotNull(reduced2);
			Assert.IsTrue((bool)reduced1.Value);
			Assert.IsTrue((bool)reduced2.Value);

			Console.WriteLine("Original expressions");
			Console.WriteLine("");
			Console.WriteLine(ex1.ToString());
			Console.WriteLine(ex2.ToString());
			Console.WriteLine("");
			Console.WriteLine("Reduced expressions");
			Console.WriteLine("");
			Console.WriteLine(reduced1.ToString());
			Console.WriteLine(reduced2.ToString());
		}

		[Test]
		public void CanReduceEqualWithTrue()
		{
			var left = Expression.Constant(true);
			var right = Expression.GreaterThan(Expression.Constant(3), Expression.Constant(1));
			var ex1 = Expression.Equal(left, right);
			var ex2 = Expression.Equal(right, left);
			var reduced1 = Reduce(ex1);
			var reduced2 = Reduce(ex2);
			Assert.IsNotNull(reduced1);
			Assert.IsNotNull(reduced2);
			Assert.AreEqual(reduced1,right);
			Assert.AreEqual(reduced2, right);

			Console.WriteLine("Original expressions");
			Console.WriteLine("");
			Console.WriteLine(ex1.ToString());
			Console.WriteLine(ex2.ToString());
			Console.WriteLine("");
			Console.WriteLine("Reduced expressions");
			Console.WriteLine("");
			Console.WriteLine(reduced1.ToString());
			Console.WriteLine(reduced2.ToString());
		}
		[Test]
		public void CanReduceEqualWithFalse()
		{
			var left = Expression.Constant(false);
			var right = Expression.GreaterThan(Expression.Constant(3), Expression.Constant(1));
			var ex1 = Expression.Equal(left, right);
			var ex2 = Expression.Equal(right, left);
			var reduced1 = Reduce(ex1) as UnaryExpression;
			var reduced2 = Reduce(ex2) as UnaryExpression;
			Assert.IsNotNull(reduced1);
			Assert.IsNotNull(reduced2);
			Assert.AreEqual(ExpressionType.Not, reduced1.NodeType);
			Assert.AreEqual(ExpressionType.Not, reduced2.NodeType);
			Assert.AreEqual(right,reduced1.Operand);
			Assert.AreEqual(right, reduced2.Operand);

			Console.WriteLine("Original expressions");
			Console.WriteLine("");
			Console.WriteLine(ex1.ToString());
			Console.WriteLine(ex2.ToString());
			Console.WriteLine("");
			Console.WriteLine("Reduced expressions");
			Console.WriteLine("");
			Console.WriteLine(reduced1.ToString());
			Console.WriteLine(reduced2.ToString());
		}

		[Test]
		public void CanReduceNotEqualWithTrue()
		{
			var left = Expression.Constant(true);
			var right = Expression.GreaterThan(Expression.Constant(3), Expression.Constant(1));
			var ex1 = Expression.NotEqual(left, right);
			var ex2 = Expression.NotEqual(right, left);
			var reduced1 = Reduce(ex1) as UnaryExpression;
			var reduced2 = Reduce(ex2) as UnaryExpression;
	
			Assert.IsNotNull(reduced1);
			Assert.IsNotNull(reduced2);
			Assert.AreEqual(ExpressionType.Not, reduced1.NodeType);
			Assert.AreEqual(ExpressionType.Not, reduced2.NodeType);
			Assert.AreEqual(right, reduced1.Operand);
			Assert.AreEqual(right, reduced2.Operand);

			Console.WriteLine("Original expressions");
			Console.WriteLine("");
			Console.WriteLine(ex1.ToString());
			Console.WriteLine(ex2.ToString());
			Console.WriteLine("");
			Console.WriteLine("Reduced expressions");
			Console.WriteLine(reduced1.ToString());
			Console.WriteLine(reduced2.ToString());
		}
		[Test]
		public void CanReduceNotEqualWithFalse()
		{
			var left = Expression.Constant(false);
			var right = Expression.GreaterThan(Expression.Constant(3), Expression.Constant(1));
			var ex1 = Expression.NotEqual(left, right);
			var ex2 = Expression.NotEqual(right, left);
			var reduced1 = Reduce(ex1);
			var reduced2 = Reduce(ex2);
			Assert.IsNotNull(reduced1);
			Assert.IsNotNull(reduced2);
			Assert.AreEqual(reduced1, right);
			Assert.AreEqual(reduced2, right);

			Console.WriteLine("Original expressions\n");
			Console.WriteLine("");
			Console.WriteLine(ex1.ToString());
			Console.WriteLine(ex2.ToString());
			Console.WriteLine("");
			Console.WriteLine("Reduced expressions\n");
			Console.WriteLine("");
			Console.WriteLine(reduced1.ToString());
			Console.WriteLine(reduced2.ToString());
		}		
		

		[Test]
		public void CanReduceNested()
		{
			var trueExpression = Expression.Constant(true);
			var falseExpression = Expression.Constant(false);
			var testExpression = Expression.Equal(Expression.Equal(Expression.Equal(falseExpression, falseExpression), falseExpression),falseExpression);
			var reduced = Reduce(testExpression) as ConstantExpression;
			Assert.IsNotNull(reduced);
			Assert.IsTrue((bool)reduced.Value);

			Console.WriteLine("Original expressions\n");
			Console.WriteLine(testExpression.ToString());

			Console.WriteLine("Reduced expressions\n");
			Console.WriteLine(reduced.ToString());
		}
		[Test]
		public void CanReduceNested2()
		{
			var trueExpression = Expression.Constant(true);
			var falseExpression = Expression.Constant(false);
			var testExpression = Expression.Equal(Expression.Equal(Expression.NotEqual(trueExpression, falseExpression), falseExpression), falseExpression);
			var reduced = Reduce(testExpression) as ConstantExpression;
			Assert.IsNotNull(reduced);
			Assert.IsTrue((bool)reduced.Value);

			Console.WriteLine("Original expressions\n");
			Console.WriteLine(testExpression.ToString());

			Console.WriteLine("Reduced expressions\n");
			Console.WriteLine(reduced.ToString());
		}

		protected Expression Reduce(Expression expr)
		{
			var reducer = new LogicalExpressionReducer();
			return reducer.Visit(expr);
		}

	}
}