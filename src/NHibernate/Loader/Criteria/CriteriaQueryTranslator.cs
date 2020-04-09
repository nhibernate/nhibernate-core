using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Hql.Util;
using NHibernate.Impl;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate_Persister_Entity = NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;
using IQueryable = NHibernate.Persister.Entity.IQueryable;
using static NHibernate.Impl.CriteriaImpl;

namespace NHibernate.Loader.Criteria
{
	public class CriteriaQueryTranslator : ICriteriaQuery, ISupportEntityProjectionCriteriaQuery
	{
		public class EntityJoinInfo
		{
			public ICriteria Criteria;
			public IQueryable Persister;
		}

		public static readonly string RootSqlAlias = CriteriaSpecification.RootAlias + '_';
		private static readonly INHibernateLogger logger = NHibernateLogger.For(typeof(CriteriaQueryTranslator));
		
		private const int AliasCount = 0;
		
		private readonly ICriteriaQuery outerQueryTranslator;
		private readonly CriteriaImpl rootCriteria;
		private readonly string rootSQLAlias;
		private int indexForAlias = 0;
		private readonly List<EntityProjection> entityProjections = new List<EntityProjection>();

		private readonly Dictionary<ICriteria, ICriteriaInfoProvider> criteriaInfoMap =
			new Dictionary<ICriteria, ICriteriaInfoProvider>();

		private readonly Dictionary<String, ICriteriaInfoProvider> nameCriteriaInfoMap =
			new Dictionary<string, ICriteriaInfoProvider>();

		private readonly HashSet<ICollectionPersister> uncacheableCollectionPersisters = new HashSet<ICollectionPersister>();
		private readonly HashSet<ICollectionPersister> criteriaCollectionPersisters = new HashSet<ICollectionPersister>();
		private readonly Dictionary<ICriteria, string> criteriaSQLAliasMap = new Dictionary<ICriteria, string>();
		private readonly Dictionary<string, string> sqlAliasToCriteriaAliasMap = new Dictionary<string, string>();
		private readonly Dictionary<string, HashSet<string>> associationAliasToChildrenAliasesMap = new Dictionary<string, HashSet<string>>();
		private readonly Dictionary<string, ICriteria> aliasCriteriaMap = new Dictionary<string, ICriteria>();
		private readonly Dictionary<AliasKey, CriteriaImpl.Subcriteria> associationPathCriteriaMap = new Dictionary<AliasKey, CriteriaImpl.Subcriteria>();
		private readonly Dictionary<AliasKey, JoinType> associationPathJoinTypesMap = new Dictionary<AliasKey, JoinType>();
		private readonly Dictionary<AliasKey, ICriterion> withClauseMap = new Dictionary<AliasKey, ICriterion>();
		private readonly ISessionFactoryImplementor sessionFactory;
		private SessionFactoryHelper helper;

		private readonly ICollection<IParameterSpecification> collectedParameterSpecifications;
		private readonly ICollection<NamedParameter> namedParameters;
		private readonly HashSet<string> subQuerySpaces = new HashSet<string>();

		private Dictionary<string, EntityJoinInfo> entityJoins = new Dictionary<string, EntityJoinInfo>();
		private readonly IQueryable rootPersister;

		//Key for the dictionary sub-criteria
		private class AliasKey : IEquatable<AliasKey>
		{
			public AliasKey(string alias, string path)
			{
				Alias = alias;
				Path = path;
			}

			public string Alias { get; }
			public string Path { get; }

			public bool Equals(AliasKey other)
			{
				return other != null && string.Equals(Alias, other.Alias) && string.Equals(Path, other.Path);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ((Alias != null ? Alias.GetHashCode() : 0) * 397) ^ (Path != null ? Path.GetHashCode() : 0);
				}
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(this, obj))
					return true;
				return Equals(obj as AliasKey);
			}

