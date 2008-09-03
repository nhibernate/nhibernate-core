using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An <see cref="ICriterion"/> that combines two <see cref="ICriterion"/>s 
	/// with a operator (either "<c>and</c>" or "<c>or</c>") between them.
	/// </summary>
	[Serializable]
	public abstract class LogicalExpression : AbstractCriterion
	{
		private ICriterion _lhs;
		private ICriterion _rhs;

		/// <summary>
		/// Initialize a new instance of the <see cref="LogicalExpression" /> class that
		/// combines two other <see cref="ICriterion"/>s.
		/// </summary>
		/// <param name="lhs">The <see cref="ICriterion"/> to use in the Left Hand Side.</param>
		/// <param name="rhs">The <see cref="ICriterion"/> to use in the Right Hand Side.</param>
		protected LogicalExpression(ICriterion lhs, ICriterion rhs)
		{
			_lhs = lhs;
			_rhs = rhs;
		}

		/// <summary>
		/// Gets the <see cref="ICriterion"/> that will be on the Left Hand Side of the Op.
		/// </summary>
		protected ICriterion LeftHandSide
		{
			get { return _lhs; }
		}

		/// <summary>
		/// Gets the <see cref="ICriterion" /> that will be on the Right Hand Side of the Op.
		/// </summary>
		protected ICriterion RightHandSide
		{
			get { return _rhs; }
		}

		/// <summary>
		/// Combines the <see cref="TypedValue"/> for the Left Hand Side and the 
		/// Right Hand Side of the Expression into one array.
		/// </summary>
		/// <returns>An array of <see cref="TypedValue"/>s.</returns>
		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			TypedValue[] lhstv = _lhs.GetTypedValues(criteria, criteriaQuery);
			TypedValue[] rhstv = _rhs.GetTypedValues(criteria, criteriaQuery);
			TypedValue[] result = new TypedValue[lhstv.Length + rhstv.Length];
			Array.Copy(lhstv, 0, result, 0, lhstv.Length);
			Array.Copy(rhstv, 0, result, lhstv.Length, rhstv.Length);
			return result;
		}

		public override IProjection[] GetProjections()
		{		
			return null;
		}

		/// <summary>
		/// Converts the LogicalExpression to a <see cref="SqlString"/>.
		/// </summary>
		/// <returns>A well formed SqlString for the Where clause.</returns>
		/// <remarks>The SqlString will be enclosed by <c>(</c> and <c>)</c>.</remarks>
		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			SqlString lhSqlString = _lhs.ToSqlString(criteria, criteriaQuery, enabledFilters);
			SqlString rhSqlString = _rhs.ToSqlString(criteria, criteriaQuery, enabledFilters);

			sqlBuilder.Add(new SqlString[] {lhSqlString, rhSqlString},
			               "(",
			               Op,
			               ")",
			               false // not wrapping because the prefix and postfix params already take care of that	
				);


			return sqlBuilder.ToSqlString();
		}

		/// <summary>
		/// Get the Sql operator to put between the two <see cref="Expression"/>s.
		/// </summary>
		protected abstract string Op { get; } //protected ???

		/// <summary>
		/// Gets a string representation of the LogicalExpression.  
		/// </summary>
		/// <returns>
		/// The String contains the LeftHandSide.ToString() and the RightHandSide.ToString()
		/// joined by the Op.
		/// </returns>
		/// <remarks>
		/// This is not a well formed Sql fragment.  It is useful for logging what Expressions
		/// are being combined.
		/// </remarks>
		public override string ToString()
		{
			return '(' + _lhs.ToString() + ' ' + Op + ' ' + _rhs.ToString() + ')';
		}
	}
}
