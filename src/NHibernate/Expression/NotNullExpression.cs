using System;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents "not null" constraint.
	/// </summary>
	public class NotNullExpression : AbstractCriterion
	{
		private readonly string _propertyName;

		private static readonly TypedValue[ ] NoValues = new TypedValue[0];

		/// <summary>
		/// Initialize a new instance of the <see cref="NotNullExpression" /> class for a named
		/// Property that should not be null.
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		public NotNullExpression( string propertyName )
		{
			_propertyName = propertyName;
		}

		public override SqlString ToSqlString( ICriteria criteria, ICriteriaQuery criteriaQuery )
		{
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			string[ ] columnNames = criteriaQuery.GetColumnsUsingProjection( criteria, _propertyName );

			bool opNeeded = false;

			for( int i = 0; i < columnNames.Length; i++ )
			{
				if( opNeeded )
				{
					sqlBuilder.Add( " or " );
				}
				opNeeded = true;

				sqlBuilder.Add( columnNames[ i ] )
					.Add( " is not null" );

			}

			if( columnNames.Length > 1 )
			{
				sqlBuilder.Insert( 0, "(" );
				sqlBuilder.Add( ")" );
			}

			return sqlBuilder.ToSqlString();
		}

		public override TypedValue[ ] GetTypedValues( ICriteria criteria, ICriteriaQuery criteriaQuery )
		{
			return NoValues;
		}

		public override string ToString()
		{
			return _propertyName + " is not null";
		}
	}
}