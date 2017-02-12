using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Criterion.Lambda;
using NHibernate.Impl;

namespace NHibernate.Criterion
{
	/// <summary> 
	/// The <see cref="NHibernate.Criterion"/> namespace may be used by applications as a framework for building
	/// new kinds of <see cref="ICriterion"/>. 
	/// However, it is intended that most applications will
	/// simply use the built-in criterion types via the static factory methods of this class.
	/// </summary>
	/// <seealso cref="ICriteria"/>
	/// <seealso cref="Projections"/>
	public class Restrictions
	{
		internal Restrictions()
		{
			//cannot be instantiated
		}

		/// <summary>
		/// Apply an "equal" constraint to the identifier property
		/// </summary>
		/// <param name="value"></param>
		/// <returns>ICriterion</returns>
		public static AbstractCriterion IdEq(object value)
		{
			return new IdentifierEqExpression(value);
		}

		/// <summary>
		/// Apply an "equal" constraint from the projection to the identifier property
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <returns>ICriterion</returns>
		public static AbstractCriterion IdEq(IProjection projection)
		{
			return new IdentifierEqExpression(projection);
		}

		/// <summary>
		/// Apply an "equal" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		public static SimpleExpression Eq(string propertyName, object value)
		{
			return new SimpleExpression(propertyName, value, " = ");
		}

		/// <summary>
		/// Apply an "equal" constraint to the projection
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="value">The value for the Property.</param>
		public static SimpleExpression Eq(IProjection projection, object value)
		{
			return new SimpleExpression(projection, value, " = ");
		}

		/// <summary>
		/// Apply a "like" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>A <see cref="LikeExpression" />.</returns>
		public static SimpleExpression Like(string propertyName, object value)
		{
			return new SimpleExpression(propertyName, value, " like ");
		}

		public static AbstractCriterion Like(string propertyName, string value, MatchMode matchMode, char? escapeChar)
		{
			return new LikeExpression(propertyName, value, matchMode, escapeChar, false);
		}

		/// <summary>
		/// Apply a "like" constraint to the project
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>A <see cref="LikeExpression"/>.</returns>
		public static SimpleExpression Like(IProjection projection, object value)
		{
			return new SimpleExpression(projection, value, " like ");
		}

		/// <summary>
		/// Apply a "like" constraint to the project
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="value">The value for the Property.</param>
		/// <param name="matchMode">The match mode.</param>
		/// <returns>A <see cref="LikeExpression"/>.</returns>
		public static SimpleExpression Like(IProjection projection, string value, MatchMode matchMode)
		{
			return new SimpleExpression(projection, matchMode.ToMatchString(value), " like ");
		}

		public static SimpleExpression Like(string propertyName, string value, MatchMode matchMode)
		{
			return new SimpleExpression(propertyName, matchMode.ToMatchString(value), " like ");
		}

		public static AbstractCriterion InsensitiveLike(string propertyName, string value, MatchMode matchMode)
		{
			return new InsensitiveLikeExpression(propertyName, value, matchMode);
		}

