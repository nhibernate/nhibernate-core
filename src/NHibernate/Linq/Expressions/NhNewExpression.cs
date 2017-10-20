using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Expressions
{
	public class NhNewExpression : NhExpression
	{
		public NhNewExpression(IList<string> members, IList<Expression> arguments)
		{
			Members = new ReadOnlyCollection<string>(members);
			Arguments = new ReadOnlyCollection<Expression>(arguments);
		}

		public override System.Type Type => typeof(object);

		public ReadOnlyCollection<Expression> Arguments { get; }

		public ReadOnlyCollection<string> Members { get; }

		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			var arguments = visitor.VisitAndConvert(Arguments, "VisitNhNew");

			return arguments != Arguments
				? new NhNewExpression(Members, arguments)
				: this;
		}

		protected override Expression Accept(NhExpressionVisitor visitor)
		{
			return visitor.VisitNhNew(this);
		}
	}
}
