using System;
using System.Text;
using System.Collections;

using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Expression 
{

	/// <summary>
	/// An Expression that combines two other <see cref="Expression"/>s with an 
	/// <c>"and"</c> or <c>"or"</c> between them.
	/// </summary>
	public abstract class LogicalExpression : Expression 
	{

		/// <summary>
		/// The Expression that will be on the Left Hand Side of the Op.
		/// </summary>
		protected Expression lhs;
		
		/// <summary>
		/// The Expression that will be on the Right Hand Side of the Op.
		/// </summary>
		protected Expression rhs;

		/// <summary>
		/// Initialize a new instance of the LogicalExpression class that
		/// combines two other Expressions.
		/// </summary>
		/// <param name="lhs">The Expression to use in the Left Hand Side.</param>
		/// <param name="rhs">The Expression to use in the Right Hand Side.</param>
		internal LogicalExpression(Expression lhs, Expression rhs) 
		{
			this.lhs = lhs;
			this.rhs = rhs;
		}

		/// <summary>
		/// Combines the <see cref="TypedValue"/> for the Left Hand Side and the 
		/// Right Hand Side of the Expression into one array.
		/// </summary>
		/// <param name="sessionFactory">The ISessionFactory to get the Persistence information from.</param>
		/// <param name="persistentClass">The Type we are constructing the Expression for.</param>
		/// <returns>An arry of <see cref="TypeValue"/>s.</returns>
		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor	sessionFactory, System.Type persistentClass) 
		{
			TypedValue[] lhstv = lhs.GetTypedValues(sessionFactory, persistentClass);
			TypedValue[] rhstv = rhs.GetTypedValues(sessionFactory, persistentClass);
			TypedValue[] result = new TypedValue[ lhstv.Length + rhstv.Length ];
			Array.Copy(lhstv, 0, result, 0, lhstv.Length);
			Array.Copy(rhstv, 0, result, lhstv.Length, rhstv.Length);
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
		public override SqlString ToSqlString(ISessionFactoryImplementor factory, System.Type persistentClass, string alias) 
		{
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			SqlString lhSqlString = lhs.ToSqlString(factory, persistentClass, alias);
			SqlString rhSqlString = rhs.ToSqlString(factory, persistentClass, alias);

			sqlBuilder.Add(new SqlString[] {lhSqlString, rhSqlString},
				"(", 
				Op, 
				")");
				
			
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
			return lhs.ToString() + ' ' + Op + ' ' + rhs.ToString();
		}
	}
}