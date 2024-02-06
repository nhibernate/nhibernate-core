using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Engine
{
	//Generates table group join if neccessary. Example of generated query with table group join:
	// SELECT *
	// FROM Person person0_
	// INNER JOIN (
	// IndividualCustomer individual1_ 
	// INNER JOIN Customer individual1_1_ ON individual1_.IndividualCustomerID = individual1_1_.Id
	// ) ON person0_.Id = individual1_.PersonID AND individual1_1_.Deleted = @p0
	internal class TableGroupJoinHelper
	{
		internal static bool ProcessAsTableGroupJoin(IReadOnlyList<IJoin> tableGroupJoinables, SqlString[] withClauseFragments, bool includeAllSubclassJoins, JoinFragment joinFragment, Func<string, bool> isSubclassIncluded)
		{
			if (!NeedsTableGroupJoin(tableGroupJoinables, withClauseFragments, includeAllSubclassJoins, isSubclassIncluded))
				return false;

			var first = tableGroupJoinables[0];
			string joinString = ANSIJoinFragment.GetJoinString(first.JoinType);
			joinFragment.AddFromFragmentString(
				new SqlString(
					joinString,
					" (",
					first.Joinable.TableName,
					" ",
					first.Alias
				));

			foreach (var join in tableGroupJoinables)
			{
				if (join != first)
					joinFragment.AddJoin(
						join.Joinable.TableName,
						join.Alias,
						join.LHSColumns,
						join.RHSColumns,
						join.JoinType,
						SqlString.Empty);

				bool include = includeAllSubclassJoins && isSubclassIncluded(join.Alias);
				// TODO (from hibernate): Think about if this could be made always true 
				// NH Specific: made always true (original check: join.JoinType == JoinType.InnerJoin)
				const bool innerJoin = true;
				joinFragment.AddJoins(
					join.Joinable.FromJoinFragment(join.Alias, innerJoin, include),
					join.Joinable.WhereJoinFragment(join.Alias, innerJoin, include));
			}

			var withClause = GetTableGroupJoinWithClause(withClauseFragments, first);
			joinFragment.AddFromFragmentString(withClause);
			return true;
		}

		// detect cases when withClause is used on multiple tables or when join keys depend on subclass columns
		private static bool NeedsTableGroupJoin(IReadOnlyList<IJoin> joins, SqlString[] withClauseFragments, bool includeSubclasses, Func<string, bool> isSubclassIncluded)
		{
			bool hasWithClause = withClauseFragments.Any(x => SqlStringHelper.IsNotEmpty(x));

			//NH Specific: No alias processing (see hibernate JoinSequence.NeedsTableGroupJoin)
			if (joins.Count > 1 && hasWithClause)
				return true;

			foreach (var join in joins)
			{
				var entityPersister = GetEntityPersister(join.Joinable, out var manyToManyType);
				if (manyToManyType?.IsNullable == true)
					return true;

				if (entityPersister?.HasSubclassJoins(includeSubclasses && isSubclassIncluded(join.Alias)) != true)
					continue;

				if (hasWithClause)
					return true;

				if (manyToManyType == null // many-to-many keys are stored in separate table
				    && entityPersister.ColumnsDependOnSubclassJoins(join.RHSColumns))
				{
					return true;
				}
			}

			return false;
		}

		private static SqlString GetTableGroupJoinWithClause(SqlString[] withClauseFragments, IJoin first)
		{
			SqlStringBuilder fromFragment = new SqlStringBuilder();
			fromFragment.Add(")").Add(" on ");

			string[] lhsColumns = first.LHSColumns;
			var isAssociationJoin = lhsColumns.Length > 0;
			if (isAssociationJoin)
			{
				var entityPersister = GetEntityPersister(first.Joinable, out var manyToManyType);
				string rhsAlias = first.Alias;
				string[] rhsColumns = first.RHSColumns;
				for (int j = 0; j < lhsColumns.Length; j++)
				{
					fromFragment.Add(lhsColumns[j])
								.Add("=")
								.Add((entityPersister == null || manyToManyType != null) // many-to-many keys are stored in separate table
									     ? rhsAlias
									     : entityPersister.GenerateTableAliasForColumn(rhsAlias, rhsColumns[j]))
								.Add(".")
								.Add(rhsColumns[j]);
					if (j != lhsColumns.Length - 1)
						fromFragment.Add(" and ");
				}
			}

			AppendWithClause(fromFragment, isAssociationJoin, withClauseFragments);

			return fromFragment.ToSqlString();
		}

		private static AbstractEntityPersister GetEntityPersister(IJoinable joinable, out EntityType manyToManyType)
		{
			manyToManyType = null;
			if (!joinable.IsCollection)
				return joinable as AbstractEntityPersister;

			var collection = (IQueryableCollection) joinable;
			if (!collection.ElementType.IsEntityType)
				return null;
			if (collection.IsManyToMany)
				manyToManyType = (EntityType) collection.ElementType;
			return collection.ElementPersister as AbstractEntityPersister;
		}

		private static void AppendWithClause(SqlStringBuilder fromFragment, bool hasConditions, SqlString[] withClauseFragments)
		{
			for (var i = 0; i < withClauseFragments.Length; i++)
			{
				var withClause = withClauseFragments[i];
				if (SqlStringHelper.IsEmpty(withClause))
					continue;

				if (withClause.StartsWithCaseInsensitive(" and "))
				{
					if (!hasConditions)
					{
						withClause = withClause.Substring(4);
					}
				}
				else if (hasConditions)
				{
					fromFragment.Add(" and ");
				}

				fromFragment.Add(withClause);
				hasConditions = true;
			}
		}
	}
}
