using System;
using System.Collections;
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
			return new BatcherDataReaderWrapper(batcher, command).Initialize();
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
		private DbDataReader reader;

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
		}

		public BatcherDataReaderWrapper Initialize()
		{
			reader = batcher.ExecuteReader(command);
			return this;
		}

		public override string GetName(int i)
		{
			return reader.GetName(i);
		}

		public override string GetDataTypeName(int i)
		{
			return reader.GetDataTypeName(i);
		}

		public override IEnumerator GetEnumerator()
		{
			return reader.GetEnumerator();
		}

		public override System.Type GetFieldType(int i)
		{
			return reader.GetFieldType(i);
		}

		public override object GetValue(int i)
		{
			return reader.GetValue(i);
		}

		public override int GetValues(object[] values)
		{
			return reader.GetValues(values);
		}

		public override int GetOrdinal(string name)
		{
			return reader.GetOrdinal(name);
		}

		public override bool GetBoolean(int i)
		{
			return reader.GetBoolean(i);
		}

		public override byte GetByte(int i)
		{
			return reader.GetByte(i);
		}

		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		public override char GetChar(int i)
		{
			return reader.GetChar(i);
		}

		public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		public override Guid GetGuid(int i)
		{
			return reader.GetGuid(i);
		}

		public override short GetInt16(int i)
		{
			return reader.GetInt16(i);
		}

		public override int GetInt32(int i)
		{
			return reader.GetInt32(i);
		}

		public override long GetInt64(int i)
		{
			return reader.GetInt64(i);
		}

		public override float GetFloat(int i)
		{
			return reader.GetFloat(i);
		}

		public override double GetDouble(int i)
		{
			return reader.GetDouble(i);
		}

		public override string GetString(int i)
		{
			return reader.GetString(i);
		}

		public override decimal GetDecimal(int i)
		{
			return reader.GetDecimal(i);
		}

		public override DateTime GetDateTime(int i)
		{
			return reader.GetDateTime(i);
		}

		protected override DbDataReader GetDbDataReader(int ordinal)
		{
			return reader.GetData(ordinal);
		}

		public override bool IsDBNull(int i)
		{
			return reader.IsDBNull(i);
		}

		public override int FieldCount
		{
			get { return reader.FieldCount; }
		}

		public override bool HasRows
		{
			get { return reader.HasRows; }
		}

		public override object this[int i]
		{
			get { return reader[i]; }
		}

		public override object this[string name]
		{
			get { return reader[name]; }
		}

		public override bool Equals(object obj)
		{
			return reader.Equals(obj);
		}

		public override int GetHashCode()
		{
			return reader.GetHashCode();
		}

		public override void Close()
		{
			batcher.CloseCommand(command, reader);
		}

		public override DataTable GetSchemaTable()
		{
			return reader.GetSchemaTable();
		}

		public override bool NextResult()
		{
			return reader.NextResult();
		}

		public override bool Read()
		{
			return reader.Read();
		}

		public override int Depth
		{
			get { return reader.Depth; }
		}

		public override bool IsClosed
		{
			get { return reader.IsClosed; }
		}

		public override int RecordsAffected
		{
			get { return reader.RecordsAffected; }
		}
	}
}