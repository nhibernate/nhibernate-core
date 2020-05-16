using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Param;
using NHibernate.Type;

namespace NHibernate.Linq
{
	public partial interface INhQueryProvider : IQueryProvider
	{
		//Since 5.2
		[Obsolete("Replaced by ISupportFutureBatchNhQueryProvider interface")]
		IFutureEnumerable<TResult> ExecuteFuture<TResult>(Expression expression);

		//Since 5.2
		[Obsolete("Replaced by ISupportFutureBatchNhQueryProvider interface")]
		IFutureValue<TResult> ExecuteFutureValue<TResult>(Expression expression);
		//Since v5.3
		[Obsolete("Use SetResultTransformerAndExecuteRegisteredDelegates extension method instead.")]
		void SetResultTransformerAndAdditionalCriteria(IQuery query, NhLinqExpression nhExpression, IDictionary<string, Tuple<object, IType>> parameters);

		int ExecuteDml<T>(QueryMode queryMode, Expression expression);
		Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken);
	}

	// TODO 6.0 Move to INhQueryProvider
	public static class NhQueryProviderExtensions
	{
		public static void SetResultTransformerAndExecuteRegisteredDelegates(
			this INhQueryProvider nhQueryProvider,
			IQuery query,
			NhLinqExpression nhExpression,
			IDictionary<string, NamedParameter> parameters)
		{
			if (nhQueryProvider is DefaultQueryProvider defaultQueryProvider)
			{
				defaultQueryProvider.SetResultTransformerAndExecuteRegisteredDelegates(query, nhExpression, parameters);
				return;
			}

			var param = parameters.ToDictionary(
				o => o.Key,
				o => new Tuple<object, IType>(o.Value.Value, o.Value.Type));
#pragma warning disable CS0618
			nhQueryProvider.SetResultTransformerAndAdditionalCriteria(query, nhExpression, param);
#pragma warning restore CS0618
		}
	}
}
