
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{

	public class QueryOverProjectionBuilder<T>
	{

		private ProjectionList projectionList;
		private IProjection lastProjection = null;

		public QueryOverProjectionBuilder()
		{
			projectionList = Projections.ProjectionList();
		}

		private void AddLastProjection()
		{
			if (lastProjection != null)
				projectionList.Add(lastProjection);
		}

		private void PushProjection(IProjection projection)
		{
			AddLastProjection();
			lastProjection = projection;
		}

		internal ProjectionList ProjectionList
		{
			get
			{
				AddLastProjection();
				return projectionList;
			}
		}

		/// <summary>
		/// Create an alias for the previous projection
		/// </summary>
		public QueryOverProjectionBuilder<T> WithAlias(Expression<Func<object>> alias)
		{
			string aliasContainer = ExpressionProcessor.FindPropertyExpression(alias.Body);
			lastProjection = Projections.Alias(lastProjection, aliasContainer);
			return this;
		}

		/// <summary>
		/// Select an arbitrary projection
		/// </summary>
		public QueryOverProjectionBuilder<T> Select(IProjection projection)
		{
			PushProjection(projection);
			return this;
		}

		/// <summary>
		/// A property average value
		/// </summary>
		public QueryOverProjectionBuilder<T> SelectAvg(Expression<Func<T, object>> expression)
		{
			PushProjection(Projections.Avg(expression));
			return this;
		}

		/// <summary>
		/// A property average value
		/// </summary>
		public QueryOverProjectionBuilder<T> SelectAvg(Expression<Func<object>> expression)
		{
			PushProjection(Projections.Avg(expression));
			return this;
		}

		/// <summary>
		/// A property value count
		/// </summary>
		public QueryOverProjectionBuilder<T> SelectCount(Expression<Func<T, object>> expression)
		{
			PushProjection(Projections.Count(expression));
			return this;
		}

		/// <summary>
		/// A property value count
		/// </summary>
		public QueryOverProjectionBuilder<T> SelectCount(Expression<Func<object>> expression)
		{
			PushProjection(Projections.Count(expression));
			return this;
		}

		/// <summary>
		/// A distinct property value count
		/// </summary>
		public QueryOverProjectionBuilder<T> SelectCountDistinct(Expression<Func<T, object>> expression)
		{
			PushProjection(Projections.CountDistinct(expression));
			return this;
		}

		/// <summary>
		/// A distinct property value count
		/// </summary>
		public QueryOverProjectionBuilder<T> SelectCountDistinct(Expression<Func<object>> expression)
		{
			PushProjection(Projections.CountDistinct(expression));
			return this;
		}

		/// <summary>
		/// A grouping property value
		/// </summary>
		public QueryOverProjectionBuilder<T> SelectGroup(Expression<Func<T, object>> expression)
		{
			PushProjection(Projections.Group(expression));
			return this;
		}

		/// <summary>
		/// A grouping property value
		/// </summary>
		public QueryOverProjectionBuilder<T> SelectGroup(Expression<Func<object>> expression)
		{
			PushProjection(Projections.Group(expression));
			return this;
		}

		/// <summary>
		/// A property maximum value
		/// </summary>
		public QueryOverProjectionBuilder<T> SelectMax(Expression<Func<T, object>> expression)
		{
			PushProjection(Projections.Max(expression));
			return this;
		}

		/// <summary>
		/// A property maximum value
		/// </summary>
		public QueryOverProjectionBuilder<T> SelectMax(Expression<Func<object>> expression)
		{
			PushProjection(Projections.Max(expression));
			return this;
		}

		/// <summary>
		/// A property minimum value
		/// </summary>
		public QueryOverProjectionBuilder<T> SelectMin(Expression<Func<T, object>> expression)
		{
			PushProjection(Projections.Min(expression));
			return this;
		}

		/// <summary>
		/// A property minimum value
		/// </summary>
		public QueryOverProjectionBuilder<T> SelectMin(Expression<Func<object>> expression)
		{
			PushProjection(Projections.Min(expression));
			return this;
		}

		/// <summary>
		/// A projected property value
		/// </summary>
		public QueryOverProjectionBuilder<T> Select(Expression<Func<T, object>> expression)
		{
			PushProjection(ExpressionProcessor.FindMemberProjection(expression.Body).AsProjection());
			return this;
		}

		/// <summary>
		/// A projected property value
		/// </summary>
		public QueryOverProjectionBuilder<T> Select(Expression<Func<object>> expression)
		{
			PushProjection(ExpressionProcessor.FindMemberProjection(expression.Body).AsProjection());
			return this;
		}

		public QueryOverProjectionBuilder<T> SelectSubQuery<U>(QueryOver<U> detachedQueryOver)
		{
			PushProjection(Projections.SubQuery(detachedQueryOver));
			return this;
		}

		/// <summary>
		/// A property value sum
		/// </summary>
		public QueryOverProjectionBuilder<T> SelectSum(Expression<Func<T, object>> expression)
		{
			PushProjection(Projections.Sum(expression));
			return this;
		}

		/// <summary>
		/// A property value sum
		/// </summary>
		public QueryOverProjectionBuilder<T> SelectSum(Expression<Func<object>> expression)
		{
			PushProjection(Projections.Sum(expression));
			return this;
		}

	}

}
