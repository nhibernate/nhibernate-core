using System.Collections;
using log4net;
using NHibernate.Engine;
using NHibernate.Engine.Query.Sql;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Loader.Custom.Sql
{
	public class SQLQueryReturnProcessor
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(SQLQueryReturnProcessor));

		private readonly INativeSQLQueryReturn[] queryReturns;

		private readonly IDictionary alias2Return = new Hashtable();
		private readonly IDictionary alias2OwnerAlias = new Hashtable();

		private readonly IDictionary alias2Persister = new Hashtable();
		private readonly IDictionary alias2Suffix = new Hashtable();

		private readonly IDictionary alias2CollectionPersister = new Hashtable();
		private readonly IDictionary alias2CollectionSuffix = new Hashtable();

		private readonly IDictionary entityPropertyResultMaps = new Hashtable();
		private readonly IDictionary collectionPropertyResultMaps = new Hashtable();

		private readonly ISessionFactoryImplementor factory;

		private int entitySuffixSeed = 0;
		private int collectionSuffixSeed = 0;

		private ISessionFactoryImplementor Factory
		{
			get { return factory; }
		}

		public SQLQueryReturnProcessor(
			INativeSQLQueryReturn[] queryReturns,
			ISessionFactoryImplementor factory)
		{
			this.queryReturns = queryReturns;
			this.factory = factory;
		}

		public class ResultAliasContext
		{
			private SQLQueryReturnProcessor parent;

			public ResultAliasContext(SQLQueryReturnProcessor parent)
			{
				this.parent = parent;
			}

			public ISqlLoadable GetEntityPersister(string alias)
			{
				return (ISqlLoadable) parent.alias2Persister[alias];
			}

			public ISqlLoadableCollection GetCollectionPersister(string alias)
			{
				return (ISqlLoadableCollection) parent.alias2CollectionPersister[alias];
			}

			public string GetEntitySuffix(string alias)
			{
				return (string) parent.alias2Suffix[alias];
			}

			public string GetCollectionSuffix(string alias)
			{
				return (string) parent.alias2CollectionSuffix[alias];
			}

			public string GetOwnerAlias(string alias)
			{
				return (string) parent.alias2OwnerAlias[alias];
			}

			public IDictionary GetPropertyResultsMap(string alias)
			{
				return parent.InternalGetPropertyResultsMap(alias);
			}
		}

		private IDictionary InternalGetPropertyResultsMap(string alias)
		{
			INativeSQLQueryReturn rtn = (INativeSQLQueryReturn) alias2Return[alias];
			if (rtn is NativeSQLQueryNonScalarReturn)
			{
				return ((NativeSQLQueryNonScalarReturn) rtn).PropertyResultsMap;
			}
			else
			{
				return null;
			}
		}

		private bool HasPropertyResultMap(string alias)
		{
			IDictionary propertyMaps = InternalGetPropertyResultsMap(alias);
			return propertyMaps != null && propertyMaps.Count != 0;
		}

		public ResultAliasContext Process()
		{
			// first, break down the returns into maps keyed by alias
			// so that role returns can be more easily resolved to their owners
			for (int i = 0; i < queryReturns.Length; i++)
			{
				if (queryReturns[i] is NativeSQLQueryNonScalarReturn)
				{
					NativeSQLQueryNonScalarReturn rtn = (NativeSQLQueryNonScalarReturn) queryReturns[i];
					alias2Return[rtn.Alias] = rtn;
					if (rtn is NativeSQLQueryJoinReturn)
					{
						NativeSQLQueryJoinReturn roleReturn = (NativeSQLQueryJoinReturn) queryReturns[i];
						alias2OwnerAlias[roleReturn.Alias] = roleReturn.OwnerAlias;
					}
				}
			}

			// Now, process the returns
			for (int i = 0; i < queryReturns.Length; i++)
			{
				ProcessReturn(queryReturns[i]);
			}

			return new ResultAliasContext(this);
		}

		private ISqlLoadable GetSQLLoadable(string entityName)
		{
			IEntityPersister persister = factory.GetEntityPersister(entityName);
			if (!(persister is ISqlLoadable))
			{
				throw new MappingException("class persister is not ISqlLoadable: " + entityName);
			}
			return (ISqlLoadable) persister;
		}

		private string GenerateEntitySuffix()
		{
			return BasicLoader.GenerateSuffixes(entitySuffixSeed++, 1)[0];
		}

		private string GenerateCollectionSuffix()
		{
			return collectionSuffixSeed++ + "__";
		}

		private void ProcessReturn(INativeSQLQueryReturn rtn)
		{
			if (rtn is NativeSQLQueryScalarReturn)
			{
				ProcessScalarReturn((NativeSQLQueryScalarReturn) rtn);
			}
			else if (rtn is NativeSQLQueryRootReturn)
			{
				ProcessRootReturn((NativeSQLQueryRootReturn) rtn);
			}
			else if (rtn is NativeSQLQueryCollectionReturn)
			{
				ProcessCollectionReturn((NativeSQLQueryCollectionReturn) rtn);
			}
			else
			{
				ProcessJoinReturn((NativeSQLQueryJoinReturn) rtn);
			}
		}

		private void ProcessScalarReturn(NativeSQLQueryScalarReturn typeReturn)
		{
		}

		private void ProcessRootReturn(NativeSQLQueryRootReturn rootReturn)
		{
			if (alias2Persister.Contains(rootReturn.Alias))
			{
				// already been processed...
				return;
			}

			ISqlLoadable persister = GetSQLLoadable(rootReturn.ReturnEntityName);
			AddPersister(rootReturn.Alias, rootReturn.PropertyResultsMap, persister);
		}

		private void AddPersister(string alias, IDictionary propertyResult, ISqlLoadable persister)
		{
			alias2Persister[alias] = persister;
			string suffix = GenerateEntitySuffix();
			log.Debug("mapping alias [" + alias + "] to entity-suffix [" + suffix + "]");
			alias2Suffix[alias] = suffix;
			entityPropertyResultMaps[alias] = propertyResult;
		}

		private void AddCollection(string role, string alias, IDictionary propertyResults)
		{
			IQueryableCollection collectionPersister = (IQueryableCollection) Factory.GetCollectionPersister(role);
			alias2CollectionPersister[alias] = collectionPersister;
			string suffix = GenerateCollectionSuffix();
			log.Debug("mapping alias [" + alias + "] to collection-suffix [" + suffix + "]");
			alias2CollectionSuffix[alias] = suffix;
			collectionPropertyResultMaps[alias] = propertyResults;

			if (collectionPersister.IsOneToMany)
			{
				ISqlLoadable persister = (ISqlLoadable) collectionPersister.ElementPersister;
				AddPersister(alias, Filter(propertyResults), persister);
			}
		}

		private IDictionary Filter(IDictionary propertyResults)
		{
			IDictionary result = new Hashtable(propertyResults.Count);

			string keyPrefix = "element.";

			foreach (DictionaryEntry element in propertyResults)
			{
				string path = (string) element.Key;
				if (path.StartsWith(keyPrefix))
				{
					result[path.Substring(keyPrefix.Length)] = element.Value;
				}
			}

			return result;
		}

		private void ProcessCollectionReturn(NativeSQLQueryCollectionReturn collectionReturn)
		{
			// we are initializing an owned collection
			string role = collectionReturn.OwnerEntityName + '.' + collectionReturn.OwnerProperty;
			AddCollection(
				role,
				collectionReturn.Alias,
				collectionReturn.PropertyResultsMap
				);
		}

		private void ProcessJoinReturn(NativeSQLQueryJoinReturn fetchReturn)
		{
			string alias = fetchReturn.Alias;
			if (alias2Persister.Contains(alias) || alias2CollectionPersister.Contains(alias))
			{
				// already been processed...
				return;
			}

			string ownerAlias = fetchReturn.OwnerAlias;

			// Make sure the owner alias is known...
			if (!alias2Return.Contains(ownerAlias))
			{
				throw new HibernateException(
					"Owner alias [" + ownerAlias + "] is unknown for alias [" +
					alias + "]"
					);
			}

			// If this return's alias has not been processed yet, do so b4 further processing of this return
			if (!alias2Persister.Contains(ownerAlias))
			{
				NativeSQLQueryNonScalarReturn ownerReturn = (NativeSQLQueryNonScalarReturn) alias2Return[ownerAlias];
				ProcessReturn(ownerReturn);
			}

			ISqlLoadable ownerPersister = (ISqlLoadable) alias2Persister[ownerAlias];
			IType returnType = ownerPersister.GetPropertyType(fetchReturn.OwnerProperty);

			if (returnType.IsCollectionType)
			{
				string role = ownerPersister.EntityName + '.' + fetchReturn.OwnerProperty;
				AddCollection(role, alias, fetchReturn.PropertyResultsMap);
			}
			else if (returnType.IsEntityType)
			{
				EntityType eType = (EntityType) returnType;
				string returnEntityName = eType.GetAssociatedEntityName();
				ISqlLoadable persister = GetSQLLoadable(returnEntityName);
				AddPersister(alias, fetchReturn.PropertyResultsMap, persister);
			}
		}

		public IList GenerateCustomReturns(bool queryHadAliases)
		{
			IList customReturns = new ArrayList();
			IDictionary customReturnsByAlias = new Hashtable();
			for (int i = 0; i < queryReturns.Length; i++)
			{
				if (queryReturns[i] is NativeSQLQueryScalarReturn)
				{
					NativeSQLQueryScalarReturn rtn = (NativeSQLQueryScalarReturn) queryReturns[i];
					customReturns.Add(new ScalarReturn(rtn.Type, rtn.ColumnAlias));
				}
				else if (queryReturns[i] is NativeSQLQueryRootReturn)
				{
					NativeSQLQueryRootReturn rtn = (NativeSQLQueryRootReturn) queryReturns[i];
					string alias = rtn.Alias;
					IEntityAliases entityAliases;
					if (queryHadAliases || HasPropertyResultMap(alias))
					{
						entityAliases = new DefaultEntityAliases(
							(IDictionary) entityPropertyResultMaps[alias],
							(ISqlLoadable) alias2Persister[alias],
							(string) alias2Suffix[alias]
							);
					}
					else
					{
						entityAliases = new ColumnEntityAliases(
							(IDictionary) entityPropertyResultMaps[alias],
							(ISqlLoadable) alias2Persister[alias],
							(string) alias2Suffix[alias]
							);
					}
					RootReturn customReturn = new RootReturn(
						alias,
						rtn.ReturnEntityName,
						entityAliases,
						rtn.LockMode
						);
					customReturns.Add(customReturn);
					customReturnsByAlias[rtn.Alias] = customReturn;
				}
				else if (queryReturns[i] is NativeSQLQueryCollectionReturn)
				{
					NativeSQLQueryCollectionReturn rtn = (NativeSQLQueryCollectionReturn) queryReturns[i];
					string alias = rtn.Alias;
					ISqlLoadableCollection persister = (ISqlLoadableCollection) alias2CollectionPersister[alias];
					bool isEntityElements = persister.ElementType.IsEntityType;
					ICollectionAliases collectionAliases;
					IEntityAliases elementEntityAliases = null;
					if (queryHadAliases || HasPropertyResultMap(alias))
					{
						collectionAliases = new GeneratedCollectionAliases(
							(IDictionary) collectionPropertyResultMaps[alias],
							(ISqlLoadableCollection) alias2CollectionPersister[alias],
							(string) alias2CollectionSuffix[alias]
							);
						if (isEntityElements)
						{
							elementEntityAliases = new DefaultEntityAliases(
								(IDictionary) entityPropertyResultMaps[alias],
								(ISqlLoadable) alias2Persister[alias],
								(string) alias2Suffix[alias]
								);
						}
					}
					else
					{
						collectionAliases = new ColumnCollectionAliases(
							(IDictionary) collectionPropertyResultMaps[alias],
							(ISqlLoadableCollection) alias2CollectionPersister[alias]
							);
						if (isEntityElements)
						{
							elementEntityAliases = new ColumnEntityAliases(
								(IDictionary) entityPropertyResultMaps[alias],
								(ISqlLoadable) alias2Persister[alias],
								(string) alias2Suffix[alias]
								);
						}
					}
					CollectionReturn customReturn = new CollectionReturn(
						alias,
						rtn.OwnerEntityName,
						rtn.OwnerProperty,
						collectionAliases,
						elementEntityAliases,
						rtn.LockMode
						);
					customReturns.Add(customReturn);
					customReturnsByAlias[rtn.Alias] = customReturn;
				}
				else if (queryReturns[i] is NativeSQLQueryJoinReturn)
				{
					NativeSQLQueryJoinReturn rtn = (NativeSQLQueryJoinReturn) queryReturns[i];
					string alias = rtn.Alias;
					FetchReturn customReturn;
					NonScalarReturn ownerCustomReturn = (NonScalarReturn) customReturnsByAlias[rtn.OwnerAlias];
					if (alias2CollectionPersister.Contains(alias))
					{
						ISqlLoadableCollection persister = (ISqlLoadableCollection) alias2CollectionPersister[alias];
						bool isEntityElements = persister.ElementType.IsEntityType;
						ICollectionAliases collectionAliases;
						IEntityAliases elementEntityAliases = null;
						if (queryHadAliases || HasPropertyResultMap(alias))
						{
							collectionAliases = new GeneratedCollectionAliases(
								(IDictionary) collectionPropertyResultMaps[alias],
								persister,
								(string) alias2CollectionSuffix[alias]
								);
							if (isEntityElements)
							{
								elementEntityAliases = new DefaultEntityAliases(
									(IDictionary) entityPropertyResultMaps[alias],
									(ISqlLoadable) alias2Persister[alias],
									(string) alias2Suffix[alias]
									);
							}
						}
						else
						{
							collectionAliases = new ColumnCollectionAliases(
								(IDictionary) collectionPropertyResultMaps[alias],
								persister
								);
							if (isEntityElements)
							{
								elementEntityAliases = new ColumnEntityAliases(
									(IDictionary) entityPropertyResultMaps[alias],
									(ISqlLoadable) alias2Persister[alias],
									(string) alias2Suffix[alias]
									);
							}
						}
						customReturn = new CollectionFetchReturn(
							alias,
							ownerCustomReturn,
							rtn.OwnerProperty,
							collectionAliases,
							elementEntityAliases,
							rtn.LockMode
							);
					}
					else
					{
						IEntityAliases entityAliases;
						if (queryHadAliases || HasPropertyResultMap(alias))
						{
							entityAliases = new DefaultEntityAliases(
								(IDictionary) entityPropertyResultMaps[alias],
								(ISqlLoadable) alias2Persister[alias],
								(string) alias2Suffix[alias]
								);
						}
						else
						{
							entityAliases = new ColumnEntityAliases(
								(IDictionary) entityPropertyResultMaps[alias],
								(ISqlLoadable) alias2Persister[alias],
								(string) alias2Suffix[alias]
								);
						}
						customReturn = new EntityFetchReturn(
							alias,
							entityAliases,
							ownerCustomReturn,
							rtn.OwnerProperty,
							rtn.LockMode
							);
					}
					customReturns.Add(customReturn);
					customReturnsByAlias[alias] = customReturn;
				}
			}
			return customReturns;
		}
	}
}