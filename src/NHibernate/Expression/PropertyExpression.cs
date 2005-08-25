using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Expression
{
	/// <summary>
	/// Superclass for an <see cref="ICriterion"/> that represents a
	/// constraint between two properties (with SQL binary operators).
	/// </summary>
	public abstract class PropertyExpression : AbstractCriterion
	{
		private string _lhsPropertyName;
		private string _rhsPropertyName;

		private static TypedValue[ ] NoTypedValues = new TypedValue[0];

		/// <summary>
		/// Initialize a new instance of the <see cref="PropertyExpression" /> class 
		/// that compares two mapped properties.
		/// </summary>
		/// <param name="lhsPropertyName">The name of the Property to use as the left hand side.</param>
		/// <param name="rhsPropertyName">The name of the Property to use as the right hand side.</param>
		protected PropertyExpression(string lhsPropertyName, string rhsPropertyName)
		{
			_lhsPropertyName = lhsPropertyName;
			_rhsPropertyName = rhsPropertyName;
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
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			string[ ] columnNames = AbstractCriterion.GetColumns( factory, persistentClass, _lhsPropertyName, alias, aliasClasses );
			string[ ] otherColumnNames = AbstractCriterion.GetColumns( factory, persistentClass, _rhsPropertyName, alias, aliasClasses );

			string result = string.Join(
				" and ",
				StringHelper.Add( columnNames, Op, otherColumnNames )
			);

			if( columnNames.Length > 1 )
			{
				result = StringHelper.OpenParen + result + StringHelper.ClosedParen;
			}

			return new SqlString( result );
			//TODO: get SQL rendering out of this package!
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public override TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, IDictionary aliasClasses )
		{
			return NoTypedValues;
		}

		/// <summary></summary>
		public override string ToString()
		{
			return _lhsPropertyName + Op + _rhsPropertyName;
		}

		/// <summary>
		/// Get the Sql operator to use for the property expression.
		/// </summary>
		protected abstract string Op { get; }
	}
}