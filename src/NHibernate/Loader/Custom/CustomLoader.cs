using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;
using IQueryable = NHibernate.Persister.Entity.IQueryable;

namespace NHibernate.Loader.Custom
{
	/// <summary> 
	/// Extension point for loaders which use a SQL result set with "unexpected" column aliases. 
	/// </summary>
	public class CustomLoader : Loader
	{
		// Currently *not* cachable if autodiscover types is in effect (e.g. "select * ...")

		private readonly SqlString sql;
		private readonly ISet<string> querySpaces = new HashSet<string>();
		private List<IParameterSpecification> parametersSpecifications;

		private readonly IQueryable[] entityPersisters;
		private readonly int[] entityOwners;
		private readonly IEntityAliases[] entityAliases;

		private readonly IQueryableCollection[] collectionPersisters;
		private readonly int[] collectionOwners;
		private readonly ICollectionAliases[] collectionAliases;

		private readonly LockMode[] lockModes;
		private readonly ResultRowProcessor rowProcessor;

		private readonly bool[] includeInResultRow;

		private IType[] resultTypes;
		private string[] transformerAliases;

		public CustomLoader(ICustomQuery customQuery, ISessionFactoryImplementor factory) : base(factory)
		{
			sql = customQuery.SQL;
			querySpaces.UnionWith(customQuery.QuerySpaces);
			parametersSpecifications = customQuery.CollectedParametersSpecifications.ToList();

			List<IQueryable> entitypersisters = new List<IQueryable>();
			List<int> entityowners = new List<int>();
			List<IEntityAliases> entityaliases = new List<IEntityAliases>();

			List<IQueryableCollection> collectionpersisters = new List<IQueryableCollection>();
			List<int> collectionowners = new List<int>();
			List<ICollectionAliases> collectionaliases = new List<ICollectionAliases>();

			List<LockMode> lockmodes = new List<LockMode>();
			List<IResultColumnProcessor> resultColumnProcessors = new List<IResultColumnProcessor>();
			List<IReturn> nonScalarReturnList = new List<IReturn>();
			List<IType> resulttypes = new List<IType>();
			List<string> specifiedAliases = new List<string>();

			List<bool> includeInResultRowList = new List<bool>();

			int returnableCounter = 0;
			bool hasScalars = false;

			foreach (IReturn rtn in customQuery.CustomQueryReturns)
			{
				if (rtn is ScalarReturn)
				{
					ScalarReturn scalarRtn = (ScalarReturn) rtn;
					resulttypes.Add(scalarRtn.Type);
					specifiedAliases.Add(scalarRtn.ColumnAlias);
					resultColumnProcessors.Add(new ScalarResultColumnProcessor(scalarRtn.ColumnAlias, scalarRtn.Type));
					includeInResultRowList.Add(true);
					hasScalars = true;
				}
				else if (rtn is RootReturn)
				{
					RootReturn rootRtn = (RootReturn) rtn;
					IQueryable persister = (IQueryable) factory.GetEntityPersister(rootRtn.EntityName);
					entitypersisters.Add(persister);
					lockmodes.Add(rootRtn.LockMode);
					resultColumnProcessors.Add(new NonScalarResultColumnProcessor(returnableCounter++));
					nonScalarReturnList.Add(rtn);
					entityowners.Add(-1);
					resulttypes.Add(persister.Type);
					specifiedAliases.Add(rootRtn.Alias);
					entityaliases.Add(rootRtn.EntityAliases);
					querySpaces.UnionWith(persister.QuerySpaces);
					includeInResultRowList.Add(true);
				}
				else if (rtn is CollectionReturn)
				{
					CollectionReturn collRtn = (CollectionReturn) rtn;
					string role = collRtn.OwnerEntityName + "." + collRtn.OwnerProperty;
					IQueryableCollection persister = (IQueryableCollection) factory.GetCollectionPersister(role);
					collectionpersisters.Add(persister);
					lockmodes.Add(collRtn.LockMode);
					resultColumnProcessors.Add(new NonScalarResultColumnProcessor(returnableCounter++));
					nonScalarReturnList.Add(rtn);
					collectionowners.Add(-1);
					resulttypes.Add(persister.Type);
					specifiedAliases.Add(collRtn.Alias);
					collectionaliases.Add(collRtn.CollectionAliases);
					// determine if the collection elements are entities...
					IType elementType = persister.ElementType;
					if (elementType.IsEntityType)
					{
						IQueryable elementPersister = (IQueryable) ((EntityType) elementType).GetAssociatedJoinable(factory);
						entitypersisters.Add(elementPersister);
						entityowners.Add(-1);
						entityaliases.Add(collRtn.ElementEntityAliases);
						querySpaces.UnionWith(elementPersister.QuerySpaces);
					}
					includeInResultRowList.Add(true);
				}
				else if (rtn is EntityFetchReturn)
				{
					EntityFetchReturn fetchRtn = (EntityFetchReturn) rtn;
					NonScalarReturn ownerDescriptor = fetchRtn.Owner;
					int ownerIndex = nonScalarReturnList.IndexOf(ownerDescriptor);
					entityowners.Add(ownerIndex);
					lockmodes.Add(fetchRtn.LockMode);
					IQueryable ownerPersister = DetermineAppropriateOwnerPersister(ownerDescriptor);
					EntityType fetchedType = (EntityType) ownerPersister.GetPropertyType(fetchRtn.OwnerProperty);
					string entityName = fetchedType.GetAssociatedEntityName(Factory);
					IQueryable persister = (IQueryable) factory.GetEntityPersister(entityName);
					entitypersisters.Add(persister);
					nonScalarReturnList.Add(rtn);
					specifiedAliases.Add(fetchRtn.Alias);
					entityaliases.Add(fetchRtn.EntityAliases);
					querySpaces.UnionWith(persister.QuerySpaces);
					includeInResultRowList.Add(false);
				}
				else if (rtn is CollectionFetchReturn)
				{
					CollectionFetchReturn fetchRtn = (CollectionFetchReturn) rtn;
					NonScalarReturn ownerDescriptor = fetchRtn.Owner;
					int ownerIndex = nonScalarReturnList.IndexOf(ownerDescriptor);
					collectionowners.Add(ownerIndex);
					lockmodes.Add(fetchRtn.LockMode);
					IQueryable ownerPersister = DetermineAppropriateOwnerPersister(ownerDescriptor);
					string role = ownerPersister.EntityName + '.' + fetchRtn.OwnerProperty;
					IQueryableCollection persister = (IQueryableCollection) factory.GetCollectionPersister(role);
					collectionpersisters.Add(persister);
					nonScalarReturnList.Add(rtn);
					specifiedAliases.Add(fetchRtn.Alias);
					collectionaliases.Add(fetchRtn.CollectionAliases);
					// determine if the collection elements are entities...
					IType elementType = persister.ElementType;
					if (elementType.IsEntityType)
					{
						IQueryable elementPersister = (IQueryable) ((EntityType) elementType).GetAssociatedJoinable(factory);
						entitypersisters.Add(elementPersister);
						entityowners.Add(ownerIndex);
						entityaliases.Add(fetchRtn.ElementEntityAliases);
						querySpaces.UnionWith(elementPersister.QuerySpaces);
					}
					includeInResultRowList.Add(false);
				}
				else
				{
					throw new HibernateException("unexpected custom query return type : " + rtn.GetType().FullName);
				}
			}

			entityPersisters = entitypersisters.ToArray();
			entityOwners = entityowners.ToArray();
			entityAliases = entityaliases.ToArray();
			collectionPersisters = collectionpersisters.ToArray();
			collectionOwners = collectionowners.ToArray();
			collectionAliases = collectionaliases.ToArray();
			lockModes = lockmodes.ToArray();
			resultTypes = resulttypes.ToArray();
			transformerAliases = specifiedAliases.ToArray();
			rowProcessor = new ResultRowProcessor(hasScalars, resultColumnProcessors.ToArray());
			includeInResultRow = includeInResultRowList.ToArray();
		}

