using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

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
		private bool _ignoreCase;

		/// <summary>
		/// Initialize a new instance of the <see cref="SimpleExpression" /> class for a named
		/// Property and its value.
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		public SimpleExpression( string propertyName, object value )
		{
			_propertyName = propertyName;
			_value = value;
		}

		public SimpleExpression( string propertyName, object value, bool ignoreCase )
		{
			_propertyName = propertyName;
			_value = value;
			_ignoreCase = ignoreCase;
		}

		public SimpleExpression IgnoreCase()
		{
			_ignoreCase = true;
			return this;
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
			string[ ] columnNames = AbstractCriterion.GetColumns( factory, persistentClass, _propertyName, alias, aliasClasses );
			IType propertyType = AbstractCriterion.GetType( factory, persistentClass, _propertyName, aliasClasses );

			if( propertyType.IsCollectionType )
			{
				throw new QueryException( string.Format (
					"cannot use collection property ({0}.{1}) directly in a criterion,"
					+ " use ICriteria.CreateCriteria instead",
					persistentClass.FullName, _propertyName ) );
			}

			Parameter[ ] parameters = Parameter.GenerateParameters( factory, columnNames, propertyType );

			if( _ignoreCase )
			{
				if( columnNames.Length != 1 )
				{
					throw new HibernateException(
						"case insensitive expression may only be applied to single-column properties: " +
						_propertyName );
				}

				return new SqlStringBuilder( 6 )
					.Add( factory.Dialect.LowercaseFunction )
					.Add( StringHelper.OpenParen )
					.Add( columnNames[ 0 ] )
					.Add( StringHelper.ClosedParen )
					.Add( Op )
					.Add( parameters[ 0 ] )
					.ToSqlString();
			}
			else
			{
				//TODO: add default capacity
				SqlStringBuilder sqlBuilder = new SqlStringBuilder( 4 * columnNames.Length );

				for( int i = 0; i < columnNames.Length; i++ )
				{
					if( i > 0 )
					{
						sqlBuilder.Add( " and " );
					}

					sqlBuilder.Add( columnNames[ i ] )
						.Add( Op )
						.Add( parameters[ i ] );

				}
				return sqlBuilder.ToSqlString();
			}
		}

		public override TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, IDictionary aliasClasses )
		{
			object icvalue = _ignoreCase ? _value.ToString().ToLower() : _value;
			return new TypedValue[ ]
				{
					AbstractCriterion.GetTypedValue( sessionFactory, persistentClass, _propertyName, icvalue, aliasClasses )
				};
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