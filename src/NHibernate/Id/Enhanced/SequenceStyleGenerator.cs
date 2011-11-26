using System.Collections.Generic;

using NHibernate.Engine;
using NHibernate.Mapping;
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
	///     <td><see cref="ForceTableParam"/></td>
	///     <td><b><i>false<i/></b></td>
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
	public class SequenceStyleGenerator : IPersistentIdentifierGenerator, IConfigurable
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


		#region Implementation of IIdentifierGenerator

		public virtual object Generate(ISessionImplementor session, object obj)
		{
			return Optimizer.Generate(DatabaseStructure.BuildCallback(session));
		}

		#endregion

		#region Implementation of IPersistentIdentifierGenerator

		public virtual string[] SqlCreateStrings(Dialect.Dialect dialect)
		{
			return DatabaseStructure.SqlCreateStrings(dialect);
		}

		public virtual string[] SqlDropString(Dialect.Dialect dialect)
		{
			return DatabaseStructure.SqlDropStrings(dialect);
		}

		public virtual string GeneratorKey()
		{
			return DatabaseStructure.Name;
		}

		#endregion

		#region Implementation of IConfigurable

		public virtual void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			IdentifierType = type;

			bool forceTableUse = PropertiesHelper.GetBoolean(ForceTableParam, parms, false);
			string sequenceName = PropertiesHelper.GetString(SequenceParam, parms, DefaultSequenceName);

			if (sequenceName.IndexOf('.') < 0)
			{
				string schemaName;
				string catalogName;
				parms.TryGetValue(PersistentIdGeneratorParmsNames.Schema, out schemaName);
				parms.TryGetValue(PersistentIdGeneratorParmsNames.Catalog, out catalogName);
				sequenceName = Table.Qualify(catalogName, schemaName, sequenceName);
			}

			int initialValue = PropertiesHelper.GetInt32(InitialParam, parms, DefaultInitialValue);
			int incrementSize = PropertiesHelper.GetInt32(IncrementParam, parms, DefaultIncrementSize);
			string valueColumnName = PropertiesHelper.GetString(ValueColumnParam, parms, DefaultValueColumnName);

			string defOptStrategy = incrementSize <= 1 ? OptimizerFactory.None : OptimizerFactory.Pool;
			string optimizationStrategy = PropertiesHelper.GetString(OptimizerParam, parms, defOptStrategy);

			if (OptimizerFactory.None.Equals(optimizationStrategy) && incrementSize > 1)
			{
				Log.Warn("config specified explicit optimizer of [" + OptimizerFactory.None + "], but [" + IncrementParam + "=" + incrementSize + "; honoring optimizer setting");
				incrementSize = 1;
			}

			if (dialect.SupportsSequences && !forceTableUse)
			{
				if (OptimizerFactory.Pool.Equals(optimizationStrategy) && !dialect.SupportsPooledSequences)
				{
					// TODO : may even be better to fall back to a pooled table strategy here so that the db stored values remain consistent...
					optimizationStrategy = OptimizerFactory.HiLo;
				}
				DatabaseStructure = new SequenceStructure(dialect, sequenceName, initialValue, incrementSize);
			}
			else
			{
				DatabaseStructure = new TableStructure(dialect, sequenceName, valueColumnName, initialValue, incrementSize);
			}

			Optimizer = OptimizerFactory.BuildOptimizer(
				optimizationStrategy,
				IdentifierType.ReturnedClass,
				incrementSize,
				PropertiesHelper.GetInt32(InitialParam, parms, -1)); // Use -1 as default initial value here to signal that it's not set.

			DatabaseStructure.Prepare(Optimizer);
		}

		#endregion
	}
}