		public ISet<string> QuerySpaces
		{
			get { return querySpaces; }
		}

		protected override int[] CollectionOwners
		{
			get { return collectionOwners; }
		}

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

		private IQueryable DetermineAppropriateOwnerPersister(NonScalarReturn ownerDescriptor)
		{
			string entityName = null;
			RootReturn odrr = ownerDescriptor as RootReturn;
			if (odrr != null)
			{
				entityName = odrr.EntityName;
			}
			else if (ownerDescriptor is CollectionReturn)
			{
				CollectionReturn collRtn = (CollectionReturn) ownerDescriptor;
				string role = collRtn.OwnerEntityName + "." + collRtn.OwnerProperty;
				ICollectionPersister persister = Factory.GetCollectionPersister(role);
				EntityType ownerType = (EntityType) persister.ElementType;
				entityName = ownerType.GetAssociatedEntityName(Factory);
			}
			else if (ownerDescriptor is FetchReturn)
			{
				FetchReturn fetchRtn = (FetchReturn) ownerDescriptor;
				IQueryable persister = DetermineAppropriateOwnerPersister(fetchRtn.Owner);
				IType ownerType = persister.GetPropertyType(fetchRtn.OwnerProperty);
				if (ownerType.IsEntityType)
				{
					entityName = ((EntityType) ownerType).GetAssociatedEntityName(Factory);
				}
				else if (ownerType.IsCollectionType)
				{
					IType ownerCollectionElementType = ((CollectionType) ownerType).GetElementType(Factory);
					if (ownerCollectionElementType.IsEntityType)
					{
						entityName = ((EntityType) ownerCollectionElementType).GetAssociatedEntityName(Factory);
					}
				}
			}

			if (entityName == null)
			{
				throw new HibernateException("Could not determine fetch owner : " + ownerDescriptor);
			}

			return (IQueryable) Factory.GetEntityPersister(entityName);
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

		protected override object GetResultColumnOrRow(object[] row, IResultTransformer resultTransformer, DbDataReader rs,
		                                               ISessionImplementor session)
		{
			return rowProcessor.BuildResultRow(row, rs, resultTransformer != null, session);
		}


		protected override IResultTransformer ResolveResultTransformer(IResultTransformer resultTransformer)
		{
			return HolderInstantiator.ResolveResultTransformer(null, resultTransformer);
		}

		protected override bool[] IncludeInResultRow
		{
			get { return includeInResultRow; }
		}

		public override IList GetResultList(IList results, IResultTransformer resultTransformer)
		{
			// meant to handle dynamic instantiation queries...(Copy from QueryLoader)
			HolderInstantiator holderInstantiator =
				HolderInstantiator.GetHolderInstantiator(null, resultTransformer, ReturnAliasesForTransformer);
			if (holderInstantiator.IsRequired)
			{
				for (int i = 0; i < results.Count; i++)
				{
					object[] row = (object[]) results[i];
					object result = holderInstantiator.Instantiate(row);
					results[i] = result;
				}

				return resultTransformer.TransformList(results);
			}
			else
			{
				return results;
			}
		}

		protected internal override void AutoDiscoverTypes(DbDataReader rs)
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
			public object BuildResultRow(object[] data, DbDataReader resultSet, bool hasTransformer, ISessionImplementor session)
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
			object Extract(object[] data, DbDataReader resultSet, ISessionImplementor session);
			void PerformDiscovery(MetaData metadata, IList<IType> types, IList<string> aliases);
		}

		public class NonScalarResultColumnProcessor : IResultColumnProcessor
		{
			private readonly int position;

			public NonScalarResultColumnProcessor(int position)
			{
				this.position = position;
			}

			public object Extract(object[] data, DbDataReader resultSet, ISessionImplementor session)
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

			public object Extract(object[] data, DbDataReader resultSet, ISessionImplementor session)
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
			private readonly DbDataReader resultSet;

			/// <summary>
			/// Initializes a new instance of the <see cref="MetaData"/> class.
			/// </summary>
			/// <param name="resultSet">The result set.</param>
			public MetaData(DbDataReader resultSet)
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