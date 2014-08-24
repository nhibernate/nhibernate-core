using System.Linq.Expressions;

namespace NHibernate.Linq.NestedSelects
{
	class ExpressionHolder
	{
		public int Tuple { get; set; }
		public Expression Expression { get; set; }
	}
}