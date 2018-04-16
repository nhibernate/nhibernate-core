using System;
using System.Linq;
using System.Linq.Expressions;

namespace NHibernate.Linq
{
	/// <summary>
	/// NHibernate LINQ DML extension methods. They are meant to work with <see cref="NhQueryable{T}"/>. Supplied <see cref="IQueryable{T}"/> parameters
	/// should at least have an <see cref="INhQueryProvider"/> <see cref="IQueryable.Provider"/>. <see cref="ISession.Query{T}()"/> and
	/// its overloads supply such queryables.
	/// </summary>
	public static partial class DmlExtensionMethods
	{
		/// <summary>
		/// Delete all entities selected by the specified query. The delete operation is performed in the database without reading the entities out of it.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">The query matching the entities to delete.</param>
		/// <returns>The number of deleted entities.</returns>
		public static int Delete<TSource>(this IQueryable<TSource> source)
		{
			var provider = source.GetNhProvider();
			return provider.ExecuteDml<TSource>(QueryMode.Delete, source.Expression);
		}

		/// <summary>
		/// Update all entities selected by the specified query. The update operation is performed in the database without reading the entities out of it.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">The query matching the entities to update.</param>
		/// <param name="expression">The update setters expressed as a member initialization of updated entities, e.g.
		/// <c>x => new Dog { Name = x.Name, Age = x.Age + 5 }</c>. Unset members are ignored and left untouched.</param>
		/// <returns>The number of updated entities.</returns>
		public static int Update<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, TSource>> expression)
		{
			return ExecuteUpdate(source, DmlExpressionRewriter.PrepareExpression(source.Expression, expression), false);
		}

		/// <summary>
		/// Update all entities selected by the specified query, using an anonymous initializer for specifying setters. The update operation is performed
		/// in the database without reading the entities out of it.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">The query matching the entities to update.</param>
		/// <param name="expression">The assignments expressed as an anonymous object, e.g.
		/// <c>x => new { Name = x.Name, Age = x.Age + 5 }</c>. Unset members are ignored and left untouched.</param>
		/// <returns>The number of updated entities.</returns>
		public static int Update<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> expression)
		{
			return ExecuteUpdate(source, DmlExpressionRewriter.PrepareExpressionFromAnonymous(source.Expression, expression), false);
		}

		/// <summary>
		/// Perform an <c>update versioned</c> on all entities selected by the specified query. The update operation is performed in the database without 
		/// reading the entities out of it.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">The query matching the entities to update.</param>
		/// <param name="expression">The update setters expressed as a member initialization of updated entities, e.g.
		/// <c>x => new Dog { Name = x.Name, Age = x.Age + 5 }</c>. Unset members are ignored and left untouched.</param>
		/// <returns>The number of updated entities.</returns>
		public static int UpdateVersioned<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, TSource>> expression)
		{
			return ExecuteUpdate(source, DmlExpressionRewriter.PrepareExpression(source.Expression, expression), true);
		}

		/// <summary>
		/// Perform an <c>update versioned</c> on all entities selected by the specified query, using an anonymous initializer for specifying setters. 
		/// The update operation is performed in the database without reading the entities out of it.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">The query matching the entities to update.</param>
		/// <param name="expression">The assignments expressed as an anonymous object, e.g.
		/// <c>x => new { Name = x.Name, Age = x.Age + 5 }</c>. Unset members are ignored and left untouched.</param>
		/// <returns>The number of updated entities.</returns>
		public static int UpdateVersioned<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, object>> expression)
		{
			return ExecuteUpdate(source, DmlExpressionRewriter.PrepareExpressionFromAnonymous(source.Expression, expression), true);
		}

		/// <summary>
		/// Initiate an update for the entities selected by the query. Return
		/// a builder allowing to set properties and allowing to execute the update.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">The query matching the entities to update.</param>
		/// <returns>An update builder.</returns>
		public static UpdateBuilder<TSource> UpdateBuilder<TSource>(this IQueryable<TSource> source)
		{
			return new UpdateBuilder<TSource>(source);
		}

		internal static int ExecuteUpdate<TSource>(this IQueryable<TSource> source, Expression updateExpression, bool versioned)
		{
			var provider = source.GetNhProvider();
			return provider.ExecuteDml<TSource>(versioned ? QueryMode.UpdateVersioned : QueryMode.Update, updateExpression);
		}

		/// <summary>
		/// Insert all entities selected by the specified query. The insert operation is performed in the database without reading the entities out of it.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TTarget">The type of the entities to insert.</typeparam>
		/// <param name="source">The query matching entities source of the data to insert.</param>
		/// <param name="expression">The expression projecting a source entity to the entity to insert.</param>
		/// <returns>The number of inserted entities.</returns>
		public static int InsertInto<TSource, TTarget>(this IQueryable<TSource> source, Expression<Func<TSource, TTarget>> expression)
		{
			return ExecuteInsert<TSource, TTarget>(source, DmlExpressionRewriter.PrepareExpression(source.Expression, expression));
		}

		/// <summary>
		/// Insert all entities selected by the specified query, using an anonymous initializer for specifying setters. <typeparamref name="TTarget"/>
		/// must be explicitly provided, e.g. <c>source.InsertInto&lt;Cat, Dog&gt;(c => new {...})</c>. The insert operation is performed in the
		/// database without reading the entities out of it.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TTarget">The type of the entities to insert. Must be explicitly provided.</typeparam>
		/// <param name="source">The query matching entities source of the data to insert.</param>
		/// <param name="expression">The expression projecting a source entity to an anonymous object representing 
		/// the entity to insert.</param>
		/// <returns>The number of inserted entities.</returns>
		public static int InsertInto<TSource, TTarget>(this IQueryable<TSource> source, Expression<Func<TSource, object>> expression)
		{
			return ExecuteInsert<TSource, TTarget>(source, DmlExpressionRewriter.PrepareExpressionFromAnonymous(source.Expression, expression));
		}

		/// <summary>
		/// Initiate an insert using selected entities as a source. Return
		/// a builder allowing to set properties to insert and allowing to execute the update.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">The query matching the entities to update.</param>
		/// <returns>An update builder.</returns>
		public static InsertBuilder<TSource> InsertBuilder<TSource>(this IQueryable<TSource> source)
		{
			return new InsertBuilder<TSource>(source);
		}

		internal static int ExecuteInsert<TSource, TTarget>(this IQueryable<TSource> source, Expression insertExpression)
		{
			var provider = source.GetNhProvider();
			return provider.ExecuteDml<TTarget>(QueryMode.Insert, insertExpression);
		}
	}
}
