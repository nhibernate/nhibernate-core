using System.Collections;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Expression
{
	/// <summary>
	/// The base class for an <see cref="ICriterion"/> that compares a single Property
	/// to a value.
	/// </summary>
	public abstract class SimpleExpression : AbstractCriterion
	{
		private readonly string _propertyName;

		private readonly object _value;

		/// <summary>
		/// Initialize a new instance of the <see cref="SimpleExpression" /> class for a named
		/// Property and its value.
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		internal SimpleExpression( string propertyName, object value )
		{
			_propertyName = propertyName;
			_value = value;
		}

		/// <summary>
		/// Gets the named Property for the Expression.
		/// </summary>
		/// <value>A string that is the name of the Property.</value>
		public string PropertyName
		{
			get { return _propertyName; }
		}

		/// <summary>
		/// Gets the Value for the Expression.
		/// </summary>
		/// <value>An object that is the value for the Expression.</value>
		public object Value
		{
			get { return _value; }
		}

		/// <summary>
		/// Converts the SimpleExpression to a <see cref="SqlString"/>.
		/// </summary>
		/// <param name="factory">The ISessionFactory that contains the mapping for the Type.</param>
		/// <param name="persistentClass">The Class the Expression is being built for.</param>
		/// <param name="alias">The alias to use for the table.</param>
		/// <returns>A SqlString that contains a valid Sql fragment.</returns>
		public override SqlString ToSqlString( ISessionFactoryImplementor factory, System.Type persistentClass, string alias, IDictionary aliasClasses )
		{
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			IType propertyType = AbstractCriterion.GetType( factory, persistentClass, _propertyName, aliasClasses );
			string[ ] columnNames = AbstractCriterion.GetColumns( factory, persistentClass, _propertyName, alias, aliasClasses );
			// don't need to worry about aliasing or aliasClassing for parameter column names
			string[ ] paramColumnNames = AbstractCriterion.GetColumns( factory, persistentClass, _propertyName );
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
		public override TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, IDictionary aliasClasses )
		{
			return new TypedValue[ ] {AbstractCriterion.GetTypedValue( sessionFactory, persistentClass, _propertyName, _value, aliasClasses )};
		}

		/// <summary></summary>
		public override string ToString()
		{
			return _propertyName + Op + _value;
		}

		/// <summary>
		/// Get the Sql operator to use for the specific 
		/// subclass of <see cref="SimpleExpression"/>.
		/// </summary>
		protected abstract string Op { get; } 
	}
}