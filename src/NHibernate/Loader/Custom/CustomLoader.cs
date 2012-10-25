using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;

namespace NHibernate.Loader.Custom
{
	/// <summary> 
	/// Extension point for loaders which use a SQL result set with "unexpected" column aliases. 
	/// </summary>
	public class CustomLoader : Loader
	{
		// Currently *not* cachable if autodiscover types is in effect (e.g. "select * ...")

		private readonly SqlString sql;
		private readonly ISet<string> querySpaces = new HashedSet<string>();
		private readonly List<IParameterSpecification> parametersSpecifications;

		private readonly ILoadable[] entityPersisters;
		private readonly int[] entityOwners;
		private readonly IEntityAliases[] entityAliases;

		private readonly ICollectionPersister[] collectionPersisters;
		private readonly int[] collectionOwners;
		private readonly ICollectionAliases[] collectionAliases;

		private readonly LockMode[] lockModes;
		private readonly ResultRowProcessor rowProcessor;

		private IType[] resultTypes;
		private string[] transformerAliases;

		public CustomLoader(ICustomQuery customQuery, ISessionFactoryImplementor factory) : base(factory)
		{
			this.sql = customQuery.SQL;
			this.querySpaces.AddAll(customQuery.QuerySpaces);
			this.parametersSpecifications = customQuery.CollectedParametersSpecifications.ToList();

			var entityPersisters = new List<IEntityPersister>();
			var entityOwners = new List<int>();
			var entityAliases = new List<IEntityAliases>();
			var collectionPersisters = new List<ICollectionPersister>();
			var collectionOwners = new List<int>();
			var collectionAliases = new List<ICollectionAliases>();
			var lockModes = new List<LockMode>();
			var resultColumnProcessors = new List<IResultColumnProcessor>();
			var resultTypes = new List<IType>();
			var transformerAliases = new List<string>();

			int returnableCounter = 0;
			bool hasScalars = false;

			foreach (IReturn rtn in customQuery.CustomQueryReturns)
			{
				transformerAliases.Add(rtn.Alias);

				if (rtn is ScalarReturn)
				{
					resultTypes.Add(rtn.Type);
					resultColumnProcessors.Add(new ScalarResultColumnProcessor(rtn.Alias, rtn.Type));
					hasScalars = true;
					continue;
				}

				var nonScalarRtn = rtn as NonScalarReturn;
				if (nonScalarRtn != null)
				{
					lockModes.Add(nonScalarRtn.LockMode);

					var ownerIndex = nonScalarRtn.Owner != null
						? entityPersisters.IndexOf(nonScalarRtn.Owner.EntityPersister)
						: -1;
					if (nonScalarRtn.EntityPersister != null)
					{
						entityPersisters.Add(nonScalarRtn.EntityPersister);
						entityAliases.Add(nonScalarRtn.EntityAliases);
						entityOwners.Add(ownerIndex);
						querySpaces.AddAll(nonScalarRtn.EntityPersister.QuerySpaces);
					}
					if (nonScalarRtn.CollectionPersister != null)
					{
						collectionPersisters.Add(nonScalarRtn.CollectionPersister);
						collectionAliases.Add(nonScalarRtn.CollectionAliases);
						collectionOwners.Add(ownerIndex);
					}
					if (nonScalarRtn.Owner == null)
					{
						resultTypes.Add(nonScalarRtn.Type);
						resultColumnProcessors.Add(new NonScalarResultColumnProcessor(returnableCounter++));
					}

					continue;
				}
					
				throw new HibernateException("unexpected custom query return type : " + rtn.GetType().FullName);
			}

			this.entityPersisters = entityPersisters.Cast<ILoadable>().ToArray();
			this.entityOwners = entityOwners.ToArray();
			this.entityAliases = entityAliases.ToArray();
			this.collectionPersisters = collectionPersisters.ToArray();
			this.collectionOwners = collectionOwners.ToArray();
			this.collectionAliases = collectionAliases.ToArray();
			this.lockModes = lockModes.ToArray();
			this.resultTypes = resultTypes.ToArray();
			this.transformerAliases = transformerAliases.ToArray();
			this.rowProcessor = new ResultRowProcessor(hasScalars, resultColumnProcessors.ToArray());
		}

		public ISet<string> QuerySpaces
		{
			get { return querySpaces; }
		}

		protected override int[] CollectionOwners
		{
			get { return collectionOwners; }
		}

