using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Exceptions;

namespace NHibernate.Multi
{
	/// <inheritdoc />
	public partial class QueryBatch : IQueryBatch
	{
		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(QueryBatch));

		private readonly bool _autoReset;
		private readonly List<IQueryBatchItem> _queries = new List<IQueryBatchItem>();
		private readonly Dictionary<string, IQueryBatchItem> _queriesByKey = new Dictionary<string, IQueryBatchItem>();
		private bool _executed;

		public QueryBatch(ISessionImplementor session, bool autoReset)
		{
			Session = session;
			_autoReset = autoReset;
		}

		protected ISessionImplementor Session { get; }

		/// <inheritdoc />
		public int? Timeout { get; set; }

		/// <inheritdoc />
		public FlushMode? FlushMode { get; set; }

		/// <inheritdoc />
		public void Execute()
		{
			if (_queries.Count == 0)
				return;
			var sessionFlushMode = Session.FlushMode;
			if (FlushMode.HasValue)
				Session.FlushMode = FlushMode.Value;
			try
			{
				Init();

				if (!Session.Factory.ConnectionProvider.Driver.SupportsMultipleQueries)
				{
					foreach (var query in _queries)
					{
						query.ExecuteNonBatched();
					}
					return;
				}

				using (Session.BeginProcess())
				{
					DoExecute();
				}
			}
			finally
			{
				if (_autoReset)
				{
					_queries.Clear();
					_queriesByKey.Clear();
				}
				else
					_executed = true;

				if (FlushMode.HasValue)
					Session.FlushMode = sessionFlushMode;
			}
		}

		/// <inheritdoc />
		public void Add(IQueryBatchItem query)
		{
			if (query == null)
				throw new ArgumentNullException(nameof(query));
			if (_executed)
				throw new InvalidOperationException("The batch has already been executed, use another batch");
			_queries.Add(query);
		}

		/// <inheritdoc />
		public void Add(string key, IQueryBatchItem query)
		{
			Add(query);
			_queriesByKey.Add(key, query);
		}

		/// <inheritdoc />
		public IList<TResult> GetResult<TResult>(int queryIndex)
		{
			return GetResults<TResult>(_queries[queryIndex]);
		}

		/// <inheritdoc />
		public IList<TResult> GetResult<TResult>(string querykey)
		{
			return GetResults<TResult>(_queriesByKey[querykey]);
		}

		private IList<TResult> GetResults<TResult>(IQueryBatchItem query)
		{
			if (!_executed)
				Execute();
			return ((IQueryBatchItem<TResult>)query).GetResults();
		}

		private void Init()
		{
			foreach (var query in _queries)
			{
				query.Init(Session);
			}
		}

		private void CombineQueries(IResultSetsCommand resultSetsCommand)
		{
			foreach (var multiSource in _queries)
			foreach (var cmd in multiSource.GetCommands())
			{
				resultSetsCommand.Append(cmd);
			}
		}

		protected void DoExecute()
		{
			var resultSetsCommand = Session.Factory.ConnectionProvider.Driver.GetResultSetsCommand(Session);
			CombineQueries(resultSetsCommand);

			var querySpaces = new HashSet<string>(_queries.SelectMany(t => t.GetQuerySpaces()));
			if (resultSetsCommand.HasQueries)
			{
				Session.AutoFlushIfRequired(querySpaces);
			}

			bool statsEnabled = Session.Factory.Statistics.IsStatisticsEnabled;
			Stopwatch stopWatch = null;
			if (statsEnabled)
			{
				stopWatch = new Stopwatch();
				stopWatch.Start();
			}
			if (Log.IsDebugEnabled())
			{
				Log.Debug("Multi query with {0} queries: {1}", _queries.Count, resultSetsCommand.Sql);
			}

			int rowCount = 0;
			try
			{
				if (resultSetsCommand.HasQueries)
				{
					using (var reader = resultSetsCommand.GetReader(Timeout))
					{
						foreach (var multiSource in _queries)
						{
							foreach (var resultSetHandler in multiSource.GetResultSetHandler())
							{
								rowCount += resultSetHandler(reader);
								reader.NextResult();
							}
						}
					}
				}

				foreach (var multiSource in _queries)
				{
					multiSource.ProcessResults();
				}
			}
			catch (Exception sqle)
			{
				Log.Error(sqle, "Failed to execute multi query: [{0}]", resultSetsCommand.Sql);
				throw ADOExceptionHelper.Convert(Session.Factory.SQLExceptionConverter, sqle, "Failed to execute multi query", resultSetsCommand.Sql);
			}

			if (statsEnabled)
			{
				stopWatch.Stop();
				Session.Factory.StatisticsImplementor.QueryExecuted($"{_queries.Count} queries", rowCount, stopWatch.Elapsed);
			}
		}
	}
}
