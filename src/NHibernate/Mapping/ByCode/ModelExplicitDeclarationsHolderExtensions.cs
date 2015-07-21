using System.Linq;

namespace NHibernate.Mapping.ByCode
{
	public static class ModelExplicitDeclarationsHolderExtensions
	{
		public static void Merge(this IModelExplicitDeclarationsHolder destination, IModelExplicitDeclarationsHolder source)
		{
			if (destination == null || source == null)
			{
				return;
			}

			System.Array.ForEach(source.RootEntities.ToArray(), destination.AddAsRootEntity);
			System.Array.ForEach(source.Components.ToArray(), destination.AddAsComponent);
			System.Array.ForEach(source.TablePerClassEntities.ToArray(), destination.AddAsTablePerClassEntity);
			System.Array.ForEach(source.TablePerClassHierarchyEntities.ToArray(), destination.AddAsTablePerClassHierarchyEntity);
			System.Array.ForEach(source.TablePerConcreteClassEntities.ToArray(), destination.AddAsTablePerConcreteClassEntity);

			System.Array.ForEach(source.OneToOneRelations.ToArray(), destination.AddAsOneToOneRelation);
			System.Array.ForEach(source.ManyToOneRelations.ToArray(), destination.AddAsManyToOneRelation);
			System.Array.ForEach(source.ManyToManyRelations.ToArray(), destination.AddAsManyToManyRelation);
			System.Array.ForEach(source.ManyToAnyRelations.ToArray(), destination.AddAsManyToAnyRelation);
			System.Array.ForEach(source.OneToManyRelations.ToArray(), destination.AddAsOneToManyRelation);
			System.Array.ForEach(source.Any.ToArray(), destination.AddAsAny);

			System.Array.ForEach(source.Poids.ToArray(), destination.AddAsPoid);
			System.Array.ForEach(source.ComposedIds.ToArray(), destination.AddAsPartOfComposedId);
			System.Array.ForEach(source.VersionProperties.ToArray(), destination.AddAsVersionProperty);
			System.Array.ForEach(source.NaturalIds.ToArray(), destination.AddAsNaturalId);

			System.Array.ForEach(source.Sets.ToArray(), destination.AddAsSet);
			System.Array.ForEach(source.Bags.ToArray(), destination.AddAsBag);
			System.Array.ForEach(source.IdBags.ToArray(), destination.AddAsIdBag);
			System.Array.ForEach(source.Lists.ToArray(), destination.AddAsList);
			System.Array.ForEach(source.Arrays.ToArray(), destination.AddAsArray);
			System.Array.ForEach(source.Dictionaries.ToArray(), destination.AddAsMap);
			System.Array.ForEach(source.Properties.ToArray(), destination.AddAsProperty);
			System.Array.ForEach(source.PersistentMembers.ToArray(), destination.AddAsPersistentMember);
			System.Array.ForEach(source.SplitDefinitions.ToArray(), destination.AddAsPropertySplit);
			foreach (var dynamicComponent in source.DynamicComponents)
			{
				var template = source.GetDynamicComponentTemplate(dynamicComponent);
				destination.AddAsDynamicComponent(dynamicComponent, template);
			}
		}
	}
}