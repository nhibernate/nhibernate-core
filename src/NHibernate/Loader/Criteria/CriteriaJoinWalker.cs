using System;
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

		private static readonly INHibernateLogger logger = NHibernateLogger.For(typeof(CriteriaJoinWalker));

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
					translator.GetSelect(),
					translator.GetWhereCondition(),
					translator.GetOrderBy(),
					translator.GetGroupBy(),
					translator.GetHavingCondition(),
					enabledFilters,
					LockMode.None,
					translator.GetEntityProjections());

				resultTypes = translator.ProjectedTypes;
				userAliases = translator.ProjectedAliases;
				includeInResultRow = new bool[resultTypes.Length];
				ArrayHelper.Fill(IncludeInResultRow, true);
			}
			else
			{
				InitAll(translator.GetWhereCondition(), translator.GetOrderBy(), LockMode.None);

				// root entity comes last
				userAliasList.Add(criteria.Alias); //root entity comes *last*
				resultTypeList.Add(translator.ResultType(criteria));
				includeInResultRowList.Add(true);
				userAliases = userAliasList.ToArray();
				resultTypes = resultTypeList.ToArray();
				includeInResultRow = includeInResultRowList.ToArray();
			}
		}

		protected override void AddAssociations()
		{
			base.AddAssociations();
			foreach (var entityJoinInfo in translator.GetEntityJoins().Values)
			{
				var tableAlias = translator.GetSQLAlias(entityJoinInfo.Criteria);
				var criteriaPath = entityJoinInfo.Criteria.Alias; //path for entity join is equal to alias
				var criteriaAlias = entityJoinInfo.Criteria.Alias;
				var persister = entityJoinInfo.Persister as IOuterJoinLoadable;
				AddExplicitEntityJoinAssociation(persister, tableAlias, translator.GetJoinType(criteriaPath, criteriaAlias), criteriaPath, criteriaAlias);
				IncludeInResultIfNeeded(persister, entityJoinInfo.Criteria, tableAlias, criteriaPath);
				//collect mapped associations for entity join
				WalkEntityTree(persister, tableAlias, criteriaPath, 1);
			}
		}

		protected override void WalkEntityTree(IOuterJoinLoadable persister, string alias, string path, int currentDepth)
		{
			// NH different behavior (NH-1476, NH-1760, NH-1785)
			base.WalkEntityTree(persister, alias, path, currentDepth);
			WalkCompositeComponentIdTree(persister, alias, path);
		}

		protected override OuterJoinableAssociation CreateRootAssociation()
		{
			var selectMode = GetSelectMode(string.Empty);
			if (selectMode == SelectMode.JoinOnly || selectMode == SelectMode.Skip)
			{
				throw new NotSupportedException($"SelectMode {selectMode} for root entity is not supported. Use {nameof(SelectMode)}.{nameof(SelectMode.ChildFetch)} instead.");
			}

			return new OuterJoinableAssociation(
				Persister.EntityType,
				null,
				null,
				Alias,
				JoinType.LeftOuterJoin,
				null,
				Factory,
				CollectionHelper.EmptyDictionary<string, IFilter>(),
				selectMode);
		}

		protected override SelectMode GetSelectMode(string path)
		{
			return translator.RootCriteria.GetSelectMode(path);
		}

		private void WalkCompositeComponentIdTree(IOuterJoinLoadable persister, string alias, string path)
		{
			IType type = persister.IdentifierType;
			string propertyName = persister.IdentifierPropertyName;
			if (type != null && type.IsComponentType)
			{
				ILhsAssociationTypeSqlInfo associationTypeSQLInfo = JoinHelper.GetIdLhsSqlInfo(alias, persister, Factory);
				WalkComponentTree((IAbstractComponentType) type, 0, alias, SubPath(path, propertyName), 0, associationTypeSQLInfo);
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

		/// <inheritdoc />
		protected override IReadOnlyCollection<string> GetChildAliases(string parentSqlAlias, string childPath)
		{
			var alias = translator.GetChildAliases(parentSqlAlias, childPath);
			if (alias.Count == 0)
				return base.GetChildAliases(parentSqlAlias, childPath);
			return alias;
		}

		protected override JoinType GetJoinType(IAssociationType type, FetchMode config, string path, string pathAlias,
			string lhsTable, string[] lhsColumns, bool nullable, int currentDepth, CascadeStyle cascadeStyle)
		{
			if (translator.IsJoin(path, pathAlias))
			{
				return translator.GetJoinType(path, pathAlias);
			}

			if (translator.HasProjection)
			{
				return JoinType.None;
			}

			var selectMode = translator.RootCriteria.GetSelectMode(path);
			switch (selectMode)
			{
				case SelectMode.Undefined:
					return base.GetJoinType(type, config, path, pathAlias, lhsTable, lhsColumns, nullable, currentDepth, cascadeStyle);

				case SelectMode.Fetch:
				case SelectMode.FetchLazyProperties:
				case SelectMode.ChildFetch:
				case SelectMode.JoinOnly:
					IsDuplicateAssociation(lhsTable, lhsColumns, type); //deliberately ignore return value!
					return GetJoinType(nullable, currentDepth);
				
				case SelectMode.Skip:
					return JoinType.None;
				default:
					throw new ArgumentOutOfRangeException(nameof(selectMode), selectMode.ToString());
			}
		}

		protected override string GenerateTableAlias(int n, string path, string pathAlias, IJoinable joinable)
		{
			// TODO: deal with side-effects (changes to includeInSelectList, userAliasList, resultTypeList)!!!

			// for collection-of-entity, we are called twice for given "path"
			// once for the collection Joinable, once for the entity Joinable.
			// the second call will/must "consume" the alias + perform side effects according to consumesEntityAlias()
			// for collection-of-other, however, there is only one call 
			// it must "consume" the alias + perform side effects, despite what consumeEntityAlias() return says
			// 
			// note: the logic for adding to the userAliasList is still strictly based on consumesEntityAlias return value

			bool shouldCreateUserAlias = joinable.ConsumesEntityAlias();
			if (!shouldCreateUserAlias && joinable.IsCollection)
			{
				// is it a collection-of-other (component or value) ?
				var elementType = ((ICollectionPersister) joinable).ElementType;
				if (elementType != null)
					shouldCreateUserAlias = elementType.IsComponentType || !elementType.IsEntityType;
			}

			string sqlAlias = null;

			if (shouldCreateUserAlias)
			{
				var subcriteria = translator.GetCriteria(path, pathAlias);
				sqlAlias = subcriteria == null ? null : translator.GetSQLAlias(subcriteria);

				IncludeInResultIfNeeded(joinable, subcriteria, sqlAlias, path);
			}

			if (sqlAlias == null)
				sqlAlias = base.GenerateTableAlias(n + translator.SQLAliasCount, path, pathAlias, joinable);

			return sqlAlias;
		}

		private void IncludeInResultIfNeeded(IJoinable joinable, ICriteria subcriteria, string sqlAlias, string path)
		{
			if (joinable.ConsumesEntityAlias() && !translator.HasProjection)
			{
				var includedInSelect = translator.RootCriteria.GetSelectMode(path) != SelectMode.JoinOnly;
				includeInResultRowList.Add(subcriteria != null && subcriteria.Alias != null && includedInSelect);

				if (sqlAlias != null && includedInSelect)
				{
					if (subcriteria.Alias != null)
					{
						userAliasList.Add(subcriteria.Alias); //alias may be null
						resultTypeList.Add(translator.ResultType(subcriteria));
					}
				}
			}
		}

		protected override string GenerateRootAlias(string tableName)
		{
			return CriteriaQueryTranslator.RootSqlAlias;
			// NH: really not used (we are using a different ctor to support SubQueryCriteria)
		}

		protected override SqlString GetWithClause(string path, string pathAlias)
		{
			return translator.GetWithClause(path, pathAlias);
		}
	}
}
