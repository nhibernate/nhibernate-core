using System;
using System.Collections;
using System.Text;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.Expression;
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

		private ICriteriaQuery outerQueryTranslator;

		private readonly CriteriaImpl rootCriteria;
		private readonly System.Type rootEntityName;
		private readonly string rootSQLAlias;
		private int aliasCount = 0;

		private readonly IDictionary criteriaEntityNames = new SequencedHashMap();
		private readonly ISet criteriaCollectionPersisters = new HashedSet();

		private readonly IDictionary criteriaSQLAliasMap = new Hashtable();
		private readonly IDictionary aliasCriteriaMap = new Hashtable();
		private readonly IDictionary associationPathCriteriaMap = new SequencedHashMap();
		private readonly IDictionary associationPathJoinTypesMap = new SequencedHashMap();

		private readonly ISessionFactoryImplementor sessionFactory;

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
			this.rootCriteria = criteria;
			this.rootEntityName = rootEntityName;
			this.sessionFactory = factory;
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

		private ICriteria GetAliasedCriteria(string alias)
		{
			return (ICriteria) aliasCriteriaMap[alias];
		}

		public bool IsJoin(string path)
		{
			return associationPathCriteriaMap.Contains(path);
		}

		public JoinType GetJoinType(string path)
		{
			object result = associationPathJoinTypesMap[path];
			return (result == null ? JoinType.InnerJoin : (JoinType) result);
		}

		public ICriteria GetCriteria(string path)
		{
			return (ICriteria) associationPathCriteriaMap[path];
		}

		public ISet GetQuerySpaces()
		{
			ISet result = new HashedSet();

			foreach (System.Type entityName in criteriaEntityNames.Values)
			{
				result.AddAll(Factory.GetEntityPersister(entityName).QuerySpaces);
			}

			foreach (ICollectionPersister collectionPersister in criteriaCollectionPersisters)
			{
				result.Add(collectionPersister.CollectionSpace);
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
					object old = aliasCriteriaMap[subcriteria.Alias];
					aliasCriteriaMap[subcriteria.Alias] = subcriteria;
					if (old != null)
					{
						throw new QueryException("duplicate alias: " + subcriteria.Alias);
					}
				}
			}
		}

		private void CreateAssociationPathCriteriaMap()
		{
			foreach (CriteriaImpl.Subcriteria crit in rootCriteria.IterateSubcriteria())
			{
				string wholeAssociationPath = GetWholeAssociationPath(crit);
				object old = associationPathCriteriaMap[wholeAssociationPath];
				associationPathCriteriaMap[wholeAssociationPath] = crit;
				if (old != null)
				{
					throw new QueryException("duplicate association path: " + wholeAssociationPath);
				}
				JoinType joinType = crit.JoinType;

				old = associationPathJoinTypesMap[wholeAssociationPath];
				associationPathJoinTypesMap[wholeAssociationPath] = joinType;

				if (old != null)
				{
					// TODO : not so sure this is needed...
					throw new QueryException("duplicate association path: " + wholeAssociationPath);
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
					parent = (ICriteria) aliasCriteriaMap[testAlias];
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
				return GetWholeAssociationPath((CriteriaImpl.Subcriteria) parent) + '.' + path;
			}
		}

		private void CreateCriteriaEntityNameMap()
		{
			criteriaEntityNames[rootCriteria] = rootEntityName;

			foreach (DictionaryEntry me in associationPathCriteriaMap)
			{
				criteriaEntityNames[me.Value] = GetPathEntityName((string) me.Key);
			}
		}

		private void CreateCriteriaCollectionPersisters()
		{
			foreach (DictionaryEntry me in associationPathCriteriaMap)
			{
				IJoinable joinable = GetPathJoinable((string) me.Key);
				if (joinable.IsCollection)
				{
					criteriaCollectionPersisters.Add(joinable);
				}
			}
		}

		private IJoinable GetPathJoinable(string path)
		{
			IJoinable last = (IJoinable) Factory.GetEntityPersister(rootEntityName);
			IPropertyMapping lastEntity = (IPropertyMapping) last;

			string componentPath = "";

			StringTokenizer tokens = new StringTokenizer(path, ".", false);
			foreach (string token in tokens)
			{
				componentPath += token;
				IType type = lastEntity.ToType(componentPath);
				if (type.IsAssociationType)
				{
					IAssociationType atype = (IAssociationType) type;
					last = atype.GetAssociatedJoinable(Factory);
					lastEntity = (IPropertyMapping) Factory.GetEntityPersister(atype.GetAssociatedClass(Factory));
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

		private System.Type GetPathEntityName(string path)
		{
			IPropertyMapping propertyMapping = (IPropertyMapping) GetPathJoinable(path);
			return ((IAssociationType) propertyMapping.Type).GetAssociatedClass(Factory);
		}

		public int SQLAliasCount
		{
			get { return criteriaSQLAliasMap.Count; }
		}

		private void CreateCriteriaSQLAliasMap()
		{
			int i = 0;

			foreach (DictionaryEntry me in criteriaEntityNames)
			{
				ICriteria crit = (ICriteria) me.Key;
				string alias = crit.Alias;
				if (alias == null)
				{
					alias = ((System.Type) me.Value).FullName; // the entity name
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

			object[] valueArray = values.ToArray();
			IType[] typeArray = (IType[]) types.ToArray(typeof(IType));

			RowSelection selection = new RowSelection();
			selection.FirstRow = rootCriteria.FirstResult;
			selection.MaxRows = rootCriteria.MaxResults;
			selection.Timeout = rootCriteria.Timeout;
			selection.FetchSize = rootCriteria.FetchSize;

			IDictionary lockModes = new Hashtable();
			foreach (DictionaryEntry me in rootCriteria.LockModes)
			{
				ICriteria subcriteria = GetAliasedCriteria((string) me.Key);
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
				"", // TODO H3: rootCriteria.Comment,
				rootCriteria.IsLookupByNaturalKey()
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
					.ToGroupSqlString(rootCriteria.ProjectionCriteria, this);
			}
			else
			{
				return SqlString.Empty;
			}
		}

		public SqlString GetSelect()
		{
			return rootCriteria.Projection.ToSqlString(
				rootCriteria.ProjectionCriteria,
				0,
				this
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

		public SqlString GetWhereCondition(IDictionary enabledFilters)
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
			return (string) criteriaSQLAliasMap[criteria];
		}

		public System.Type GetEntityName(ICriteria criteria)
		{
			return (System.Type) criteriaEntityNames[criteria];
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
				((ILoadable) GetPropertyMapping(GetEntityName(subcriteria))).IdentifierColumnNames;
			return StringHelper.Qualify(GetSQLAlias(subcriteria), idcols);
		}

		public IType GetIdentifierType(ICriteria subcriteria)
		{
			return ((ILoadable) GetPropertyMapping(GetEntityName(subcriteria))).IdentifierType;
		}

		public TypedValue GetTypedIdentifierValue(ICriteria subcriteria, object value)
		{
			ILoadable loadable = (ILoadable) GetPropertyMapping(GetEntityName(subcriteria));
			return new TypedValue(
				loadable.IdentifierType,
				value
				// TODO H3: EntityMode.POJO
				);
		}

		private string[] GetColumns(
			string propertyName,
			ICriteria subcriteria)
		{
		    System.Type type = GetEntityName(subcriteria, propertyName);
            if (type == null)
                throw new QueryException("Could not find property " + propertyName + " on " + subcriteria.CriteriaClass);
		    return GetPropertyMapping(type)
				.ToColumns(
				GetSQLAlias(subcriteria, propertyName),
				GetPropertyName(propertyName)
				);
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
				System.Type entityClass = (System.Type) value;
				IQueryable q = SessionFactoryHelper.FindQueryableUsingImports(sessionFactory, entityClass.FullName);

				if (q != null)
				{
					return new TypedValue(
						q.DiscriminatorType,
						q.DiscriminatorValue
						// TODO H3: EntityMode.POJO
						);
				}
			}
			// Otherwise, this is an ordinary value.
			return new TypedValue(
				GetTypeUsingProjection(subcriteria, propertyName),
				value
				// TODO H3: EntityMode.POJO
				);
		}

		private IPropertyMapping GetPropertyMapping(System.Type entityName)
		{
			return (IPropertyMapping) sessionFactory.GetEntityPersister(entityName);
		}

		//TODO: use these in methods above

		public System.Type GetEntityName(ICriteria subcriteria, string propertyName)
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
