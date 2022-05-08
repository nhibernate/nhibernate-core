using System.Data;
using System.Data.Common;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// A <see cref="DbCommand"/> wrapper that implements the required members.
	/// </summary>
	internal partial class DbCommandWrapper : DbCommand
	{
		public DbCommandWrapper(DbCommand command)
		{
			Command = command;
		}

		/// <summary>
		/// The wrapped command.
		/// </summary>
		public DbCommand Command { get; }

		/// <inheritdoc />
		public override void Cancel()
		{
			Command.Cancel();
		}

		/// <inheritdoc />
		public override int ExecuteNonQuery()
		{
			return Command.ExecuteNonQuery();
		}

		/// <inheritdoc />
		public override object ExecuteScalar()
		{
			return Command.ExecuteScalar();
		}

		/// <inheritdoc />
		public override void Prepare()
		{
			Command.Prepare();
		}

		/// <inheritdoc />
		public override string CommandText
		{
			get => Command.CommandText;
			set => Command.CommandText = value;
		}

		/// <inheritdoc />
		public override int CommandTimeout
		{
			get => Command.CommandTimeout;
			set => Command.CommandTimeout = value;
		}

		/// <inheritdoc />
		public override CommandType CommandType
		{
			get => Command.CommandType;
			set => Command.CommandType = value;
		}

		/// <inheritdoc />
		public override UpdateRowSource UpdatedRowSource
		{
			get => Command.UpdatedRowSource;
			set => Command.UpdatedRowSource = value;
		}

		/// <inheritdoc />
		protected override DbConnection DbConnection
		{
			get => Command.Connection;
			set => Command.Connection = value;
		}

		/// <inheritdoc />
		protected override DbParameterCollection DbParameterCollection => Command.Parameters;

		/// <inheritdoc />
		protected override DbTransaction DbTransaction
		{
			get => Command.Transaction;
			set => Command.Transaction = value;
		}

		/// <inheritdoc />
		public override bool DesignTimeVisible
		{
			get => Command.DesignTimeVisible;
			set => Command.DesignTimeVisible = value;
		}

		/// <inheritdoc />
		protected override DbParameter CreateDbParameter()
		{
			return Command.CreateParameter();
		}

		/// <inheritdoc />
		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return Command.ExecuteReader(behavior);
		}
	}
}
