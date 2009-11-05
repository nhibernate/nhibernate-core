using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class NhNewExpression : Expression
	{
		private readonly ReadOnlyCollection<string> _members;
		private readonly ReadOnlyCollection<Expression> _arguments;

		public NhNewExpression(IList<string> members, IList<Expression> arguments)
			: base((ExpressionType)NhExpressionType.New, typeof(object))
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
	}
}