		/// <summary>
		/// An array of indexes of the entity that owns a one-to-one association
		/// to the entity at the given index (-1 if there is no "owner")
		/// </summary>
		protected override int[] Owners
		{
			get { return entityOwners; }
		}

		private string[] ReturnAliasesForTransformer
		{
			get { return transformerAliases; }
		}

		protected override IEntityAliases[] EntityAliases
		{
			get { return entityAliases; }
		}

		protected override ICollectionAliases[] CollectionAliases
		{
			get { return collectionAliases; }
		}

		public override string QueryIdentifier
		{
			get { return sql.ToString(); }
		}

		public override SqlString SqlString
		{
			get { return sql; }
		}

		public override LockMode[] GetLockModes(IDictionary<string, LockMode> lockModesMap)
		{
			return lockModes;
		}

		public override ILoadable[] EntityPersisters
		{
			get { return entityPersisters; }
		}

		protected override ICollectionPersister[] CollectionPersisters
		{
			get { return collectionPersisters; }
		}

		public IList List(ISessionImplementor session, QueryParameters queryParameters)
		{
			return List(session, queryParameters, querySpaces, resultTypes);
		}

		// Not ported: scroll

		protected override object GetResultColumnOrRow(object[] row, IResultTransformer resultTransformer, IDataReader rs,
		                                               ISessionImplementor session)
		{
			return rowProcessor.BuildResultRow(row, rs, resultTransformer != null, session);
		}

		public override IList GetResultList(IList results, IResultTransformer resultTransformer)
		{
			// meant to handle dynamic instantiation queries...(Copy from QueryLoader)
			HolderInstantiator holderInstantiator =
				HolderInstantiator.GetHolderInstantiator(null, resultTransformer, ReturnAliasesForTransformer);
			if (!holderInstantiator.IsRequired) return results;

			for (int i = 0; i < results.Count; i++)
			{
				object[] row = (object[]) results[i];
				object result = holderInstantiator.Instantiate(row);
				results[i] = result;
			}

			return resultTransformer.TransformList(results);
		}

		protected override void AutoDiscoverTypes(IDataReader rs)
		{
			MetaData metadata = new MetaData(rs);
			List<string> aliases = new List<string>();
			List<IType> types = new List<IType>();

			rowProcessor.PrepareForAutoDiscovery(metadata);

			foreach (IResultColumnProcessor columnProcessor in rowProcessor.ColumnProcessors)
			{
				columnProcessor.PerformDiscovery(metadata, types, aliases);
			}

			resultTypes = types.ToArray();
			transformerAliases = aliases.ToArray();
		}

		protected override void ResetEffectiveExpectedType(IEnumerable<IParameterSpecification> parameterSpecs, QueryParameters queryParameters)
		{
			parameterSpecs.ResetEffectiveExpectedType(queryParameters);
		}

		protected override IEnumerable<IParameterSpecification> GetParameterSpecifications()
		{
			return parametersSpecifications;
		}

		public IType[] ResultTypes
		{
			get { return resultTypes; }
		}

		public string[] ReturnAliases
		{
			get { return transformerAliases; }
		}

		public IEnumerable<string> NamedParameters
		{
			get { return parametersSpecifications.OfType<NamedParameterSpecification>().Select(np=> np.Name ); }
		}

		public class ResultRowProcessor
		{
			private readonly bool hasScalars;
			private IResultColumnProcessor[] columnProcessors;

			public IResultColumnProcessor[] ColumnProcessors
			{
				get { return columnProcessors; }
			}

			public ResultRowProcessor(bool hasScalars, IResultColumnProcessor[] columnProcessors)
			{
				this.hasScalars = hasScalars || (columnProcessors == null || columnProcessors.Length == 0);
				this.columnProcessors = columnProcessors;
			}

