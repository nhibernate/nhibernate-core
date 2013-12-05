using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Driver
{
	public class BasicResultSetsCommand: IResultSetsCommand
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(BasicResultSetsCommand));
		private SqlString sqlString = SqlString.Empty;

		public BasicResultSetsCommand(ISessionImplementor session)
		{
			Commands = new List<ISqlCommand>();
			Session = session;
		}

		protected List<ISqlCommand> Commands { get; private set; }

		protected ISessionImplementor Session { get; private set; }

		public virtual void Append(ISqlCommand command)
		{
			Commands.Add(command);
			sqlString = sqlString.Append(command.Query).Append(";").Append(Environment.NewLine);
		}

		public bool HasQueries
		{
			get { return Commands.Count > 0; }
		}

		public virtual SqlString Sql
		{
			get { return sqlString; }
		}

		public virtual DbDataReader GetReader(int? commandTimeout)
		{
			var batcher = Session.Batcher;
			SqlType[] sqlTypes = Commands.SelectMany(c => c.ParameterTypes).ToArray();
			ForEachSqlCommand((sqlLoaderCommand, offset) => sqlLoaderCommand.ResetParametersIndexesForTheCommand(offset));
			var command = batcher.PrepareQueryCommand(CommandType.Text, sqlString, sqlTypes);
			if (commandTimeout.HasValue)
			{
				command.CommandTimeout = commandTimeout.Value;
			}
			log.Info(command.CommandText);
			BindParameters(command);
			return new BatcherDataReaderWrapper(batcher, command);
		}

		protected virtual void BindParameters(DbCommand command)
		{
			var wholeQueryParametersList = Sql.GetParameters().ToList();
			ForEachSqlCommand((sqlLoaderCommand, offset) => sqlLoaderCommand.Bind(command, wholeQueryParametersList, offset, Session));
		}

		/// <summary>
		/// Execute the given <paramref name="actionToDo"/> for each command of the resultset.
		/// </summary>
		/// <param name="actionToDo">The action to perform where the first parameter is the <see cref="ISqlCommand"/> and the second parameter is the parameters offset of the <see cref="ISqlCommand"/>.</param>
		protected void ForEachSqlCommand(Action<ISqlCommand, int> actionToDo)
		{
			var singleQueryParameterOffset = 0;
			foreach (var sqlLoaderCommand in Commands)
			{
				actionToDo(sqlLoaderCommand, singleQueryParameterOffset);
				singleQueryParameterOffset += sqlLoaderCommand.ParameterTypes.Length;
			}
		}
	}

	/// <summary>
	/// Datareader wrapper with the same life cycle of its command (through the batcher)
	/// </summary>
	public class BatcherDataReaderWrapper: DbDataReader
	{
		private readonly IBatcher batcher;
		private readonly DbCommand command;
		private readonly DbDataReader reader;

		public BatcherDataReaderWrapper(IBatcher batcher, DbCommand command)
		{
			if (batcher == null)
			{
				throw new ArgumentNullException("batcher");
			}
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			this.batcher = batcher;
			this.command = command;
			reader = batcher.ExecuteReader(command);
		}

		public void Dispose()
		{
			batcher.CloseCommand(command, reader);
		}

		#region IDataRecord Members

		public string GetName(int i)
		{
			return reader.GetName(i);
		}

		public string GetDataTypeName(int i)
		{
			return reader.GetDataTypeName(i);
		}

		public System.Type GetFieldType(int i)
		{
			return reader.GetFieldType(i);
		}

		public object GetValue(int i)
		{
			return reader.GetValue(i);
		}

		public int GetValues(object[] values)
		{
			return reader.GetValues(values);
		}

		public int GetOrdinal(string name)
		{
			return reader.GetOrdinal(name);
		}

		public bool GetBoolean(int i)
		{
			return reader.GetBoolean(i);
		}

		public byte GetByte(int i)
		{
			return reader.GetByte(i);
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		public char GetChar(int i)
		{
			return reader.GetChar(i);
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		public Guid GetGuid(int i)
		{
			return reader.GetGuid(i);
		}

		public short GetInt16(int i)
		{
			return reader.GetInt16(i);
		}

		public int GetInt32(int i)
		{
			return reader.GetInt32(i);
		}

		public long GetInt64(int i)
		{
			return reader.GetInt64(i);
		}

		public float GetFloat(int i)
		{
			return reader.GetFloat(i);
		}

		public double GetDouble(int i)
		{
			return reader.GetDouble(i);
		}

		public string GetString(int i)
		{
			return reader.GetString(i);
		}

		public decimal GetDecimal(int i)
		{
			return reader.GetDecimal(i);
		}

		public DateTime GetDateTime(int i)
		{
			return reader.GetDateTime(i);
		}

		public DbDataReader GetData(int i)
		{
			return reader.GetData(i);
		}

		public bool IsDBNull(int i)
		{
			return reader.IsDBNull(i);
		}

		public int FieldCount
		{
			get { return reader.FieldCount; }
		}

		public object this[int i]
		{
			get { return reader[i]; }
		}

		public object this[string name]
		{
			get { return reader[name]; }
		}

		#endregion

		public override bool Equals(object obj)
		{
			return reader.Equals(obj);
		}

		public override int GetHashCode()
		{
			return reader.GetHashCode();
		}

		public void Close()
		{
			batcher.CloseCommand(command, reader);
		}

		public DataTable GetSchemaTable()
		{
			return reader.GetSchemaTable();
		}

		public bool NextResult()
		{
			return reader.NextResult();
		}

		public bool Read()
		{
			return reader.Read();
		}

		public int Depth
		{
			get { return reader.Depth; }
		}

		public bool IsClosed
		{
			get { return reader.IsClosed; }
		}

		public int RecordsAffected
		{
			get { return reader.RecordsAffected; }
		}
	}
}