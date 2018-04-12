using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using NHibernate.AdoNet.Util;
using NHibernate.Exceptions;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// Summary description for HanaBatchingBatcher.
	/// By Jonathan Bregler
	/// </summary>
	public partial class HanaBatchingBatcher : AbstractBatcher
	{
		private int _batchSize;
		private int _countOfCommands;
		private int _totalExpectedRowsAffected;
		private DbCommand _currentBatch;
		private readonly IList<DbCommand> _currentBatchCommands = new List<DbCommand>();
		private StringBuilder _currentBatchCommandsLog;

		public HanaBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
			: base(connectionManager, interceptor)
		{
			_batchSize = Factory.Settings.AdoBatchSize;
			//we always create this, because we need to deal with a scenario in which
			//the user change the logging configuration at runtime. Trying to put this
			//behind an if(log.IsDebugEnabled) will cause a null reference exception 
			//at that point.
			_currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
		}

		public override void AddToBatch(IExpectation expectation)
		{
			Debug.Assert(CurrentCommand is ICloneable); // HanaCommands are cloneable

			var batchUpdate = CurrentCommand;
			Prepare(batchUpdate);
			Driver.AdjustCommand(batchUpdate);

			_totalExpectedRowsAffected += expectation.ExpectedRowCount;
			string lineWithParameters = null;
			var sqlStatementLogger = Factory.Settings.SqlStatementLogger;
			if (sqlStatementLogger.IsDebugEnabled || Log.IsDebugEnabled())
			{
				lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
				var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
				lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
				_currentBatchCommandsLog.Append("command ")
					.Append(_countOfCommands)
					.Append(":")
					.AppendLine(lineWithParameters);
			}
			if (Log.IsDebugEnabled())
			{
				Log.Debug("Adding to batch:{0}", lineWithParameters);
			}

			if (_currentBatch == null)
			{
				// use first command as the batching command
				_currentBatch = (batchUpdate as ICloneable).Clone() as DbCommand;
			}

			_currentBatchCommands.Add((batchUpdate as ICloneable).Clone() as DbCommand);

			_countOfCommands++;

			if (_countOfCommands >= _batchSize)
			{
				DoExecuteBatch(batchUpdate);
			}
		}

		protected override void DoExecuteBatch(DbCommand ps)
		{
			Log.Info("Executing batch");
			CheckReaders();

			if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
			{
				Factory.Settings.SqlStatementLogger.LogBatchCommand(_currentBatchCommandsLog.ToString());
				_currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
			}

			try
			{
				int rowCount = 0;

				if (_countOfCommands > 0)
				{
					_currentBatch.Parameters.Clear();

					foreach (var command in _currentBatchCommands)
					{
						foreach (DbParameter parameter in command.Parameters)
						{
							_currentBatch.Parameters.Add(parameter);
						}
					}

					_currentBatch.Prepare();

					try
					{
						rowCount = _currentBatch.ExecuteNonQuery();
					}
					catch (DbException e)
					{
						throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "could not execute batch command.");
					}
				}

				Expectations.VerifyOutcomeBatched(_totalExpectedRowsAffected, rowCount);
			}
			finally
			{
				// Cleaning up even if batched outcome is invalid
				_totalExpectedRowsAffected = 0;
				_countOfCommands = 0;
				CloseBatchCommands();
			}
		}

		protected override int CountOfStatementsInCurrentBatch
		{
			get { return _countOfCommands; }
		}

		public override int BatchSize
		{
			get { return _batchSize; }
			set { _batchSize = value; }
		}

		public override void CloseCommands()
		{
			base.CloseCommands();

			CloseBatchCommands();
		}

		protected override void Dispose(bool isDisposing)
		{
			base.Dispose(isDisposing);

			CloseBatchCommands();
		}

		private void CloseBatchCommands()
		{
			try
			{
				foreach (var currentBatchCommand in _currentBatchCommands)
				{
					currentBatchCommand.Dispose();
				}
				_currentBatchCommands.Clear();

				_currentBatch?.Dispose();
				_currentBatch = null;
			}
			catch (Exception e)
			{
				// Prevent exceptions when clearing the batch from hiding any original exception
				// (We do not know here if this batch closing occurs after a failure or not.)
				Log.Warn(e, "Exception clearing batch");
			}
		}
	}
}
