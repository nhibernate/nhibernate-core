using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Criterion;
using NHibernate.Hql.Util;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Criteria
{
	public class CriteriaQueryTranslator : ICriteriaQuery
	{
		public static readonly string RootSqlAlias = CriteriaUtil.RootAlias + '_';

		private readonly ICriteriaQuery outerQueryTranslator;

		private readonly CriteriaImpl rootCriteria;
		private readonly System.Type rootEntityName;
		private readonly string rootSQLAlias;
		private readonly int aliasCount = 0;

		private readonly IDictionary<ICriteria, string> criteriaEntityNames = new LinkedHashMap<ICriteria, string>();
		private readonly ISet<ICollectionPersister> criteriaCollectionPersisters = new HashedSet<ICollectionPersister>();

		private readonly IDictionary<ICriteria, string> criteriaSQLAliasMap = new Dictionary<ICriteria, string>();
		private readonly IDictionary<string, ICriteria> aliasCriteriaMap = new Dictionary<string, ICriteria>();
		private readonly IDictionary<string, ICriteria> associationPathCriteriaMap = new LinkedHashMap<string, ICriteria>();
		private readonly IDictionary<string, JoinType> associationPathJoinTypesMap = new LinkedHashMap<string, JoinType>();

		private readonly ISessionFactoryImplementor sessionFactory;
		private int indexForAlias = 0;

		public CriteriaQueryTranslator(
			ISessionFactoryImplementor factory,
			CriteriaImpl criteria,
			System.Type rootEntityName,
			string rootSQLAlias,
			ICriteriaQuery outerQuery)
			: this(factory, criteria, rootEntityName, rootSQLAlias)
		{
			outerQueryTranslator = outerQuery;
		}

		public CriteriaQueryTranslator(
			ISessionFactoryImplementor factory,
			CriteriaImpl criteria,
			System.Type rootEntityName,
			string rootSQLAlias)
		{
			rootCriteria = criteria;
			this.rootEntityName = rootEntityName;
			sessionFactory = factory;
			this.rootSQLAlias = rootSQLAlias;

			CreateAliasCriteriaMap();
			CreateAssociationPathCriteriaMap();
			CreateCriteriaEntityNameMap();
			CreateCriteriaCollectionPersisters();
			CreateCriteriaSQLAliasMap();
		}

		public string GenerateSQLAlias()
		{
			return StringHelper.GenerateAlias(rootSQLAlias, aliasCount);
		}


		public int GetIndexForAlias()
		{
			return indexForAlias++;
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
				return result;
			else
				return JoinType.InnerJoin;
		}

		public ICriteria GetCriteria(string path)
		{
			ICriteria result;
			associationPathCriteriaMap.TryGetValue(path, out result);
			return result;
		}

		public ISet<string> GetQuerySpaces()
		{
			ISet<string> result = new HashedSet<string>();

			foreach (string entityName in criteriaEntityNames.Values)
			{
				result.AddAll(Factory.GetEntityPersister(entityName).QuerySpaces);
			}

			foreach (ICollectionPersister collectionPersister in criteriaCollectionPersisters)
			{
				result.AddAll(collectionPersister.CollectionSpaces);
			}
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
			criteriaEntityNames[rootCriteria] = rootEntityName.FullName;

			foreach (KeyValuePair<string, ICriteria> me in associationPathCriteriaMap)
			{
				criteriaEntityNames[me.Value] = GetPathEntityName(me.Key);
			}
		}

		private void CreateCriteriaCollectionPersisters()
		{
			foreach (KeyValuePair<string, ICriteria> me in associationPathCriteriaMap)
			{
				IJoinable joinable = GetPathJoinable(me.Key);
				if (joinable.IsCollection)
				{
					criteriaCollectionPersisters.Add((ICollectionPersister)joinable);
				}
			}
		}

		private IJoinable GetPathJoinable(string path)
		{
			IJoinable last = (IJoinable)Factory.GetEntityPersister(rootEntityName);
			IPropertyMapping lastEntity = (IPropertyMapping)last;

			string componentPath = "";

			StringTokenizer tokens = new StringTokenizer(path, ".", false);
			foreach (string token in tokens)
			{
				componentPath += token;
				IType type = lastEntity.ToType(componentPath);
				if (type.IsAssociationType)
				{
					IAssociationType atype = (IAssociationType)type;
					last = atype.GetAssociatedJoinable(Factory);
					lastEntity = (IPropertyMapping)Factory.GetEntityPersister(atype.GetAssociatedEntityName(Factory));
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

		private string GetPathEntityName(string path)
		{
			IQueryable persister = (IQueryable)sessionFactory.GetEntityPersister(rootEntityName);
			StringTokenizer tokens = new StringTokenizer(path, ".", false);
			string componentPath = "";
			foreach (string token in tokens)
			{
				componentPath += token;
				IType type = persister.ToType(componentPath);
				if (type.IsAssociationType)
				{
					IAssociationType atype = (IAssociationType)type;
					persister = (IQueryable)sessionFactory.GetEntityPersister(atype.GetAssociatedEntityName(sessionFactory));
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
			return persister.EntityName;
		}

		public int SQLAliasCount
		{
			get { return criteriaSQLAliasMap.Count; }
		}

		private void CreateCriteriaSQLAliasMap()
		{
			int i = 0;

			foreach (KeyValuePair<ICriteria, string> me in criteriaEntityNames)
			{
				ICriteria crit = me.Key;
				string alias = crit.Alias;
				if (alias == null)
				{
					alias = me.Value;
				}
				criteriaSQLAliasMap[crit] = StringHelper.GenerateAlias(alias, i++);
			}
			criteriaSQLAliasMap[rootCriteria] = rootSQLAlias;
		}

		public CriteriaImpl RootCriteria
		{
			get { return rootCriteria; }
		}

		public QueryParameters GetQueryParameters()
		{
			ArrayList values = new ArrayList();
			ArrayList types = new ArrayList();

			foreach (CriteriaImpl.CriterionEntry ce in rootCriteria.IterateExpressionEntries())
			{
				TypedValue[] tv = ce.Criterion.GetTypedValues(ce.Criteria, this);
				for (int i = 0; i < tv.Length; i++)
				{
					values.Add(tv[i].Value);
					types.Add(tv[i].Type);
				}
			}
			if (rootCriteria.Projection != null)
			{
				TypedValue[] tv = rootCriteria.Projection.GetTypedValues(rootCriteria.ProjectionCriteria, this);
				for (int i = 0; i < tv.Length; i++)
				{
					values.Add(tv[i].Value);
					types.Add(tv[i].Type);
				}
			}

			object[] valueArray = values.ToArray();
			IType[] typeArray = (IType[])types.ToArray(typeof(IType));

			RowSelection selection = new RowSelection();
			selection.FirstRow = rootCriteria.FirstResult;
			selection.MaxRows = rootCriteria.MaxResults;
			selection.Timeout = rootCriteria.Timeout;
			selection.FetchSize = rootCriteria.FetchSize;

			IDictionary lockModes = new Hashtable();
			foreach (DictionaryEntry me in rootCriteria.LockModes)
			{
				ICriteria subcriteria = GetAliasedCriteria((string)me.Key);
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

			return new QueryParameters(
				typeArray,
				valueArray,
				lockModes,
				selection,
				rootCriteria.Cacheable,
				rootCriteria.CacheRegion,
				string.Empty, // TODO H3: rootCriteria.Comment,
				rootCriteria.IsLookupByNaturalKey(),
				null
				);
		}

		public bool HasProjection
		{
			get { return rootCriteria.Projection != null; }
		}

		public SqlString GetGroupBy()
		{
			if (rootCriteria.Projection.IsGrouped)
			{
				return rootCriteria.Projection
					.ToGroupSqlString(rootCriteria.ProjectionCriteria, this, new CollectionHelper.EmptyMapClass<string, IFilter>());
			}
			else
			{
				return SqlString.Empty;
			}
		}

		public SqlString GetSelect(IDictionary<string, IFilter> enabledFilters)
		{
			return rootCriteria.Projection.ToSqlString(
				rootCriteria.ProjectionCriteria,
				0,
				this,
				enabledFilters
				);
		}

		public IType[] ProjectedTypes
		{
			get { return rootCriteria.Projection.GetTypes(rootCriteria, this); }
		}

		public string[] ProjectedColumnAliases
		{
			get { return rootCriteria.Projection.GetColumnAliases(0); }
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
				if (!first)
				{
					condition.Add(" and ");
				}
				first = false;
				SqlString sqlString = entry.Criterion.ToSqlString(entry.Criteria, this, enabledFilters);
				condition.Add(sqlString);
			}
			return condition.ToSqlString();
		}

		public string GetOrderBy()
		{
			StringBuilder orderBy = new StringBuilder(30);

			bool first = true;
			foreach (CriteriaImpl.OrderEntry oe in rootCriteria.IterateOrderings())
			{
				if (!first)
				{
					orderBy.Append(", ");
				}
				first = false;
				orderBy.Append(oe.Order.ToSqlString(oe.Criteria, this));
			}
			return orderBy.ToString();
		}

		public ISessionFactoryImplementor Factory
		{
			get { return sessionFactory; }
		}

		[CLSCompliant(false)] // TODO: Why does this cause a problem in 1.1
		public string RootSQLAlias
		{
			get { return rootSQLAlias; }
		}

		public string GetSQLAlias(ICriteria criteria)
		{
			return criteriaSQLAliasMap[criteria];
		}

		public string GetEntityName(ICriteria criteria)
		{
			return criteriaEntityNames[criteria];
		}

		public string GetColumn(ICriteria criteria, string propertyName)
		{
			string[] cols = GetColumns(propertyName, criteria);
			if (cols.Length != 1)
			{
				throw new QueryException("property does not map to a single column: " + propertyName);
			}
			return cols[0];
		}

		/// <summary>
		/// Get the names of the columns constrained
		/// by this criterion.
		/// </summary>
		public string[] GetColumnsUsingProjection(
			ICriteria subcriteria,
			string propertyName)
		{
			try
			{
				return GetColumns(propertyName, subcriteria);
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

		/// <summary>
		/// Get the aliases of the columns constrained
		/// by this criterion (for use in ORDER BY clause).
		/// </summary>
		public string[] GetColumnAliasesUsingProjection(
			ICriteria subcriteria,
			string propertyName)
		{
			//first look for a reference to a projection alias

			IProjection projection = rootCriteria.Projection;
			string[] projectionColumns = projection == null ?
										 null :
										 projection.GetColumnAliases(propertyName, 0);

			if (projectionColumns == null)
			{
				//it does not refer to an alias of a projection,
				//look for a property
				try
				{
					return GetColumns(propertyName, subcriteria);
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

		public string[] GetIdentifierColumns(ICriteria subcriteria)
		{
			string[] idcols =
				((ILoadable)GetPropertyMapping(GetEntityName(subcriteria))).IdentifierColumnNames;
			return StringHelper.Qualify(GetSQLAlias(subcriteria), idcols);
		}

		public IType GetIdentifierType(ICriteria subcriteria)
		{
			return ((ILoadable)GetPropertyMapping(GetEntityName(subcriteria))).IdentifierType;
		}

		public TypedValue GetTypedIdentifierValue(ICriteria subcriteria, object value)
		{
			ILoadable loadable = (ILoadable)GetPropertyMapping(GetEntityName(subcriteria));
			return new TypedValue(loadable.IdentifierType, value, EntityMode.Poco);
		}

		private string[] GetColumns(string propertyName, ICriteria subcriteria)
		{
			string entName = GetEntityName(subcriteria, propertyName);
			if (entName == null)
				throw new QueryException("Could not find property " + propertyName + " on " + subcriteria.CriteriaClass);
			return GetPropertyMapping(GetEntityName(subcriteria, propertyName)).ToColumns(GetSQLAlias(subcriteria, propertyName), GetPropertyName(propertyName));
		}

		public IType GetTypeUsingProjection(ICriteria subcriteria, string propertyName)
		{
			//first look for a reference to a projection alias
			IProjection projection = rootCriteria.Projection;
			IType[] projectionTypes = projection == null ?
									  null :
									  projection.GetTypes(propertyName, subcriteria, this);

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
			return GetPropertyMapping(GetEntityName(subcriteria, propertyName))
				.ToType(GetPropertyName(propertyName));
		}

		/// <summary>
		/// Get the a typed value for the given property value.
		/// </summary>
		public TypedValue GetTypedValue(ICriteria subcriteria, string propertyName, object value)
		{
			// Detect discriminator values...
			if (value is System.Type)
			{
				System.Type entityClass = (System.Type)value;
				IQueryable q = SessionFactoryHelper.FindQueryableUsingImports(sessionFactory, entityClass.FullName);

				if (q != null)
				{
					return new TypedValue(q.DiscriminatorType, q.DiscriminatorValue, EntityMode.Poco);
				}
			}
			// Otherwise, this is an ordinary value.
			return new TypedValue(GetTypeUsingProjection(subcriteria, propertyName), value, EntityMode.Poco);
		}

		private IPropertyMapping GetPropertyMapping(string entityName)
		{
			return (IPropertyMapping)sessionFactory.GetEntityPersister(entityName);
		}

		//TODO: use these in methods above

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
	}
}
