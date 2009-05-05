using System;
using Antlr.Runtime.Tree;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Param;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary> 
	/// Encapsulates the information relating to an individual assignment within the
	/// set clause of an HQL update statement.  This information is used during execution
	/// of the update statements when the updates occur against "multi-table" stuff. 
	/// </summary>
	[CLSCompliant(false)]
	public class AssignmentSpecification
	{
		private readonly IASTNode eq;
		private readonly ISessionFactoryImplementor factory;
		private readonly ISet<string> tableNames;
		private readonly IParameterSpecification[] hqlParameters;
		private SqlString sqlAssignmentString;

		public AssignmentSpecification(IASTNode eq, IQueryable persister)
		{
			if (eq.Type != HqlSqlWalker.EQ)
			{
				throw new QueryException("assignment in set-clause not associated with equals");
			}

			this.eq = eq;
			factory = persister.Factory;

			// Needed to bump this up to DotNode, because that is the only thing which currently
			// knows about the property-ref path in the correct format; it is either this, or
			// recurse over the DotNodes constructing the property path just like DotNode does
			// internally
			var lhs = (DotNode)eq.GetFirstChild();
			var rhs = (SqlNode)lhs.NextSibling;

			ValidateLhs(lhs);

			string propertyPath = lhs.PropertyPath;
			var temp = new HashedSet<string>();
			// yuck!
			var usep = persister as UnionSubclassEntityPersister;
			if (usep!=null)
			{
				temp.AddAll(persister.ConstraintOrderedTableNameClosure);
			}
			else
			{
				temp.Add(persister.GetSubclassTableName(persister.GetSubclassPropertyTableNumber(propertyPath)));
			}
			tableNames = new ImmutableSet<string>(temp);

			if (rhs == null)
			{
				hqlParameters = new IParameterSpecification[0];
			}
			else if (IsParam(rhs))
			{
				hqlParameters = new[] { ((ParameterNode)rhs).HqlParameterSpecification };
			}
			else
			{
				var parameterList = ASTUtil.CollectChildren(rhs, IsParam);
				hqlParameters = new IParameterSpecification[parameterList.Count];
				int i = 0;
				foreach (ParameterNode parameterNode in parameterList)
				{
					hqlParameters[i++] = parameterNode.HqlParameterSpecification;
				}
			}
		}
		public bool AffectsTable(string tableName)
		{
			return tableNames.Contains(tableName);
		}

		private static bool IsParam(IASTNode node)
		{
			return node.Type == HqlSqlWalker.PARAM || node.Type == HqlSqlWalker.NAMED_PARAM;
		}

		private static void ValidateLhs(FromReferenceNode lhs)
		{
			// make sure the lhs is "assignable"...
			if (!lhs.IsResolved)
			{
				throw new NotSupportedException("cannot validate assignablity of unresolved node");
			}

			if (lhs.DataType.IsCollectionType)
			{
				throw new QueryException("collections not assignable in update statements");
			}
			else if (lhs.DataType.IsComponentType)
			{
				throw new QueryException("Components currently not assignable in update statements");
			}
			else if (lhs.DataType.IsEntityType)
			{
				// currently allowed...
			}

			// TODO : why aren't these the same?
			if (lhs.GetImpliedJoin() != null || lhs.FromElement.IsImplied)
			{
				throw new QueryException("Implied join paths are not assignable in update statements");
			}
		}

		public IParameterSpecification[] Parameters
		{
			get{return hqlParameters;}
		}

		public SqlString SqlAssignmentFragment
		{
			get
			{
				if (sqlAssignmentString == null)
				{
					try
					{
						var gen = new SqlGenerator(factory, new CommonTreeNodeStream(eq));
						gen.comparisonExpr(false); // false indicates to not generate parens around the assignment
						sqlAssignmentString = gen.GetSQL();
					}
					catch (Exception t)
					{
						throw new QueryException("cannot interpret set-clause assignment", t);
					}
				}
				return sqlAssignmentString;
			}

		}
	}
}