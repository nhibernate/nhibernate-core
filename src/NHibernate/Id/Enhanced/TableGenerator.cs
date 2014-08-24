using System;
using System.Data;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;
using NHibernate.SqlCommand;
using NHibernate.AdoNet.Util;

namespace NHibernate.Id.Enhanced
{
	/// <summary>
	/// An enhanced version of table-based id generation.
	/// </summary>
	/// <remarks>
	/// Unlike the simplistic legacy one (which, btw, was only ever intended for subclassing
	/// support) we "segment" the table into multiple values. Thus a single table can
	/// actually serve as the persistent storage for multiple independent generators.  One
	/// approach would be to segment the values by the name of the entity for which we are
	/// performing generation, which would mean that we would have a row in the generator
	/// table for each entity name.  Or any configuration really; the setup is very flexible.
	/// <para>
	/// In this respect it is very similar to the legacy
	/// MultipleHiLoPerTableGenerator (not available in NHibernate) in terms of the
	/// underlying storage structure (namely a single table capable of holding
	/// multiple generator values). The differentiator is, as with
	/// <see cref="SequenceStyleGenerator"/> as well, the externalized notion
	/// of an optimizer.
	/// </para>
	/// <para>
	/// <b>NOTE</b> that by default we use a single row for all generators (based
	/// on <see cref="DefaultSegmentValue"/>).  The configuration parameter
	/// <see cref="ConfigPreferSegmentPerEntity"/> can be used to change that to
	/// instead default to using a row for each entity name.
	/// </para>
	/// Configuration parameters:
	///<table>
	///	 <tr>
	///    <td><b>NAME</b></td>
	///    <td><b>DEFAULT</b></td>
	///    <td><b>DESCRIPTION</b></td>
	///  </tr>
	///  <tr>
	///    <td><see cref="TableParam"/></td>
	///    <td><see cref="DefaultTable"/></td>
	///    <td>The name of the table to use to store/retrieve values</td>
	///  </tr>
	///  <tr>
	///    <td><see cref="ValueColumnParam"/></td>
	///    <td><see cref="DefaultValueColumn"/></td>
	///    <td>The name of column which holds the sequence value for the given segment</td>
	///  </tr>
	///  <tr>
	///    <td><see cref="SegmentColumnParam"/></td>
	///    <td><see cref="DefaultSegmentColumn"/></td>
	///    <td>The name of the column which holds the segment key</td>
	///  </tr>
	///  <tr>
	///    <td><see cref="SegmentValueParam"/></td>
	///    <td><see cref="DefaultSegmentValue"/></td>
	///    <td>The value indicating which segment is used by this generator; refers to values in the <see cref="SegmentColumnParam"/> column</td>
	///  </tr>
	///  <tr>
	///    <td><see cref="SegmentLengthParam"/></td>
	///    <td><see cref="DefaultSegmentLength"/></td>
	///    <td>The data length of the <see cref="SegmentColumnParam"/> column; used for schema creation</td>
	///  </tr>
	///  <tr>
	///    <td><see cref="InitialParam"/></td>
	///    <td><see cref="DefaltInitialValue"/></td>
	///    <td>The initial value to be stored for the given segment</td>
	///  </tr>
	///  <tr>
	///    <td><see cref="IncrementParam"/></td>
	///    <td><see cref="DefaultIncrementSize"/></td>
	///    <td>The increment size for the underlying segment; see the discussion on <see cref="Optimizer"/> for more details.</td>
	///  </tr>
	///  <tr>
	///    <td><see cref="OptimizerParam"/></td>
	///    <td><i>depends on defined increment size</i></td>
	///    <td>Allows explicit definition of which optimization strategy to use</td>
	///  </tr>
	///</table>
	/// </remarks>
	public class TableGenerator : TransactionHelper, IPersistentIdentifierGenerator, IConfigurable
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(SequenceStyleGenerator));


		public const string ConfigPreferSegmentPerEntity = "prefer_entity_table_as_segment_value";

		public const string TableParam = "table_name";
		public const string DefaultTable = "hibernate_sequences";

		public const string ValueColumnParam = "value_column_name";
		public const string DefaultValueColumn = "next_val";

		public const string SegmentColumnParam = "segment_column_name";
		public const string DefaultSegmentColumn = "sequence_name";

		public const string SegmentValueParam = "segment_value";
		public const string DefaultSegmentValue = "default";

		public const string SegmentLengthParam = "segment_value_length";
		public const int DefaultSegmentLength = 255;

		public const string InitialParam = "initial_value";
		public const int DefaltInitialValue = 1;

		public const string IncrementParam = "increment_size";
		public const int DefaultIncrementSize = 1;

		public const string OptimizerParam = "optimizer";


		/// <summary>
		/// Type mapping for the identifier.
		/// </summary>
		public IType IdentifierType { get; private set; }


		/// <summary>
		/// The name of the table in which we store this generator's persistent state.
		/// </summary>
		public string TableName { get; private set; }


		/// <summary>
		/// The name of the column in which we store the segment to which each row
		/// belongs. The value here acts as primary key.
		/// </summary>
		public string SegmentColumnName { get; private set; }


		/// <summary>
		/// The value in the column identified by <see cref="SegmentColumnName"/> which
		/// corresponds to this generator instance.  In other words, this value
		/// indicates the row in which this generator instance will store values.
		/// </summary>
		public string SegmentValue { get; private set; }


		/// <summary>
		/// The size of the column identified by <see cref="SegmentColumnName"/>
		/// in the underlying table.
		/// </summary>
		/// <remarks>
		/// Should really have been called 'segmentColumnLength' or even better 'segmentColumnSize'.
		/// </remarks>
		public int SegmentValueLength { get; private set; }


		/// <summary>
		/// The name of the column in which we store our persistent generator value.
		/// </summary>
		public string ValueColumnName { get; private set; }


		/// <summary>
		/// The initial value to use when we find no previous state in the
		/// generator table corresponding to this instance.
		/// </summary>
		public int InitialValue { get; private set; }


		/// <summary>
		/// The amount of increment to use.  The exact implications of this
		/// depends on the optimizer being used, see <see cref="Optimizer"/>.
		/// </summary>
		public int IncrementSize { get; private set; }


		/// <summary>
		/// The optimizer being used by this generator. This mechanism
		/// allows avoiding calling the database each time a new identifier
		/// is needed.
		/// </summary>
		public IOptimizer Optimizer { get; private set; }


		/// <summary>
		/// The table access count. Only really useful for unit test assertions.
		/// </summary>
		public long TableAccessCount { get; private set; }


		private SqlString selectQuery;
		private SqlTypes.SqlType[] selectParameterTypes;
		private SqlString insertQuery;
		private SqlTypes.SqlType[] insertParameterTypes;
		private SqlString updateQuery;
		private SqlTypes.SqlType[] updateParameterTypes;


		public virtual string GeneratorKey()
		{
			return TableName;
		}


		#region Implementation of IConfigurable

		public virtual void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			IdentifierType = type;

			TableName = DetermineGeneratorTableName(parms, dialect);
			SegmentColumnName = DetermineSegmentColumnName(parms, dialect);
			ValueColumnName = DetermineValueColumnName(parms, dialect);

			SegmentValue = DetermineSegmentValue(parms);

			SegmentValueLength = DetermineSegmentColumnSize(parms);
			InitialValue = DetermineInitialValue(parms);
			IncrementSize = DetermineIncrementSize(parms);

			BuildSelectQuery(dialect);
			BuildUpdateQuery();
			BuildInsertQuery();


			// if the increment size is greater than one, we prefer pooled optimization; but we
			// need to see if the user prefers POOL or POOL_LO...
			string defaultPooledOptimizerStrategy = PropertiesHelper.GetBoolean(Cfg.Environment.PreferPooledValuesLo, parms, false)
				? OptimizerFactory.PoolLo
				: OptimizerFactory.Pool;

			string defaultOptimizerStrategy = IncrementSize <= 1 ? OptimizerFactory.None : defaultPooledOptimizerStrategy;
			string optimizationStrategy = PropertiesHelper.GetString(OptimizerParam, parms, defaultOptimizerStrategy);
			Optimizer = OptimizerFactory.BuildOptimizer(
					optimizationStrategy,
					IdentifierType.ReturnedClass,
					IncrementSize,
					PropertiesHelper.GetInt32(InitialParam, parms, -1)  // Use -1 as default initial value here to signal that it's not set.
				);
		}

		#endregion


		/// <summary>
		///  Determine the table name to use for the generator values. Called during configuration.
		/// </summary>
		/// <param name="parms">The parameters supplied in the generator config (plus some standard useful extras).</param>
		protected string DetermineGeneratorTableName(IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			string name = PropertiesHelper.GetString(TableParam, parms, DefaultTable);
			bool isGivenNameUnqualified = name.IndexOf('.') < 0;
			if (isGivenNameUnqualified)
			{
				// NHibernate doesn't seem to have anything resembling this ObjectNameNormalizer. Ignore that for now.
				//ObjectNameNormalizer normalizer = ( ObjectNameNormalizer ) params.get( IDENTIFIER_NORMALIZER );
				//name = normalizer.normalizeIdentifierQuoting( name );
				//// if the given name is un-qualified we may neen to qualify it
				//string schemaName = normalizer.normalizeIdentifierQuoting( params.getProperty( SCHEMA ) );
				//string catalogName = normalizer.normalizeIdentifierQuoting( params.getProperty( CATALOG ) );

				string schemaName;
				string catalogName;
				parms.TryGetValue(PersistentIdGeneratorParmsNames.Schema, out schemaName);
				parms.TryGetValue(PersistentIdGeneratorParmsNames.Catalog, out catalogName);
				name = dialect.Qualify(catalogName, schemaName, name);
			}
			else
			{
				// if already qualified there is not much we can do in a portable manner so we pass it
				// through and assume the user has set up the name correctly.
			}
			return name;
		}

		/// <summary>
		/// Determine the name of the column used to indicate the segment for each
		/// row.  This column acts as the primary key.
		/// Called during configuration.
		/// </summary>
		/// <param name="parms">The parameters supplied in the generator config (plus some standard useful extras).</param>
		protected string DetermineSegmentColumnName(IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			// NHibernate doesn't seem to have anything resembling this ObjectNameNormalizer. Ignore that for now.
			//ObjectNameNormalizer normalizer = ( ObjectNameNormalizer ) params.get( IDENTIFIER_NORMALIZER );
			string name = PropertiesHelper.GetString(SegmentColumnParam, parms, DefaultSegmentColumn);
			return dialect.QuoteForColumnName(name);
			//return dialect.quote( normalizer.normalizeIdentifierQuoting( name ) );
		}


		/// <summary>
		/// Determine the name of the column in which we will store the generator persistent value.
		/// Called during configuration.
		/// </summary>
		protected string DetermineValueColumnName(IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			// NHibernate doesn't seem to have anything resembling this ObjectNameNormalizer. Ignore that for now.
			//ObjectNameNormalizer normalizer = ( ObjectNameNormalizer ) params.get( IDENTIFIER_NORMALIZER );
			string name = PropertiesHelper.GetString(ValueColumnParam, parms, DefaultValueColumn);
			//return dialect.quote( normalizer.normalizeIdentifierQuoting( name ) );

			return dialect.QuoteForColumnName(name);
		}


		/// <summary>
		/// Determine the segment value corresponding to this generator instance. Called during configuration.
		/// </summary>
		protected string DetermineSegmentValue(IDictionary<string, string> parms)
		{
			string segmentValue = PropertiesHelper.GetString(SegmentValueParam, parms, "");
			if (string.IsNullOrEmpty(segmentValue))
				segmentValue = DetermineDefaultSegmentValue(parms);
			return segmentValue;
		}


		/// <summary>
		/// Used in the cases where <see cref="DetermineSegmentValue"/> is unable to
		/// determine the value to use.
		/// </summary>
		protected string DetermineDefaultSegmentValue(IDictionary<string, string> parms)
		{
			bool preferSegmentPerEntity = PropertiesHelper.GetBoolean(ConfigPreferSegmentPerEntity, parms, false);
			string defaultToUse = preferSegmentPerEntity ? parms[PersistentIdGeneratorParmsNames.Table] : DefaultSegmentValue;

			log.DebugFormat("Explicit segment value for id generator [{0}.{1}] suggested; using default [{2}].", TableName, SegmentColumnName, defaultToUse);

			return defaultToUse;
		}


		/// <summary>
		/// Determine the size of the <see cref="SegmentColumnName"/> segment column.
		/// Called during configuration.
		/// </summary>
		protected int DetermineSegmentColumnSize(IDictionary<string, string> parms)
		{
			return PropertiesHelper.GetInt32(SegmentLengthParam, parms, DefaultSegmentLength);
		}


		protected int DetermineInitialValue(IDictionary<string, string> parms)
		{
			return PropertiesHelper.GetInt32(InitialParam, parms, DefaltInitialValue);
		}


		protected int DetermineIncrementSize(IDictionary<string, string> parms)
		{
			return PropertiesHelper.GetInt32(IncrementParam, parms, DefaultIncrementSize);
		}


		protected void BuildSelectQuery(Dialect.Dialect dialect)
		{
			const string alias = "tbl";
			SqlString select = new SqlString(
				"select ", StringHelper.Qualify(alias, ValueColumnName), 
				" from ", TableName, " ", alias,
				" where ", StringHelper.Qualify(alias, SegmentColumnName), " = ", Parameter.Placeholder, 
				"  ");

			Dictionary<string, LockMode> lockOptions = new Dictionary<string, LockMode>();
			lockOptions[alias] = LockMode.Upgrade;

			Dictionary<string, string[]> updateTargetColumnsMap = new Dictionary<string, string[]>();
			updateTargetColumnsMap[alias] = new[] { ValueColumnName };

			selectQuery = dialect.ApplyLocksToSql(select, lockOptions, updateTargetColumnsMap);

			selectParameterTypes = new[] { SqlTypes.SqlTypeFactory.GetAnsiString(SegmentValueLength) };
		}


		protected void BuildUpdateQuery()
		{
			updateQuery = new SqlString(
				"update ", TableName,
				" set ", ValueColumnName, " = ", Parameter.Placeholder,
				" where ", ValueColumnName, " = ", Parameter.Placeholder,
				" and ", SegmentColumnName, " = ", Parameter.Placeholder);
			updateParameterTypes = new[]
			{
				SqlTypes.SqlTypeFactory.Int64,
				SqlTypes.SqlTypeFactory.Int64,
				SqlTypes.SqlTypeFactory.GetAnsiString(SegmentValueLength)
			};
		}


		protected void BuildInsertQuery()
		{
			insertQuery = new SqlString(
				"insert into ", TableName,
				" (", SegmentColumnName, ", ", ValueColumnName, ") ",
				" values (", Parameter.Placeholder, ", ", Parameter.Placeholder, ")");
			insertParameterTypes = new[] {
				SqlTypes.SqlTypeFactory.GetAnsiString(SegmentValueLength),
				SqlTypes.SqlTypeFactory.Int64
			};
		}


		[MethodImpl(MethodImplOptions.Synchronized)]
		public virtual object Generate(ISessionImplementor session, object obj)
		{
			return Optimizer.Generate(new TableAccessCallback(session, this));
		}


		private class TableAccessCallback : IAccessCallback
		{
			private TableGenerator owner;
			private readonly ISessionImplementor session;

			public TableAccessCallback(ISessionImplementor session, TableGenerator owner)
			{
				this.session = session;
				this.owner = owner;
			}

			#region IAccessCallback Members

			public long GetNextValue()
			{
				return Convert.ToInt64(owner.DoWorkInNewTransaction(session));
			}

			#endregion
		}


		public override object DoWorkInCurrentTransaction(ISessionImplementor session, System.Data.IDbConnection conn, System.Data.IDbTransaction transaction)
		{
			long result;
			int updatedRows;

			do
			{
				object selectedValue;

				try
				{
					IDbCommand selectCmd = session.Factory.ConnectionProvider.Driver.GenerateCommand(CommandType.Text, selectQuery, selectParameterTypes);
					using (selectCmd)
					{
						selectCmd.Connection = conn;
						selectCmd.Transaction = transaction;
						string s = selectCmd.CommandText;
						((IDataParameter)selectCmd.Parameters[0]).Value = SegmentValue;
						PersistentIdGeneratorParmsNames.SqlStatementLogger.LogCommand(selectCmd, FormatStyle.Basic);

						selectedValue = selectCmd.ExecuteScalar();
					}

					if (selectedValue == null)
					{
						result = InitialValue;

						IDbCommand insertCmd = session.Factory.ConnectionProvider.Driver.GenerateCommand(CommandType.Text, insertQuery, insertParameterTypes);
						using (insertCmd)
						{
							insertCmd.Connection = conn;
							insertCmd.Transaction = transaction;

							((IDataParameter)insertCmd.Parameters[0]).Value = SegmentValue;
							((IDataParameter)insertCmd.Parameters[1]).Value = result;

							PersistentIdGeneratorParmsNames.SqlStatementLogger.LogCommand(insertCmd, FormatStyle.Basic);
							insertCmd.ExecuteNonQuery();
						}
					}
					else
					{
						result = Convert.ToInt64(selectedValue);
					}
				}
				catch (Exception ex)
				{
					log.Error("Unable to read or initialize hi value in " + TableName, ex);
					throw;
				}


				try
				{
					IDbCommand updateCmd = session.Factory.ConnectionProvider.Driver.GenerateCommand(CommandType.Text, updateQuery, updateParameterTypes);
					using (updateCmd)
					{
						updateCmd.Connection = conn;
						updateCmd.Transaction = transaction;

						int increment = Optimizer.ApplyIncrementSizeToSourceValues ? IncrementSize : 1;
						((IDataParameter)updateCmd.Parameters[0]).Value = result + increment;
						((IDataParameter)updateCmd.Parameters[1]).Value = result;
						((IDataParameter)updateCmd.Parameters[2]).Value = SegmentValue;
						PersistentIdGeneratorParmsNames.SqlStatementLogger.LogCommand(updateCmd, FormatStyle.Basic);
						updatedRows = updateCmd.ExecuteNonQuery();
					}
				}
				catch (Exception ex)
				{
					log.Error("Unable to update hi value in " + TableName, ex);
					throw;
				}
			}
			while (updatedRows == 0);

			TableAccessCount++;

			return result;
		}


		public virtual string[] SqlCreateStrings(Dialect.Dialect dialect)
		{
			string createString = dialect.CreateTableString + " " + TableName
				+ " ( "
				+ SegmentColumnName + " " + dialect.GetTypeName(SqlTypes.SqlTypeFactory.GetAnsiString(SegmentValueLength)) + " not null, "
				+ ValueColumnName + " " + dialect.GetTypeName(SqlTypes.SqlTypeFactory.Int64) + ", "
				+ dialect.PrimaryKeyString + " ( " + SegmentColumnName + ") "
				+ ")";

			return new string[] { createString };
		}


		public virtual string[] SqlDropString(Dialect.Dialect dialect)
		{
			return new[] { dialect.GetDropTableString(TableName) };
		}
	}
}
