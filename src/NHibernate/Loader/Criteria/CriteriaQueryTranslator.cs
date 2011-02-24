using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iesi.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Hql.Util;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate_Persister_Entity = NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Criteria
{
	public class CriteriaQueryTranslator : ICriteriaQuery
	{
		public static readonly string RootSqlAlias = CriteriaSpecification.RootAlias + '_';

		private static readonly IInternalLogger logger = LoggerProvider.LoggerFor(typeof(CriteriaQueryTranslator));
		
		private const int AliasCount = 0;
		
		private readonly ICriteriaQuery outerQueryTranslator;
		private readonly CriteriaImpl rootCriteria;
		private readonly string rootEntityName;
		private readonly string rootSQLAlias;
		private int indexForAlias = 0;		
		private int _tempPagingParameterIndex = -1;
		private IDictionary<int, int> _tempPagingParameterIndexes = new Dictionary<int, int>();

		private readonly IDictionary<ICriteria, ICriteriaInfoProvider> criteriaInfoMap =
			new Dictionary<ICriteria, ICriteriaInfoProvider>();

		private readonly IDictionary<String, ICriteriaInfoProvider> nameCriteriaInfoMap =
			new Dictionary<string, ICriteriaInfoProvider>();

		private readonly ISet<ICollectionPersister> criteriaCollectionPersisters = new HashedSet<ICollectionPersister>();
		private readonly IDictionary<ICriteria, string> criteriaSQLAliasMap = new Dictionary<ICriteria, string>();
		private readonly IDictionary<string, ICriteria> aliasCriteriaMap = new Dictionary<string, ICriteria>();
		private readonly IDictionary<string, ICriteria> associationPathCriteriaMap = new LinkedHashMap<string, ICriteria>();
		private readonly IDictionary<string, JoinType> associationPathJoinTypesMap = new LinkedHashMap<string, JoinType>();
		private readonly IDictionary<string, ICriterion> withClauseMap = new Dictionary<string, ICriterion>();
		private readonly ISessionFactoryImplementor sessionFactory;
		private SessionFactoryHelper helper;

		public CriteriaQueryTranslator(ISessionFactoryImplementor factory, CriteriaImpl criteria, string rootEntityName,
									   string rootSQLAlias, ICriteriaQuery outerQuery)
			: this(factory, criteria, rootEntityName, rootSQLAlias)
		{
			outerQueryTranslator = outerQuery;
		}

		public CriteriaQueryTranslator(ISessionFactoryImplementor factory, CriteriaImpl criteria, string rootEntityName,
									   string rootSQLAlias)
		{
			rootCriteria = criteria;
			this.rootEntityName = rootEntityName;
			sessionFactory = factory;
			this.rootSQLAlias = rootSQLAlias;
			helper = new SessionFactoryHelper(factory);

			CreateAliasCriteriaMap();
			CreateAssociationPathCriteriaMap();
			CreateCriteriaEntityNameMap();
			CreateCriteriaCollectionPersisters();
			CreateCriteriaSQLAliasMap();
		}

		[CLSCompliant(false)] // TODO: Why does this cause a problem in 1.1
		public string RootSQLAlias
		{
			get { return rootSQLAlias; }
		}

		public ISet<string> GetQuerySpaces()
		{
			ISet<string> result = new HashedSet<string>();

			foreach (ICriteriaInfoProvider info in criteriaInfoMap.Values)
			{
				result.AddAll(info.Spaces);
			}

			foreach (ICollectionPersister collectionPersister in criteriaCollectionPersisters)
			{
				result.AddAll(collectionPersister.CollectionSpaces);
			}
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

		public QueryParameters GetQueryParameters()
		{
			RowSelection selection = new RowSelection();
			selection.FirstRow = rootCriteria.FirstResult;
			selection.MaxRows = rootCriteria.MaxResults;
			selection.Timeout = rootCriteria.Timeout;
			selection.FetchSize = rootCriteria.FetchSize;

			Dictionary<string, LockMode> lockModes = new Dictionary<string, LockMode>();
			foreach (KeyValuePair<string, LockMode> me in rootCriteria.LockModes)
			{
				ICriteria subcriteria = GetAliasedCriteria(me.Key);
				lockModes[GetSQLAlias(subcriteria)] = me.Value;
			}
			
			List<TypedValue> typedValues = new List<TypedValue>();			
			
			// NH-specific: Get parameters for projections first
			if (this.HasProjection)
			{
				typedValues.AddRange(rootCriteria.Projection.GetTypedValues(rootCriteria, this));
			}

			foreach (CriteriaImpl.Subcriteria subcriteria in rootCriteria.IterateSubcriteria())
			{
				LockMode lm = subcriteria.LockMode;
				if (lm != null)
				{
					lockModes[GetSQLAlias(subcriteria)] = lm;
				}
				// Get parameters that may be used in JOINs
				if (subcriteria.WithClause != null)
				{
					typedValues.AddRange(subcriteria.WithClause.GetTypedValues(subcriteria, this));
			}
			}
			
			List<TypedValue> groupedTypedValues = new List<TypedValue>();
			
			// Type and value gathering for the WHERE clause needs to come AFTER lock mode gathering,
			// because the lock mode gathering loop now contains join clauses which can contain
			// parameter bindings (as in the HQL WITH clause).
			foreach(CriteriaImpl.CriterionEntry ce in rootCriteria.IterateExpressionEntries())
			{
				bool criteriaContainsGroupedProjections = false;
				IProjection[] projections = ce.Criterion.GetProjections();

				if (projections != null)
				{
					foreach (IProjection projection in projections)
					{
						if (projection.IsGrouped)
						{
							criteriaContainsGroupedProjections = true;
							break;
						}
					}
				}
				
				if (criteriaContainsGroupedProjections)
					// GROUP BY/HAVING parameters need to be added after WHERE parameters - so don't add them
					// to typedValues yet
					groupedTypedValues.AddRange(ce.Criterion.GetTypedValues(ce.Criteria, this));
				else
					typedValues.AddRange(ce.Criterion.GetTypedValues(ce.Criteria, this));
			}
			
			// NH-specific: GROUP BY/HAVING parameters need to appear after WHERE parameters
			if (groupedTypedValues.Count > 0)
			{
				typedValues.AddRange(groupedTypedValues);
			}
			
			// NH-specific: To support expressions/projections used in ORDER BY
			foreach(CriteriaImpl.OrderEntry oe in rootCriteria.IterateOrderings())
			{
				typedValues.AddRange(oe.Order.GetTypedValues(oe.Criteria, this));
			}
			
			return
				new QueryParameters(
					typedValues.Select(tv => tv.Type).ToArray(),
					typedValues.Select(tv => tv.Value).ToArray(),
					lockModes,
					selection,
					rootCriteria.IsReadOnlyInitialized,
					rootCriteria.IsReadOnlyInitialized ? rootCriteria.IsReadOnly : false,
					rootCriteria.Cacheable,
					rootCriteria.CacheRegion,
					rootCriteria.Comment,
					rootCriteria.LookupByNaturalKey,
					rootCriteria.ResultTransformer,
					_tempPagingParameterIndexes);
		}
		
		public SqlString GetGroupBy()
		{
			if (rootCriteria.Projection.IsGrouped)
			{
				return
					rootCriteria.Projection.ToGroupSqlString(rootCriteria.ProjectionCriteria, this,
															 new CollectionHelper.EmptyMapClass<string, IFilter>());
			}
			else
			{
				return SqlString.Empty;
			}
		}

		public SqlString GetSelect(IDictionary<string, IFilter> enabledFilters)
		{
			return rootCriteria.Projection.ToSqlString(rootCriteria.ProjectionCriteria, 0, this, enabledFilters);
		}

		public IType[] ProjectedTypes
		{
			get { return rootCriteria.Projection.GetTypes(rootCriteria, this); }
		}

		public string[] ProjectedColumnAliases
		{
			get
			{
				return rootCriteria.Projection is IEnhancedProjection
					? ((IEnhancedProjection)rootCriteria.Projection).GetColumnAliases(0, rootCriteria, this)
					: rootCriteria.Projection.GetColumnAliases(0);
			}
		}

		public string[] ProjectedAliases
		{
			get { return rootCriteria.Projection.Aliases; }
		}

		public SqlString GetWhereCondition(IDictionary<string, IFilter> enabledFilters)
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
					SqlString sqlString = entry.Criterion.ToSqlString(entry.Criteria, this, enabledFilters);
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

		public bool IsJoin(string path)
		{
			return associationPathCriteriaMap.ContainsKey(path);
		}

		public JoinType GetJoinType(string path)
		{
			JoinType result;
			if (associationPathJoinTypesMap.TryGetValue(path, out result))
			{
				return result;
			}
			else
			{
				return JoinType.InnerJoin;
			}
		}

		public ICriteria GetCriteria(string path)
		{
			ICriteria result;
			associationPathCriteriaMap.TryGetValue(path, out result);
			logger.DebugFormat("getCriteria for path={0} crit={1}", path, result);
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
			foreach (CriteriaImpl.Subcriteria crit in rootCriteria.IterateSubcriteria())
			{
				string wholeAssociationPath = GetWholeAssociationPath(crit);
				try
				{
					associationPathCriteriaMap.Add(wholeAssociationPath, crit);
				}
				catch (ArgumentException ae)
				{
					throw new QueryException("duplicate association path: " + wholeAssociationPath, ae);
				}

				try
				{
					associationPathJoinTypesMap.Add(wholeAssociationPath, crit.JoinType);
				}
				catch (ArgumentException ae)
				{
					throw new QueryException("duplicate association path: " + wholeAssociationPath, ae);
				}

				try
				{
					if (crit.WithClause != null)
					{
						withClauseMap.Add(wholeAssociationPath, crit.WithClause);
					}
				}
				catch (ArgumentException ae)
				{
					throw new QueryException("duplicate association path: " + wholeAssociationPath, ae);
				}
			}
		}

		private string GetWholeAssociationPath(CriteriaImpl.Subcriteria subcriteria)
		{
			string path = subcriteria.Path;

			// some messy, complex stuff here, since createCriteria() can take an
			// aliased path, or a path rooted at the creating criteria instance
			ICriteria parent = null;
			if (path.IndexOf('.') > 0)
			{
				// if it is a compound path
				string testAlias = StringHelper.Root(path);
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

			if (parent.Equals(rootCriteria))
			{
				// if its the root criteria, we are done
				return path;
			}
			else
			{
				// otherwise, recurse
				return GetWholeAssociationPath((CriteriaImpl.Subcriteria)parent) + '.' + path;
			}
		}

		private void CreateCriteriaEntityNameMap()
		{
			// initialize the rootProvider first
			ICriteriaInfoProvider rootProvider = new EntityCriteriaInfoProvider((NHibernate_Persister_Entity.IQueryable)sessionFactory.GetEntityPersister(rootEntityName));
			criteriaInfoMap.Add(rootCriteria, rootProvider);
			nameCriteriaInfoMap.Add(rootProvider.Name, rootProvider);


			foreach (KeyValuePair<string, ICriteria> me in associationPathCriteriaMap)
			{
				ICriteriaInfoProvider info = GetPathInfo(me.Key);
				criteriaInfoMap.Add(me.Value, info);
				nameCriteriaInfoMap[info.Name] =  info;
			}
		}


		private void CreateCriteriaCollectionPersisters()
		{
			foreach (KeyValuePair<string, ICriteria> me in associationPathCriteriaMap)
			{
				NHibernate_Persister_Entity.IJoinable joinable = GetPathJoinable(me.Key);
				if (joinable != null && joinable.IsCollection)
				{
					criteriaCollectionPersisters.Add((ICollectionPersister)joinable);
				}
			}
		}

		private Persister.Entity.IJoinable GetPathJoinable(string path)
		{
			NHibernate_Persister_Entity.IJoinable last = (NHibernate_Persister_Entity.IJoinable)Factory.GetEntityPersister(rootEntityName);
			NHibernate_Persister_Entity.IPropertyMapping lastEntity = (NHibernate_Persister_Entity.IPropertyMapping)last;

			string componentPath = "";

			StringTokenizer tokens = new StringTokenizer(path, ".", false);
			foreach (string token in tokens)
			{
				componentPath += token;
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

		private ICriteriaInfoProvider GetPathInfo(string path)
		{
			StringTokenizer tokens = new StringTokenizer(path, ".", false);
			string componentPath = string.Empty;

			// start with the 'rootProvider'
			ICriteriaInfoProvider provider;
			if (nameCriteriaInfoMap.TryGetValue(rootEntityName, out provider) == false)
				throw new ArgumentException("Could not find ICriteriaInfoProvider for: " + path);


			foreach (string token in tokens)
			{
				componentPath += token;
				logger.DebugFormat("searching for {0}", componentPath);
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
						provider = new EntityCriteriaInfoProvider((NHibernate_Persister_Entity.IQueryable)sessionFactory.GetEntityPersister(
																				   atype.GetAssociatedEntityName(
																					   sessionFactory)
																				   ));
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

			logger.DebugFormat("returning entity name={0} for path={1} class={2}",
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
				criteriaSQLAliasMap[crit] = StringHelper.GenerateAlias(alias, i++);
				logger.DebugFormat("put criteria={0} alias={1}",
					crit, criteriaSQLAliasMap[crit]);
			}
			criteriaSQLAliasMap[rootCriteria] = rootSQLAlias;
		}

		public bool HasProjection
		{
			get { return rootCriteria.Projection != null; }
		}

		public string GetSQLAlias(ICriteria criteria)
		{
			String alias = criteriaSQLAliasMap[criteria];
			logger.DebugFormat("returning alias={0} for criteria={1}", alias, criteria);
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
			try
			{
				return GetColumns(subcriteria, propertyName);
			}
			catch (HibernateException)
			{
				//not found in inner query , try the outer query
				if (outerQueryTranslator != null)
				{
					return outerQueryTranslator.GetColumnsUsingProjection(subcriteria, propertyName);
				}
				else
				{
					throw;
				}
			}
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
			return new TypedValue(loadable.IdentifierType, value, EntityMode.Poco);
		}

		public string[] GetColumns(ICriteria subcriteria, string propertyName)
		{
			string entName = GetEntityName(subcriteria, propertyName);
			if (entName == null)
			{
				throw new QueryException("Could not find property " + propertyName);
			}
			return GetPropertyMapping(entName).ToColumns(GetSQLAlias(subcriteria, propertyName), GetPropertyName(propertyName));
		}

		public IType GetTypeUsingProjection(ICriteria subcriteria, string propertyName)
		{
			//first look for a reference to a projection alias
			IProjection projection = rootCriteria.Projection;
			IType[] projectionTypes = projection == null ? null : projection.GetTypes(propertyName, subcriteria, this);

			if (projectionTypes == null)
			{
				try
				{
					//it does not refer to an alias of a projection,
					//look for a property
					return GetType(subcriteria, propertyName);
				}
				catch (HibernateException)
				{
					//not found in inner query , try the outer query
					if (outerQueryTranslator != null)
					{
						return outerQueryTranslator.GetType(subcriteria, propertyName);
					}
					else
					{
						throw;
					}
				}
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
			return GetPropertyMapping(GetEntityName(subcriteria, propertyName)).ToType(GetPropertyName(propertyName));
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
					return new TypedValue(q.DiscriminatorType, q.DiscriminatorValue, EntityMode.Poco);
				}
			}
			// Otherwise, this is an ordinary value.
			return new TypedValue(GetTypeUsingProjection(subcriteria, propertyName), value, EntityMode.Poco);
		}

		private Persister.Entity.IPropertyMapping GetPropertyMapping(string entityName)
		{
			ICriteriaInfoProvider info ;
			if (nameCriteriaInfoMap.TryGetValue(entityName, out info)==false)
				throw new InvalidOperationException("Could not find criteria info provider for: " + entityName);
			return info.PropertyMapping;
		}

		public string GetEntityName(ICriteria subcriteria, string propertyName)
		{
			if (propertyName.IndexOf('.') > 0)
			{
				string root = StringHelper.Root(propertyName);
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
			if (propertyName.IndexOf('.') > 0)
			{
				string root = StringHelper.Root(propertyName);
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
			if (propertyName.IndexOf('.') > 0)
			{
				string root = StringHelper.Root(propertyName);
				ICriteria crit = GetAliasedCriteria(root);
				if (crit != null)
				{
					return propertyName.Substring(root.Length + 1);
				}
			}
			return propertyName;
		}

		public SqlString GetWithClause(string path, IDictionary<string, IFilter> enabledFilters)
		{
			if (withClauseMap.ContainsKey(path))
			{
				ICriterion crit = (ICriterion)withClauseMap[path];
				return crit == null ? null : crit.ToSqlString(GetCriteria(path), this, enabledFilters);
			}
			return null;
		}

		#region NH specific

		public int GetIndexForAlias()
		{
			return indexForAlias++;
		}

		public int? CreatePagingParameter(int value)
		{
			if (!Factory.Dialect.SupportsVariableLimit)
				return null;

			_tempPagingParameterIndexes.Add(_tempPagingParameterIndex, value);
			return _tempPagingParameterIndex--;
		}

		public SqlString GetHavingCondition(IDictionary<string, IFilter> enabledFilters)
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
					SqlString sqlString = entry.Criterion.ToSqlString(entry.Criteria, this, enabledFilters);
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
			string[] projectionColumns = projection == null ? null : projection.GetColumnAliases(propertyName, 0);

			if (projectionColumns == null)
			{
				//it does not refer to an alias of a projection,
				//look for a property
				try
				{
					return GetColumns(subcriteria, propertyName);
				}
				catch (HibernateException)
				{
					//not found in inner query , try the outer query
					if (outerQueryTranslator != null)
					{
						return outerQueryTranslator.GetColumnAliasesUsingProjection(subcriteria, propertyName);
					}
					else
					{
						throw;
					}
				}
			}
			else
			{
				//it refers to an alias of a projection
				return projectionColumns;
			}
		}

		#endregion
	}
}
