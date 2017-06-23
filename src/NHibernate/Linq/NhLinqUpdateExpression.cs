using System.Linq.Expressions;
using NHibernate.Engine;

namespace NHibernate.Linq
{
	public class NhLinqUpdateExpression<T> : NhLinqExpression
	{
		protected override QueryMode QueryMode => _versioned ? QueryMode.UpdateVersioned : QueryMode.Update;

		/// <summary>
		/// Entity type to insert or update when the expression is a DML.
		/// </summary>
		protected override System.Type TargetType => typeof(T);

		private readonly bool _versioned;

		public NhLinqUpdateExpression(ISessionFactoryImplementor sessionFactory, Expression expression, bool versioned)
			: base(expression, sessionFactory)
		{
			_versioned = versioned;
			Key = (versioned ? "UPDATE VERSIONED " : "UPDATE ") + Key;
		}
	}
}
