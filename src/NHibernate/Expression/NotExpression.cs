using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that negates another <see cref="ICriterion"/>.
	/// </summary>
	public class NotExpression : AbstractCriterion
	{
		private ICriterion _criterion;

		/// <summary>
		/// Initialize a new instance of the <see cref="NotExpression" /> class for an
		/// <see cref="ICriterion"/>
		/// </summary>
		/// <param name="criterion">The <see cref="ICriterion"/> to negate.</param>
		public NotExpression( ICriterion criterion )
		{
			_criterion = criterion;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public override SqlString ToSqlString( ISessionFactoryImplementor factory, System.Type persistentClass, string alias, IDictionary aliasClasses )
		{
			//TODO: set default capacity
			SqlStringBuilder builder = new SqlStringBuilder();

			if( factory.Dialect is Dialect.MySQLDialect )
			{
				builder.Add( "not (" );
			}
			else
			{
				builder.Add( "not " );
			}

			builder.Add( _criterion.ToSqlString( factory, persistentClass, alias, aliasClasses ) );

			if( factory.Dialect is Dialect.MySQLDialect )
			{
				builder.Add( ")" );
			}

			return builder.ToSqlString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public override TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, IDictionary aliasClasses )
		{
			return _criterion.GetTypedValues( sessionFactory, persistentClass, aliasClasses );
		}

		/// <summary></summary>
		public override string ToString()
		{
			return "not " + _criterion.ToString();
		}
	}
}