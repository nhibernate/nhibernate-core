using System;
using System.Data;

using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.AdoNet.Util;

namespace NHibernate.Id.Enhanced
{
	/// <summary>
	/// Describes a table used to mimic sequence behavior
	/// </summary>
	public class TableStructure : TransactionHelper, IDatabaseStructure
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(IDatabaseStructure));

		private readonly int _incrementSize;
		private readonly int _initialValue;
		private readonly string _tableName;
		private readonly string _valueColumnName;

		private readonly SqlString _selectQuery;
		private readonly SqlString _updateQuery;
		private readonly SqlType[] _updateParameterTypes;

		private int _accessCounter;
		private bool _applyIncrementSizeToSourceValues;

		public TableStructure(Dialect.Dialect dialect, string tableName, string valueColumnName, int initialValue, int incrementSize)
		{
			_tableName = tableName;
			_valueColumnName = valueColumnName;
			_initialValue = initialValue;
			_incrementSize = incrementSize;

			_selectQuery = new SqlString(
				"select ", valueColumnName, " as id_val from ",
				dialect.AppendLockHint(LockMode.Upgrade, tableName),
				dialect.ForUpdateString);

			_updateQuery = new SqlString(
				"update ", tableName,
				" set ", valueColumnName, " = ", Parameter.Placeholder,
				" where ", valueColumnName, " = ", Parameter.Placeholder);

			_updateParameterTypes = new[]
			{
				SqlTypeFactory.Int64,
				SqlTypeFactory.Int64,
			};
		}

		#region Implementation of IDatabaseStructure

		public string Name
		{
			get { return _tableName; }
		}

		public int TimesAccessed
		{
			get { return _accessCounter; }
		}

		public int IncrementSize
		{
			get { return _incrementSize; }
		}

		public virtual IAccessCallback BuildCallback(ISessionImplementor session)
		{
			return new TableAccessCallback(session, this);
		}

		public virtual void Prepare(IOptimizer optimizer)
		{
			_applyIncrementSizeToSourceValues = optimizer.ApplyIncrementSizeToSourceValues;
		}

		public virtual string[] SqlCreateStrings(Dialect.Dialect dialect)
		{
			return new[]
			{
				"create table " + _tableName + " ( " + _valueColumnName + " " + dialect.GetTypeName(SqlTypeFactory.Int64)
				+ " )", "insert into " + _tableName + " values ( " + _initialValue + " )"
			};
		}

		public virtual string[] SqlDropStrings(Dialect.Dialect dialect)
		{
			return new[] {dialect.GetDropTableString(_tableName) };
		}

		#endregion

		#region Overrides of TransactionHelper

		public override object DoWorkInCurrentTransaction(ISessionImplementor session, IDbConnection conn, IDbTransaction transaction)
		{
			long result;
			int updatedRows;

			do
			{
				try
				{
					object selectedValue;

					IDbCommand selectCmd = session.Factory.ConnectionProvider.Driver.GenerateCommand(CommandType.Text, _selectQuery, SqlTypeFactory.NoTypes);
					using (selectCmd)
					{
						selectCmd.Connection = conn;
						selectCmd.Transaction = transaction;
						PersistentIdGeneratorParmsNames.SqlStatementLogger.LogCommand(selectCmd, FormatStyle.Basic);

						selectedValue = selectCmd.ExecuteScalar();
					}

					if (selectedValue ==null)
					{
						string err = "could not read a hi value - you need to populate the table: " + _tableName;
						Log.Error(err);
						throw new IdentifierGenerationException(err);
					}
					result = Convert.ToInt64(selectedValue);
				}
				catch (Exception sqle)
				{
					Log.Error("could not read a hi value", sqle);
					throw;
				}

				try
				{
					IDbCommand updateCmd = session.Factory.ConnectionProvider.Driver.GenerateCommand(CommandType.Text, _updateQuery, _updateParameterTypes);
					using (updateCmd)
					{
						updateCmd.Connection = conn;
						updateCmd.Transaction = transaction;
						PersistentIdGeneratorParmsNames.SqlStatementLogger.LogCommand(updateCmd, FormatStyle.Basic);

						int increment = _applyIncrementSizeToSourceValues ? _incrementSize : 1;
						((IDataParameter)updateCmd.Parameters[0]).Value = result + increment;
						((IDataParameter)updateCmd.Parameters[1]).Value = result;
						updatedRows = updateCmd.ExecuteNonQuery();
					}
				}
				catch (Exception sqle)
				{
					Log.Error("could not update hi value in: " + _tableName, sqle);
					throw;
				}
			}
			while (updatedRows == 0);

			_accessCounter++;

			return result;
		}

		#endregion

		#region Nested type: TableAccessCallback

		private class TableAccessCallback : IAccessCallback
		{
			private readonly TableStructure _owner;
			private readonly ISessionImplementor _session;

			public TableAccessCallback(ISessionImplementor session, TableStructure owner)
			{
				_session = session;
				_owner = owner;
			}

			#region IAccessCallback Members

			public virtual long GetNextValue()
			{
				return Convert.ToInt64(_owner.DoWorkInNewTransaction(_session));
			}

			#endregion
		}

		#endregion
	}
}