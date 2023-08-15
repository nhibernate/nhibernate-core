using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine
{
	public class JoinSequence
	{
		private readonly ISessionFactoryImplementor factory;
		private readonly List<Join> joins = new List<Join>();
		private bool useThetaStyle = false;
		private readonly SqlStringBuilder conditions = new SqlStringBuilder();
		private string rootAlias;
		private IJoinable rootJoinable;
		private ISelector selector;
		private JoinSequence next;
		private bool isFromPart = false;

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append("JoinSequence{");
			if (rootJoinable != null)
			{
				buf.Append(rootJoinable)
					.Append('[')
					.Append(rootAlias)
					.Append(']');
			}
			for (int i = 0; i < joins.Count; i++)
			{
				buf.Append("->").Append(joins[i]);
			}
			return buf.Append('}').ToString();
		}

		private sealed class Join : IJoin
		{
			private readonly IAssociationType associationType;
			private readonly IJoinable joinable;
			private JoinType joinType;
			private readonly string alias;
			private readonly string[] lhsColumns;
			private readonly string[] rhsColumns;

			public Join(ISessionFactoryImplementor factory, IAssociationType associationType, string alias, JoinType joinType,
						string[] lhsColumns)
			{
				this.associationType = associationType;
				this.joinable = associationType.GetAssociatedJoinable(factory);
				this.alias = alias;
				this.joinType = joinType;
				this.lhsColumns = lhsColumns;
				this.rhsColumns = lhsColumns.Length > 0
					? JoinHelper.GetRHSColumnNames(joinable, associationType)
					: Array.Empty<string>();
			}

			public string Alias
			{
				get { return alias; }
			}

			public IAssociationType AssociationType
			{
				get { return associationType; }
			}

			public IJoinable Joinable
			{
				get { return joinable; }
			}

			public JoinType JoinType
			{
				get { return joinType; }
				internal set { joinType = value; }
			}

			public string[] LHSColumns
			{
				get { return lhsColumns; }
			}

			public string[] RHSColumns
			{
				get { return rhsColumns; }
			}

			public override string ToString()
			{
				return joinable.ToString() + '[' + alias + ']';
			}
		}

		public JoinSequence(ISessionFactoryImplementor factory)
		{
			this.factory = factory;
		}

		public JoinSequence GetFromPart()
		{
			JoinSequence fromPart = new JoinSequence(factory);
			fromPart.joins.AddRange(this.joins);
			fromPart.useThetaStyle = this.useThetaStyle;
			fromPart.rootAlias = this.rootAlias;
			fromPart.rootJoinable = this.rootJoinable;
			fromPart.selector = this.selector;
			fromPart.next = this.next == null ? null : this.next.GetFromPart();
			fromPart.isFromPart = true;
			return fromPart;
		}

		public JoinSequence Copy()
		{
			JoinSequence copy = new JoinSequence(factory);
			copy.joins.AddRange(this.joins);
			copy.useThetaStyle = this.useThetaStyle;
			copy.rootAlias = this.rootAlias;
			copy.rootJoinable = this.rootJoinable;
			copy.selector = this.selector;
			copy.next = this.next == null ? null : this.next.Copy();
			copy.isFromPart = this.isFromPart;
			copy.conditions.Add(this.conditions.ToSqlString());
			return copy;
		}

		public JoinSequence AddJoin(IAssociationType associationType, string alias, JoinType joinType, string[] referencingKey)
		{
			joins.Add(new Join(factory, associationType, alias, joinType, referencingKey));
			return this;
		}

		public JoinFragment ToJoinFragment()
		{
			return ToJoinFragment(CollectionHelper.EmptyDictionary<string, IFilter>(), true);
		}

		public JoinFragment ToJoinFragment(IDictionary<string, IFilter> enabledFilters, bool includeExtraJoins)
		{
			return ToJoinFragment(enabledFilters, includeExtraJoins, true, null);
		}

		public JoinFragment ToJoinFragment(IDictionary<string, IFilter> enabledFilters, bool includeAllSubclassJoins, SqlString withClause)
		{
			return ToJoinFragment(enabledFilters, includeAllSubclassJoins, true, withClause);
		}

		// Since 5.4
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public JoinFragment ToJoinFragment(
			IDictionary<string, IFilter> enabledFilters,
			bool includeExtraJoins,
			SqlString withClauseFragment,
			string withClauseJoinAlias)
		{
			return ToJoinFragment(enabledFilters, includeExtraJoins, true, withClauseFragment);
		}

		internal virtual JoinFragment ToJoinFragment(
			IDictionary<string, IFilter> enabledFilters,
			bool includeAllSubclassJoins,
			bool renderSubclassJoins,
			SqlString withClauseFragment)
		{
			QueryJoinFragment joinFragment = new QueryJoinFragment(factory.Dialect, useThetaStyle);
			if (rootJoinable != null)
			{
				joinFragment.AddCrossJoin(rootJoinable.TableName, rootAlias);
				string filterCondition = rootJoinable.FilterFragment(rootAlias, enabledFilters);
				// JoinProcessor needs to know if the where clause fragment came from a dynamic filter or not so it
				// can put the where clause fragment in the right place in the SQL AST.   'hasFilterCondition' keeps track
				// of that fact.
				joinFragment.HasFilterCondition = joinFragment.AddCondition(filterCondition);
				AddSubclassJoins(joinFragment, rootAlias, rootJoinable, true, includeAllSubclassJoins);
			}

			var withClauses = new SqlString[joins.Count];
			IJoinable last = rootJoinable;
			for (int i = 0; i < joins.Count; i++)
			{
				Join join = joins[i];

				withClauses[i] = GetWithClause(enabledFilters, ref withClauseFragment, join, last);
				last = join.Joinable;
			}

			if (rootJoinable == null && !IsThetaStyle && TableGroupJoinHelper.ProcessAsTableGroupJoin(joins, withClauses, includeAllSubclassJoins, joinFragment, alias => IsIncluded(alias)))
			{
				return joinFragment;
			}

			for (int i = 0; i < joins.Count; i++)
			{
				Join join = joins[i];

				// NH: the variable "condition" have to be a SqlString because it may contains Parameter instances with BackTrack
				joinFragment.AddJoin(
					join.Joinable.TableName,
					join.Alias,
					join.LHSColumns,
					join.RHSColumns,
					join.JoinType,
					withClauses[i]
				);

				AddSubclassJoins(joinFragment, join.Alias, join.Joinable, join.JoinType == JoinType.InnerJoin, renderSubclassJoins);
			}

			if (next != null)
			{
				joinFragment.AddFragment(next.ToJoinFragment(enabledFilters, includeAllSubclassJoins));
			}
			joinFragment.AddCondition(conditions.ToSqlString());
			if (isFromPart)
				joinFragment.ClearWherePart();
			return joinFragment;
		}

		private SqlString GetWithClause(IDictionary<string, IFilter> enabledFilters, ref SqlString withClauseFragment, Join join, IJoinable last)
		{
			string on = join.AssociationType.GetOnCondition(join.Alias, factory, enabledFilters);
			var withConditions = new List<object>();

			if (!string.IsNullOrEmpty(on))
				withConditions.Add(on);

			if (last != null &&
				IsManyToManyRoot(last) &&
				((IQueryableCollection) last).ElementType == join.AssociationType)
			{
				// the current join represents the join between a many-to-many association table
				// and its "target" table.  Here we need to apply any additional filters
				// defined specifically on the many-to-many
				string manyToManyFilter = ((IQueryableCollection) last)
					.GetManyToManyFilterFragment(join.Alias, enabledFilters);

				if (!string.IsNullOrEmpty(manyToManyFilter))
					withConditions.Add(manyToManyFilter);
			}
			else if (string.IsNullOrEmpty(on))
			{
				// NH Different behavior : NH1179 and NH1293
				// Apply filters for entity joins and Many-To-One association
				var enabledFiltersForJoin = ForceFilter ? enabledFilters : FilterHelper.GetEnabledForManyToOne(enabledFilters);
				if (ForceFilter || enabledFiltersForJoin.Count > 0)
					withConditions.Add(join.Joinable.FilterFragment(join.Alias, enabledFiltersForJoin));
			}

			if (withClauseFragment != null && !IsManyToManyRoot(join.Joinable))
			{
				withConditions.Add(withClauseFragment);
				withClauseFragment = null;
			}

			return SqlStringHelper.JoinParts(" and ", withConditions);
		}

		private bool IsManyToManyRoot(IJoinable joinable)
		{
			if (joinable != null && joinable.IsCollection)
			{
				IQueryableCollection persister = (IQueryableCollection) joinable;
				return persister.IsManyToMany;
			}
			return false;
		}

		private bool IsIncluded(string alias)
		{
			return selector != null && selector.IncludeSubclasses(alias);
		}

		private void AddSubclassJoins(JoinFragment joinFragment, String alias, IJoinable joinable, bool innerJoin, bool includeSubclassJoins)
		{
			bool include = includeSubclassJoins && IsIncluded(alias);
			joinFragment.AddJoins(joinable.FromJoinFragment(alias, innerJoin, include),
			                      joinable.WhereJoinFragment(alias, innerJoin, include));
		}

		public JoinSequence AddCondition(SqlString condition)
		{
			if (!condition.IsEmptyOrWhitespace())
			{
				if (!condition.StartsWithCaseInsensitive(" and "))
					conditions.Add(" and ");
				conditions.Add(condition);
			}
			return this;
		}

		public JoinSequence AddCondition(string alias, string[] columns, string condition, bool appendParameter)
		{
			for (int i = 0; i < columns.Length; i++)
			{
				conditions.Add(" and ")
					.Add(alias)
					.Add(".")
					.Add(columns[i])
					.Add(condition);
				if (appendParameter)
				{
					conditions.AddParameter();
				}
			}
			return this;
		}

		public JoinSequence SetRoot(IJoinable joinable, string alias)
		{
			this.rootAlias = alias;
			this.rootJoinable = joinable;
			return this;
		}

		public JoinSequence SetNext(JoinSequence next)
		{
			this.next = next;
			return this;
		}

		public JoinSequence SetSelector(ISelector s)
		{
			this.selector = s;
			return this;
		}

		public JoinSequence SetUseThetaStyle(bool useThetaStyle)
		{
			this.useThetaStyle = useThetaStyle;
			return this;
		}

		public bool IsThetaStyle
		{
			get { return useThetaStyle; }
		}

		public int JoinCount
		{
			get { return joins.Count; }
		}

		public interface ISelector
		{
			bool IncludeSubclasses(string alias);
		}

		internal string RootAlias => rootAlias;

		public ISessionFactoryImplementor Factory => factory;

		internal bool ForceFilter { get; set; }

		public JoinSequence AddJoin(FromElement fromElement)
		{
			joins.AddRange(fromElement.JoinSequence.joins);
			return this;
		}

		internal void SetJoinType(JoinType joinType)
		{
			joins[0].JoinType = joinType;
			SetUseThetaStyle(false);
		}
	}
}
