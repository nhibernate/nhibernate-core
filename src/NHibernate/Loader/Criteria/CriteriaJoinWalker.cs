using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using log4net;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Criteria
{
	/// <summary>
	/// A <see cref="JoinWalker" /> for <see cref="ICriteria" /> queries.
	/// </summary>
	/// <seealso cref="CriteriaLoader"/>
	public class CriteriaJoinWalker : AbstractEntityJoinWalker
	{
		//TODO: add a CriteriaImplementor interface
		//      this class depends directly upon CriteriaImpl in the impl package...

		private readonly CriteriaQueryTranslator translator;
		private readonly ISet<string> querySpaces;
		private readonly IType[] resultTypes;
		//the user visible aliases, which are unknown to the superclass,
		//these are not the actual "physical" SQL aliases
		private readonly string[] userAliases;
		private readonly IList<string> userAliasList = new List<string>();

	    private static readonly ILog logger = LogManager.GetLogger(typeof (CriteriaJoinWalker));

		public CriteriaJoinWalker(IOuterJoinLoadable persister, CriteriaQueryTranslator translator,
		                          ISessionFactoryImplementor factory, ICriteria criteria, string rootEntityName,
		                          IDictionary<string, IFilter> enabledFilters)
			: base(translator.RootSQLAlias, persister, factory, enabledFilters)
		{
			this.translator = translator;

			querySpaces = translator.GetQuerySpaces();

			if (translator.HasProjection)
			{
				resultTypes = translator.ProjectedTypes;

				InitProjection(translator.GetSelect(enabledFilters), translator.GetWhereCondition(enabledFilters),
				               translator.GetOrderBy(), translator.GetGroupBy().ToString(),
				               translator.GetHavingCondition(enabledFilters), LockMode.None);
			}
			else
			{
				resultTypes = new IType[] {TypeFactory.ManyToOne(persister.EntityName)};

				InitAll(translator.GetWhereCondition(enabledFilters), translator.GetOrderBy(), LockMode.None);
			}

			userAliasList.Add(criteria.Alias); //root entity comes *last*
			userAliases = ArrayHelper.ToStringArray(userAliasList);
		}

		protected override void InitAll(SqlString whereString, SqlString orderByString, LockMode lockMode)
		{
			// NH different behavior (NH-1760)
			WalkCompositeComponentIdTree();
			base.InitAll(whereString, orderByString, lockMode);
		}

		private void WalkCompositeComponentIdTree()
		{
			IType type = Persister.IdentifierType;
			string propertyName = Persister.IdentifierPropertyName;
			if (type != null && type.IsComponentType && !(type is EmbeddedComponentType))
			{
				ILhsAssociationTypeSqlInfo associationTypeSQLInfo = JoinHelper.GetIdLhsSqlInfo(Alias, Persister, Factory);
				WalkComponentTree((IAbstractComponentType) type, 0, Alias, SubPath(string.Empty, propertyName), 0,
				                  associationTypeSQLInfo);
			}
		}

		public IType[] ResultTypes
		{
			get { return resultTypes; }
		}

		public string[] UserAliases
		{
			get { return userAliases; }
		}

		/// <summary>
		/// Use the discriminator, to narrow the select to instances
		/// of the queried subclass, also applying any filters.
		/// </summary>
		protected override SqlString WhereFragment
		{
			get { return base.WhereFragment.Append(((IQueryable) Persister).FilterFragment(Alias, EnabledFilters)); }
		}

		public ISet<string> QuerySpaces
		{
			get { return querySpaces; }
		}

		public override string Comment
		{
			get { return "criteria query"; }
		}

		protected override JoinType GetJoinType(IAssociationType type, FetchMode config, string path, string lhsTable,
		                                        string[] lhsColumns, bool nullable, int currentDepth,
		                                        CascadeStyle cascadeStyle)
		{
			if (translator.IsJoin(path))
			{
				return translator.GetJoinType(path);
			}
			else
			{
				if (translator.HasProjection)
				{
					return JoinType.None;
				}
				else
				{
					FetchMode fetchMode = translator.RootCriteria.GetFetchMode(path);
					if (IsDefaultFetchMode(fetchMode))
					{
						return base.GetJoinType(type, config, path, lhsTable, lhsColumns, nullable, currentDepth, cascadeStyle);
					}
					else
					{
						if (fetchMode == FetchMode.Join)
						{
							IsDuplicateAssociation(lhsTable, lhsColumns, type); //deliberately ignore return value!
							return GetJoinType(nullable, currentDepth);
						}
						else
						{
							return JoinType.None;
						}
					}
				}
			}
		}

		private static bool IsDefaultFetchMode(FetchMode fetchMode)
		{
			return fetchMode == FetchMode.Default;
		}

		protected override string GenerateTableAlias(int n, string path, IJoinable joinable)
		{
			bool shouldCreateUserAlias = joinable.ConsumesEntityAlias(); 
			if(shouldCreateUserAlias == false  && joinable.IsCollection)
			{
				var elementType = ((ICollectionPersister)joinable).ElementType;
				if (elementType != null)
					shouldCreateUserAlias = elementType.IsComponentType;
			}
			if (shouldCreateUserAlias)
			{
				ICriteria subcriteria = translator.GetCriteria(path);
				string sqlAlias = subcriteria == null ? null : translator.GetSQLAlias(subcriteria);
				if (sqlAlias != null)
				{
					userAliasList.Add(subcriteria.Alias); //alias may be null
					return sqlAlias; //EARLY EXIT
				}

				userAliasList.Add(null);
			}
			return base.GenerateTableAlias(n + translator.SQLAliasCount, path, joinable);
		}

		protected override string GenerateRootAlias(string tableName)
		{
			return CriteriaQueryTranslator.RootSqlAlias;
			// NH: really not used (we are using a different ctor to support SubQueryCriteria)
		}
	}
}