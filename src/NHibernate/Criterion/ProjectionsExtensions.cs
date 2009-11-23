using System;
using System.Linq.Expressions;

using NHibernate.Impl;

namespace NHibernate.Criterion
{
	public static class ProjectionsExtensions
	{
		/// <summary>
		/// Create an alias for a projection
		/// </summary>
		/// <param name="projection">the projection instance</param>
		/// <param name="alias">LambdaExpression returning an alias</param>
		/// <returns>return NHibernate.Criterion.IProjection</returns>
		public static IProjection WithAlias(this IProjection			projection,
											Expression<Func<object>>    alias)
		{
			string aliasContainer = ExpressionProcessor.FindMemberExpression(alias.Body);
			return Projections.Alias(projection, aliasContainer);
		}
	}
}
