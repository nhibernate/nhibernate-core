using System;
using System.Linq;
using NHibernate.Hql.Ast;
using NHibernate.Type;
using Remotion.Linq.EagerFetching;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessFetch
	{
		public void Process(FetchRequestBase resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			var querySource = QuerySourceLocator.FindQuerySource(
				queryModelVisitor.Model,
				resultOperator.RelationMember.DeclaringType);
			var name = queryModelVisitor.VisitorParameters.QuerySourceNamer.GetName(querySource);

			Process(resultOperator, queryModelVisitor, tree, name);
		}

		public void Process(FetchRequestBase resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree, string sourceAlias)
		{
			var memberPath = tree.TreeBuilder.Dot(
				tree.TreeBuilder.Ident(sourceAlias),
				tree.TreeBuilder.Ident(resultOperator.RelationMember.Name));

			Process(resultOperator, queryModelVisitor, tree, memberPath, null);
		}

		private void Process(FetchRequestBase resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree, HqlDot memberPath, IType propType)
		{
			if (resultOperator is FetchOneRequest)
			{
				if (propType == null)
				{
					var metadata = queryModelVisitor.VisitorParameters.SessionFactory
													.GetClassMetadata(resultOperator.RelationMember.ReflectedType);
					propType = metadata?.GetPropertyType(resultOperator.RelationMember.Name);
				}
				
				if (propType != null && !propType.IsAssociationType)
				{
					tree.AddFromLastChildClause(tree.TreeBuilder.Fetch());
					tree.AddFromLastChildClause(memberPath);

					ComponentType componentType = null;
					foreach (var innerFetch in resultOperator.InnerFetchRequests)
					{
						if (componentType == null)
						{
							componentType = propType as ComponentType;
							if (componentType == null)
							{
								throw new InvalidOperationException(
									$"Property {innerFetch.RelationMember.Name} cannot be fetched from a non component type property {resultOperator.RelationMember.Name}.");
							}
						}

						var subTypeIndex = componentType.GetPropertyIndex(innerFetch.RelationMember.Name);
						memberPath = tree.TreeBuilder.Dot(
							memberPath,
							tree.TreeBuilder.Ident(innerFetch.RelationMember.Name));

						Process(innerFetch, queryModelVisitor, tree, memberPath, componentType.Subtypes[subTypeIndex]);
					}

					return;
				}
			}

			var alias = queryModelVisitor.Model.GetNewName("_");
			tree.AddFromClause(tree.TreeBuilder.LeftFetchJoin(memberPath, tree.TreeBuilder.Alias(alias)));
			tree.AddDistinctRootOperator();

			foreach (var innerFetch in resultOperator.InnerFetchRequests)
			{
				Process(innerFetch, queryModelVisitor, tree, alias);
			}
		}
	}
}