		public static AbstractCriterion InsensitiveLike(IProjection projection, string value, MatchMode matchMode)
		{
			return new InsensitiveLikeExpression(projection, value, matchMode);
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
		/// A case-insensitive "like", similar to Postgres "ilike" operator
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="value">The value for the Property.</param>
		/// <returns>
		/// An <see cref="InsensitiveLikeExpression"/>.
		/// </returns>
		public static AbstractCriterion InsensitiveLike(IProjection projection, object value)
		{
			return new InsensitiveLikeExpression(projection, value);
		}

		/// <summary>
		/// Apply a "greater than" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		public static SimpleExpression Gt(string propertyName, object value)
		{
			return new SimpleExpression(propertyName, value, " > ");
		}

		/// <summary>
		/// Apply a "greater than" constraint to the projection
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="value">The value for the Property.</param>
		public static SimpleExpression Gt(IProjection projection, object value)
		{
			return new SimpleExpression(projection, value, " > ");
		}

		/// <summary>
		/// Apply a "less than" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		public static SimpleExpression Lt(string propertyName, object value)
		{
			return new SimpleExpression(propertyName, value, " < ");
		}


		/// <summary>
		/// Apply a "less than" constraint to the projection
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="value">The value for the Property.</param>
		public static SimpleExpression Lt(IProjection projection, object value)
		{
			return new SimpleExpression(projection, value, " < ");
		}

		/// <summary>
		/// Apply a "less than or equal" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		public static SimpleExpression Le(string propertyName, object value)
		{
			return new SimpleExpression(propertyName, value, " <= ");
		}

		/// <summary>
		/// Apply a "less than or equal" constraint to the projection
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="value">The value for the Property.</param>
		public static SimpleExpression Le(IProjection projection, object value)
		{
			return new SimpleExpression(projection, value, " <= ");
		}

		/// <summary>
		/// Apply a "greater than or equal" constraint to the named property
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		public static SimpleExpression Ge(string propertyName, object value)
		{
			return new SimpleExpression(propertyName, value, " >= ");
		}

		/// <summary>
		/// Apply a "greater than or equal" constraint to the projection
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="value">The value for the Property.</param>
		public static SimpleExpression Ge(IProjection projection, object value)
		{
			return new SimpleExpression(projection, value, " >= ");
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
		/// Apply a "between" constraint to the projection
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="lo">The low value for the Property.</param>
		/// <param name="hi">The high value for the Property.</param>
		/// <returns>A <see cref="BetweenExpression"/>.</returns>
		public static AbstractCriterion Between(IProjection projection, object lo, object hi)
		{
			return new BetweenExpression(projection, lo, hi);
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
		/// Apply an "in" constraint to the projection
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="values">An array of values.</param>
		/// <returns>An <see cref="InExpression"/>.</returns>
		public static AbstractCriterion In(IProjection projection, object[] values)
		{
			return new InExpression(projection, values);
		}

		/// <summary>
		/// Apply an "in" constraint to the projection
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="values">An ICollection of values.</param>
		/// <returns>An <see cref="InExpression"/>.</returns>
		public static AbstractCriterion In(IProjection projection, ICollection values)
		{
			object[] ary = new object[values.Count];
			values.CopyTo(ary, 0);
			return new InExpression(projection, ary);
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
		/// <param name="values">An <see cref="System.Collections.Generic.IEnumerable{T}" />
		/// of values.</param>
		/// <returns>An <see cref="InExpression" />.</returns>
		public static AbstractCriterion InG<T>(string propertyName, IEnumerable<T> values)
		{
			var array = new object[values.Count()];
			var i = 0;
			foreach (var item in values)
			{
				array[i] = item;
				i++;
			}
			return new InExpression(propertyName, array);
		}

		/// <summary>
		/// Apply an "in" constraint to the projection. This is the generic equivalent
		/// of <see cref="In(string, ICollection)"/>, renamed to avoid ambiguity.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="projection">The projection.</param>
		/// <param name="values">An <see cref="System.Collections.Generic.IEnumerable{T}"/>
		/// of values.</param>
		/// <returns>An <see cref="InExpression"/>.</returns>
		public static AbstractCriterion InG<T>(IProjection projection, IEnumerable<T> values)
		{
			var array = new object[values.Count()];
			var i = 0;
			foreach (var item in values)
			{
				array[i] = item;
				i++;
			}
			return new InExpression(projection, array);
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
		/// Apply an "is null" constraint to the projection
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <returns>A <see cref="NullExpression"/>.</returns>
		public static AbstractCriterion IsNull(IProjection projection)
		{
			return new NullExpression(projection);
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
		/// Apply an "equal" constraint to projection and property
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="EqPropertyExpression"/> .</returns>
		public static AbstractCriterion EqProperty(IProjection projection, string otherPropertyName)
		{
			return new EqPropertyExpression(projection, otherPropertyName);
		}


		/// <summary>
		/// Apply an "equal" constraint to lshProjection and rshProjection
		/// </summary>
		/// <param name="lshProjection">The LHS projection.</param>
		/// <param name="rshProjection">The RSH projection.</param>
		/// <returns>A <see cref="EqPropertyExpression"/> .</returns>
		public static AbstractCriterion EqProperty(IProjection lshProjection, IProjection rshProjection)
		{
			return new EqPropertyExpression(lshProjection, rshProjection);
		}


		/// <summary>
		/// Apply an "equal" constraint to the property and rshProjection
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="rshProjection">The RSH projection.</param>
		/// <returns>A <see cref="EqPropertyExpression"/> .</returns>
		public static AbstractCriterion EqProperty(string propertyName, IProjection rshProjection)
		{
			return new EqPropertyExpression(propertyName, rshProjection);
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
		/// Apply an "not equal" constraint to projection and property
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="EqPropertyExpression"/> .</returns>
		public static AbstractCriterion NotEqProperty(IProjection projection, string otherPropertyName)
		{
			return new NotExpression(new EqPropertyExpression(projection, otherPropertyName));
		}

		/// <summary>
		/// Apply an "not equal" constraint to the projections
		/// </summary>
		/// <param name="lhsProjection">The LHS projection.</param>
		/// <param name="rhsProjection">The RHS projection.</param>
		/// <returns>A <see cref="EqPropertyExpression"/> .</returns>
		public static AbstractCriterion NotEqProperty(IProjection lhsProjection, IProjection rhsProjection)
		{
			return new NotExpression(new EqPropertyExpression(lhsProjection, rhsProjection));
		}

		/// <summary>
		/// Apply an "not equal" constraint to the projections
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="rhsProjection">The RHS projection.</param>
		/// <returns>A <see cref="EqPropertyExpression"/> .</returns>
		public static AbstractCriterion NotEqProperty(string propertyName, IProjection rhsProjection)
		{
			return new NotExpression(new EqPropertyExpression(propertyName, rhsProjection));
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
		/// Apply a "greater than" constraint to two properties
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="LtPropertyExpression"/> .</returns>
		public static AbstractCriterion GtProperty(IProjection projection, string otherPropertyName)
		{
			return new GtPropertyExpression(projection, otherPropertyName);
		}

		/// <summary>
		/// Apply a "greater than" constraint to two properties
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="projection">The projection.</param>
		/// <returns>A <see cref="LtPropertyExpression"/> .</returns>
		public static AbstractCriterion GtProperty(string propertyName, IProjection projection)
		{
			return new GtPropertyExpression(propertyName, projection);
		}

		/// <summary>
		/// Apply a "greater than" constraint to two properties
		/// </summary>
		/// <param name="lhsProjection">The LHS projection.</param>
		/// <param name="rhsProjection">The RHS projection.</param>
		/// <returns>A <see cref="LtPropertyExpression"/> .</returns>
		public static AbstractCriterion GtProperty(IProjection lhsProjection, IProjection rhsProjection)
		{
			return new GtPropertyExpression(lhsProjection, rhsProjection);
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
		/// Apply a "greater than or equal" constraint to two properties
		/// </summary>
		/// <param name="lhsProjection">The LHS projection.</param>
		/// <param name="rhsProjection">The RHS projection.</param>
		/// <returns>A <see cref="LePropertyExpression"/> .</returns>
		public static AbstractCriterion GeProperty(IProjection lhsProjection, IProjection rhsProjection)
		{
			return new GePropertyExpression(lhsProjection, rhsProjection);
		}

		/// <summary>
		/// Apply a "greater than or equal" constraint to two properties
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="LePropertyExpression"/> .</returns>
		public static AbstractCriterion GeProperty(IProjection projection, string otherPropertyName)
		{
			return new GePropertyExpression(projection, otherPropertyName);
		}


		/// <summary>
		/// Apply a "greater than or equal" constraint to two properties
		/// </summary>
		/// <param name="propertyName">The lhs Property Name</param>
		/// <param name="projection">The projection.</param>
		/// <returns>A <see cref="LePropertyExpression"/> .</returns>
		public static AbstractCriterion GeProperty(string propertyName, IProjection projection)
		{
			return new GePropertyExpression(propertyName, projection);
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
		/// Apply a "less than" constraint to two properties
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="LtPropertyExpression"/> .</returns>
		public static AbstractCriterion LtProperty(IProjection projection, string otherPropertyName)
		{
			return new LtPropertyExpression(projection, otherPropertyName);
		}

		/// <summary>
		/// Apply a "less than" constraint to two properties
		/// </summary>
		/// <param name="propertyName">The lhs Property Name</param>
		/// <param name="projection">The projection.</param>
		/// <returns>A <see cref="LtPropertyExpression"/> .</returns>
		public static AbstractCriterion LtProperty(string propertyName, IProjection projection)
		{
			return new LtPropertyExpression(propertyName, projection);
		}

		/// <summary>
		/// Apply a "less than" constraint to two properties
		/// </summary>
		/// <param name="lhsProjection">The LHS projection.</param>
		/// <param name="rhsProjection">The RHS projection.</param>
		/// <returns>A <see cref="LtPropertyExpression"/> .</returns>
		public static AbstractCriterion LtProperty(IProjection lhsProjection, IProjection rhsProjection)
		{
			return new LtPropertyExpression(lhsProjection, rhsProjection);
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
		/// Apply a "less than or equal" constraint to two properties
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="otherPropertyName">The rhs Property Name</param>
		/// <returns>A <see cref="LePropertyExpression"/> .</returns>
		public static AbstractCriterion LeProperty(IProjection projection, string otherPropertyName)
		{
			return new LePropertyExpression(projection, otherPropertyName);
		}


		/// <summary>
		/// Apply a "less than or equal" constraint to two properties
		/// </summary>
		/// <param name="propertyName">The lhs Property Name</param>
		/// <param name="projection">The projection.</param>
		/// <returns>A <see cref="LePropertyExpression"/> .</returns>
		public static AbstractCriterion LeProperty(string propertyName, IProjection projection)
		{
			return new LePropertyExpression(propertyName, projection);
		}


		/// <summary>
		/// Apply a "less than or equal" constraint to two properties
		/// </summary>
		/// <param name="lhsProjection">The LHS projection.</param>
		/// <param name="rhsProjection">The RHS projection.</param>
		/// <returns>A <see cref="LePropertyExpression"/> .</returns>
		public static AbstractCriterion LeProperty(IProjection lhsProjection, IProjection rhsProjection)
		{
			return new LePropertyExpression(lhsProjection, rhsProjection);
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
		/// Apply an "is not null" constraint to the named property
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <returns>A <see cref="NotNullExpression"/>.</returns>
		public static AbstractCriterion IsNotNull(IProjection projection)
		{
			return new NotNullExpression(projection);
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
		/// Return the conjunction of two expressions
		/// </summary>
		/// <param name="lhs">The Expression to use as the Left Hand Side.</param>
		/// <param name="rhs">The Expression to use as the Right Hand Side.</param>
		/// <returns>An <see cref="AndExpression" />.</returns>
		public static AbstractCriterion And(ICriterion lhs, ICriterion rhs)
		{
			return new AndExpression(lhs, rhs);
		}

		/// <summary>
		/// Return the disjunction of two expressions
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

		public static NaturalIdentifier NaturalId()
		{
			return new NaturalIdentifier();
		}

		/// <summary>
		/// Create an ICriterion for the supplied LambdaExpression
		/// </summary>
		/// <typeparam name="T">generic type</typeparam>
		/// <param name="expression">lambda expression</param>
		/// <returns>return NHibernate.Criterion.ICriterion</returns>
		public static ICriterion Where<T>(Expression<Func<T, bool>> expression)
		{
			ICriterion criterion = ExpressionProcessor.ProcessExpression<T>(expression);
			return criterion;
		}

		/// <summary>
		/// Create an ICriterion for the supplied LambdaExpression
		/// </summary>
		/// <param name="expression">lambda expression</param>
		/// <returns>return NHibernate.Criterion.ICriterion</returns>
		public static ICriterion Where(Expression<Func<bool>> expression)
		{
			ICriterion criterion = ExpressionProcessor.ProcessExpression(expression);
			return criterion;
		}

		/// <summary>
		/// Create an ICriterion for the negation of the supplied LambdaExpression
		/// </summary>
		/// <typeparam name="T">generic type</typeparam>
		/// <param name="expression">lambda expression</param>
		/// <returns>return NHibernate.Criterion.ICriterion</returns>
		public static ICriterion WhereNot<T>(Expression<Func<T, bool>> expression)
		{
			ICriterion criterion = ExpressionProcessor.ProcessExpression<T>(expression);
			return Restrictions.Not(criterion);
		}

		/// <summary>
		/// Create an ICriterion for the negation of the supplied LambdaExpression
		/// </summary>
		/// <param name="expression">lambda expression</param>
		/// <returns>return NHibernate.Criterion.ICriterion</returns>
		public static ICriterion WhereNot(Expression<Func<bool>> expression)
		{
			ICriterion criterion = ExpressionProcessor.ProcessExpression(expression);
			return Restrictions.Not(criterion);
		}

		/// <summary>
		/// Build an ICriterion for the given property
		/// </summary>
		/// <param name="expression">lambda expression identifying property</param>
		/// <returns>returns LambdaRestrictionBuilder</returns>
		public static LambdaRestrictionBuilder On<T>(Expression<Func<T, object>> expression)
		{
			ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(expression.Body);
			return new LambdaRestrictionBuilder(projection);
		}

		/// <summary>
		/// Build an ICriterion for the given property
		/// </summary>
		/// <param name="expression">lambda expression identifying property</param>
		/// <returns>returns LambdaRestrictionBuilder</returns>
		public static LambdaRestrictionBuilder On(Expression<Func<object>> expression)
		{
			ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(expression.Body);
			return new LambdaRestrictionBuilder(projection);
		}

	}
}
