using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that combines two <see cref="ICriterion"/>s 
	/// with a operator (either "<c>and</c>" or "<c>or</c>") between them.
	/// </summary>
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
		internal LogicalExpression( ICriterion lhs, ICriterion rhs )
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
		/// <param name="sessionFactory">The ISessionFactory to get the Persistence information from.</param>
		/// <param name="persistentClass">The Type we are constructing the Expression for.</param>
		/// <returns>An arry of <see cref="TypedValue"/>s.</returns>
		public override TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, IDictionary aliasClasses )
		{
			TypedValue[ ] lhstv = _lhs.GetTypedValues( sessionFactory, persistentClass, aliasClasses );
			TypedValue[ ] rhstv = _rhs.GetTypedValues( sessionFactory, persistentClass, aliasClasses );
			TypedValue[ ] result = new TypedValue[lhstv.Length + rhstv.Length];
			Array.Copy( lhstv, 0, result, 0, lhstv.Length );
			Array.Copy( rhstv, 0, result, lhstv.Length, rhstv.Length );
			return result;
		}

		/// <summary>
		/// Converts the LogicalExpression to a <see cref="SqlString"/>.
		/// </summary>
		/// <param name="factory">The ISessionFactory to use to build the SqlString.</param>
		/// <param name="persistentClass">The Type we are constructing the Expression for.</param>
		/// <param name="alias">The alias to use when referencing the table.</param>
		/// <returns>A well formed SqlString for the Where clause.</returns>
		/// <remarks>The SqlString will be enclosed by <c>(</c> and <c>)</c>.</remarks>
		public override SqlString ToSqlString( ISessionFactoryImplementor factory, System.Type persistentClass, string alias, IDictionary aliasClasses )
		{
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			SqlString lhSqlString = _lhs.ToSqlString( factory, persistentClass, alias, aliasClasses );
			SqlString rhSqlString = _rhs.ToSqlString( factory, persistentClass, alias, aliasClasses );

			sqlBuilder.Add( new SqlString[ ] {lhSqlString, rhSqlString},
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
			return _lhs.ToString() + ' ' + Op + ' ' + _rhs.ToString();
		}
	}
}