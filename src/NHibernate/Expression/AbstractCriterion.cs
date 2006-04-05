using System;
using System.Collections;

using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	/// <summary>
	/// Base class for <see cref="ICriterion"/> implementations.
	/// </summary>
	public abstract class AbstractCriterion : ICriterion
	{
		/// <summary>
		/// Gets a string representation of the <see cref="AbstractCriterion"/>.  
		/// </summary>
		/// <returns>
		/// A String that shows the contents of the <see cref="AbstractCriterion"/>.
		/// </returns>
		/// <remarks>
		/// This is not a well formed Sql fragment.  It is useful for logging what the <see cref="AbstractCriterion"/>
		/// looks like.
		/// </remarks>
		public abstract override string ToString();

		#region ICriterion Members

		/// <summary>
		/// Render a SqlString for the expression.
		/// </summary>
		/// <returns>A SqlString that contains a valid Sql fragment.</returns>
		public abstract SqlString ToSqlString( ICriteria criteria, ICriteriaQuery criteriaQuery );

		/// <summary>
		/// Return typed values for all parameters in the rendered SQL fragment
		/// </summary>
		/// <returns>An array of TypedValues for the Expression.</returns>
		public abstract TypedValue[ ] GetTypedValues( ICriteria criteria, ICriteriaQuery criteriaQuery );

		#endregion
	}
}