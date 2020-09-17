using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;

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
		internal static bool ProcessAsTableGroupJoin(IReadOnlyList<IJoin> tableGroupJoinables, SqlString[] withClauseFragments, bool includeAllSubclassJoins, JoinFragment joinFragment, Func<string, bool> isSubclassIncluded, ISessionFactoryImplementor sessionFactoryImplementor)
		{
			if (!TableGroupJoinHelper.NeedsTableGroupJoin(tableGroupJoinables, withClauseFragments, includeAllSubclassJoins))
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
						JoinHelper.GetRHSColumnNames(join.AssociationType, sessionFactoryImplementor),
						join.JoinType,
						SqlString.Empty);

				bool include = includeAllSubclassJoins && isSubclassIncluded(join.Alias);
				// TODO (from hibernate): Think about if this could be made always true 
				// NH Specific: made always true (original check: join.JoinType == JoinType.InnerJoin)
				var innerJoin = true;
				joinFragment.AddJoins(
					join.Joinable.FromJoinFragment(join.Alias, innerJoin, include),
					join.Joinable.WhereJoinFragment(join.Alias, innerJoin, include));
			}

			var withClause = TableGroupJoinHelper.GetTableGroupJoinWithClause(withClauseFragments, first, sessionFactoryImplementor);
			joinFragment.AddFromFragmentString(withClause);
			return true;
		}

		private static bool NeedsTableGroupJoin(IReadOnlyList<IJoin> joins, SqlString[] withClauseFragments, bool includeSubclasses)
		{
			// If the rewrite is disabled or we don't have a with clause, we don't need a table group join
			if ( /*!collectionJoinSubquery ||*/ withClauseFragments.All(x => SqlStringHelper.IsEmpty(x)))
			{
				return false;
			}

			// If we only have one join, a table group join is only necessary if subclass columns are used in the with clause
			if (joins.Count == 1)
			{
				return joins[0].Joinable is AbstractEntityPersister persister && persister.HasSubclassJoins(includeSubclasses);
				//NH Specific: No alias processing
				//return isSubclassAliasDereferenced( joins[ 0], withClauseFragment );
			}

			//NH Specific: No alias processing (see hibernate JoinSequence.NeedsTableGroupJoin)
			return true;
		}

		private static SqlString GetTableGroupJoinWithClause(SqlString[] withClauseFragments, IJoin first, ISessionFactoryImplementor factory)
		{
			SqlStringBuilder fromFragment = new SqlStringBuilder();
			fromFragment.Add(")").Add(" on ");

			string[] lhsColumns = first.LHSColumns;
			var isAssociationJoin = lhsColumns.Length > 0;
			if (isAssociationJoin)
			{
				string rhsAlias = first.Alias;
				string[] rhsColumns = JoinHelper.GetRHSColumnNames(first.AssociationType, factory);
				for (int j = 0; j < lhsColumns.Length; j++)
				{
					fromFragment.Add(lhsColumns[j]);
					fromFragment.Add("=");
					fromFragment.Add(rhsAlias);
					fromFragment.Add(".");
					fromFragment.Add(rhsColumns[j]);
					if (j < lhsColumns.Length - 1)
					{
						fromFragment.Add(" and ");
					}
				}
			}

			AppendWithClause(fromFragment, isAssociationJoin, withClauseFragments);

			return fromFragment.ToSqlString();
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
