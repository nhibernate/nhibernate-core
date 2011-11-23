using System;
using System.Data;
using System.Text;

using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Id.Enhanced
{
	/// <summary>
	/// Describes a table used to mimic sequence behavior
	/// </summary>
	public class TableStructure : TransactionHelper, IDatabaseStructure
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(IDatabaseStructure));
		private static readonly IInternalLogger SqlLog = LoggerProvider.LoggerFor("NHibernate.SQL");

		private readonly int _incrementSize;
		private readonly int _initialValue;
		private readonly string _tableName;
		private readonly string _valueColumnName;
		private int _accessCounter;
		private bool _applyIncrementSizeToSourceValues;
		private readonly SqlString _select;
		private readonly SqlString _update;

		public TableStructure(Dialect.Dialect dialect, string tableName, string valueColumnName, int initialValue, int incrementSize)
		{
			_tableName = tableName;
			_valueColumnName = valueColumnName;
			_initialValue = initialValue;
			_incrementSize = incrementSize;

			var b = new SqlStringBuilder();
			b.Add("select ").Add(valueColumnName).Add(" id_val").Add(" from ").Add(dialect.AppendLockHint(LockMode.Upgrade, tableName))
				.Add(dialect.ForUpdateString);

			_select = b.ToSqlString();

			b = new SqlStringBuilder();
			b.Add("update ").Add(tableName).Add(" set ").Add(valueColumnName).Add(" = ").Add(Parameter.Placeholder).Add(" where ")
				.Add(valueColumnName).Add(" = ").Add(Parameter.Placeholder);
			_update = b.ToSqlString();
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
			StringBuilder sqlDropString = new StringBuilder().Append("drop table ");
			if (dialect.SupportsIfExistsBeforeTableName)
			{
				sqlDropString.Append("if exists ");
			}
			sqlDropString.Append(_tableName).Append(dialect.CascadeConstraintsString);
			if (dialect.SupportsIfExistsAfterTableName)
			{
				sqlDropString.Append(" if exists");
			}
			return new[] { sqlDropString.ToString() };
		}

		#endregion

		#region Overrides of TransactionHelper

		public override object DoWorkInCurrentTransaction(ISessionImplementor session, IDbConnection conn, IDbTransaction transaction)
		{
			long result;
			int rows;
			do
			{
				string query = _select.ToString();
				SqlLog.Debug(query);
				IDbCommand qps = conn.CreateCommand();
				IDataReader rs = null;
				qps.CommandText = query;
				qps.CommandType = CommandType.Text;
				qps.Transaction = conn.BeginTransaction();
				try
				{
					rs = qps.ExecuteReader();
					if (!rs.Read())
					{
						string err = "could not read a hi value - you need to populate the table: " + _tableName;
						Log.Error(err);
						throw new IdentifierGenerationException(err);
					}
					result = rs.GetInt64(0);
					rs.Close();
				}
				catch (Exception sqle)
				{
					Log.Error("could not read a hi value", sqle);
					throw;
				}
				finally
				{
					if (rs != null) rs.Close();
					qps.Dispose();
				}

				query = _update.ToString();

				IDbCommand ups = conn.CreateCommand();
				ups.CommandType = CommandType.Text;
				ups.CommandText = query;
				ups.Connection = conn;
				ups.Transaction = conn.BeginTransaction();
				try
				{
					int increment = _applyIncrementSizeToSourceValues ? _incrementSize : 1;
					((IDataParameter)ups.Parameters[0]).Value = result + increment;
					((IDataParameter)ups.Parameters[1]).Value = result;
					rows = ups.ExecuteNonQuery();
				}
				catch (Exception sqle)
				{
					Log.Error("could not update hi value in: " + _tableName, sqle);
					throw;
				}
				finally
				{
					ups.Dispose();
				}
			}
			while (rows == 0);

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