using System;
using System.Data;
using System.Text;
using log4net;
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
		private static readonly ILog log = LogManager.GetLogger(typeof (IDatabaseStructure));
		private static readonly ILog SqlLog = LogManager.GetLogger("NHibernate.SQL");
		private readonly int incrementSize;
		private readonly int initialValue;
		private readonly string tableName;
		private readonly string valueColumnName;
		private int accessCounter;
		private bool applyIncrementSizeToSourceValues;
		private readonly SqlString select;
		private readonly SqlString update;

		public TableStructure(Dialect.Dialect dialect, string tableName, string valueColumnName, int initialValue,
		                      int incrementSize)
		{
			this.tableName = tableName;
			this.valueColumnName = valueColumnName;
			this.initialValue = initialValue;
			this.incrementSize = incrementSize;

			SqlStringBuilder b = new SqlStringBuilder();
			b.Add("select ").Add(valueColumnName).Add(" id_val").Add(" from ").Add(dialect.AppendLockHint(LockMode.Upgrade,
			                                                                                              tableName)).Add(
				dialect.ForUpdateString);
			select = b.ToSqlString();

			b = new SqlStringBuilder();
			b.Add("update ").Add(tableName).Add(" set ").Add(valueColumnName).Add(" = ").Add(Parameter.Placeholder).Add(" where ")
				.Add(valueColumnName).Add(" = ").Add(Parameter.Placeholder);
			update = b.ToSqlString();
		}

		#region Implementation of IDatabaseStructure

		public string Name
		{
			get { return tableName; }
		}

		public int TimesAccessed
		{
			get { return accessCounter; }
		}

		public int IncrementSize
		{
			get { return incrementSize; }
		}

		public virtual IAccessCallback BuildCallback(ISessionImplementor session)
		{
			return new TableAccessCallback(session, this);
		}

		public virtual void Prepare(IOptimizer optimizer)
		{
			applyIncrementSizeToSourceValues = optimizer.ApplyIncrementSizeToSourceValues;
		}

		public virtual string[] SqlCreateStrings(Dialect.Dialect dialect)
		{
			return new String[]
			       	{
			       		"create table " + tableName + " ( " + valueColumnName + " " + dialect.GetTypeName(SqlTypeFactory.Int64)
			       		+ " )", "insert into " + tableName + " values ( " + initialValue + " )"
			       	};
		}

		public virtual string[] SqlDropStrings(Dialect.Dialect dialect)
		{
			StringBuilder sqlDropString = new StringBuilder().Append("drop table ");
			if (dialect.SupportsIfExistsBeforeTableName)
			{
				sqlDropString.Append("if exists ");
			}
			sqlDropString.Append(tableName).Append(dialect.CascadeConstraintsString);
			if (dialect.SupportsIfExistsAfterTableName)
			{
				sqlDropString.Append(" if exists");
			}
			return new String[] {sqlDropString.ToString()};
		}

		#endregion

		#region Overrides of TransactionHelper

		public override object DoWorkInCurrentTransaction(ISessionImplementor session, IDbConnection conn, IDbTransaction transaction)
		{
			long result;
			int rows;
			do
			{
				string query = select.ToString();
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
						string err = "could not read a hi value - you need to populate the table: " + tableName;
						log.Error(err);
						throw new IdentifierGenerationException(err);
					}
					result = rs.GetInt64(0);
					rs.Close();
				}
				catch (Exception sqle)
				{
					log.Error("could not read a hi value", sqle);
					throw;
				}
				finally
				{
					if (rs != null) rs.Close();
					qps.Dispose();
				}

				query = update.ToString();
				
				IDbCommand ups = conn.CreateCommand();
				ups.CommandType = CommandType.Text;
				ups.CommandText = query;
				ups.Connection = conn;
				ups.Transaction = conn.BeginTransaction();
				try
				{
					int increment = applyIncrementSizeToSourceValues ? incrementSize : 1;
					((IDataParameter) ups.Parameters[0]).Value = result + increment;
					((IDataParameter)ups.Parameters[1]).Value = result;
					rows = ups.ExecuteNonQuery();
				}
				catch (Exception sqle)
				{
					log.Error("could not update hi value in: " + tableName, sqle);
					throw;
				}
				finally
				{
					ups.Dispose();
				}
			}
			while (rows == 0);

			accessCounter++;

			return result;
		}

		#endregion

		#region Nested type: TableAccessCallback

		private class TableAccessCallback : IAccessCallback
		{
			private readonly TableStructure owner;
			private readonly ISessionImplementor session;

			public TableAccessCallback(ISessionImplementor session, TableStructure owner)
			{
				this.session = session;
				this.owner = owner;
			}

			#region IAccessCallback Members

			public virtual long NextValue
			{
				get { return Convert.ToInt64(owner.DoWorkInNewTransaction(session)); }
			}

			#endregion
		}

		#endregion


	}
}