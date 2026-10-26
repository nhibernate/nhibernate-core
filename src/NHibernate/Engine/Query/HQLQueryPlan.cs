using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Event;
using NHibernate.Hql;
using NHibernate.Hql.Ast.ANTLR;
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
				// In memory evaluation is only problematic if items are skipped or if there is an order by clause, thus
				// correctness is not ensured. Otherwise the database still limits the row count of each query (see
				// SetMaxRows), so no more rows than needed are loaded.
				if (Translators.Any(t => (t as QueryTranslatorImpl)?.ContainsOrderByClause != false))
				{
					Log.Warn("firstResult/maxResults specified on polymorphic query with order by; applying in memory!");
				}
				else if (queryParameters.RowSelection.FirstRow > 0)
				{
					Log.Warn("firstResult specified on polymorphic query; applying in memory!");
				}

				RowSelection selection = new RowSelection();
				SetMaxRows(selection, queryParameters.RowSelection, 0);
				selection.FetchSize = queryParameters.RowSelection.FetchSize;
				selection.Timeout = queryParameters.RowSelection.Timeout;
				queryParametersToUse = queryParameters.CreateCopyUsing(selection);
			}
			else
			{
				queryParametersToUse = queryParameters;
			}

			IList combinedResults = results ?? new List<object>();
			var distinction = new HashSet<object>(ReferenceComparer<object>.Instance);
			int includedCount = 0;
			for (int i = 0; i < Translators.Length; i++)
			{
				IList tmp = Translators[i].List(session, queryParametersToUse);
				if (needsLimit)
				{
					if (AddLimitedResults(tmp))
					{
						// break the outer loop !!!
						return;
					}

					SetMaxRows(queryParametersToUse.RowSelection, queryParameters.RowSelection, includedCount);
				}
				else
					ArrayHelper.AddAll(combinedResults, tmp);
			}

			// Returns true when the requested number of results has been reached.
			bool AddLimitedResults(IList tmp)
			{
				// NOTE : firstRow is zero-based
				int first = queryParameters.RowSelection.FirstRow == RowSelection.NoValue
											? 0
											: queryParameters.RowSelection.FirstRow;

				// The total of included rows, skipped ones included.
				int max = queryParameters.RowSelection.MaxRows == RowSelection.NoValue
											? RowSelection.NoValue
											: first + queryParameters.RowSelection.MaxRows;

				int size = tmp.Count;
				for (int x = 0; x < size; x++)
				{
					object result = tmp[x];
					if (!distinction.Add(result))
					{
						continue;
					}
					includedCount++;
					if (includedCount <= first)
					{
						continue;
					}
					if (max != RowSelection.NoValue && includedCount > max)
					{
						return true;
					}
					combinedResults.Add(result);
					if (includedCount == max)
					{
						return true;
					}
				}

				return false;
			}
		}

		/// <summary>
		/// Set the max rows of the next query to the number of rows still missing for the original selection.
		/// </summary>
		/// <param name="actualSelection">The row selection of the next query.</param>
		/// <param name="originalSelection">The row selection of the polymorphic query.</param>
		/// <param name="alreadyIncluded">The count of rows already included, skipped ones included.</param>
		private static void SetMaxRows(RowSelection actualSelection, RowSelection originalSelection, int alreadyIncluded)
		{
			if (originalSelection.MaxRows == RowSelection.NoValue)
				return;

			var first = originalSelection.FirstRow == RowSelection.NoValue ? 0 : originalSelection.FirstRow;
			actualSelection.MaxRows = Math.Max(0, first + originalSelection.MaxRows - alreadyIncluded);
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

			return Enumerate(results);
		}

		private static IEnumerable Enumerate(IEnumerable[] enumerables)
		{
			foreach (var enumerable in enumerables)
			{
				foreach (var o in enumerable)
				{
					yield return o;
				}
			}
		}

		public IEnumerable<T> PerformIterate<T>(QueryParameters queryParameters, IEventSource session)
		{
			return PerformIterate(queryParameters, session).CastOrDefault<T>();
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