			/// <summary> Build a logical result row. </summary>
			/// <param name="data">
			/// Entity data defined as "root returns" and already handled by the normal Loader mechanism.
			/// </param>
			/// <param name="resultSet">The ADO result set (positioned at the row currently being processed). </param>
			/// <param name="hasTransformer">Does this query have an associated <see cref="IResultTransformer"/>. </param>
			/// <param name="session">The session from which the query request originated.</param>
			/// <returns> The logical result row </returns>
			/// <remarks>
			/// At this point, Loader has already processed all non-scalar result data.  We
			/// just need to account for scalar result data here...
			/// </remarks>
			public object BuildResultRow(object[] data, IDataReader resultSet, bool hasTransformer, ISessionImplementor session)
			{
				object[] resultRow;
				// NH Different behavior (patched in NH-1612 to solve Hibernate issue HHH-2831).
				if (!hasScalars && (hasTransformer || data.Length == 0))
				{
					resultRow = data;
				}
				else
				{
					// build an array with indices equal to the total number
					// of actual returns in the result Hibernate will return
					// for this query (scalars + non-scalars)
					resultRow = new object[columnProcessors.Length];
					for (int i = 0; i < columnProcessors.Length; i++)
					{
						resultRow[i] = columnProcessors[i].Extract(data, resultSet, session);
					}
				}

				return (hasTransformer) ? resultRow : (resultRow.Length == 1) ? resultRow[0] : resultRow;
			}

			public void PrepareForAutoDiscovery(MetaData metadata)
			{
				if (columnProcessors == null || columnProcessors.Length == 0)
				{
					int columns = metadata.GetColumnCount();
					columnProcessors = new IResultColumnProcessor[columns];
					for (int i = 0; i < columns; i++)
					{
						columnProcessors[i] = new ScalarResultColumnProcessor(i);
					}
				}
			}
		}

		public interface IResultColumnProcessor
		{
			object Extract(object[] data, IDataReader resultSet, ISessionImplementor session);
			void PerformDiscovery(MetaData metadata, IList<IType> types, IList<string> aliases);
		}

		public class NonScalarResultColumnProcessor : IResultColumnProcessor
		{
			private readonly int position;

			public NonScalarResultColumnProcessor(int position)
			{
				this.position = position;
			}

			public object Extract(object[] data, IDataReader resultSet, ISessionImplementor session)
			{
				return data[position];
			}

			public void PerformDiscovery(MetaData metadata, IList<IType> types, IList<string> aliases) {}
		}

		public class ScalarResultColumnProcessor : IResultColumnProcessor
		{
			private int position = -1;
			private string alias;
			private IType type;

			public ScalarResultColumnProcessor(int position)
			{
				this.position = position;
			}

			public ScalarResultColumnProcessor(string alias, IType type)
			{
				this.alias = alias;
				this.type = type;
			}

			public object Extract(object[] data, IDataReader resultSet, ISessionImplementor session)
			{
				return type.NullSafeGet(resultSet, alias, session, null);
			}

			public void PerformDiscovery(MetaData metadata, IList<IType> types, IList<string> aliases)
			{
				if (string.IsNullOrEmpty(alias))
				{
					alias = metadata.GetColumnName(position);
				}
				else if (position < 0)
				{
					position = metadata.GetColumnPosition(alias);
				}
				if (type == null)
				{
					type = metadata.GetHibernateType(position);
				}
				types.Add(type);
				aliases.Add(alias);
			}
		}

		/// <summary>
		/// Encapsulates the metadata available from the database result set.
		/// </summary>
		public class MetaData
		{
			private readonly IDataReader resultSet;

			/// <summary>
			/// Initializes a new instance of the <see cref="MetaData"/> class.
			/// </summary>
			/// <param name="resultSet">The result set.</param>
			public MetaData(IDataReader resultSet)
			{
				this.resultSet = resultSet;
			}

			/// <summary>
			/// Gets the column count in the result set.
			/// </summary>
			/// <returns>The column count.</returns>
			public int GetColumnCount()
			{
				return resultSet.FieldCount;
			}

			/// <summary>
			/// Gets the (zero-based) position of the column with the specified name.
			/// </summary>
			/// <param name="columnName">Name of the column.</param>
			/// <returns>The column position.</returns>
			public int GetColumnPosition(string columnName)
			{
				return resultSet.GetOrdinal(columnName);
			}

			/// <summary>
			/// Gets the name of the column at the specified position.
			/// </summary>
			/// <param name="position">The (zero-based) position.</param>
			/// <returns>The column name.</returns>
			public string GetColumnName(int position)
			{
				return resultSet.GetName(position);
			}

			/// <summary>
			/// Gets the Hibernate type of the specified column.
			/// </summary>
			/// <param name="columnPos">The column position.</param>
			/// <returns>The Hibernate type.</returns>
			public IType GetHibernateType(int columnPos)
			{
				return TypeFactory.Basic(resultSet.GetFieldType(columnPos).Name);
			}
		}
	}
}