using System;
using System.Collections;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Expression
{
	/// <summary>
	/// The <c>Expression</c> namespace may be used by applications as a framework for building 
	/// new kinds of <see cref="ICriterion" />. However, it is intended that most applications will 
	/// simply use the built-in criterion types via the static factory methods of this class.
	/// </summary>
	public sealed class Expression
	{
		private Expression()
		{
			// can not be instantiated
		}

		/// <summary>
		/// Apply an "equal" constraint to the identifier property
		/// </summary>
		/// <param name="value"></param>
		/// <returns>ICriterion</returns>
		public static ICriterion IdEq(object value)
		{
			return new IdentifierEqExpression(value);
		}

		/// <summary>
		/// Apply an "equal" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>An <see cref="EqExpression" />.</returns>
		public static SimpleExpression Eq(string propertyName, object value)
		{
			return new EqExpression(propertyName, value);
		}

		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>A <see cref="LikeExpression" />.</returns>
		public static SimpleExpression Like(string propertyName, object value)
		{
			return new LikeExpression(propertyName, value);
		}

		public static SimpleExpression Like(string propertyName, string value, MatchMode matchMode)
		{
			return new LikeExpression(propertyName, value, matchMode);
		}

		public static AbstractCriterion InsensitiveLike(string propertyName, string value, MatchMode matchMode)
		{
			return new InsensitiveLikeExpression(propertyName, value, matchMode);
		}

		/// <summary>
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>An <see cref="InsensitiveLikeExpression" />.</returns>
		public static AbstractCriterion InsensitiveLike(string propertyName, object value)
		{
			return new InsensitiveLikeExpression(propertyName, value);
		}

		/// <summary>
		/// Apply a "greater than" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>A <see cref="GtExpression" />.</returns>
		public static SimpleExpression Gt(string propertyName, object value)
		{
			return new GtExpression(propertyName, value);
		}

		/// <summary>
		/// Apply a "less than" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>A <see cref="LtExpression" />.</returns>
		public static SimpleExpression Lt(string propertyName, object value)
		{
			return new LtExpression(propertyName, value);
		}

		/// <summary>
		/// Apply a "less than or equal" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>A <see cref="LeExpression" />.</returns>
		public static SimpleExpression Le(string propertyName, object value)
		{
			return new LeExpression(propertyName, value);
		}

		/// <summary>
		/// Apply a "greater than or equal" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>A <see cref="GtExpression" />.</returns>
		public static SimpleExpression Ge(string propertyName, object value)
		{
			return new GeExpression(propertyName, value);
		}

		/// <summary>
		/// Apply a "between" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="lo">The low value for the Property.</param>
		/// <param name="hi">The high value for the Property.</param>
		/// <returns>A <see cref="BetweenExpression" />.</returns>
		public static AbstractCriterion Between(string propertyName, object lo, object hi)
		{
			return new BetweenExpression(propertyName, lo, hi);
		}

		/// <summary>
		/// Apply an "in" constraint to the named property 
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="values">An array of values.</param>
		/// <returns>An <see cref="InExpression" />.</returns>
		public static AbstractCriterion In(string propertyName, object[] values)
		{
			return new InExpression(propertyName, values);
		}

		/// <summary>
		/// Apply an "in" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="values">An ICollection of values.</param>
		/// <returns>An <see cref="InExpression" />.</returns>
		public static AbstractCriterion In(string propertyName, ICollection values)
		{
			object[] ary = new object[values.Count];
			values.CopyTo(ary, 0);
			return new InExpression(propertyName, ary);
		}

		/// <summary>
		/// Apply an "in" constraint to the named property. This is the generic equivalent
		/// of <see cref="In(string, ICollection)" />, renamed to avoid ambiguity.
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="values">An <see cref="System.Collections.Generic.ICollection{T}" />
		/// of values.</param>
		/// <returns>An <see cref="InExpression" />.</returns>
		public static AbstractCriterion InG<T>(string propertyName, System.Collections.Generic.ICollection<T> values)
		{
			object[] array = new object[values.Count];
			int i = 0;
			foreach (T item in values)
			{
				array[i] = item;
				i++;
			}
			return new InExpression(propertyName, array);
		}

		/// <summary>
		/// Apply an "is null" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <returns>A <see cref="NullExpression" />.</returns>
		public static AbstractCriterion IsNull(string propertyName)
		{
			return new NullExpression(propertyName);
		}

		/// <summary>
		/// Apply an "equal" constraint to two properties
		/// </summary>
		/// <param name="propertyName">The lhs Property Name</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="EqPropertyExpression"/> .</returns>
		public static AbstractCriterion EqProperty(string propertyName, string otherPropertyName)
		{
			return new EqPropertyExpression(propertyName, otherPropertyName);
		}

		/// <summary>
		/// Apply an "not equal" constraint to two properties
		/// </summary>
		/// <param name="propertyName">The lhs Property Name</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="EqPropertyExpression"/> .</returns>
		public static AbstractCriterion NotEqProperty(string propertyName, string otherPropertyName)
		{
			return new NotExpression(new EqPropertyExpression(propertyName, otherPropertyName));
		}

		/// <summary>
		/// Apply a "greater than" constraint to two properties
		/// </summary>
		/// <param name="propertyName">The lhs Property Name</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="LtPropertyExpression"/> .</returns>
		public static AbstractCriterion GtProperty(string propertyName, string otherPropertyName)
		{
			return new GtPropertyExpression(propertyName, otherPropertyName);
		}

		/// <summary>
		/// Apply a "greater than or equal" constraint to two properties
		/// </summary>
		/// <param name="propertyName">The lhs Property Name</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="LePropertyExpression"/> .</returns>
		public static AbstractCriterion GeProperty(string propertyName, string otherPropertyName)
		{
			return new GePropertyExpression(propertyName, otherPropertyName);
		}

		/// <summary>
		/// Apply a "less than" constraint to two properties
		/// </summary>
		/// <param name="propertyName">The lhs Property Name</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="LtPropertyExpression"/> .</returns>
		public static AbstractCriterion LtProperty(string propertyName, string otherPropertyName)
		{
			return new LtPropertyExpression(propertyName, otherPropertyName);
		}

		/// <summary>
		/// Apply a "less than or equal" constraint to two properties
		/// </summary>
		/// <param name="propertyName">The lhs Property Name</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="LePropertyExpression"/> .</returns>
		public static AbstractCriterion LeProperty(string propertyName, string otherPropertyName)
		{
			return new LePropertyExpression(propertyName, otherPropertyName);
		}

		/// <summary>
		/// Apply an "is not null" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <returns>A <see cref="NotNullExpression" />.</returns>
		public static AbstractCriterion IsNotNull(string propertyName)
		{
			return new NotNullExpression(propertyName);
		}

		/// <summary>
		/// Apply an "is not empty" constraint to the named property 
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <returns>A <see cref="IsNotEmptyExpression" />.</returns>
		public static AbstractEmptinessExpression IsNotEmpty(string propertyName)
		{
			return new IsNotEmptyExpression(propertyName);
		}

		/// <summary>
		/// Apply an "is not empty" constraint to the named property 
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <returns>A <see cref="IsEmptyExpression" />.</returns>
		public static AbstractEmptinessExpression IsEmpty(string propertyName)
		{
			return new IsEmptyExpression(propertyName);
		}

		/// <summary>
		/// Return the conjuction of two expressions
		/// </summary>
		/// <param name="lhs">The Expression to use as the Left Hand Side.</param>
		/// <param name="rhs">The Expression to use as the Right Hand Side.</param>
		/// <returns>An <see cref="AndExpression" />.</returns>
		public static AbstractCriterion And(ICriterion lhs, ICriterion rhs)
		{
			return new AndExpression(lhs, rhs);
		}

		/// <summary>
		/// Return the disjuction of two expressions
		/// </summary>
		/// <param name="lhs">The Expression to use as the Left Hand Side.</param>
		/// <param name="rhs">The Expression to use as the Right Hand Side.</param>
		/// <returns>An <see cref="OrExpression" />.</returns>
		public static AbstractCriterion Or(ICriterion lhs, ICriterion rhs)
		{
			return new OrExpression(lhs, rhs);
		}

		/// <summary>
		/// Return the negation of an expression
		/// </summary>
		/// <param name="expression">The Expression to negate.</param>
		/// <returns>A <see cref="NotExpression" />.</returns>
		public static AbstractCriterion Not(ICriterion expression)
		{
			return new NotExpression(expression);
		}

		/// <summary>
		/// Apply a constraint expressed in SQL, with the given SQL parameters
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public static AbstractCriterion Sql(SqlString sql, object[] values, IType[] types)
		{
			return new SQLCriterion(sql, values, types);
		}

		/// <summary>
		/// Apply a constraint expressed in SQL, with the given SQL parameter
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static AbstractCriterion Sql(SqlString sql, object value, IType type)
		{
			return Sql(sql, new object[] {value}, new IType[] {type});
		}

		/// <summary>
		/// Apply a constraint expressed in SQL, with the given SQL parameter
		/// </summary>
		public static AbstractCriterion Sql(string sql, object value, IType type)
		{
			return Sql(sql, new object[] {value}, new IType[] {type});
		}

		public static AbstractCriterion Sql(string sql, object[] values, IType[] types)
		{
			return new SQLCriterion(SqlString.Parse(sql), values, types);
		}

		/// <summary>
		/// Apply a constraint expressed in SQL
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static AbstractCriterion Sql(SqlString sql)
		{
			return Sql(sql, ArrayHelper.EmptyObjectArray, ArrayHelper.EmptyTypeArray);
		}

		/// <summary>
		/// Apply a constraint expressed in SQL
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static AbstractCriterion Sql(string sql)
		{
			return Sql(sql, ArrayHelper.EmptyObjectArray, ArrayHelper.EmptyTypeArray);
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
		public static AbstractCriterion AllEq(IDictionary propertyNameValues)
		{
			Conjunction conj = Conjunction();

			foreach (DictionaryEntry item in propertyNameValues)
			{
				conj.Add(Eq(item.Key.ToString(), item.Value));
			}

			return conj;
		}
	}
}
