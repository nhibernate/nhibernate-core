using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class NhCountExpression : NhAggregatedExpression
	{
        public NhCountExpression(Expression expression, System.Type type)
			: base(expression, type, NhExpressionType.Count)
		{
		}
	}

    public class NhShortCountExpression : NhCountExpression
    {
        public NhShortCountExpression(Expression expression)
            : base(expression, typeof(int))
        {
        }
    }

    public class NhLongCountExpression : NhCountExpression
    {
        public NhLongCountExpression(Expression expression)
            : base(expression, typeof(long))
        {
        }
    }
}
