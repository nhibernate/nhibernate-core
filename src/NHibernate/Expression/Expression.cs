using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Expression
{
	/// <summary>
	/// An object-oriented representation of expressions that may be used as constraints
	/// in a Criteria. 
	/// </summary>
	/// <remarks>
	/// The expression package may be used by applications
	/// as a framework for building new kinds of <c>Expressions</c>. However, it is
	/// intended that most applications will simply use the built-in expression types via
	/// the static accessors on this class.
	/// </remarks>
	public abstract class Expression
	{
		private static readonly object[ ] NoObjects = new object[0];
		private static readonly IType[ ] NoTypes = new IType[0];

		/// <summary>
		/// Apply an "equal" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>An <see cref="EqExpression" />.</returns>
		public static Expression Eq( string propertyName, object value )
		{
			return new EqExpression( propertyName, value );
		}

		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>A <see cref="LikeExpression" />.</returns>
		public static Expression Like( string propertyName, object value )
		{
			return new LikeExpression( propertyName, value );
		}

		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>An <see cref="InsensitiveLikeExpression" />.</returns>
		public static Expression InsensitiveLike( string propertyName, object value )
		{
			return new InsensitiveLikeExpression( propertyName, value );
		}

		/// <summary>
		/// Apply a "greater than" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>A <see cref="GtExpression" />.</returns>
		public static Expression Gt( string propertyName, object value )
		{
			return new GtExpression( propertyName, value );
		}

		/// <summary>
		/// Apply a "less than" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>A <see cref="LtExpression" />.</returns>
		public static Expression Lt( string propertyName, object value )
		{
			return new LtExpression( propertyName, value );
		}

		/// <summary>
		/// Apply a "less than or equal" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>A <see cref="LeExpression" />.</returns>
		public static Expression Le( string propertyName, object value )
		{
			return new LeExpression( propertyName, value );
		}

		/// <summary>
		/// Apply a "greater than or equal" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>A <see cref="GtExpression" />.</returns>
		public static Expression Ge( string propertyName, object value )
		{
			return new GeExpression( propertyName, value );
		}

		/// <summary>
		/// Apply a "between" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="lo">The low value for the Property.</param>
		/// <param name="hi">The high value for the Property.</param>
		/// <returns>A <see cref="BetweenExpression" />.</returns>
		public static Expression Between( string propertyName, object lo, object hi )
		{
			return new BetweenExpression( propertyName, lo, hi );
		}

		/// <summary>
		/// Apply an "in" constraint to the named property 
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="values">An array of values.</param>
		/// <returns>An <see cref="InExpression" />.</returns>
		public static Expression In( string propertyName, object[ ] values )
		{
			return new InExpression( propertyName, values );
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="values">An ICollection of values.</param>
		/// <returns>An <see cref="InExpression" />.</returns>
		public static Expression In( string propertyName, ICollection values )
		{
			ArrayList ary = new ArrayList( values ); //HACK
			return new InExpression( propertyName, ary.ToArray() );
		}

		/// <summary>
		/// Apply an "is null" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <returns>A <see cref="NullExpression" />.</returns>
		public static Expression IsNull( string propertyName )
		{
			return new NullExpression( propertyName );
		}

		/// <summary>
		/// Apply an "is not null" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <returns>A <see cref="NotNullExpression" />.</returns>
		public static Expression IsNotNull( string propertyName )
		{
			return new NotNullExpression( propertyName );
		}


		/// <summary>
		/// Apply an "equal" constraint to two properties
		/// </summary>
		/// <param name="propertyName">The lhs Property Name</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="EqPropertyExpression"/> .</returns>
		public static Expression EqProperty( String propertyName, String otherPropertyName )
		{
			return new EqPropertyExpression( propertyName, otherPropertyName );
		}

		/// <summary>
		/// Apply a "less than" constraint to two properties
		/// </summary>
		/// <param name="propertyName">The lhs Property Name</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="LtPropertyExpression"/> .</returns>
		public static Expression LtProperty( String propertyName, String otherPropertyName )
		{
			return new LtPropertyExpression( propertyName, otherPropertyName );
		}

		/// <summary>
		/// Apply a "less than or equal" constraint to two properties
		/// </summary>
		/// <param name="propertyName">The lhs Property Name</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="LePropertyExpression"/> .</returns>
		public static Expression LeProperty( String propertyName, String otherPropertyName )
		{
			return new LePropertyExpression( propertyName, otherPropertyName );
		}

		/// <summary>
		/// Return the conjuction of two expressions
		/// </summary>
		/// <param name="lhs">The Expression to use as the Left Hand Side.</param>
		/// <param name="rhs">The Expression to use as the Right Hand Side.</param>
		/// <returns>An <see cref="AndExpression" />.</returns>
		public static Expression And( Expression lhs, Expression rhs )
		{
			return new AndExpression( lhs, rhs );
		}

		/// <summary>
		/// Return the disjuction of two expressions
		/// </summary>
		/// <param name="lhs">The Expression to use as the Left Hand Side.</param>
		/// <param name="rhs">The Expression to use as the Right Hand Side.</param>
		/// <returns>An <see cref="OrExpression" />.</returns>
		public static Expression Or( Expression lhs, Expression rhs )
		{
			return new OrExpression( lhs, rhs );
		}

		/// <summary>
		/// Return the negation of an expression
		/// </summary>
		/// <param name="expression">The Expression to negate.</param>
		/// <returns>A <see cref="NotExpression" />.</returns>
		public static Expression Not( Expression expression )
		{
			return new NotExpression( expression );
		}

		/// <summary>
		/// Apply a constraint expressed in SQL, with the given SQL parameters
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public static Expression Sql( SqlString sql, object[ ] values, IType[ ] types )
		{
			return new SQLExpression( sql, values, types );
		}

		/// <summary>
		/// Apply a constraint expressed in SQL, with the given SQL parameter
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Expression Sql( SqlString sql, object value, IType type )
		{
			return new SQLExpression( sql, new object[ ] {value}, new IType[ ] {type} );
		}

		/// <summary>
		/// Apply a constraint expressed in SQL
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static Expression Sql( SqlString sql )
		{
			return new SQLExpression( sql, NoObjects, NoTypes );
		}

		/// <summary>
		/// Apply a constraint expressed in SQL
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static Expression Sql( string sql )
		{
			return new SQLExpression( new SqlString( sql ), NoObjects, NoTypes );
		}


		/// <summary>
		/// Group expressions together in a single conjunction (A and B and C...)
		/// </summary>
		public static Conjunction Conjunction()
		{
			return new Conjunction();
		}


		/// <summary>
		/// Group expressions together in a single disjunction (A or B or C...)
		/// </summary>
		public static Disjunction Disjunction()
		{
			return new Disjunction();
		}


		/// <summary>
		/// Apply an "equals" constraint to each property in the key set of a IDictionary
		/// </summary>
		/// <param name="propertyNameValues">a dictionary from property names to values</param>
		/// <returns></returns>
		public static Expression AllEq( IDictionary propertyNameValues )
		{
			Conjunction conj = Conjunction();

			foreach( DictionaryEntry item in propertyNameValues )
			{
				conj.Add( Eq( item.Key.ToString(), item.Value ) );
			}

			return conj;
		}

		/// <summary>
		/// Render a SqlString for the expression.
		/// </summary>
		/// <param name="factory">The ISessionFactory that contains the mapping for the Type.</param>
		/// <param name="persistentClass">The Class the Expression is being built for.</param>
		/// <param name="alias">The alias to use for the table.</param>
		/// <returns>A SqlString that contains a valid Sql fragment.</returns>
		public abstract SqlString ToSqlString( ISessionFactoryImplementor factory, System.Type persistentClass, string alias );

		/// <summary>
		/// Return typed values for all parameters in the rendered SQL fragment
		/// </summary>
		/// <param name="sessionFactory">The ISessionFactory that contains the mapping for the Type.</param>
		/// <param name="persistentClass">The Class the Expression is being built for.</param>
		/// <returns>An array of TypedValues for the Expression.</returns>
		public abstract TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass );

		/// <summary>
		/// Gets a string representation of the Expression.  
		/// </summary>
		/// <returns>
		/// A String that shows the contents of the Expression.
		/// </returns>
		/// <remarks>
		/// This is not a well formed Sql fragment.  It is useful for logging what the Expressions
		/// looks like.
		/// </remarks>
		public abstract override string ToString();

		// --- PORT NOTE ---
		// Access modifier was protected.
		// I modified it to internal because Order use it.
		// ---
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="property"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		internal static string[ ] GetColumns( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string property, string alias )
		{
			return ( ( IQueryable ) sessionFactory.GetPersister( persistentClass ) ).ToColumns( alias, property );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected static TypedValue GetTypedValue( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string propertyName, object value )
		{
			return new TypedValue( ( ( IQueryable ) sessionFactory.GetPersister( persistentClass ) ).GetPropertyType( propertyName ), value );
		}

	}
}