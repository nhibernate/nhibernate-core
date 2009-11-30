
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{

	public class QueryOverProjectionBuilder<TReturn, TRoot, TSubType>
	{

		private TReturn fluentReturn;
		private IQueryOver<TRoot,TSubType> criteria;
		private ProjectionList projectionList;
		private IProjection lastProjection = null;

		public QueryOverProjectionBuilder(TReturn fluentReturn, IQueryOver<TRoot,TSubType> criteria)
		{
			this.fluentReturn = fluentReturn;
			this.criteria = criteria;
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

		/// <summary>
		/// Create the ProjectionList and return to the query
		/// </summary>
		public TReturn EndSelect
		{
			get
			{
				AddLastProjection();
				criteria.Select(projectionList);
				return fluentReturn;
			}
		}

		/// <summary>
		/// Create an alias for the previous projection
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> WithAlias(Expression<Func<object>> alias)
		{
			string aliasContainer = ExpressionProcessor.FindMemberExpression(alias.Body);
			lastProjection = Projections.Alias(lastProjection, aliasContainer);
			return this;
		}

		/// <summary>
		/// Select an arbitrary projection
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> Select(IProjection projection)
		{
			PushProjection(projection);
			return this;
		}

		/// <summary>
		/// A property average value
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectAvg(Expression<Func<TSubType, object>> expression)
		{
			PushProjection(Projections.Avg(expression));
			return this;
		}

		/// <summary>
		/// A property average value
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectAvg(Expression<Func<object>> expression)
		{
			PushProjection(Projections.Avg(expression));
			return this;
		}

		/// <summary>
		/// A property value count
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectCount(Expression<Func<TSubType, object>> expression)
		{
			PushProjection(Projections.Count(expression));
			return this;
		}

		/// <summary>
		/// A property value count
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectCount(Expression<Func<object>> expression)
		{
			PushProjection(Projections.Count(expression));
			return this;
		}

		/// <summary>
		/// A distinct property value count
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectCountDistinct(Expression<Func<TSubType, object>> expression)
		{
			PushProjection(Projections.CountDistinct(expression));
			return this;
		}

		/// <summary>
		/// A distinct property value count
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectCountDistinct(Expression<Func<object>> expression)
		{
			PushProjection(Projections.CountDistinct(expression));
			return this;
		}

		/// <summary>
		/// A grouping property value
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectGroup(Expression<Func<TSubType, object>> expression)
		{
			PushProjection(Projections.Group(expression));
			return this;
		}

		/// <summary>
		/// A grouping property value
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectGroup(Expression<Func<object>> expression)
		{
			PushProjection(Projections.Group(expression));
			return this;
		}

		/// <summary>
		/// A property maximum value
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectMax(Expression<Func<TSubType, object>> expression)
		{
			PushProjection(Projections.Max(expression));
			return this;
		}

		/// <summary>
		/// A property maximum value
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectMax(Expression<Func<object>> expression)
		{
			PushProjection(Projections.Max(expression));
			return this;
		}

		/// <summary>
		/// A property minimum value
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectMin(Expression<Func<TSubType, object>> expression)
		{
			PushProjection(Projections.Min(expression));
			return this;
		}

		/// <summary>
		/// A property minimum value
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectMin(Expression<Func<object>> expression)
		{
			PushProjection(Projections.Min(expression));
			return this;
		}

		/// <summary>
		/// A projected property value
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> Select(Expression<Func<TSubType, object>> expression)
		{
			PushProjection(Projections.Property(expression));
			return this;
		}

		/// <summary>
		/// A projected property value
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> Select(Expression<Func<object>> expression)
		{
			PushProjection(Projections.Property(expression));
			return this;
		}

		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectSubQuery<U>(QueryOver<U> detachedQueryOver)
		{
			PushProjection(Projections.SubQuery(detachedQueryOver));
			return this;
		}

		/// <summary>
		/// A property value sum
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectSum(Expression<Func<TSubType, object>> expression)
		{
			PushProjection(Projections.Sum(expression));
			return this;
		}

		/// <summary>
		/// A property value sum
		/// </summary>
		public QueryOverProjectionBuilder<TReturn, TRoot, TSubType> SelectSum(Expression<Func<object>> expression)
		{
			PushProjection(Projections.Sum(expression));
			return this;
		}

	}

}
