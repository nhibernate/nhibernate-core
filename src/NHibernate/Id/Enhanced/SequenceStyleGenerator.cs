using System.Collections.Generic;
using log4net;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Id.Enhanced
{
	public class SequenceStyleGenerator : IPersistentIdentifierGenerator, IConfigurable
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(SequenceStyleGenerator));

		#region General purpose parameters

		public const string SequenceParam = "sequence_name";
		public const string DefaultSequenceName = "hibernate_sequence";

		public const string InitialParam = "initial_value";
		public const int DefaultInitialValue = 1;

		public const string IncrementParam= "increment_size";
		public const int DefaultIncrementSize = 1;

		public const string OptimizerParam= "optimizer";

		public const string ForceTableParam = "force_table_use";

		#endregion

		#region table-specific parameters

		public const string ValueColumnParam= "value_column";
		public const string DefaultValueColumnName = "next_val";

		#endregion

		private IDatabaseStructure databaseStructure;
		private IOptimizer optimizer;
		private IType identifierType;

		public IDatabaseStructure DatabaseStructure
		{
			get { return databaseStructure; }
		}

		public IOptimizer Optimizer
		{
			get { return optimizer; }
		}

		public IType IdentifierType
		{
			get { return identifierType; }
		}

		#region Implementation of IIdentifierGenerator

		public virtual object Generate(ISessionImplementor session, object obj)
		{
			return optimizer.Generate(databaseStructure.BuildCallback(session));
		}

		#endregion

		#region Implementation of IPersistentIdentifierGenerator

		public virtual string[] SqlCreateStrings(Dialect.Dialect dialect)
		{
			return databaseStructure.SqlCreateStrings(dialect);
		}

		public virtual string[] SqlDropString(Dialect.Dialect dialect)
		{
			return databaseStructure.SqlDropStrings(dialect);
		}

		public virtual string GeneratorKey()
		{
			return databaseStructure.Name;
		}

		#endregion

		#region Implementation of IConfigurable

		public virtual void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			identifierType = type;
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
				log.Warn("config specified explicit optimizer of [" + OptimizerFactory.None + "], but [" + IncrementParam + "=" + incrementSize + "; honoring optimizer setting");
				incrementSize = 1;
			}
			if (dialect.SupportsSequences && !forceTableUse)
			{
				if (OptimizerFactory.Pool.Equals(optimizationStrategy) && !dialect.SupportsPooledSequences)
				{
					// TODO : may even be better to fall back to a pooled table strategy here so that the db stored values remain consistent...
					optimizationStrategy = OptimizerFactory.HiLo;
				}
				databaseStructure = new SequenceStructure(dialect, sequenceName, initialValue, incrementSize);
			}
			else
			{
				databaseStructure = new TableStructure(dialect, sequenceName, valueColumnName, initialValue, incrementSize);
			}

			optimizer = OptimizerFactory.BuildOptimizer(optimizationStrategy, identifierType.ReturnedClass, incrementSize);
			databaseStructure.Prepare(optimizer);
		}

		#endregion
	}
}