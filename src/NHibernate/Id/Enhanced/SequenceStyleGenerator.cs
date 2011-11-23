using System.Collections.Generic;

using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Id.Enhanced
{
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

		private IDatabaseStructure _databaseStructure;
		private IOptimizer _optimizer;
		private IType _identifierType;

		public IDatabaseStructure DatabaseStructure
		{
			get { return _databaseStructure; }
		}

		public IOptimizer Optimizer
		{
			get { return _optimizer; }
		}

		public IType IdentifierType
		{
			get { return _identifierType; }
		}

		#region Implementation of IIdentifierGenerator

		public virtual object Generate(ISessionImplementor session, object obj)
		{
			return _optimizer.Generate(_databaseStructure.BuildCallback(session));
		}

		#endregion

		#region Implementation of IPersistentIdentifierGenerator

		public virtual string[] SqlCreateStrings(Dialect.Dialect dialect)
		{
			return _databaseStructure.SqlCreateStrings(dialect);
		}

		public virtual string[] SqlDropString(Dialect.Dialect dialect)
		{
			return _databaseStructure.SqlDropStrings(dialect);
		}

		public virtual string GeneratorKey()
		{
			return _databaseStructure.Name;
		}

		#endregion

		#region Implementation of IConfigurable

		public virtual void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			_identifierType = type;

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
				_databaseStructure = new SequenceStructure(dialect, sequenceName, initialValue, incrementSize);
			}
			else
			{
				_databaseStructure = new TableStructure(dialect, sequenceName, valueColumnName, initialValue, incrementSize);
			}

			_optimizer = OptimizerFactory.BuildOptimizer(
				optimizationStrategy,
				_identifierType.ReturnedClass,
				incrementSize,
				PropertiesHelper.GetInt32(InitialParam, parms, -1)); // Use -1 as default initial value here to signal that it's not set.

			_databaseStructure.Prepare(_optimizer);
		}

		#endregion
	}
}