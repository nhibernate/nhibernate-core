using System.Linq;
using NHibernate.Util;

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

			source.RootEntities.ForEach(destination.AddAsRootEntity);
			source.Components.ForEach(destination.AddAsComponent);
			source.TablePerClassEntities.ForEach(destination.AddAsTablePerClassEntity);
			source.TablePerClassHierarchyEntities.ForEach(destination.AddAsTablePerClassHierarchyEntity);
			source.TablePerConcreteClassEntities.ForEach(destination.AddAsTablePerConcreteClassEntity);

			source.OneToOneRelations.ForEach(destination.AddAsOneToOneRelation);
			source.ManyToOneRelations.ForEach(destination.AddAsManyToOneRelation);
			source.KeyManyToManyRelations.ForEach(destination.AddAsManyToManyKeyRelation);
			source.ItemManyToManyRelations.ForEach(destination.AddAsManyToManyItemRelation);
			source.ManyToAnyRelations.ForEach(destination.AddAsManyToAnyRelation);
			source.OneToManyRelations.ForEach(destination.AddAsOneToManyRelation);
			source.Any.ForEach(destination.AddAsAny);

			source.Poids.ForEach(destination.AddAsPoid);
			source.ComposedIds.ForEach(destination.AddAsPartOfComposedId);
			source.VersionProperties.ForEach(destination.AddAsVersionProperty);
			source.NaturalIds.ForEach(destination.AddAsNaturalId);

			source.Sets.ForEach(destination.AddAsSet);
			source.Bags.ForEach(destination.AddAsBag);
			source.IdBags.ForEach(destination.AddAsIdBag);
			source.Lists.ForEach(destination.AddAsList);
			source.Arrays.ForEach(destination.AddAsArray);
			source.Dictionaries.ForEach(destination.AddAsMap);
			source.Properties.ForEach(destination.AddAsProperty);
			source.PersistentMembers.ForEach(destination.AddAsPersistentMember);
			source.SplitDefinitions.ForEach(destination.AddAsPropertySplit);
			foreach (var dynamicComponent in source.DynamicComponents)
			{
				var template = source.GetDynamicComponentTemplate(dynamicComponent);
				destination.AddAsDynamicComponent(dynamicComponent, template);
			}
		}
	}
}
