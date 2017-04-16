using System;
using NHibernate.Param;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	internal class ProcessOptions : IResultOperatorProcessor<OptionsResultOperator>
	{
		public void Process(OptionsResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			queryModelVisitor.VisitorParameters.ConstantToParameterMap.TryGetValue(resultOperator.SetOptions,
				out NamedParameter parameterName);

			if (parameterName != null)
			{
				tree.AddAdditionalCriteria(
					(q, p) => setOptions(q, (Action<IQueryableOptions>)p[parameterName.Name].Item1));
			}
			else
			{
				tree.AddAdditionalCriteria(
					(q, p) => setOptions(q, (Action<IQueryableOptions>)resultOperator.SetOptions.Value));
			}

			void setOptions(IQuery query, Action<IQueryableOptions> set)
			{
				var options = new NhQueryableOptions();
				set(options);

				if (options.Timeout.HasValue)
				{
					query.SetTimeout(options.Timeout.Value);
				}
				if (options.Cacheable.HasValue)
				{
					query.SetCacheable(options.Cacheable.Value);
				}
				if (options.CacheMode.HasValue)
				{
					query.SetCacheMode(options.CacheMode.Value);
				}
				if (options.CacheRegion != null)
				{
					query.SetCacheRegion(options.CacheRegion);
				}
			}
		}
	}
}