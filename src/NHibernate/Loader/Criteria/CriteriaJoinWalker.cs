using System.Collections.Generic;
using System.Linq;

using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;
using IQueryable = NHibernate.Persister.Entity.IQueryable;

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
		private readonly bool[] includeInResultRow;

		//the user visible aliases, which are unknown to the superclass,
		//these are not the actual "physical" SQL aliases
		private readonly string[] userAliases;
		private readonly IList<string> userAliasList = new List<string>();
		private readonly IList<IType> resultTypeList = new List<IType>();
		private readonly IList<bool> includeInResultRowList = new List<bool>();

		private static readonly IInternalLogger logger = LoggerProvider.LoggerFor(typeof(CriteriaJoinWalker));

		public CriteriaJoinWalker(IOuterJoinLoadable persister, CriteriaQueryTranslator translator,
		                          ISessionFactoryImplementor factory, ICriteria criteria, string rootEntityName,
		                          IDictionary<string, IFilter> enabledFilters)
			: base(translator.RootSQLAlias, persister, factory, enabledFilters)
		{
			this.translator = translator;

			querySpaces = translator.GetQuerySpaces();

			if (translator.HasProjection)
			{
				InitProjection(
					translator.GetSelect(enabledFilters),
					translator.GetWhereCondition(enabledFilters),
					translator.GetOrderBy(),
					translator.GetGroupBy(),
					translator.GetHavingCondition(enabledFilters),
					enabledFilters, 
					LockMode.None);

				resultTypes = translator.ProjectedTypes;
				userAliases = translator.ProjectedAliases;
				includeInResultRow = new bool[resultTypes.Length];
				ArrayHelper.Fill(IncludeInResultRow, true);
			}
			else
			{
				InitAll(translator.GetWhereCondition(enabledFilters), translator.GetOrderBy(), LockMode.None);

				resultTypes = new IType[] { TypeFactory.ManyToOne(persister.EntityName) };

				// root entity comes last
				userAliasList.Add(criteria.Alias); //root entity comes *last*
				resultTypeList.Add(translator.ResultType(criteria));
				includeInResultRowList.Add(true);
				userAliases = userAliasList.ToArray();
				resultTypes = resultTypeList.ToArray();
				includeInResultRow = includeInResultRowList.ToArray();
			}
		}

		protected override void WalkEntityTree(IOuterJoinLoadable persister, string alias, string path, int currentDepth)
		{
			// NH different behavior (NH-1476, NH-1760, NH-1785)
			base.WalkEntityTree(persister, alias, path, currentDepth);
			WalkCompositeComponentIdTree(persister, alias, path);
		}

		private void WalkCompositeComponentIdTree(IOuterJoinLoadable persister, string alias, string path)
		{
			IType type = persister.IdentifierType;
			string propertyName = persister.IdentifierPropertyName;
			if (type != null && type.IsComponentType)
			{
				ILhsAssociationTypeSqlInfo associationTypeSQLInfo = JoinHelper.GetIdLhsSqlInfo(alias, persister, Factory);
				WalkComponentTree((IAbstractComponentType)type, 0, alias, SubPath(path, propertyName), 0, associationTypeSQLInfo);
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

		public bool[] IncludeInResultRow
		{
			get { return includeInResultRow; }
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
			// TODO: deal with side-effects (changes to includeInSelectList, userAliasList, resultTypeList)!!!
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
					if (!translator.HasProjection)
					{
						includeInResultRowList.Add(subcriteria.Alias != null);
						if (subcriteria.Alias!=null)
						{
							userAliasList.Add(subcriteria.Alias); //alias may be null
							resultTypeList.Add(translator.ResultType(subcriteria));
						}
					}
					return sqlAlias; //EARLY EXIT
				}
				else
				{
					if (!translator.HasProjection)
						includeInResultRowList.Add(false);
				}
			}
			return base.GenerateTableAlias(n + translator.SQLAliasCount, path, joinable);
		}

		protected override string GenerateRootAlias(string tableName)
		{
			return CriteriaQueryTranslator.RootSqlAlias;
			// NH: really not used (we are using a different ctor to support SubQueryCriteria)
		}

		protected override SqlString GetWithClause(string path)
		{
			return translator.GetWithClause(path, EnabledFilters);
		}
	}
}