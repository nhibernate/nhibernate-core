using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents a "between" constraint.
	/// </summary>
	public class BetweenExpression : AbstractCriterion
	{
		private readonly string _propertyName;
		private readonly object _lo;
		private readonly object _hi;

		/// <summary>
		/// Initialize a new instance of the <see cref="BetweenExpression" /> class for
		/// the named Property.
		/// </summary>
		/// <param name="propertyName">The name of the Property of the Class.</param>
		/// <param name="lo">The low value for the BetweenExpression.</param>
		/// <param name="hi">The high value for the BetweenExpression.</param>
		internal BetweenExpression( string propertyName, object lo, object hi )
		{
			_propertyName = propertyName;
			_lo = lo;
			_hi = hi;
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
			//TODO: add a default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			IType propertyType = AbstractCriterion.GetType( factory, persistentClass,_propertyName, aliasClasses );
			string[ ] columnNames = AbstractCriterion.GetColumns( factory, persistentClass, _propertyName, alias, aliasClasses );
			
			// don't need to worry about aliasing or aliasClassing for parameter column names
			string[ ] paramColumnNames = AbstractCriterion.GetColumns( factory, persistentClass, _propertyName );
			string[ ] loParamColumnNames = new string[paramColumnNames.Length];
			string[ ] hiParamColumnNames = new string[paramColumnNames.Length];

			// we need to create a _lo and _hi parameter for each column.  The 	columnNames
			// doesn't return a seperate column for the _lo and _hi so we need to...
			for( int i = 0; i < paramColumnNames.Length; i++ )
			{
				loParamColumnNames[ i ] = paramColumnNames[ i ] + "_lo";
				hiParamColumnNames[ i ] = paramColumnNames[ i ] + "_hi";
			}

			Parameter[ ] loParameters = Parameter.GenerateParameters( factory, alias, loParamColumnNames, propertyType );
			Parameter[ ] hiParameters = Parameter.GenerateParameters( factory, alias, hiParamColumnNames, propertyType );
			bool andNeeded = false;

			for( int i = 0; i < columnNames.Length; i++ )
			{
				if( andNeeded )
				{
					sqlBuilder.Add( " AND " );
				}
				andNeeded = true;

				sqlBuilder.Add( columnNames[ i ] )
					.Add( " between " )
					.Add( loParameters[ i ] )
					.Add( " and " )
					.Add( hiParameters[ i ] );
			}

			return sqlBuilder.ToSqlString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public override TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, IDictionary aliasClasses )
		{
			return new TypedValue[ ]
				{
					AbstractCriterion.GetTypedValue( sessionFactory, persistentClass, _propertyName, _lo, aliasClasses ),
					AbstractCriterion.GetTypedValue( sessionFactory, persistentClass, _propertyName, _hi, aliasClasses )
				};
		}

		/// <summary></summary>
		public override string ToString()
		{
			return _propertyName + " between " + _lo + " and " + _hi;
		}
	}
}