using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq
{
	public class ContextualNhExpressionTreeVisitor : NhExpressionTreeVisitor
	{
		private Stack<VisitorContext> _contextStack;

		public ContextualNhExpressionTreeVisitor()
		{
			_contextStack = new Stack<VisitorContext>();
			_contextStack.Push(null);
		}

		public override Expression VisitExpression(Expression expression)
		{
			if (expression == null)
				return null;

			VisitorContext context = new VisitorContext(_contextStack.Peek(), expression);
			_contextStack.Push(context);

			Expression result = base.VisitExpression(expression);

			_contextStack.Pop();

			return result;
		}

		protected VisitorContext Context
		{
			get { return _contextStack.Peek(); }
		}
	}
}
