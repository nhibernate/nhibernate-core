using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Expression
{
	/// <summary>
	/// An Expression that represents a "between" constraint.
	/// </summary>
	public class BetweenExpression : Expression
	{
		private readonly string propertyName;
		private readonly object lo;
		private readonly object hi;

		/// <summary>
		/// Initialize a new instance of the BetweenExpression class for
		/// the named Property.
		/// </summary>
		/// <param name="propertyName">The name of the Property of the Class.</param>
		/// <param name="lo">The low value for the BetweenExpression.</param>
		/// <param name="hi">The high value for the BetweenExpression.</param>
		internal BetweenExpression( string propertyName, object lo, object hi )
		{
			this.propertyName = propertyName;
			this.lo = lo;
			this.hi = hi;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public override SqlString ToSqlString( ISessionFactoryImplementor factory, System.Type persistentClass, string alias )
		{
			//TODO: add a default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			IType propertyType = ( ( IQueryable ) factory.GetPersister( persistentClass ) ).GetPropertyType( propertyName );
			string[ ] columnNames = GetColumns( factory, persistentClass, propertyName, alias );
			string[ ] paramColumnNames = GetColumns( factory, persistentClass, propertyName, null );
			string[ ] loParamColumnNames = new string[paramColumnNames.Length];
			string[ ] hiParamColumnNames = new string[paramColumnNames.Length];

			// we need to create a _lo and _hi parameter for each column.  The 	columnNames
			// doesn't return a seperate column for the _lo and _hi so we need to...
			for( int i = 0; i < paramColumnNames.Length; i++ )
			{
				loParamColumnNames[ i ] = paramColumnNames[ i ] + "_lo";
				hiParamColumnNames[ i ] = paramColumnNames[ i ] + "_hi";
			}

			Parameter[ ] parameters = new Parameter[paramColumnNames.Length*2];
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
		public override TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass )
		{
			return new TypedValue[ ]
				{
					GetTypedValue( sessionFactory, persistentClass, propertyName, lo ),
					GetTypedValue( sessionFactory, persistentClass, propertyName, hi )
				};
		}

		/// <summary></summary>
		public override string ToString()
		{
			return propertyName + " between " + lo + " and " + hi;
		}
	}
}