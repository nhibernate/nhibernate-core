﻿using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Hql.Ast;
using NHibernate.Persister.Entity;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessOfType : IResultOperatorProcessor<OfTypeResultOperator>
	{
		#region IResultOperatorProcessor<OfTypeResultOperator> Members

		public void Process(
			OfTypeResultOperator resultOperator,
			QueryModelVisitor queryModelVisitor,
			IntermediateHqlTree tree)
		{
			var fromItemEnumerableType = queryModelVisitor.Model.MainFromClause.FromExpression.Type;
			var fromItemType = typeof(object);
			var asEnumerable =
				fromItemEnumerableType.GetInterfaces()
									.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
			if (asEnumerable != null)
			{
				fromItemType = asEnumerable.GetGenericArguments()[0];
			}

			var fromItemImplementorQueue =
				new Queue<string>(queryModelVisitor.VisitorParameters.SessionFactory.GetImplementors(fromItemType.FullName));
			var fromItemTypeNamesProcessed = new HashSet<string>();
			var fromItemTypePersisters = new List<IEntityPersister>();
			while (fromItemImplementorQueue.Any())
			{
				var currentImplementor = fromItemImplementorQueue.Dequeue();
				if (fromItemTypeNamesProcessed.Add(currentImplementor))
				{
					var persister = queryModelVisitor.VisitorParameters.SessionFactory.TryGetEntityPersister(currentImplementor);
					if (persister != null)
					{
						fromItemTypePersisters.Add(persister);
						persister.EntityMetamodel.SubclassEntityNames.ToList().ForEach(fromItemImplementorQueue.Enqueue);
					}
				}
			}

			// Which of the mapped types are assignable to both the source -- meaning that the property could actually
			// be of the mapped type -- and to the searched item type?
			var persistersToUse =
				fromItemTypePersisters.Where(
					p =>
					{
						var mappedClass = p.GetMappedClass(EntityMode.Poco);
						return fromItemType.IsAssignableFrom(mappedClass) && resultOperator.SearchedItemType.IsAssignableFrom(mappedClass);
					}).ToList();

			// If the persisters are not among the subclass persister types, there will be no class property, so the query would fail
			// if we tried to include the class literal in the query anyway.  Also, if there are no applicable persisters, no results
			// can be returned, so add "WHERE 1 = 0" in those cases.
			if (persistersToUse.Count == 0)
			{
				tree.AddWhereClause(tree.TreeBuilder.Equality(tree.TreeBuilder.Constant(1), tree.TreeBuilder.Constant(0)));
				// Because the rest of the query may be invalid (e.g., by referencing properties that do not exist),
				// delete any remaining body clauses.
				queryModelVisitor.Model.BodyClauses.Clear();
				return;
			}
			if (!persistersToUse.Any(p => p.EntityMetamodel.HasSubclasses || p.EntityMetamodel.SuperclassType != null))
			{
				// All results should be returned, so no point adding a where clause
				return;
			}

			var typesToUse =
				fromItemTypePersisters.Select(p => p.GetMappedClass(EntityMode.Poco))
									.Where(t => fromItemType.IsAssignableFrom(t) && resultOperator.SearchedItemType.IsAssignableFrom(t)).ToList();
			var classesToUse = typesToUse.Select(t => t.FullName).ToList();

			// It appears that ReLinq's QuerySourceLocator.FindQuerySource(QueryModel, Type) method may find the source
			// only sometimes when specifying fromItemType and only sometimes when specifying the return type of the
			// select clause.  There may be other scenarios not yet considered here.  For now, try both, in order,
			// plus the actual output types.  Give up if we can't find the correct query source.
			var querySourceLocatorCandidateTypeList = new List<System.Type>()
			{
				fromItemType,
				queryModelVisitor.Model.SelectClause.GetOutputDataInfo().ResultItemType
			};
			querySourceLocatorCandidateTypeList.AddRange(typesToUse);
			var querySource =
				querySourceLocatorCandidateTypeList.Select(t => QuerySourceLocator.FindQuerySource(queryModelVisitor.Model, t))
													.FirstOrDefault(qs => qs != null);

			if (querySource == null)
			{
				throw new QueryException(
					String.Format(
						"Unable to find QuerySource for any of these types: {0}",
						String.Join(", ", querySourceLocatorCandidateTypeList.Select(t => t.FullName))));
			}

			var querySourceName = queryModelVisitor.VisitorParameters.QuerySourceNamer.GetName(querySource);

			var dotNode =
				tree.TreeBuilder.Dot(
					tree.TreeBuilder.Ident(querySourceName),
					tree.TreeBuilder.Class());

			// For now, use the name of the persisted class as a literal identifier.  The string value containing
			// the full class name is translated to a discriminator column value in
			// NHibernate.Hql.Ast.ANTLR.Util.LiteralProcessor.ProcessConstant(SqlNode, bool).
			if (classesToUse.Count == 1)
			{
				tree.AddWhereClause(tree.TreeBuilder.Equality(dotNode, tree.TreeBuilder.Ident(classesToUse[0])));
			}
			else
			{
				var implementorNodes = classesToUse.Select(tree.TreeBuilder.Ident).OfType<HqlTreeNode>().ToArray();
				tree.AddWhereClause(tree.TreeBuilder.In(dotNode, implementorNodes));
			}
		}

		#endregion
	}
}