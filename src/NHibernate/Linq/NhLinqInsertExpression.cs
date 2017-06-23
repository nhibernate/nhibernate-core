using System.Linq.Expressions;
using NHibernate.Engine;

namespace NHibernate.Linq
{
	public class NhLinqInsertExpression<TTarget> : NhLinqExpression
	{
		protected override QueryMode QueryMode => QueryMode.Insert;

		/// <summary>
		/// Entity type to insert or update when the expression is a DML.
		/// </summary>
		protected override System.Type TargetType => typeof(TTarget);

		public NhLinqInsertExpression(ISessionFactoryImplementor sessionFactory, Expression expression)
			: base(expression, sessionFactory)
		{
			Key = "INSERT " + Key;
		}
	}
}
