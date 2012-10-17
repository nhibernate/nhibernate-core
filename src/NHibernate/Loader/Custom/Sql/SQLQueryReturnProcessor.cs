using System.Collections;
using System.Collections.Generic;

using NHibernate.Engine;
using NHibernate.Engine.Query.Sql;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Loader.Custom.Sql
{
	public class SQLQueryReturnProcessor
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (SQLQueryReturnProcessor));

		private readonly INativeSQLQueryReturn[] queryReturns;

		private readonly Dictionary<string, INativeSQLQueryReturn> alias2Return =
			new Dictionary<string, INativeSQLQueryReturn>();

		private readonly Dictionary<string, string> alias2OwnerAlias = new Dictionary<string, string>();

		private readonly Dictionary<string, ISqlLoadable> alias2Persister = new Dictionary<string, ISqlLoadable>();
		private readonly Dictionary<string, string> alias2Suffix = new Dictionary<string, string>();

		private readonly Dictionary<string, ISqlLoadableCollection> alias2CollectionPersister =
			new Dictionary<string, ISqlLoadableCollection>();

		private readonly Dictionary<string, string> alias2CollectionSuffix = new Dictionary<string, string>();

		private readonly Dictionary<string, IDictionary<string, string[]>> entityPropertyResultMaps =
			new Dictionary<string, IDictionary<string, string[]>>();

		private readonly Dictionary<string, IDictionary<string, string[]>> collectionPropertyResultMaps =
			new Dictionary<string, IDictionary<string, string[]>>();

		private readonly ISessionFactoryImplementor factory;

		private int entitySuffixSeed = 0;
		private int collectionSuffixSeed = 0;

		private ISessionFactoryImplementor Factory
		{
			get { return factory; }
		}

		public SQLQueryReturnProcessor(INativeSQLQueryReturn[] queryReturns, ISessionFactoryImplementor factory)
		{
			this.queryReturns = queryReturns;
			this.factory = factory;
		}

		public class ResultAliasContext
		{
			private readonly SQLQueryReturnProcessor parent;

			public ResultAliasContext(SQLQueryReturnProcessor parent)
			{
				this.parent = parent;
			}

			public ISqlLoadable GetEntityPersister(string alias)
			{
				ISqlLoadable result;
				parent.alias2Persister.TryGetValue(alias, out result);
				return result;
			}

			public ISqlLoadableCollection GetCollectionPersister(string alias)
			{
				ISqlLoadableCollection result;
				parent.alias2CollectionPersister.TryGetValue(alias, out result);
				return result;
			}

			public string GetEntitySuffix(string alias)
			{
				string result;
				parent.alias2Suffix.TryGetValue(alias, out result);
				return result;
			}

			public string GetCollectionSuffix(string alias)
			{
				string result;
				parent.alias2CollectionSuffix.TryGetValue(alias, out result);
				return result;
			}

			public string GetOwnerAlias(string alias)
			{
				string result;
				parent.alias2OwnerAlias.TryGetValue(alias, out result);
				return result;
			}

			public IDictionary<string, string[]> GetPropertyResultsMap(string alias)
			{
				return parent.InternalGetPropertyResultsMap(alias);
			}
		}

		private IDictionary<string, string[]> InternalGetPropertyResultsMap(string alias)
		{
			NativeSQLQueryNonScalarReturn rtn = alias2Return[alias] as NativeSQLQueryNonScalarReturn;
			return rtn != null ? rtn.PropertyResultsMap : null;
		}

		private bool HasPropertyResultMap(string alias)
		{
			IDictionary<string, string[]> propertyMaps = InternalGetPropertyResultsMap(alias);
			return propertyMaps != null && propertyMaps.Count != 0;
		}

		public ResultAliasContext Process()
		{
			// first, break down the returns into maps keyed by alias
			// so that role returns can be more easily resolved to their owners
			for (int i = 0; i < queryReturns.Length; i++)
			{
				var rtn = queryReturns[i] as NativeSQLQueryNonScalarReturn;
				if (rtn != null)
				{
					alias2Return[rtn.Alias] = rtn;
					var roleReturn = queryReturns[i] as NativeSQLQueryJoinReturn;
					if (roleReturn != null)
					{
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
			var persisterAsSqlLoadable = persister as ISqlLoadable;
			if (persisterAsSqlLoadable == null)
			{
				throw new MappingException("class persister is not ISqlLoadable: " + entityName);
			}
			return persisterAsSqlLoadable;
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

		private void ProcessScalarReturn(NativeSQLQueryScalarReturn typeReturn) {}

		private void ProcessRootReturn(NativeSQLQueryRootReturn rootReturn)
		{
			if (alias2Persister.ContainsKey(rootReturn.Alias))
			{
				// already been processed...
				return;
			}

			ISqlLoadable persister = GetSQLLoadable(rootReturn.ReturnEntityName);
			AddPersister(rootReturn.Alias, rootReturn.PropertyResultsMap, persister);
		}

		private void AddPersister(string alias, IDictionary<string, string[]> propertyResult, ISqlLoadable persister)
		{
			alias2Persister[alias] = persister;
			string suffix = GenerateEntitySuffix();
			log.Debug("mapping alias [" + alias + "] to entity-suffix [" + suffix + "]");
			alias2Suffix[alias] = suffix;
			entityPropertyResultMaps[alias] = propertyResult;
		}

		private void AddCollection(string role, string alias, IDictionary<string, string[]> propertyResults)
		{
			ISqlLoadableCollection collectionPersister = (ISqlLoadableCollection) Factory.GetCollectionPersister(role);
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

		private IDictionary<string, string[]> Filter(IDictionary<string, string[]> propertyResults)
		{
			Dictionary<string, string[]> result = new Dictionary<string, string[]>(propertyResults.Count);

			string keyPrefix = "element.";

			foreach (KeyValuePair<string, string[]> element in propertyResults)
			{
				string path = element.Key;
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
			AddCollection(role, collectionReturn.Alias, collectionReturn.PropertyResultsMap);
		}

		private void ProcessJoinReturn(NativeSQLQueryJoinReturn fetchReturn)
		{
			string alias = fetchReturn.Alias;
			if (alias2Persister.ContainsKey(alias) || alias2CollectionPersister.ContainsKey(alias))
			{
				// already been processed...
				return;
			}

			string ownerAlias = fetchReturn.OwnerAlias;

			// Make sure the owner alias is known...
			if (!alias2Return.ContainsKey(ownerAlias))
			{
				throw new HibernateException(string.Format("Owner alias [{0}] is unknown for alias [{1}]", ownerAlias, alias));
			}

			// If this return's alias has not been processed yet, do so b4 further processing of this return
			if (!alias2Persister.ContainsKey(ownerAlias))
			{
				ProcessReturn(alias2Return[ownerAlias]);
			}

			ISqlLoadable ownerPersister = alias2Persister[ownerAlias];
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
			IList customReturns = new List<object>();
			IDictionary<string, object> customReturnsByAlias = new Dictionary<string, object>();
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
						entityAliases =
							new DefaultEntityAliases(entityPropertyResultMaps[alias], alias2Persister[alias], alias2Suffix[alias]);
					}
					else
					{
						entityAliases =
							new ColumnEntityAliases(entityPropertyResultMaps[alias], alias2Persister[alias], alias2Suffix[alias]);
					}
					RootReturn customReturn = new RootReturn(alias, rtn.ReturnEntityName, entityAliases, rtn.LockMode);
					customReturns.Add(customReturn);
					customReturnsByAlias[rtn.Alias] = customReturn;
				}
				else if (queryReturns[i] is NativeSQLQueryCollectionReturn)
				{
					NativeSQLQueryCollectionReturn rtn = (NativeSQLQueryCollectionReturn) queryReturns[i];
					string alias = rtn.Alias;
					ISqlLoadableCollection persister = alias2CollectionPersister[alias];
					bool isEntityElements = persister.ElementType.IsEntityType;
					ICollectionAliases collectionAliases;
					IEntityAliases elementEntityAliases = null;
					if (queryHadAliases || HasPropertyResultMap(alias))
					{
						collectionAliases =
							new GeneratedCollectionAliases(collectionPropertyResultMaps[alias], alias2CollectionPersister[alias],
														   alias2CollectionSuffix[alias]);
						if (isEntityElements)
						{
							elementEntityAliases =
								new DefaultEntityAliases(entityPropertyResultMaps[alias], alias2Persister[alias], alias2Suffix[alias]);
						}
					}
					else
					{
						collectionAliases =
							new ColumnCollectionAliases(collectionPropertyResultMaps[alias], alias2CollectionPersister[alias]);
						if (isEntityElements)
						{
							elementEntityAliases =
								new ColumnEntityAliases(entityPropertyResultMaps[alias], alias2Persister[alias], alias2Suffix[alias]);
						}
					}
					CollectionReturn customReturn =
						new CollectionReturn(alias, rtn.OwnerEntityName, rtn.OwnerProperty, collectionAliases, elementEntityAliases,
											 rtn.LockMode);
					customReturns.Add(customReturn);
					customReturnsByAlias[rtn.Alias] = customReturn;
				}
				else if (queryReturns[i] is NativeSQLQueryJoinReturn)
				{
					NativeSQLQueryJoinReturn rtn = (NativeSQLQueryJoinReturn) queryReturns[i];
					string alias = rtn.Alias;
					FetchReturn customReturn;
					NonScalarReturn ownerCustomReturn = (NonScalarReturn) customReturnsByAlias[rtn.OwnerAlias];
					ISqlLoadableCollection persister;
					if (alias2CollectionPersister.TryGetValue(alias, out persister))
					{
						bool isEntityElements = persister.ElementType.IsEntityType;
						ICollectionAliases collectionAliases;
						IEntityAliases elementEntityAliases = null;
						if (queryHadAliases || HasPropertyResultMap(alias))
						{
							collectionAliases =
								new GeneratedCollectionAliases(collectionPropertyResultMaps[alias], persister, alias2CollectionSuffix[alias]);
							if (isEntityElements)
							{
								elementEntityAliases =
									new DefaultEntityAliases(entityPropertyResultMaps[alias], alias2Persister[alias], alias2Suffix[alias]);
							}
						}
						else
						{
							collectionAliases = new ColumnCollectionAliases(collectionPropertyResultMaps[alias], persister);
							if (isEntityElements)
							{
								elementEntityAliases =
									new ColumnEntityAliases(entityPropertyResultMaps[alias], alias2Persister[alias], alias2Suffix[alias]);
							}
						}
						customReturn =
							new CollectionFetchReturn(alias, ownerCustomReturn, rtn.OwnerProperty, collectionAliases, elementEntityAliases,
													  rtn.LockMode);
					}
					else 
					{
						IEntityAliases entityAliases;
						if (queryHadAliases || HasPropertyResultMap(alias))
						{
							entityAliases =
								new DefaultEntityAliases(entityPropertyResultMaps[alias], alias2Persister[alias], alias2Suffix[alias]);
						}
						else
						{
							entityAliases =
								new ColumnEntityAliases(entityPropertyResultMaps[alias], alias2Persister[alias], alias2Suffix[alias]);
						}
						customReturn = new EntityFetchReturn(alias, entityAliases, ownerCustomReturn, rtn.OwnerProperty, rtn.LockMode);
					}
					customReturns.Add(customReturn);
					customReturnsByAlias[alias] = customReturn;
				}
			}
			return customReturns;
		}
	}
}