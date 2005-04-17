using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	/// <summary>
	/// An object-oriented representation of a query criterion that may be used as a constraint
	/// in a <see cref="ICriteria"/> query.
	/// </summary>
	/// <remarks>
	/// Built-in criterion types are provided by the <c>Expression</c> factory class.
	/// This interface might be implemented by application classes but, more commonly, application 
	/// criterion types would extend <c>AbstractCriterion</c>.
	/// </remarks>
	public interface ICriterion
	{
		/// <summary>
		/// Render a SqlString fragment for the expression.
		/// </summary>
		/// <param name="sessionFactory">The ISessionFactory that contains the mapping for the Type.</param>
		/// <param name="persistentClass">The Class the Expression is being built for.</param>
		/// <param name="alias">The alias to use for the table.</param>
		/// <param name="aliasClasses"></param>
		/// <returns>A SqlString that contains a valid Sql fragment.</returns>
		SqlString ToSqlString( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias, IDictionary aliasClasses );

		/// <summary>
		/// Return typed values for all parameters in the rendered SQL fragment
		/// </summary>
		/// <param name="sessionFactory">The ISessionFactory that contains the mapping for the Type.</param>
		/// <param name="persistentClass">The Class the Expression is being built for.</param>
		/// <returns>An array of TypedValues for the Expression.</returns>
		TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, IDictionary aliasClasses );
	}
}
