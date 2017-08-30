using System.Collections.Generic;

using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Id.Enhanced
{
	/// <summary>
	/// Generates identifier values based on an sequence-style database structure.
	/// Variations range from actually using a sequence to using a table to mimic
	/// a sequence. These variations are encapsulated by the <see cref="IDatabaseStructure"/>
	/// interface internally.
	/// </summary>
	/// <remarks>
	/// General configuration parameters:
	/// <table>
	///   <tr>
	///     <td><b>NAME</b></td>
	///     <td><b>DEFAULT</b></td>
	///     <td><b>DESCRIPTION</b></td>
	///   </tr>
	///   <tr>
	///     <td><see cref="SequenceParam"/></td>
	///     <td><see cref="DefaultSequenceName"/></td>
	///     <td>The name of the sequence/table to use to store/retrieve values</td>
	///   </tr>
	///   <tr>
	///     <td><see cref="InitialParam"/></td>
	///     <td><see cref="DefaultInitialValue"/></td>
	///     <td>The initial value to be stored for the given segment; the effect in terms of storage varies based on <see cref="Optimizer"/> and <see cref="DatabaseStructure"/></td>
	///   </tr>
	///   <tr>
	///     <td><see cref="IncrementParam"/></td>
	///     <td><see cref="DefaultIncrementSize"/></td>
	///     <td>The increment size for the underlying segment; the effect in terms of storage varies based on <see cref="Optimizer"/> and <see cref="DatabaseStructure"/></td>
	///   </tr>
	///   <tr>
	///     <td><see cref="OptimizerParam"/></td>
	///     <td><i>depends on defined increment size</i></td>
	///     <td>Allows explicit definition of which optimization strategy to use</td>
	///   </tr>
	///   <tr>
	///     <td><see cref="ForceTableParam"/></td>
	///     <td><b><i>false</i></b></td>
	///     <td>Allows explicit definition of which optimization strategy to use</td>
	///   </tr>
	/// </table>
	/// <p/>
	/// Configuration parameters used specifically when the underlying structure is a table:
	/// <table>
	///   <tr>
	///     <td><b>NAME</b></td>
	///     <td><b>DEFAULT</b></td>
	///     <td><b>DESCRIPTION</b></td>
	///   </tr>
	///   <tr>
	///     <td><see cref="ValueColumnParam"/></td>
	///     <td><see cref="DefaultValueColumnName"/></td>
	///     <td>The name of column which holds the sequence value for the given segment</td>
	///   </tr>
	/// </table>
	/// </remarks>
	public partial class SequenceStyleGenerator : IPersistentIdentifierGenerator, IConfigurable
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(SequenceStyleGenerator));

		public const string DefaultSequenceName = "hibernate_sequence";
		public const int DefaultInitialValue = 1;
		public const int DefaultIncrementSize = 1;

		public const string SequenceParam = "sequence_name";
		public const string InitialParam = "initial_value";
		public const string IncrementParam = "increment_size";
		public const string OptimizerParam = "optimizer";
		public const string ForceTableParam = "force_table_use";
		public const string ValueColumnParam = "value_column";
		public const string DefaultValueColumnName = "next_val";

		public IDatabaseStructure DatabaseStructure { get; private set; }
		public IOptimizer Optimizer { get; private set; }
		public IType IdentifierType { get; private set; }

		#region Implementation of IConfigurable

		public virtual void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			IdentifierType = type;

			bool forceTableUse = PropertiesHelper.GetBoolean(ForceTableParam, parms, false);

			string sequenceName = DetermineSequenceName(parms, dialect);

			int initialValue = DetermineInitialValue(parms);
			int incrementSize = DetermineIncrementSize(parms);

			string optimizationStrategy = DetermineOptimizationStrategy(parms, incrementSize);
			incrementSize = DetermineAdjustedIncrementSize(optimizationStrategy, incrementSize);

			Optimizer = OptimizerFactory.BuildOptimizer(
				optimizationStrategy,
				IdentifierType.ReturnedClass,
				incrementSize,
				PropertiesHelper.GetInt32(InitialParam, parms, -1)); // Use -1 as default initial value here to signal that it's not set.

			if (!forceTableUse && RequiresPooledSequence(initialValue, incrementSize, Optimizer) && !dialect.SupportsPooledSequences)
			{
				// force the use of a table (overriding whatever the user configured) since the dialect
				// doesn't support the sequence features we need.
				forceTableUse = true;
				Log.Info("Forcing table use for sequence-style generator due to optimizer selection where db does not support pooled sequences.");
			}

			DatabaseStructure = BuildDatabaseStructure(type, parms, dialect, forceTableUse, sequenceName, initialValue, incrementSize);
			DatabaseStructure.Prepare(Optimizer);
		}

		/// <summary>
		/// Determine the name of the sequence (or table if this resolves to a physical table) to use.
		/// Called during configuration.
		/// </summary>
		/// <param name="parms"></param>
		/// <param name="dialect"></param>
		/// <returns></returns>
		protected string DetermineSequenceName(IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			string sequenceName = PropertiesHelper.GetString(SequenceParam, parms, DefaultSequenceName);
			if (sequenceName.IndexOf('.') < 0)
			{
				string schemaName;
				string catalogName;
				parms.TryGetValue(PersistentIdGeneratorParmsNames.Schema, out schemaName);
				parms.TryGetValue(PersistentIdGeneratorParmsNames.Catalog, out catalogName);
				sequenceName = dialect.Qualify(catalogName, schemaName, sequenceName);
			}
			else
			{
				// If already qualified there is not much we can do in a portable manner so we pass it
				// through and assume the user has set up the name correctly.
			}

			return sequenceName;
		}

		/// <summary>
		/// Determine the name of the column used to store the generator value in
		/// the db. Called during configuration, if a physical table is being used.
		/// </summary>
		protected string DetermineValueColumnName(IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			return PropertiesHelper.GetString(ValueColumnParam, parms, DefaultValueColumnName);
		}

		/// <summary>
		/// Determine the initial sequence value to use. This value is used when
		/// initializing the database structure (i.e. sequence/table). Called
		/// during configuration.
		/// </summary>
		protected int DetermineInitialValue(IDictionary<string, string> parms)
		{
			return PropertiesHelper.GetInt32(InitialParam, parms, DefaultInitialValue);
		}

		/// <summary>
		/// Determine the increment size to be applied. The exact implications of
		/// this value depends on the optimizer being used. Called during configuration.
		/// </summary>
		protected int DetermineIncrementSize(IDictionary<string, string> parms)
		{
			return PropertiesHelper.GetInt32(IncrementParam, parms, DefaultIncrementSize);
		}

		/// <summary>
		/// Determine the optimizer to use. Called during configuration.
		/// </summary>
		protected string DetermineOptimizationStrategy(IDictionary<string, string> parms, int incrementSize)
		{
			// If the increment size is greater than one, we prefer pooled optimization; but we
			// need to see if the user prefers POOL or POOL_LO...
			string defaultPooledOptimizerStrategy = PropertiesHelper.GetBoolean(Cfg.Environment.PreferPooledValuesLo, parms, false)
				? OptimizerFactory.PoolLo
				: OptimizerFactory.Pool;
			string defaultOptimizerStrategy = incrementSize <= 1 ? OptimizerFactory.None : defaultPooledOptimizerStrategy;
			return PropertiesHelper.GetString(OptimizerParam, parms, defaultOptimizerStrategy);
		}

		/// <summary>
		/// In certain cases we need to adjust the increment size based on the
		/// selected optimizer. This is the hook to achieve that.
		/// </summary>
		/// <param name="optimizationStrategy">The determined optimizer strategy.</param>
		/// <param name="incrementSize">The determined, unadjusted, increment size.</param>
		protected int DetermineAdjustedIncrementSize(string optimizationStrategy, int incrementSize)
		{
			if (OptimizerFactory.None.Equals(optimizationStrategy) && incrementSize > 1)
			{
				Log.Warn("config specified explicit optimizer of [" + OptimizerFactory.None + "], but [" + IncrementParam + "=" + incrementSize + "; honoring optimizer setting");
				incrementSize = 1;
			}
			return incrementSize;
		}

		protected IDatabaseStructure BuildDatabaseStructure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect, bool forceTableUse, string sequenceName, int initialValue, int incrementSize)
		{
			bool useSequence = dialect.SupportsSequences && !forceTableUse;
			if (useSequence)
				return new SequenceStructure(dialect, sequenceName, initialValue, incrementSize);
			else
			{
				string valueColumnName = DetermineValueColumnName(parms, dialect);
				return new TableStructure(dialect, sequenceName, valueColumnName, initialValue, incrementSize);
			}
		}

		/// <summary>
		/// Do we require a sequence with the ability to set initialValue and incrementSize
		/// larger than 1?
		/// </summary>
		protected bool RequiresPooledSequence(int initialValue, int incrementSize, IOptimizer optimizer)
		{
			int sourceIncrementSize = optimizer.ApplyIncrementSizeToSourceValues ? incrementSize : 1;
			return (initialValue > 1 || sourceIncrementSize > 1);
		}


		#endregion

		#region Implementation of IIdentifierGenerator

		public virtual object Generate(ISessionImplementor session, object obj)
		{
			return Optimizer.Generate(DatabaseStructure.BuildCallback(session));
		}

		#endregion

		#region Implementation of IPersistentIdentifierGenerator

		public virtual string GeneratorKey()
		{
			return DatabaseStructure.Name;
		}

		public virtual string[] SqlCreateStrings(Dialect.Dialect dialect)
		{
			return DatabaseStructure.SqlCreateStrings(dialect);
		}

		public virtual string[] SqlDropString(Dialect.Dialect dialect)
		{
			return DatabaseStructure.SqlDropStrings(dialect);
		}

		#endregion
	}
}