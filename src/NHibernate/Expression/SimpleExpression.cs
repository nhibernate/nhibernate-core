using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Expression
{
	/// <summary>
	/// The base class for an Expression that compares a single Property
	/// to a value.
	/// </summary>
	public abstract class SimpleExpression : Expression
	{
		private readonly string propertyName;

		private readonly object expressionValue;

		/// <summary>
		/// Initialize a new instance of the SimpleExpression class for a named
		/// Property and its value.
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="expressionValue">The value for the Property.</param>
		internal SimpleExpression( string propertyName, object expressionValue )
		{
			this.propertyName = propertyName;
			this.expressionValue = expressionValue;
		}

		/// <summary>
		/// Gets the named Property for the Expression.
		/// </summary>
		/// <value>A string that is the name of the Property.</value>
		public string PropertyName
		{
			get { return propertyName; }
		}

		/// <summary>
		/// Gets the Value for the Expression.
		/// </summary>
		/// <value>An object that is the value for the Expression.</value>
		public object Value
		{
			get { return expressionValue; }
		}

		/// <summary>
		/// Converts the SimpleExpression to a <see cref="SqlString"/>.
		/// </summary>
		/// <param name="factory">The ISessionFactory that contains the mapping for the Type.</param>
		/// <param name="persistentClass">The Class the Expression is being built for.</param>
		/// <param name="alias">The alias to use for the table.</param>
		/// <returns>A SqlString that contains a valid Sql fragment.</returns>
		public override SqlString ToSqlString( ISessionFactoryImplementor factory, System.Type persistentClass, string alias )
		{
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			IType propertyType = ( ( IQueryable ) factory.GetPersister( persistentClass ) ).GetPropertyType( propertyName );
			string[ ] columnNames = GetColumns( factory, persistentClass, propertyName, alias );
			string[ ] paramColumnNames = GetColumns( factory, persistentClass, propertyName, null );
			Parameter[ ] parameters = Parameter.GenerateParameters( factory, alias, paramColumnNames, propertyType );


			for( int i = 0; i < columnNames.Length; i++ )
			{
				if( i > 0 )
				{
					sqlBuilder.Add( " AND " );
				}

				sqlBuilder.Add( columnNames[ i ] )
					.Add( Op )
					.Add( parameters[ i ] );

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
			return new TypedValue[ ] {GetTypedValue( sessionFactory, persistentClass, propertyName, expressionValue )};
		}

		/// <summary></summary>
		public override string ToString()
		{
			return propertyName + Op + expressionValue;
		}

		/// <summary></summary>
		protected abstract string Op { get; } //protected ???
	}
}