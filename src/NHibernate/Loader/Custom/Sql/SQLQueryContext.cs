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
		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof (SQLQueryContext));

		#region Instance fields

		private readonly Dictionary<string, INativeSQLQueryReturn> _alias2Return =
			new Dictionary<string, INativeSQLQueryReturn>();

		private readonly Dictionary<string, ISqlLoadable> _alias2EntityPersister = new Dictionary<string, ISqlLoadable>();
		private readonly Dictionary<string, string> _alias2EntitySuffix = new Dictionary<string, string>();
		private readonly Dictionary<string, IDictionary<string, string[]>> _entityPropertyResultMaps =
			new Dictionary<string, IDictionary<string, string[]>>();

		private readonly Dictionary<string, ISqlLoadableCollection> _alias2CollectionPersister =
			new Dictionary<string, ISqlLoadableCollection>();
		private readonly Dictionary<string, string> _alias2CollectionSuffix = new Dictionary<string, string>();
		private readonly Dictionary<string, IDictionary<string, string[]>> _collectionPropertyResultMaps =
			new Dictionary<string, IDictionary<string, string[]>>();

		private readonly ISessionFactoryImplementor _factory;

		private int _entitySuffixSeed;
		private int _collectionSuffixSeed;

		#endregion

		#region .ctor

		public SQLQueryContext(INativeSQLQueryReturn[] queryReturns, ISessionFactoryImplementor factory)
		{
			_factory = factory;

			// first, break down the returns into maps keyed by alias
			// so that role returns can be more easily resolved to their owners
			foreach (var rtn in queryReturns)
			{
				if (rtn is NativeSQLQueryNonScalarReturn nonScalarRtn)
				{
					_alias2Return[nonScalarRtn.Alias] = rtn;
				}
			}

			// Now, process the returns
			foreach (var rtn in queryReturns)
			{
				ProcessReturn(rtn);
			}
		}

		#endregion

		#region ISQLQueryAliasContext implementation

		public bool IsEntityAlias(string alias)
		{
			return _alias2EntityPersister.ContainsKey(alias);
		}

		public ISqlLoadable GetEntityPersister(string alias)
		{
			_alias2EntityPersister.TryGetValue(alias, out var result);
			return result;
		}

		public string GetEntitySuffix(string alias)
		{
			_alias2EntitySuffix.TryGetValue(alias, out var result);
			return result;
		}

		public IDictionary<string, string[]> GetEntityPropertyResultsMap(string alias)
		{
			_entityPropertyResultMaps.TryGetValue(alias, out var result);
			return result;
		}

		public bool IsCollectionAlias(string alias)
		{
			return _alias2CollectionPersister.ContainsKey(alias);
		}

		public ISqlLoadableCollection GetCollectionPersister(string alias)
		{
			_alias2CollectionPersister.TryGetValue(alias, out var result);
			return result;
		}

		public string GetCollectionSuffix(string alias)
		{
			_alias2CollectionSuffix.TryGetValue(alias, out var result);
			return result;
		}

		public IDictionary<string, string[]> GetCollectionPropertyResultsMap(string alias)
		{
			_collectionPropertyResultMaps.TryGetValue(alias, out var result);
			return result;
		}

		#endregion

		#region Private methods

		private ISqlLoadable GetSQLLoadable(string entityName)
		{
			var persister = _factory.GetEntityPersister(entityName);
			if (!(persister is ISqlLoadable persisterAsSqlLoadable))
			{
				throw new MappingException("class persister is not ISqlLoadable: " + entityName);
			}
			return persisterAsSqlLoadable;
		}

		private string GenerateEntitySuffix()
		{
			return BasicLoader.GenerateSuffixes(_entitySuffixSeed++, 1)[0];
		}

		private string GenerateCollectionSuffix()
		{
			return _collectionSuffixSeed++ + "__";
		}

		private void ProcessReturn(INativeSQLQueryReturn rtn)
		{
			switch (rtn)
			{
				case NativeSQLQueryScalarReturn _:
					break;
				case NativeSQLQueryRootReturn root:
					ProcessRootReturn(root);
					break;
				case NativeSQLQueryCollectionReturn collection:
					ProcessCollectionReturn(collection);
					break;
				default:
					ProcessJoinReturn((NativeSQLQueryJoinReturn) rtn);
					break;
			}
		}

		private void ProcessRootReturn(NativeSQLQueryRootReturn rootReturn)
		{
			if (_alias2EntityPersister.ContainsKey(rootReturn.Alias))
			{
				// already been processed...
				return;
			}

			var persister = GetSQLLoadable(rootReturn.ReturnEntityName);
			AddPersister(rootReturn.Alias, rootReturn.PropertyResultsMap, persister);
		}

		private void AddPersister(string alias, IDictionary<string, string[]> propertyResultMap, ISqlLoadable persister)
		{
			_alias2EntityPersister[alias] = persister;
			var suffix = GenerateEntitySuffix();
			Log.Debug("mapping alias [" + alias + "] to entity-suffix [" + suffix + "]");
			_alias2EntitySuffix[alias] = suffix;
			if (propertyResultMap != null && propertyResultMap.Count > 0)
			{
				_entityPropertyResultMaps[alias] = GroupComponentAliases(propertyResultMap, persister);
			}
		}

		private void AddCollection(string role, string alias, IDictionary<string, string[]> propertyResultMap)
		{
			var collectionPersister = (ISqlLoadableCollection) _factory.GetCollectionPersister(role);
			_alias2CollectionPersister[alias] = collectionPersister;
			var suffix = GenerateCollectionSuffix();
			Log.Debug("mapping alias [" + alias + "] to collection-suffix [" + suffix + "]");
			_alias2CollectionSuffix[alias] = suffix;

			if (propertyResultMap != null && propertyResultMap.Count > 0)
			{
				_collectionPropertyResultMaps[alias] = FilterCollectionProperties(propertyResultMap);
			}
			if (collectionPersister.IsOneToMany)
			{
				var persister = (ISqlLoadable) collectionPersister.ElementPersister;
				AddPersister(alias, FilterElementProperties(propertyResultMap), persister);
			}
		}

		private static IDictionary<string, string[]> FilterCollectionProperties(IDictionary<string, string[]> propertyResults)
		{
			if (propertyResults.Count == 0) return propertyResults;

			var result = new Dictionary<string, string[]>(propertyResults.Count);
			foreach (var element in propertyResults)
			{
				if (element.Key.IndexOf('.') < 0)
				{
					result.Add(element.Key, element.Value);
				}
			}
			return result;
		}

		private static IDictionary<string, string[]> FilterElementProperties(IDictionary<string, string[]> propertyResults)
		{
			const string prefix = "element.";

			if (propertyResults.Count == 0) return propertyResults;

			var result = new Dictionary<string, string[]>(propertyResults.Count);
			foreach (var element in propertyResults)
			{
				var path = element.Key;
				if (path.StartsWith(prefix))
				{
					result.Add(path.Substring(prefix.Length), element.Value);
				}
			}
			return result;
		}

		private IDictionary<string, string[]> GroupComponentAliases(IDictionary<string, string[]> propertyResults, ILoadable persister)
		{
			if (propertyResults.Count == 0) return propertyResults;

			var result = new Dictionary<string, string[]>(propertyResults.Count);

			foreach (var propertyResult in propertyResults)
			{
				var path = propertyResult.Key;
				var dotIndex = path.IndexOf('.');
				if (dotIndex >= 0)
				{
					var propertyPath = path.Substring(0, dotIndex);
					if (!result.ContainsKey(propertyPath))
					{
						var aliases = GetUserProvidedAliases(propertyResults, propertyPath, persister.GetPropertyType(propertyPath));
						if (aliases != null) result.Add(propertyPath, aliases);
					}
					continue;
				}
				result.Add(propertyResult.Key, propertyResult.Value);
			}
			return result;
		}

		private string[] GetUserProvidedAliases(IDictionary<string, string[]> propertyResults, string propertyPath, IType propertyType)
		{
			if (propertyResults.TryGetValue(propertyPath, out var result))
				return result;

			var aliases = new List<string>();
			AppendUserProvidedAliases(propertyResults, propertyPath, propertyType, aliases);
			return aliases.Count > 0 
				? aliases.ToArray() 
				: null;
		}

		private void AppendUserProvidedAliases(IDictionary<string, string[]> propertyResults, string propertyPath, IType propertyType, List<string> result)
		{
			if (propertyResults.TryGetValue(propertyPath, out var aliases))
			{
				result.AddRange(aliases);
				return;
			}

			// TODO: throw exception when no mapping is found for property name
			if (!(propertyType is IAbstractComponentType componentType))
				return;

			for (var i = 0; i < componentType.PropertyNames.Length; i++)
			{
				AppendUserProvidedAliases(propertyResults, propertyPath + '.' + componentType.PropertyNames[i], componentType.Subtypes[i], result);
			}
		}

		private void ProcessCollectionReturn(NativeSQLQueryCollectionReturn collectionReturn)
		{
			// we are initializing an owned collection
			var role = collectionReturn.OwnerEntityName + '.' + collectionReturn.OwnerProperty;
			AddCollection(role, collectionReturn.Alias, collectionReturn.PropertyResultsMap);
		}

		private void ProcessJoinReturn(NativeSQLQueryJoinReturn fetchReturn)
		{
			var alias = fetchReturn.Alias;
			if (_alias2EntityPersister.ContainsKey(alias) || _alias2CollectionPersister.ContainsKey(alias))
			{
				// already been processed...
				return;
			}

			var ownerAlias = fetchReturn.OwnerAlias;

			// Make sure the owner alias is known...
			if (!_alias2Return.TryGetValue(ownerAlias, out var ownerReturn))
			{
				throw new HibernateException($"Owner alias [{ownerAlias}] is unknown for alias [{alias}]");
			}

			// If this return's alias has not been processed yet, do so before further processing of this return
			if (!_alias2EntityPersister.ContainsKey(ownerAlias))
			{
				ProcessReturn(ownerReturn);
			}

			var ownerPersister = _alias2EntityPersister[ownerAlias];
			var returnType = ownerPersister.GetPropertyType(fetchReturn.OwnerProperty);

			if (returnType.IsCollectionType)
			{
				var role = ownerPersister.EntityName + '.' + fetchReturn.OwnerProperty;
				AddCollection(role, alias, fetchReturn.PropertyResultsMap);
			}
			else if (returnType.IsEntityType)
			{
				var eType = (EntityType) returnType;
				var returnEntityName = eType.GetAssociatedEntityName();
				var persister = GetSQLLoadable(returnEntityName);
				AddPersister(alias, fetchReturn.PropertyResultsMap, persister);
			}
		}

		#endregion
	}
}
