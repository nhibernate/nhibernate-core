using System.Collections;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Expression
{
	/// <summary>
	/// An object-oriented representation of a query criterion that may be used as a constraint
	/// in a <c>Criteria</c> query.
	/// </summary>
	/// <remarks>
	/// Built-in criterion types are provided by the <c>Expression</c> factory class.
	/// This interface might be implemented by application classes but, more commonly, application 
	/// criterion types would extend <c>AbstractCriterion</c>.
	/// </remarks>
	public interface ICriterion
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <param name="aliasClasses"></param>
		/// <returns></returns>
		SqlString ToSqlString( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias, IDictionary aliasClasses );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="aliasClasses"></param>
		/// <returns></returns>
		TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, IDictionary aliasClasses );
	}
}
