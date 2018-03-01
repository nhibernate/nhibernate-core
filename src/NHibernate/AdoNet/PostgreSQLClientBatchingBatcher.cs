using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.AdoNet.Util;
using NHibernate.Driver;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// Batcher for PostgreSQL that will batch UPDATE/INSERT/DELETE commands.
	/// </summary>
	public partial class PostgreSQLClientBatchingBatcher : AbstractBatcher
	{
		private int _totalExpectedRowsAffected;
		private PostgreSQLCommandSet _currentBatch;
		private StringBuilder _currentBatchCommandsLog;

		public PostgreSQLClientBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
			: base(connectionManager, interceptor)
		{
			BatchSize = Factory.Settings.AdoBatchSize;
			_currentBatch = CreateConfiguredBatch();

			//we always create this, because we need to deal with a scenario in which
			//the user change the logging configuration at runtime. Trying to put this
			//behind an if(log.IsDebugEnabled) will cause a null reference exception 
			//at that point.
			_currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
		}

		public sealed override int BatchSize { get; set; }

		protected override int CountOfStatementsInCurrentBatch => _currentBatch.CountOfCommands;

		public override void AddToBatch(IExpectation expectation)
		{
			_totalExpectedRowsAffected += expectation.ExpectedRowCount;
			var batchUpdate = CurrentCommand;
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

		private PostgreSQLCommandSet CreateConfiguredBatch()
		{
			return new PostgreSQLCommandSet(this);
		}

		private void ClearCurrentBatch()
		{
			_currentBatch.Dispose();
			_totalExpectedRowsAffected = 0;
			_currentBatch = CreateConfiguredBatch();

			if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
			{
				_currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
			}
		}

		public override void CloseCommands()
		{
			base.CloseCommands();

			try
			{
				ClearCurrentBatch();
			}
			catch (Exception e)
			{
				// Prevent exceptions when clearing the batch from hiding any original exception
				// (We do not know here if this batch closing occurs after a failure or not.)
				Log.Warn(e, "Exception clearing batch");
			}
		}

		protected override void Dispose(bool isDisposing)
		{
			base.Dispose(isDisposing);
			// Prevent exceptions when closing the batch from hiding any original exception
			// (We do not know here if this batch closing occurs after a failure or not.)
			try
			{
				_currentBatch.Dispose();
			}
			catch (Exception e)
			{
				Log.Warn(e, "Exception closing batcher");
			}
		}

		private partial class PostgreSQLCommandSet : IDisposable
		{
			private int _currentParameterIndex;
			private DbCommand _batchCommand;
			private readonly PostgreSQLClientBatchingBatcher _batcher;

			public PostgreSQLCommandSet(PostgreSQLClientBatchingBatcher batcher)
			{
				_batcher = batcher;
			}
			
			public int CountOfCommands { get; private set; }

			public void Append(DbParameterCollection parameters)
			{
				if (_batchCommand == null)
				{
					_batchCommand = _batcher.Driver.GenerateCommand(
						_batcher.CurrentCommand.CommandType,
						_batcher.CurrentCommandSql,
						_batcher.CurrentCommandParameterTypes);
					UpdateCommandParameters(_batchCommand, parameters);
					_currentParameterIndex = parameters.Count;
				}
				else
				{
					// We need to create a new command with different parameter names to avoid duplicates
					var command = _batcher.Driver.GenerateCommand(
						_batcher.CurrentCommand.CommandType,
						PrepareSqlString(_batcher.CurrentCommandSql),
						_batcher.CurrentCommandParameterTypes);
					UpdateCommandParameters(command, parameters);
					_batchCommand.CommandText += ";" + command.CommandText;
					foreach (DbParameter parameter in command.Parameters)
					{
						_batchCommand.Parameters.Add(CopyParameter(_batchCommand, parameter));
					}
					command.Dispose();
				}
				CountOfCommands++;
			}

			public int ExecuteNonQuery()
			{
				if (_batchCommand == null)
				{
					return 0;
				}
				// Npgsql will correctly prepare a multi SQL statement even if the parameter names are different
				// for each statement. Npgsql internally parses the query and omits parameter names when comparing two queries
				// in order to prevent having multiple prepared statements for the same query.
				_batcher.Prepare(_batchCommand);
				return _batchCommand.ExecuteNonQuery();
			}

			public void Dispose()
			{
				_batchCommand?.Dispose();
				_batchCommand = null;
				_currentParameterIndex = 0;
				CountOfCommands = 0;
			}

			private void UpdateCommandParameters(DbCommand command, DbParameterCollection parameters)
			{
				for (var i = 0; i < parameters.Count; i++)
				{
					var parameter = parameters[i];
					var cmdParam = command.Parameters[i];
					cmdParam.Value = parameter.Value;
					cmdParam.Direction = parameter.Direction;
					cmdParam.Precision = parameter.Precision;
					cmdParam.Scale = parameter.Scale;
					cmdParam.Size = parameter.Size;
				}
			}

			private DbParameter CopyParameter(DbCommand command, DbParameter parameter)
			{
				var copy = command.CreateParameter();
				copy.DbType = parameter.DbType;
				copy.IsNullable = parameter.IsNullable;
				copy.ParameterName = parameter.ParameterName;
				copy.Value = parameter.Value;
				copy.Direction = parameter.Direction;
				copy.Precision = parameter.Precision;
				copy.Scale = parameter.Scale;
				copy.Size = parameter.Size;
				copy.SourceVersion = parameter.SourceVersion;
				copy.SourceColumn = parameter.SourceColumn;
				copy.SourceColumnNullMapping = parameter.SourceColumnNullMapping;
				return copy;
			}

			private SqlString PrepareSqlString(SqlString sql)
			{
				if (_batchCommand == null)
				{
					return sql;
				}
				sql = sql.Copy();
				foreach (var part in sql)
				{
					if (part is Parameter param)
					{
						param.ParameterPosition = _currentParameterIndex++;
					}
				}
				return sql;
			}
		}
	}
}
