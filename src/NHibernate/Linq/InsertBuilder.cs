using System;
using System.Linq;
using System.Linq.Expressions;

namespace NHibernate.Linq
{
	/// <summary>
	/// An insert builder on which entities to insert can be specified.
	/// </summary>
	/// <typeparam name="TSource">The type of the entities selected as source of the insert.</typeparam>
	public class InsertBuilder<TSource>
	{
		private readonly IQueryable<TSource> _source;

		internal InsertBuilder(IQueryable<TSource> source)
		{
			_source = source;
		}

		/// <summary>
		/// Specifies the type of the entities to insert, and return an insert builder allowing to specify the values to insert.
		/// </summary>
		/// <typeparam name="TTarget">The type of the entities to insert.</typeparam>
		/// <returns>An insert builder.</returns>
		public InsertBuilder<TSource, TTarget> Into<TTarget>()
		{
			return new InsertBuilder<TSource, TTarget>(_source);
		}
	}

	/// <summary>
	/// An insert builder on which entities to insert can be specified.
	/// </summary>
	/// <typeparam name="TSource">The type of the entities selected as source of the insert.</typeparam>
	/// <typeparam name="TTarget">The type of the entities to insert.</typeparam>
	public partial class InsertBuilder<TSource, TTarget>
	{
		private readonly IQueryable<TSource> _source;
		private readonly Assignments<TSource, TTarget> _assignments = new Assignments<TSource, TTarget>();

		internal InsertBuilder(IQueryable<TSource> source)
		{
			_source = source;
		}

		/// <summary>
		/// Set the specified property value and return this builder.
		/// </summary>
		/// <typeparam name="TProp">The type of the property.</typeparam>
		/// <param name="property">The property.</param>
		/// <param name="expression">The expression that should be assigned to the property.</param>
		/// <returns>This insert builder.</returns>
		public InsertBuilder<TSource, TTarget> Value<TProp>(Expression<Func<TTarget, TProp>> property, Expression<Func<TSource, TProp>> expression)
		{
			_assignments.Set(property, expression);
			return this;
		}

		/// <summary>
		/// Set the specified property value and return this builder.
		/// </summary>
		/// <typeparam name="TProp">The type of the property.</typeparam>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		/// <returns>This insert builder.</returns>
		public InsertBuilder<TSource, TTarget> Value<TProp>(Expression<Func<TTarget, TProp>> property, TProp value)
		{
			_assignments.Set(property, value);
			return this;
		}

		/// <summary>
		/// Insert the entities. The insert operation is performed in the database without reading the entities out of it. Will use
		/// <c>INSERT INTO [...] SELECT FROM [...]</c> in the database.
		/// </summary>
		/// <returns>The number of inserted entities.</returns>
		public int Insert()
		{
			return _source.ExecuteInsert<TSource, TTarget>(DmlExpressionRewriter.PrepareExpression<TSource>(_source.Expression, _assignments.List));
		}
	}
}