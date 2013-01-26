using System.Linq.Expressions;

namespace NHibernate.Linq.NestedSelects
{
	internal class ExpressionHolder
	{
		public int Tuple { get; set; }
		public Expression Expression { get; set; }
	}
}