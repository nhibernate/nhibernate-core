using System;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Type;

namespace NHibernate.Expression
{
	/// <summary>
	/// An object-oriented representation of expressions that may be used as constraints
	/// in a Criteria. The expression package may be used by applications
	/// as a framework for building new kinds of <tt>Expression</tt>. However, it is
	/// intended that most applications will simply use the built-in expression types via
	/// the static accessors on this class.
	/// </summary>
	public abstract class Expression {

		private static readonly object[] NoObjects = new object[0];
		private static readonly IType[] NoTypes = new IType[0];

		/// <summary>
		/// Apply an "equal" constraint to the named property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Expression Eq(string propertyName, object value) {
			return new EqExpression(propertyName, value);
		}

		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Expression Like(string propertyName, object value) {
			return new LikeExpression(propertyName, value);
		}

		/// <summary>
		/// Apply a "greater than" constraint to the named property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Expression Gt(string propertyName, object value) {
			return new GtExpression(propertyName, value);
		}

		/// <summary>
		/// Apply a "less than" constraint to the named property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Expression Lt(string propertyName, object value) {
			return new LtExpression(propertyName, value);
		}

		/// <summary>
		/// Apply a "less than or equal" constraint to the named property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Expression Le(string propertyName, object value) {
			return new LeExpression(propertyName, value);
		}

		/// <summary>
		/// Apply a "greater than or equal" constraint to the named property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Expression Ge(string propertyName, object value) {
			return new GtExpression(propertyName, value);
		}

		/// <summary>
		/// Apply a "between" constraint to the named property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="lo"></param>
		/// <param name="hi"></param>
		/// <returns></returns>
		public static Expression Between(string propertyName, object lo, object hi) { 
			return new BetweenExpression(propertyName, lo, hi); 
		} 
		
		/// <summary>
		/// Apply an "in" constraint to the named property 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static Expression In(string propertyName, object[] values) { 
			return new InExpression(propertyName, values); 
		} 
	
		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static Expression In(String propertyName, ICollection values) { 
			ArrayList ary = new ArrayList(values); //HACK
			return new InExpression( propertyName, ary.ToArray() );
		} 
	
		/// <summary>
		/// Apply an "is null" constraint to the named property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static Expression IsNull(string propertyName) { 
			return new NullExpression(propertyName); 
		} 
	
		/// <summary>
		/// Apply an "is not null" constraint to the named property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static Expression IsNotNull(string propertyName) { 
			return new NotNullExpression(propertyName); 
		}

		/// <summary>
		/// Return the conjuction of two expressions
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static Expression And(Expression lhs, Expression rhs) {
			return new AndExpression(lhs, rhs);
		}

		/// <summary>
		/// Return the disjuction of two expressions
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static Expression Or(Expression lhs, Expression rhs) {
			return new OrExpression(lhs, rhs);
		}

		/// <summary>
		/// Return the negation of an expression
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static Expression Not(Expression expression) {
			return new NotExpression(expression);
		}

		/// <summary>
		/// Apply a constraint expressed in SQL, with the given SQL parameters
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public static Expression Sql(string sql, object[] values, IType[] types) {
			return new SQLExpression(sql, values, types);
		}

		/// <summary>
		/// Apply a constraint expressed in SQL, with the given SQL parameter
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Expression Sql(string sql, object value, IType type) {
			return new SQLExpression(sql, new object[] { value }, new IType[] { type } );
		}

		/// <summary>
		/// Apply a constraint expressed in SQL
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static Expression Sql(String sql) {
			return new SQLExpression(sql, NoObjects, NoTypes);
		}

		/// <summary>
		/// Render and SQL fragment
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public abstract string ToSqlString(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias);

		/// <summary>
		/// Return typed values for all parameters in the rendered SQL fragment
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public abstract TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass);

		/// <summary>
		/// For cosmetic purposes only!
		/// </summary>
		/// <returns></returns>
		public abstract override string ToString();

		// --- PORT NOTE ---
		// Access modifier was protected.
		// I modified it to internal because Order use it.
		// ---
		internal static string[] GetColumns(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string property, string alias) {
			return ( (IQueryable) sessionFactory.GetPersister(persistentClass) ).ToColumns(alias, property);
		}

		protected static TypedValue GetTypedValue(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string propertyName, object value) {
			return new TypedValue( ( (IQueryable) sessionFactory.GetPersister(persistentClass) ).GetPropertyType(propertyName), value );
		}
	}
}