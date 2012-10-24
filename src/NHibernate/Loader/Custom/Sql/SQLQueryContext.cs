using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Query.Sql;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Loader.Custom.Sql
{
	/// <summary>
	/// Provides mappings from entity and collection aliases to persisters, suffixes 
	/// and custom property result maps.
	/// </summary>
	internal class SQLQueryContext
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof (SQLQueryContext));

		#region Instance fields

		private readonly Dictionary<string, INativeSQLQueryReturn> alias2Return =
			new Dictionary<string, INativeSQLQueryReturn>();

		private readonly Dictionary<string, string> alias2OwnerAlias = new Dictionary<string, string>();

		private readonly Dictionary<string, ISqlLoadable> alias2EntityPersister = new Dictionary<string, ISqlLoadable>();
		private readonly Dictionary<string, string> alias2EntitySuffix = new Dictionary<string, string>();
		private readonly Dictionary<string, IDictionary<string, string[]>> entityPropertyResultMaps =
			new Dictionary<string, IDictionary<string, string[]>>();

		private readonly Dictionary<string, ISqlLoadableCollection> alias2CollectionPersister =
			new Dictionary<string, ISqlLoadableCollection>();
		private readonly Dictionary<string, string> alias2CollectionSuffix = new Dictionary<string, string>();
		private readonly Dictionary<string, IDictionary<string, string[]>> collectionPropertyResultMaps =
			new Dictionary<string, IDictionary<string, string[]>>();

		private readonly ISessionFactoryImplementor factory;

		private int entitySuffixSeed;
		private int collectionSuffixSeed;

		#endregion

		#region .ctor

		public SQLQueryContext(INativeSQLQueryReturn[] queryReturns, ISessionFactoryImplementor factory)
		{
			this.factory = factory;

			// first, break down the returns into maps keyed by alias
			// so that role returns can be more easily resolved to their owners
			foreach (INativeSQLQueryReturn rtn in queryReturns)
			{
				var nonScalarRtn = rtn as NativeSQLQueryNonScalarReturn;
				if (nonScalarRtn != null)
				{
					alias2Return[nonScalarRtn.Alias] = rtn;
					var joinRtn = rtn as NativeSQLQueryJoinReturn;
					if (joinRtn != null)
					{
						alias2OwnerAlias[joinRtn.Alias] = joinRtn.OwnerAlias;
					}
				}
			}

			// Now, process the returns
			foreach (INativeSQLQueryReturn rtn in queryReturns)
			{
				ProcessReturn(rtn);
			}
		}

		#endregion

		#region ISQLQueryAliasContext implementation

		public bool IsEntityAlias(string alias)
		{
			return alias2EntityPersister.ContainsKey(alias);
		}

		public ISqlLoadable GetEntityPersister(string alias)
		{
			ISqlLoadable result;
			alias2EntityPersister.TryGetValue(alias, out result);
			return result;
		}

		public string GetEntitySuffix(string alias)
		{
			string result;
			alias2EntitySuffix.TryGetValue(alias, out result);
			return result;
		}

		public IDictionary<string, string[]> GetEntityPropertyResultsMap(string alias)
		{
			IDictionary<string, string[]> result;
			entityPropertyResultMaps.TryGetValue(alias, out result);
			return result;
		}

		public bool IsCollectionAlias(string alias)
		{
			return alias2CollectionPersister.ContainsKey(alias);
		}

		public ISqlLoadableCollection GetCollectionPersister(string alias)
		{
			ISqlLoadableCollection result;
			alias2CollectionPersister.TryGetValue(alias, out result);
			return result;
		}

		public string GetCollectionSuffix(string alias)
		{
			string result;
			alias2CollectionSuffix.TryGetValue(alias, out result);
			return result;
		}

		public IDictionary<string, string[]> GetCollectionPropertyResultsMap(string alias)
		{
			IDictionary<string, string[]> result;
			collectionPropertyResultMaps.TryGetValue(alias, out result);
			return result;
		}

		#endregion

		#region Private methods

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
			if (alias2EntityPersister.ContainsKey(rootReturn.Alias))
			{
				// already been processed...
				return;
			}

			ISqlLoadable persister = GetSQLLoadable(rootReturn.ReturnEntityName);
			AddPersister(rootReturn.Alias, rootReturn.PropertyResultsMap, persister);
		}

		private void AddPersister(string alias, IDictionary<string, string[]> propertyResultMap, ISqlLoadable persister)
		{
			alias2EntityPersister[alias] = persister;
			string suffix = GenerateEntitySuffix();
			log.Debug("mapping alias [" + alias + "] to entity-suffix [" + suffix + "]");
			alias2EntitySuffix[alias] = suffix;
			if (propertyResultMap != null && propertyResultMap.Count > 0)
			{
				entityPropertyResultMaps[alias] = propertyResultMap;
			}
		}

		private void AddCollection(string role, string alias, IDictionary<string, string[]> propertyResultMap)
		{
			ISqlLoadableCollection collectionPersister = (ISqlLoadableCollection) factory.GetCollectionPersister(role);
			alias2CollectionPersister[alias] = collectionPersister;
			string suffix = GenerateCollectionSuffix();
			log.Debug("mapping alias [" + alias + "] to collection-suffix [" + suffix + "]");
			alias2CollectionSuffix[alias] = suffix;
			if (propertyResultMap != null && propertyResultMap.Count > 0)
			{
				collectionPropertyResultMaps[alias] = propertyResultMap;
			}

			if (collectionPersister.IsOneToMany)
			{
				var persister = (ISqlLoadable)collectionPersister.ElementPersister;
				AddPersister(alias, FilterElementProperties(propertyResultMap), persister);
			}
		}

		private IDictionary<string, string[]> FilterElementProperties(IDictionary<string, string[]> propertyResults)
		{
			const string PREFIX = "element.";

			var result = new Dictionary<string, string[]>(propertyResults.Count);
			foreach (KeyValuePair<string, string[]> element in propertyResults)
			{
				string path = element.Key;
				if (path.StartsWith(PREFIX))
				{
					result[path.Substring(PREFIX.Length)] = element.Value;
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
			if (alias2EntityPersister.ContainsKey(alias) || alias2CollectionPersister.ContainsKey(alias))
			{
				// already been processed...
				return;
			}

			string ownerAlias = fetchReturn.OwnerAlias;

			// Make sure the owner alias is known...
			INativeSQLQueryReturn ownerReturn;
			if (!alias2Return.TryGetValue(ownerAlias, out ownerReturn))
			{
				throw new HibernateException(string.Format("Owner alias [{0}] is unknown for alias [{1}]", ownerAlias, alias));
			}

			// If this return's alias has not been processed yet, do so b4 further processing of this return
			if (!alias2EntityPersister.ContainsKey(ownerAlias))
			{
				ProcessReturn(ownerReturn);
			}

			var ownerPersister = alias2EntityPersister[ownerAlias];
			var returnType = ownerPersister.GetPropertyType(fetchReturn.OwnerProperty);

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

		#endregion
	}
}
