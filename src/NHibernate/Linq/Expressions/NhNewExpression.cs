using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Expressions
{
	public class NhNewExpression : ExtensionExpression
	{
		private readonly ReadOnlyCollection<string> _members;
		private readonly ReadOnlyCollection<Expression> _arguments;

		public NhNewExpression(IList<string> members, IList<Expression> arguments)
			: base(typeof(object), (ExpressionType)NhExpressionType.New)
		{
			_members = new ReadOnlyCollection<string>(members);
			_arguments = new ReadOnlyCollection<Expression>(arguments);
		}

		public ReadOnlyCollection<Expression> Arguments
		{
			get { return _arguments; }
		}

		public ReadOnlyCollection<string> Members
		{
			get { return _members; }
		}

		protected override Expression VisitChildren(ExpressionTreeVisitor visitor)
		{
			var arguments = visitor.VisitAndConvert(Arguments, "VisitNhNew");

			return arguments != Arguments
					   ? new NhNewExpression(Members, arguments)
					   : this;
		}
	}

	public class NhStarExpression : ExtensionExpression
	{
		public NhStarExpression(Expression expression)
			: base(expression.Type, (ExpressionType)NhExpressionType.Star)
		{
			Expression = expression;
		}

		public Expression Expression
		{
			get;
			private set;
		}

		protected override Expression VisitChildren(ExpressionTreeVisitor visitor)
		{
			var newExpression = visitor.VisitExpression(Expression);

			return newExpression != Expression
					   ? new NhStarExpression(newExpression)
					   : this;
		}
	}
}