using System;
using System.Linq;
using System.Linq.Expressions;

namespace NHibernate.Linq
{
	/// <summary>
	/// An update builder on which values to update can be specified.
	/// </summary>
	/// <typeparam name="TSource">The type of the entities to update.</typeparam>
	public partial class UpdateBuilder<TSource>
	{
		private readonly IQueryable<TSource> _source;
		private readonly Assignments<TSource, TSource> _assignments = new Assignments<TSource, TSource>();

		internal UpdateBuilder(IQueryable<TSource> source)
		{
			_source = source;
		}

		/// <summary>
		/// Set the specified property and return this builder.
		/// </summary>
		/// <typeparam name="TProp">The type of the property.</typeparam>
		/// <param name="property">The property.</param>
		/// <param name="expression">The expression that should be assigned to the property.</param>
		/// <returns>This update builder.</returns>
		public UpdateBuilder<TSource> Set<TProp>(Expression<Func<TSource, TProp>> property, Expression<Func<TSource, TProp>> expression)
		{
			_assignments.Set(property, expression);
			return this;
		}

		/// <summary>
		/// Set the specified property and return this builder.
		/// </summary>
		/// <typeparam name="TProp">The type of the property.</typeparam>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		/// <returns>This update builder.</returns>
		public UpdateBuilder<TSource> Set<TProp>(Expression<Func<TSource, TProp>> property, TProp value)
		{
			_assignments.Set(property, value);
			return this;
		}

		/// <summary>
		/// Update the entities. The update operation is performed in the database without reading the entities out of it.
		/// </summary>
		/// <returns>The number of updated entities.</returns>
		public int Update()
		{
			return _source.ExecuteUpdate(DmlExpressionRewriter.PrepareExpression<TSource>(_source.Expression, _assignments.List), false);
		}

		/// <summary>
		/// Perform an <c>update versioned</c> on the entities. The update operation is performed in the database without reading the entities out of it.
		/// </summary>
		/// <returns>The number of updated entities.</returns>
		public int UpdateVersioned()
		{
			return _source.ExecuteUpdate(DmlExpressionRewriter.PrepareExpression<TSource>(_source.Expression, _assignments.List), true);
		}
	}
}