			public override string ToString()
			{
				return $"path: {Path}; alias: {Alias}";
			}
		}

		public CriteriaQueryTranslator(ISessionFactoryImplementor factory, CriteriaImpl criteria, string rootEntityName,
									   string rootSQLAlias, ICriteriaQuery outerQuery)
			: this(factory, criteria, rootEntityName, rootSQLAlias)
		{
			outerQueryTranslator = outerQuery;
			collectedParameterSpecifications = outerQuery.CollectedParameterSpecifications;
			namedParameters = outerQuery.CollectedParameters;
		}

		public CriteriaQueryTranslator(ISessionFactoryImplementor factory, CriteriaImpl criteria, string rootEntityName,
									   string rootSQLAlias)
		{
			rootCriteria = criteria;

			sessionFactory = factory;
			rootPersister = GetQueryablePersister(rootEntityName);
			this.rootSQLAlias = rootSQLAlias;
			helper = new SessionFactoryHelper(factory);

			collectedParameterSpecifications = new List<IParameterSpecification>();
			namedParameters = new List<NamedParameter>();

			CreateAliasCriteriaMap();
			CreateAssociationPathCriteriaMap();
			CreateEntityJoinMap();
			CreateCriteriaEntityNameMap();
			CreateCriteriaCollectionPersisters();
			CreateCriteriaSQLAliasMap();
			CreateSubQuerySpaces();
		}

		[CLSCompliant(false)] // TODO: Why does this cause a problem in 1.1
		public string RootSQLAlias
		{
			get { return rootSQLAlias; }
		}

		public ISet<string> GetQuerySpaces()
		{
			ISet<string> result = new HashSet<string>();

			foreach (ICriteriaInfoProvider info in criteriaInfoMap.Values)
			{
				result.UnionWith(info.Spaces);
			}

			foreach (ICollectionPersister collectionPersister in criteriaCollectionPersisters)
			{
				result.UnionWith(collectionPersister.CollectionSpaces);
			}

			result.UnionWith(subQuerySpaces);

			return result;
		}

		public int SQLAliasCount
		{
			get { return criteriaSQLAliasMap.Count; }
		}

		public CriteriaImpl RootCriteria
		{
			get { return rootCriteria; }
		}

		ICriteria ISupportEntityProjectionCriteriaQuery.RootCriteria => rootCriteria;

		internal IReadOnlyDictionary<string, EntityJoinInfo> GetEntityJoins()
		{
			return entityJoins;
		}

		public QueryParameters GetQueryParameters()
		{
			RowSelection selection = new RowSelection();
			selection.FirstRow = rootCriteria.FirstResult;
			selection.MaxRows = rootCriteria.MaxResults;
			selection.Timeout = rootCriteria.Timeout;
			selection.FetchSize = rootCriteria.FetchSize;

			var lockModes = new Dictionary<string, LockMode>();
			foreach (KeyValuePair<string, LockMode> me in rootCriteria.LockModes)
			{
				ICriteria subcriteria = GetAliasedCriteria(me.Key);
				lockModes[GetSQLAlias(subcriteria)] = me.Value;
			}
			
			foreach (CriteriaImpl.Subcriteria subcriteria in rootCriteria.IterateSubcriteria())
			{
				LockMode lm = subcriteria.LockMode;
				if (lm != null)
				{
					lockModes[GetSQLAlias(subcriteria)] = lm;
				}
			}
			
			IDictionary<string, TypedValue> queryNamedParameters = CollectedParameters.ToDictionary(np => np.Name, np => new TypedValue(np.Type, np.Value));

			return
				new QueryParameters(
					queryNamedParameters,
					lockModes,
					selection,
					rootCriteria.IsReadOnlyInitialized,
					rootCriteria.IsReadOnlyInitialized ? rootCriteria.IsReadOnly : false,
					rootCriteria.Cacheable,
					rootCriteria.CacheRegion,
					rootCriteria.Comment,
					rootCriteria.LookupByNaturalKey,
					rootCriteria.ResultTransformer)
				{
					CacheMode = rootCriteria.CacheMode
				};
		}
		
		public SqlString GetGroupBy()
		{
			if (rootCriteria.Projection.IsGrouped)
			{
				return rootCriteria.Projection.ToGroupSqlString(rootCriteria.ProjectionCriteria, this);
			}
			else
			{
				return SqlString.Empty;
			}
		}

		public SqlString GetSelect()
		{
			return rootCriteria.Projection.ToSqlString(rootCriteria.ProjectionCriteria, 0, this);
		}

		internal IType ResultType(ICriteria criteria)
		{
			return TypeFactory.ManyToOne(GetEntityName(criteria));
			//return Factory.getTypeResolver().getTypeFactory().manyToOne(getEntityName(criteria));
		}

		public IType[] ProjectedTypes
		{
			get { return rootCriteria.Projection.GetTypes(rootCriteria, this); }
		}

		public string[] ProjectedColumnAliases
		{
			get { return rootCriteria.Projection.GetColumnAliases(0, rootCriteria, this); }
		}

		public string[] ProjectedAliases
		{
			get { return rootCriteria.Projection.Aliases; }
		}

		public ISet<ICollectionPersister> UncacheableCollectionPersisters => uncacheableCollectionPersisters;

		public IList<EntityProjection> GetEntityProjections()
		{
			return entityProjections;
		}

		public void RegisterEntityProjection(EntityProjection projection)
		{
			entityProjections.Add(projection);
		}

		public SqlString GetWhereCondition()
		{
			SqlStringBuilder condition = new SqlStringBuilder(30);

			bool first = true;
			foreach (CriteriaImpl.CriterionEntry entry in rootCriteria.IterateExpressionEntries())
			{
				if (!HasGroupedOrAggregateProjection(entry.Criterion.GetProjections()))
				{
					if (!first)
					{
						condition.Add(" and ");
					}
					first = false;
					SqlString sqlString = entry.Criterion.ToSqlString(entry.Criteria, this);
					condition.Add(sqlString);
				}
			}
			return condition.ToSqlString();
		}

		public SqlString GetOrderBy()
		{
			SqlStringBuilder orderBy = new SqlStringBuilder(30);

			bool first = true;
			foreach (CriteriaImpl.OrderEntry oe in rootCriteria.IterateOrderings())
			{
				if (!first)
				{
					orderBy.Add(StringHelper.CommaSpace);
				}
				first = false;
				orderBy.Add(oe.Order.ToSqlString(oe.Criteria, this));
			}
			return orderBy.ToSqlString();
		}

		public ISessionFactoryImplementor Factory
		{
			get { return sessionFactory; }
		}

		public string GenerateSQLAlias()
		{
			return StringHelper.GenerateAlias(rootSQLAlias, AliasCount);
		}

		private ICriteria GetAliasedCriteria(string alias)
		{
			ICriteria result;
			aliasCriteriaMap.TryGetValue(alias, out result);
			return result;
		}

		// Since v5.2
		[Obsolete("Use overload with a critAlias additional parameter", true)]
		public bool IsJoin(string path)
		{
			return associationPathCriteriaMap.Keys.Any(k => k.Path == path);
		}

		public bool IsJoin(string path, string critAlias)
		{
			return associationPathCriteriaMap.ContainsKey(new AliasKey(critAlias, path));
		}

		/// <summary>
		/// Returns the child criteria aliases for a parent SQL alias and a child path.
		/// </summary>
		public IReadOnlyCollection<string> GetChildAliases(string parentSqlAlias, string childPath)
		{
			var alias = new List<string>();

			if (!sqlAliasToCriteriaAliasMap.TryGetValue(parentSqlAlias, out var parentAlias))
				parentAlias = rootCriteria.Alias;

			if (!associationAliasToChildrenAliasesMap.TryGetValue(parentAlias, out var children))
				return alias;

			foreach (var child in children)
			{
				if (associationPathJoinTypesMap.ContainsKey(new AliasKey(child, childPath)))
					alias.Add(child);
			}

			return alias;
		}

		// Since v5.2
		[Obsolete("Use overload with a critAlias additional parameter", true)]
		public JoinType GetJoinType(string path)
		{
			return
				associationPathJoinTypesMap
					.Where(kv => kv.Key.Path == path)
					.Select(kv => (JoinType?) kv.Value)
					.SingleOrDefault() ?? JoinType.InnerJoin;
		}

		public JoinType GetJoinType(string path, string critAlias)
		{
			if (associationPathJoinTypesMap.TryGetValue(new AliasKey(critAlias, path), out var result))
				return result;
			return JoinType.InnerJoin;
		}

		// Since v5.2
		[Obsolete("Use overload with a critAlias additional parameter", true)]
		public ICriteria GetCriteria(string path)
		{
			var result =
				associationPathCriteriaMap
					.Where(kv => kv.Key.Path == path)
					.Select(kv => kv.Value)
					.SingleOrDefault();
			logger.Debug("getCriteria for path {0}: crit={1}", path, result);
			return result;
		}

		public ICriteria GetCriteria(string path, string critAlias)
		{
			var key = new AliasKey(critAlias, path);
			associationPathCriteriaMap.TryGetValue(key, out var result);
			logger.Debug("getCriteria for {0}: crit={1}", key, result);
			return result;
		}

		private void CreateAliasCriteriaMap()
		{
			aliasCriteriaMap[rootCriteria.Alias] = rootCriteria;

			foreach (ICriteria subcriteria in rootCriteria.IterateSubcriteria())
			{
				if (subcriteria.Alias != null)
				{
					try
					{
						aliasCriteriaMap.Add(subcriteria.Alias, subcriteria);
					}
					catch (ArgumentException ae)
					{
						throw new QueryException("duplicate alias: " + subcriteria.Alias, ae);
					}
				}
			}
		}

		private void CreateAssociationPathCriteriaMap()
		{
			foreach (var crit in rootCriteria.IterateSubcriteria())
			{
				var wholeAssociationPath = GetWholeAssociationPath(crit, out var parentAlias);
				if (parentAlias == null)
					parentAlias = rootCriteria.Alias;

				if (!associationAliasToChildrenAliasesMap.TryGetValue(parentAlias, out var children))
				{
					children = new HashSet<string>();
					associationAliasToChildrenAliasesMap.Add(parentAlias, children);
				}
				children.Add(crit.Alias);

				var key = new AliasKey(crit.Alias, wholeAssociationPath);
				try
				{
					associationPathCriteriaMap.Add(key, crit);
				}
				catch (ArgumentException ae)
				{
					throw new QueryException("duplicate association path: " + key, ae);
				}

				try
				{
					associationPathJoinTypesMap.Add(key, crit.JoinType);
				}
				catch (ArgumentException ae)
				{
					throw new QueryException("duplicate association path: " + key, ae);
				}

				try
				{
					withClauseMap.Add(key, crit.WithClause);
				}
				catch (ArgumentException ae)
				{
					throw new QueryException("duplicate association path: " + key, ae);
				}
			}
		}

		private string GetWholeAssociationPath(CriteriaImpl.Subcriteria subcriteria, out string parentAlias)
		{
			string path = subcriteria.Path;

			// some messy, complex stuff here, since createCriteria() can take an
			// aliased path, or a path rooted at the creating criteria instance
			ICriteria parent = null;
			if (StringHelper.IsNotRoot(path, out var testAlias))
			{
				// if it is a compound path
				if (!testAlias.Equals(subcriteria.Alias))
				{
					// and the qualifier is not the alias of this criteria
					//      -> check to see if we belong to some criteria other
					//          than the one that created us
					aliasCriteriaMap.TryGetValue(testAlias, out parent);
				}
			}
			if (parent == null)
			{
				// otherwise assume the parent is the the criteria that created us
				parent = subcriteria.Parent;
			}
			else
			{
				path = StringHelper.Unroot(path);
			}

			parentAlias = parent.Alias;

			if (parent.Equals(rootCriteria))
			{
				// if its the root criteria, we are done
				return path;
			}
			else
			{
				// otherwise, recurse
				return GetWholeAssociationPath((CriteriaImpl.Subcriteria) parent, out _) + '.' + path;
			}
		}

		private void CreateCriteriaEntityNameMap()
		{
			// initialize the rootProvider first
			ICriteriaInfoProvider rootProvider = new EntityCriteriaInfoProvider(rootPersister);
			criteriaInfoMap.Add(rootCriteria, rootProvider);
			nameCriteriaInfoMap.Add(rootProvider.Name, rootProvider);

			foreach (var me in associationPathCriteriaMap)
			{
				var info = GetPathInfo(me.Key.Path, rootProvider);
				criteriaInfoMap.Add(me.Value, info);
				nameCriteriaInfoMap[info.Name] = info;
			}
		}

		//explicit joins with not associated entities
		private void CreateEntityJoinMap()
		{
			foreach (var criteria in rootCriteria.IterateSubcriteria())
			{
				if (criteria.IsEntityJoin)
				{
					var entityJoinPersister = GetQueryablePersister(criteria.JoinEntityName);
					entityJoins[criteria.Alias] = new EntityJoinInfo
					{
						Persister = entityJoinPersister,
						Criteria = criteria,
					};
				}
			}
		}

		private void CreateCriteriaCollectionPersisters()
		{
			foreach (var me in associationPathCriteriaMap)
			{
				if (GetPathJoinable(me.Key.Path) is ICollectionPersister collectionPersister)
				{
					criteriaCollectionPersisters.Add(collectionPersister);

					if (collectionPersister.HasCache && me.Value.HasRestrictions)
					{
						uncacheableCollectionPersisters.Add(collectionPersister);
					}
				}
			}
		}

		private Persister.Entity.IJoinable GetPathJoinable(string path)
		{
			// start with the root
			IJoinable last = rootPersister;
			
			var tokens = path.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
			if (tokens.Length == 0)
				return last;
			
			IPropertyMapping lastEntity = rootPersister;
			int i = 0;
			if (entityJoins.TryGetValue(tokens[0], out var entityJoinInfo))
			{
				last = entityJoinInfo.Persister;
				lastEntity = (IPropertyMapping) last;
				i++;
			}

			string componentPath = string.Empty;
			for (; i < tokens.Length; i++)
			{
				componentPath += tokens[i];
				IType type = lastEntity.ToType(componentPath);
				if (type.IsAssociationType)
				{
					if(type.IsCollectionType)
					{
						// ignore joinables for composite collections
						var collectionType = (CollectionType)type;
						var persister = Factory.GetCollectionPersister(collectionType.Role);
						if(persister.ElementType.IsEntityType==false)
							return null;
					}
					IAssociationType atype = (IAssociationType)type;
					
					last = atype.GetAssociatedJoinable(Factory);
					lastEntity = (NHibernate_Persister_Entity.IPropertyMapping)Factory.GetEntityPersister(atype.GetAssociatedEntityName(Factory));
					componentPath = "";
				}
				else if (type.IsComponentType)
				{
					componentPath += '.';
				}
				else
				{
					throw new QueryException("not an association: " + componentPath);
				}
			}
			return last;
		}

		private ICriteriaInfoProvider GetPathInfo(string path, ICriteriaInfoProvider rootProvider)
		{
			var tokens = path.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
			// start with the root
			ICriteriaInfoProvider provider = rootProvider;
			if (tokens.Length == 0)
				return provider;

			int i = 0;
			if (entityJoins.TryGetValue(tokens[0], out var entityJoinInfo))
			{
				provider = new EntityCriteriaInfoProvider(entityJoinInfo.Persister);
				i++;
			}

			string componentPath = string.Empty;
			for (; i < tokens.Length; i++)
			{
				componentPath += tokens[i];
				logger.Debug("searching for {0}", componentPath);
				IType type = provider.GetType(componentPath);
				if (type.IsAssociationType)
				{
					// CollectionTypes are always also AssociationTypes - but there's not always an associated entity...
					IAssociationType atype = (IAssociationType)type;

					CollectionType ctype = type.IsCollectionType ? (CollectionType)type : null;
					IType elementType = (ctype != null) ? ctype.GetElementType(sessionFactory) : null;
					// is the association a collection of components or value-types? (i.e a colloction of valued types?)
					if (ctype != null && elementType.IsComponentType)
					{
						provider = new ComponentCollectionCriteriaInfoProvider(helper.GetCollectionPersister(ctype.Role));
					}
					else if (ctype != null && !elementType.IsEntityType)
					{
						provider = new ScalarCollectionCriteriaInfoProvider(helper, ctype.Role);
					}
					else
					{
						provider = new EntityCriteriaInfoProvider(
							GetQueryablePersister(atype.GetAssociatedEntityName(sessionFactory)));
					}

					componentPath = string.Empty;
				}
				else if (type.IsComponentType)
				{
					componentPath += '.';
				}
				else
				{
					throw new QueryException("not an association: " + componentPath);
				}
			}

			logger.Debug("returning entity name={0} for path={1} class={2}",
				provider.Name, path, provider.GetType().Name);
			return provider;
		}

		private void CreateCriteriaSQLAliasMap()
		{
			int i = 0;

			foreach (KeyValuePair<ICriteria, ICriteriaInfoProvider> me in criteriaInfoMap)
			{
				ICriteria crit = me.Key;
				string alias = crit.Alias;
				if (alias == null)
				{
					alias = me.Value.Name; // the entity name
				}
				var sqlAlias = StringHelper.GenerateAlias(alias, i++);
				criteriaSQLAliasMap[crit] = sqlAlias;
				logger.Debug("put criteria={0} alias={1}", crit, sqlAlias);
				if (!string.IsNullOrEmpty(crit.Alias))
					sqlAliasToCriteriaAliasMap[sqlAlias] = alias;
			}
			criteriaSQLAliasMap[rootCriteria] = rootSQLAlias;
			sqlAliasToCriteriaAliasMap[rootSQLAlias] = rootCriteria.Alias;
		}

		public bool HasProjection
		{
			get { return rootCriteria.Projection != null; }
		}

		public string GetSQLAlias(ICriteria criteria)
		{
			String alias = criteriaSQLAliasMap[criteria];
			logger.Debug("returning alias={0} for criteria={1}", alias, criteria);
			return alias;
		}

		public string GetEntityName(ICriteria criteria)
		{
			ICriteriaInfoProvider result;
			if (criteriaInfoMap.TryGetValue(criteria, out result) == false)
				return null;
			return result.Name;
		}

		public string GetColumn(ICriteria criteria, string propertyName)
		{
			string[] cols = GetColumns(criteria, propertyName);
			if (cols.Length != 1)
			{
				throw new QueryException("property does not map to a single column: " + propertyName);
			}
			return cols[0];
		}

		/// <summary>
		/// Get the names of the columns constrained by this criterion.
		/// </summary>
		public string[] GetColumnsUsingProjection(ICriteria subcriteria, string propertyName)
		{
			// NH Different behavior: we don't use the projection alias for NH-1023
			if (TryGetColumns(subcriteria, propertyName, outerQueryTranslator != null, out var columns))
				return columns;

			//not found in inner query , try the outer query
			if (outerQueryTranslator != null)
				return outerQueryTranslator.GetColumnsUsingProjection(subcriteria, propertyName);

			throw new QueryException("Could not find property " + propertyName);
		}

		public string[] GetIdentifierColumns(ICriteria subcriteria)
		{
			string[] idcols = ((NHibernate_Persister_Entity.ILoadable)GetPropertyMapping(GetEntityName(subcriteria))).IdentifierColumnNames;
			return StringHelper.Qualify(GetSQLAlias(subcriteria), idcols);
		}

		public IType GetIdentifierType(ICriteria subcriteria)
		{
			return ((NHibernate_Persister_Entity.ILoadable)GetPropertyMapping(GetEntityName(subcriteria))).IdentifierType;
		}

		public TypedValue GetTypedIdentifierValue(ICriteria subcriteria, object value)
		{
			NHibernate_Persister_Entity.ILoadable loadable = (NHibernate_Persister_Entity.ILoadable)GetPropertyMapping(GetEntityName(subcriteria));
			return new TypedValue(loadable.IdentifierType, value);
		}

		public string[] GetColumns(ICriteria subcriteria, string propertyName)
		{
			if (TryGetColumns(subcriteria, propertyName, false, out var columns))
				return columns;

			throw new QueryException("Could not find property " + propertyName);
		}

		private bool TryGetColumns(ICriteria subcriteria, string path, bool verifyPropertyName, out string[] columns)
		{
			if (!TryParseCriteriaPath(subcriteria, path, out var entName, out var propertyName, out var pathCriteria))
			{
				columns = null;
				return false;
			}
			var propertyMapping = GetPropertyMapping(entName);

			if (verifyPropertyName && !propertyMapping.TryToType(propertyName, out var type))
			{
				columns = null;
				return false;
			}

			// here we can check if the condition belongs to a with clause
			bool useLastIndex = false;
			var withClause = pathCriteria as Subcriteria != null ? ((Subcriteria) pathCriteria).WithClause as SimpleExpression : null;
			if (withClause != null && withClause.PropertyName == propertyName)
			{
				useLastIndex = true;
			}

			columns = propertyMapping.ToColumns(GetSQLAlias(pathCriteria), propertyName, useLastIndex);
			return true;
		}

		public IType GetTypeUsingProjection(ICriteria subcriteria, string propertyName)
		{
			//first look for a reference to a projection alias
			IProjection projection = rootCriteria.Projection;
			IType[] projectionTypes = projection == null ? null : projection.GetTypes(propertyName, subcriteria, this);

			if (projectionTypes == null)
			{
					//it does not refer to an alias of a projection,
					//look for a property

				if (TryGetType(subcriteria, propertyName, out var type))
				{
					return type;
				}
				if (outerQueryTranslator != null)
				{
					return outerQueryTranslator.GetTypeUsingProjection(subcriteria, propertyName);
				}
				throw new QueryException("Could not find property " + propertyName);
			}
			else
			{
				if (projectionTypes.Length != 1)
				{
					//should never happen, i think
					throw new QueryException("not a single-length projection: " + propertyName);
				}
				return projectionTypes[0];
			}
		}

		public IType GetType(ICriteria subcriteria, string propertyName)
		{
			if(!TryParseCriteriaPath(subcriteria, propertyName, out var entityName, out var entityPropName, out _))
				throw new QueryException("Could not find property " + propertyName);

			return GetPropertyMapping(entityName).ToType(entityPropName);
		}

		public bool TryGetType(ICriteria subcriteria, string propertyName, out IType type)
		{
			if (!TryParseCriteriaPath(subcriteria, propertyName, out var entityName, out var entityPropName, out _))
			{
				type = null;
				return false;
			}

			return GetPropertyMapping(entityName).TryToType(entityPropName, out type);
		}

		/// <summary>
		/// Get the a typed value for the given property value.
		/// </summary>
		public TypedValue GetTypedValue(ICriteria subcriteria, string propertyName, object value)
		{
			// Detect discriminator values...
			var entityClass = value as System.Type;
			if (entityClass != null)
			{
				NHibernate_Persister_Entity.IQueryable q = helper.FindQueryableUsingImports(entityClass.FullName);

				if (q != null && q.DiscriminatorValue != null)
				{
					// NH Different implementation : We are using strongly typed parameter for SQL query (see DiscriminatorValue comment)
					return new TypedValue(q.DiscriminatorType, q.DiscriminatorValue);
				}
			}
			// Otherwise, this is an ordinary value.
			return new TypedValue(GetTypeUsingProjection(subcriteria, propertyName), value);
		}

		private Persister.Entity.IPropertyMapping GetPropertyMapping(string entityName)
		{
			ICriteriaInfoProvider info;
			if (nameCriteriaInfoMap.TryGetValue(entityName, out info) == false)
				throw new InvalidOperationException("Could not find criteria info provider for: " + entityName);
			return info.PropertyMapping;
		}

		public string GetEntityName(ICriteria subcriteria, string propertyName)
		{
			if (StringHelper.IsNotRoot(propertyName, out var root))
			{
				ICriteria crit = GetAliasedCriteria(root);
				if (crit != null)
				{
					return GetEntityName(crit);
				}
			}
			return GetEntityName(subcriteria);
		}

		public string GetSQLAlias(ICriteria criteria, string propertyName)
		{
			if (StringHelper.IsNotRoot(propertyName, out var root))
			{
				ICriteria subcriteria = GetAliasedCriteria(root);
				if (subcriteria != null)
				{
					return GetSQLAlias(subcriteria);
				}
			}
			return GetSQLAlias(criteria);
		}

		public string GetPropertyName(string propertyName)
		{
			if (StringHelper.IsNotRoot(propertyName, out var root))
			{
				ICriteria crit = GetAliasedCriteria(root);
				if (crit != null)
				{
					return propertyName.Substring(root.Length + 1);
				}
			}
			return propertyName;
		}

		// Since v5.2
		[Obsolete("Use overload with a critAlias additional parameter", true)]
		public SqlString GetWithClause(string path)
		{
			var crit =
				withClauseMap
					.Where(kv => kv.Key.Path == path)
					.Select(kv => kv.Value)
					.SingleOrDefault();

			return crit?.ToSqlString(GetCriteria(path), this);
		}

		public SqlString GetWithClause(string path, string pathAlias)
		{
			var key = new AliasKey(pathAlias, path);
			withClauseMap.TryGetValue(key, out var crit);

			return crit?.ToSqlString(GetCriteria(path, key.Alias), this);
		}

		#region NH specific

		public int GetIndexForAlias()
		{
			return indexForAlias++;
		}

		public IEnumerable<Parameter> NewQueryParameter(TypedValue parameter)
		{
			// the queryTranslatorId is to avoid possible conflicts using sub-queries
			const string parameterPrefix = "cp";
			return NewQueryParameter(parameterPrefix, parameter);
		}

		private IEnumerable<Parameter> NewQueryParameter(string parameterPrefix, TypedValue parameter)
		{
			string parameterName = parameterPrefix + CollectedParameterSpecifications.Count;
			var specification = new CriteriaNamedParameterSpecification(parameterName, parameter.Type);
			collectedParameterSpecifications.Add(specification);
			namedParameters.Add(new NamedParameter(parameterName, parameter.Value, parameter.Type));
			return specification.GetIdsForBackTrack(Factory).Select(x =>
																	{
																		Parameter p = Parameter.Placeholder;
																		p.BackTrack = x;
																		return p;
																	});
		}

		public ICollection<IParameterSpecification> CollectedParameterSpecifications
		{
			get
			{
				return collectedParameterSpecifications;
			}
		}

		public ICollection<NamedParameter> CollectedParameters
		{
			get
			{
				return namedParameters;
			}
		}

		public Parameter CreateSkipParameter(int value)
		{
			var typedValue = new TypedValue(NHibernateUtil.Int32, value, false);
			return NewQueryParameter("skip_", typedValue).Single();
		}
		
		public Parameter CreateTakeParameter(int value)
		{
			var typedValue = new TypedValue(NHibernateUtil.Int32, value, false);
			return NewQueryParameter("take_",typedValue).Single();
		}

		public SqlString GetHavingCondition()
		{
			SqlStringBuilder condition = new SqlStringBuilder(30);
			bool first = true;
			foreach (CriteriaImpl.CriterionEntry entry in rootCriteria.IterateExpressionEntries())
			{
				if (HasGroupedOrAggregateProjection(entry.Criterion.GetProjections()))
				{
					if (!first)
					{
						condition.Add(" and ");
					}
					first = false;
					SqlString sqlString = entry.Criterion.ToSqlString(entry.Criteria, this);
					condition.Add(sqlString);
				}
			}
			return condition.ToSqlString();
		}

		protected static bool HasGroupedOrAggregateProjection(IProjection[] projections)
		{
			if (projections != null)
			{
				foreach (IProjection projection in projections)
				{
					if (projection.IsGrouped || projection.IsAggregate)
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Get the aliases of the columns constrained
		/// by this criterion (for use in ORDER BY clause).
		/// </summary>
		public string[] GetColumnAliasesUsingProjection(ICriteria subcriteria, string propertyName)
		{
			//first look for a reference to a projection alias
			IProjection projection = rootCriteria.Projection;
			string[] projectionColumns = null;

			if (projection != null)
				projectionColumns = projection.GetColumnAliases(propertyName, 0, subcriteria, this);

			if (projectionColumns == null)
			{
				//it does not refer to an alias of a projection,
				//look for a property
				return GetColumnsUsingProjection(subcriteria, propertyName);
			}
			else
			{
				//it refers to an alias of a projection
				return projectionColumns;
			}
		}

		#endregion
		
		private void CreateSubQuerySpaces()
		{
			var subQueries =
				rootCriteria.IterateExpressionEntries()
				            .Select(x => x.Criterion)
				            .OfType<SubqueryExpression>()
				            .Select(x => x.Criteria)
				            .OfType<CriteriaImpl>();

			foreach (var criteriaImpl in subQueries)
			{
				//The RootSqlAlias is not relevant, since we're only retreiving the query spaces
				var translator = new CriteriaQueryTranslator(sessionFactory, criteriaImpl, criteriaImpl.EntityOrClassName, RootSqlAlias);
				subQuerySpaces.UnionWith(translator.GetQuerySpaces());
			}
		}

		private IQueryable GetQueryablePersister(string entityName)
		{
			return (IQueryable) sessionFactory.GetEntityPersister(entityName);
		}

		private bool TryParseCriteriaPath(ICriteria subcriteria, string path, out string entityName, out string propertyName, out ICriteria pathCriteria)
		{
			if(StringHelper.IsNotRoot(path, out var root, out var unrootPath))
			{
				ICriteria crit = GetAliasedCriteria(root);
				if (crit != null)
				{
					propertyName = unrootPath;
					entityName = GetEntityName(crit);
					pathCriteria = crit;
					return entityName != null;
				}
			}
			pathCriteria = subcriteria;
			propertyName = path;
			entityName = GetEntityName(subcriteria);
			return entityName != null;
		}
	}
}
