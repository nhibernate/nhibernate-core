using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Event;
using NHibernate.Hql;
using NHibernate.Linq;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine.Query
{
    public partial interface IQueryPlan
    {
        ParameterMetadata ParameterMetadata { get; }
        ISet<string> QuerySpaces { get; }
        IQueryTranslator[] Translators { get; }
        ReturnMetadata ReturnMetadata { get; }
        void PerformList(QueryParameters queryParameters, ISessionImplementor statelessSessionImpl, IList results);
        int PerformExecuteUpdate(QueryParameters queryParameters, ISessionImplementor statelessSessionImpl);
        IEnumerable<T> PerformIterate<T>(QueryParameters queryParameters, IEventSource session);
        IEnumerable PerformIterate(QueryParameters queryParameters, IEventSource session);
		// Since v5.3
		[Obsolete("This method has no more usages and will be removed in a future version")]
		Task<IEnumerable<T>> PerformIterateAsync<T>(QueryParameters queryParameters, IEventSource session, CancellationToken cancellationToken);
		// Since v5.3
		[Obsolete("This method has no more usages and will be removed in a future version")]
		Task<IEnumerable> PerformIterateAsync(QueryParameters queryParameters, IEventSource session, CancellationToken cancellationToken);
	}

	// 6.0 TODO: Move into IQueryPlan
	internal static class QueryPlanExtensions
	{
		/// <summary>
		/// Returns an <see cref="IAsyncEnumerable{T}" /> which can be enumerated asynchronously.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="queryPlan">The query plan.</param>
		/// <param name="queryParameters">The query parameters.</param>
		/// <param name="session">The session.</param>
		public static IAsyncEnumerable<T> PerformAsyncIterate<T>(this IQueryPlan queryPlan, QueryParameters queryParameters, IEventSource session)
		{
			return ReflectHelper.CastOrThrow<HQLQueryPlan>(queryPlan, "async enumerable")
				.PerformAsyncIterate<T>(queryParameters, session);
		}
	}

    public interface IQueryExpressionPlan : IQueryPlan
    {
        IQueryExpression QueryExpression { get; }
    }

	/// <summary> Defines a query execution plan for an HQL query (or filter). </summary>
	[Serializable]
	public partial class HQLQueryPlan : IQueryPlan
	{
		protected static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(HQLQueryPlan));

		private readonly string _sourceQuery;

        protected HQLQueryPlan(string sourceQuery, IQueryTranslator[] translators)
        {
            Translators = translators;
            _sourceQuery = sourceQuery;

            FinaliseQueryPlan();
        }

		internal HQLQueryPlan(HQLQueryPlan source)
		{
			Translators = source.Translators;
			_sourceQuery = source._sourceQuery;
			QuerySpaces = source.QuerySpaces;
			ParameterMetadata = source.ParameterMetadata;
			ReturnMetadata = source.ReturnMetadata;
			SqlStrings = source.SqlStrings;
		}

	    public ISet<string> QuerySpaces
		{
		    get;
		    private set;
		}

		public ParameterMetadata ParameterMetadata
		{
            get;
            private set;
        }

		public ReturnMetadata ReturnMetadata
		{
            get;
            private set;
        }

		public string[] SqlStrings
		{
            get;
            private set;
        }

		public IQueryTranslator[] Translators
		{
            get;
            private set;
        }

		public void PerformList(QueryParameters queryParameters, ISessionImplementor session, IList results)
		{
			if (Log.IsDebugEnabled())
			{
				Log.Debug("find: {0}", _sourceQuery);
				queryParameters.LogParameters(session.Factory);
			}

			bool hasLimit = queryParameters.RowSelection != null && queryParameters.RowSelection.DefinesLimits;
			bool needsLimit = hasLimit && Translators.Length > 1;
			QueryParameters queryParametersToUse;
			if (needsLimit)
			{
				Log.Warn("firstResult/maxResults specified on polymorphic query; applying in memory!");
				RowSelection selection = new RowSelection();
				selection.FetchSize = queryParameters.RowSelection.FetchSize;
				selection.Timeout = queryParameters.RowSelection.Timeout;
				queryParametersToUse = queryParameters.CreateCopyUsing(selection);
			}
			else
			{
				queryParametersToUse = queryParameters;
			}

			IList combinedResults = results ?? new List<object>();
			IdentitySet distinction = new IdentitySet();
			int includedCount = -1;
			for (int i = 0; i < Translators.Length; i++)
			{
				IList tmp = Translators[i].List(session, queryParametersToUse);
				if (needsLimit)
				{
					// NOTE : firstRow is zero-based
					int first = queryParameters.RowSelection.FirstRow == RowSelection.NoValue
												? 0
												: queryParameters.RowSelection.FirstRow;

					int max = queryParameters.RowSelection.MaxRows == RowSelection.NoValue
											? RowSelection.NoValue
											: queryParameters.RowSelection.MaxRows;

					int size = tmp.Count;
					for (int x = 0; x < size; x++)
					{
						object result = tmp[x];
						if (distinction.Add(result))
						{
							continue;
						}
						includedCount++;
						if (includedCount < first)
						{
							continue;
						}
						combinedResults.Add(result);
						if (max >= 0 && includedCount > max)
						{
							// break the outer loop !!!
							return;
						}
					}
				}
				else
					ArrayHelper.AddAll(combinedResults, tmp);
			}
		}

		public IEnumerable PerformIterate(QueryParameters queryParameters, IEventSource session)
		{
			if (Log.IsDebugEnabled())
			{
				Log.Debug("enumerable: {0}", _sourceQuery);
				queryParameters.LogParameters(session.Factory);
			}
			if (Translators.Length == 0)
			{
				return CollectionHelper.EmptyEnumerable;
			}
			if (Translators.Length == 1)
			{
				return Translators[0].GetEnumerable(queryParameters, session);
			}
			var results = new IEnumerable[Translators.Length];
			for (int i = 0; i < Translators.Length; i++)
			{
				var result = Translators[i].GetEnumerable(queryParameters, session);
				results[i] = result;
			}
			return new JoinedEnumerable(results);
		}

		// Since v5.3
		[Obsolete("This method has no more usages and will be removed in a future version")]
		public async Task<IEnumerable> PerformIterateAsync(QueryParameters queryParameters, IEventSource session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (Log.IsDebugEnabled())
			{
				Log.Debug("enumerable: {0}", _sourceQuery);
				queryParameters.LogParameters(session.Factory);
			}
			if (Translators.Length == 0)
			{
				return CollectionHelper.EmptyEnumerable;
			}
			if (Translators.Length == 1)
			{
				return await (Translators[0].GetEnumerableAsync(queryParameters, session, cancellationToken)).ConfigureAwait(false);
			}
			var results = new IEnumerable[Translators.Length];
			for (int i = 0; i < Translators.Length; i++)
			{
				var result = await (Translators[i].GetEnumerableAsync(queryParameters, session, cancellationToken)).ConfigureAwait(false);
				results[i] = result;
			}
			return new JoinedEnumerable(results);
		}

		// Since v5.3
		[Obsolete("This method has no more usages and will be removed in a future version")]
		public async Task<IEnumerable<T>> PerformIterateAsync<T>(QueryParameters queryParameters, IEventSource session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return new SafetyEnumerable<T>(await (PerformIterateAsync(queryParameters, session, cancellationToken)).ConfigureAwait(false));
		}

		public IAsyncEnumerable<T> PerformAsyncIterate<T>(QueryParameters queryParameters, IEventSource session)
		{
			if (Log.IsDebugEnabled())
			{
				Log.Debug("enumerable: {0}", _sourceQuery);
				queryParameters.LogParameters(session.Factory);
			}

			if (Translators.Length == 0)
			{
				return new CollectionHelper.EmptyAsyncEnumerable<T>();
			}

			if (Translators.Length == 1)
			{
				return Translators[0].GetAsyncEnumerable<T>(queryParameters, session);
			}

			var results = new IAsyncEnumerable<T>[Translators.Length];
			for (int i = 0; i < Translators.Length; i++)
			{
				var result = Translators[i].GetAsyncEnumerable<T>(queryParameters, session);
				results[i] = result;
			}

			return new JoinedAsyncEnumerable<T>(results);
		}

		public IEnumerable<T> PerformIterate<T>(QueryParameters queryParameters, IEventSource session)
		{
			return new SafetyEnumerable<T>(PerformIterate(queryParameters, session));
		}

        public int PerformExecuteUpdate(QueryParameters queryParameters, ISessionImplementor session)
        {
            if (Log.IsDebugEnabled())
            {
                Log.Debug("executeUpdate: {0}", _sourceQuery);
                queryParameters.LogParameters(session.Factory);
            }
            if (Translators.Length != 1)
            {
                Log.Warn("manipulation query [{0}] resulted in [{1}] split queries", _sourceQuery, Translators.Length);
            }
            int result = 0;
            for (int i = 0; i < Translators.Length; i++)
            {
                result += Translators[i].ExecuteUpdate(queryParameters, session);
            }
            return result;
        }

		void FinaliseQueryPlan()
        {
            BuildSqlStringsAndQuerySpaces();
            BuildMetaData();
        }

	    void BuildMetaData()
	    {
            if (Translators.Length == 0)
            {
                ParameterMetadata = new ParameterMetadata(null, null);
                ReturnMetadata = null;
            }
            else
            {
                ParameterMetadata = Translators[0].BuildParameterMetadata();

                if (Translators[0].IsManipulationStatement)
                {
                    ReturnMetadata = null;
                }
                else
                {
                    if (Translators.Length > 1)
                    {
                        int returns = Translators[0].ReturnTypes.Length;
                        ReturnMetadata = new ReturnMetadata(Translators[0].ReturnAliases, new IType[returns]);
                    }
                    else
                    {
                        ReturnMetadata = new ReturnMetadata(Translators[0].ReturnAliases, Translators[0].ReturnTypes);
                    }
                }
            }
        }

	    void BuildSqlStringsAndQuerySpaces()
        {
            var combinedQuerySpaces = new HashSet<string>();
            var sqlStringList = new List<string>();

            foreach (var translator in Translators)
            {
                foreach (var qs in translator.QuerySpaces)
                {
                    combinedQuerySpaces.Add(qs);
                }

                sqlStringList.AddRange(translator.CollectSqlStrings);
            }

            SqlStrings = sqlStringList.ToArray();
            QuerySpaces = combinedQuerySpaces;
        }
    }
}
