﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using NHibernate.AdoNet.Util;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// A generic batcher that will batch UPDATE/INSERT/DELETE commands by concatenating them with a semicolon.
	/// Use this batcher only if there are no dedicated batchers in the given environment. Unfortunately some
	/// database clients do not support concatenating commands with a semicolon. Here are the known clients
	/// that do not work with this batcher:
	/// - FirebirdSql.Data.FirebirdClient
	/// - Oracle.ManagedDataAccess
	/// - System.Data.SqlServerCe
	/// </summary>
	public partial class GenericBatchingBatcher : AbstractBatcher
	{
		private readonly int? _maxNumberOfParameters;
		private readonly BatchingCommandSet _currentBatch;
		private int _totalExpectedRowsAffected;
		private StringBuilder _currentBatchCommandsLog;

		public GenericBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor, char statementTerminator)
			: base(connectionManager, interceptor)
		{
			BatchSize = Factory.Settings.AdoBatchSize;
			StatementTerminator = statementTerminator;
			_currentBatch = new BatchingCommandSet(this);
			_maxNumberOfParameters = Factory.Dialect.MaxNumberOfParameters;

			// We always create this, because we need to deal with a scenario in which
			// the user change the logging configuration at runtime. Trying to put this
			// behind an if(log.IsDebugEnabled) will cause a null reference exception 
			// at that point.
			_currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
		}

		public char StatementTerminator { get; }

		public sealed override int BatchSize { get; set; }

		protected override int CountOfStatementsInCurrentBatch => _currentBatch.CountOfCommands;

		public override void AddToBatch(IExpectation expectation)
		{
			var batchUpdate = CurrentCommand;
			if (_maxNumberOfParameters.HasValue && 
				_currentBatch.CountOfParameters + CurrentCommand.Parameters.Count > _maxNumberOfParameters)
			{
				ExecuteBatchWithTiming(batchUpdate);
			}
			_totalExpectedRowsAffected += expectation.ExpectedRowCount;
			Driver.AdjustCommand(batchUpdate);
			string lineWithParameters = null;
			var sqlStatementLogger = Factory.Settings.SqlStatementLogger;
			if (sqlStatementLogger.IsDebugEnabled || Log.IsDebugEnabled())
			{
				lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
				var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
				lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
				_currentBatchCommandsLog.Append("command ")
				                        .Append(_currentBatch.CountOfCommands)
				                        .Append(":")
				                        .AppendLine(lineWithParameters);
			}
			if (Log.IsDebugEnabled())
			{
				Log.Debug("Adding to batch:{0}", lineWithParameters);
			}
			
			_currentBatch.Append(CurrentCommand.Parameters);

			if (_currentBatch.CountOfCommands >= BatchSize)
			{
				ExecuteBatchWithTiming(batchUpdate);
			}
		}

		protected override void DoExecuteBatch(DbCommand ps)
		{
			if (_currentBatch.CountOfCommands == 0)
			{
				Expectations.VerifyOutcomeBatched(_totalExpectedRowsAffected, 0);
				return;
			}
			try
			{
				Log.Debug("Executing batch");
				CheckReaders();
				if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
				{
					Factory.Settings.SqlStatementLogger.LogBatchCommand(_currentBatchCommandsLog.ToString());
				}

				int rowsAffected;
				try
				{
					rowsAffected = _currentBatch.ExecuteNonQuery();
				}
				catch (DbException e)
				{
					throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "could not execute batch command.");
				}

				Expectations.VerifyOutcomeBatched(_totalExpectedRowsAffected, rowsAffected);
			}
			finally
			{
				ClearCurrentBatch();
			}
		}

		private void ClearCurrentBatch()
		{
			_currentBatch.Clear();
			_totalExpectedRowsAffected = 0;

			if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
			{
				_currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
			}
		}

		public override void CloseCommands()
		{
			base.CloseCommands();
			ClearCurrentBatch();
		}

		protected override void Dispose(bool isDisposing)
		{
			base.Dispose(isDisposing);
			_currentBatch.Dispose();
		}

		private partial class BatchingCommandSet : IDisposable
		{
			private readonly GenericBatchingBatcher _batcher;
			private readonly SqlStringBuilder _sql = new SqlStringBuilder();
			private readonly List<SqlTypes.SqlType> _sqlTypes = new List<SqlTypes.SqlType>();
			private readonly List<BatchParameter> _parameters = new List<BatchParameter>();
			private CommandType _commandType;

			private class BatchParameter
			{
				public ParameterDirection Direction { get; set; }

				public byte Precision { get; set; }

				public byte Scale { get; set; }

				public int Size { get; set; }

				public object Value { get; set; }
			}

			public BatchingCommandSet(GenericBatchingBatcher batcher)
			{
				_batcher = batcher;
			}

			public int CountOfCommands { get; private set; }

			public int CountOfParameters { get; private set; }

			public void Append(DbParameterCollection parameters)
			{
				if (CountOfCommands > 0)
				{
					_sql.Add(_batcher.StatementTerminator.ToString());
				}
				else
				{
					_commandType = _batcher.CurrentCommand.CommandType;
				}
				_sql.Add(PrepareSqlString(_batcher.CurrentCommandSql));
				_sqlTypes.AddRange(_batcher.CurrentCommandParameterTypes);

				foreach (DbParameter parameter in parameters)
				{
					_parameters.Add(new BatchParameter
					{
						Direction = parameter.Direction,
						Precision = parameter.Precision,
						Scale = parameter.Scale,
						Size = parameter.Size,
						Value = parameter.Value
					});
				}
				CountOfCommands++;
			}

			public int ExecuteNonQuery()
			{
				if (CountOfCommands == 0)
				{
					return 0;
				}
				var batcherCommand = _batcher.Driver.GenerateCommand(
					_commandType,
					_sql.ToSqlString(),
					_sqlTypes.ToArray()
				);
				for (var i = 0; i < _parameters.Count; i++)
				{
					var parameter = _parameters[i];
					var cmdParam = batcherCommand.Parameters[i];
					cmdParam.Value = parameter.Value;
					cmdParam.Direction = parameter.Direction;
					cmdParam.Precision = parameter.Precision;
					cmdParam.Scale = parameter.Scale;
					cmdParam.Size = parameter.Size;
				}
				_batcher.Prepare(batcherCommand);
				return batcherCommand.ExecuteNonQuery();
			}

			private SqlString PrepareSqlString(SqlString sql)
			{
				sql = sql.Copy();
				foreach (var part in sql)
				{
					if (part is Parameter param)
					{
						param.ParameterPosition = CountOfParameters++;
					}
				}
				return sql;
			}

			public void Clear()
			{
				CountOfParameters = 0;
				CountOfCommands = 0;
				_sql.Clear();
				_sqlTypes.Clear();
				_parameters.Clear();
			}

			public void Dispose()
			{
				Clear();
			}
		}
	}
}
