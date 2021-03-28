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

			foreach (var o in source.RootEntities) destination.AddAsRootEntity(o);
			foreach (var o in source.Components) destination.AddAsComponent(o);
			foreach (var o in source.TablePerClassEntities) destination.AddAsTablePerClassEntity(o);
			foreach (var o in source.TablePerClassHierarchyEntities) destination.AddAsTablePerClassHierarchyEntity(o);
			foreach (var o in source.TablePerConcreteClassEntities) destination.AddAsTablePerConcreteClassEntity(o);

			foreach (var o in source.OneToOneRelations) destination.AddAsOneToOneRelation(o);
			foreach (var o in source.ManyToOneRelations) destination.AddAsManyToOneRelation(o);
			foreach (var o in source.KeyManyToManyRelations) destination.AddAsManyToManyKeyRelation(o);
			foreach (var o in source.ItemManyToManyRelations) destination.AddAsManyToManyItemRelation(o);
			foreach (var o in source.ManyToAnyRelations) destination.AddAsManyToAnyRelation(o);
			foreach (var o in source.OneToManyRelations) destination.AddAsOneToManyRelation(o);
			foreach (var o in source.Any) destination.AddAsAny(o);

			foreach (var o in source.Poids) destination.AddAsPoid(o);
			foreach (var o in source.ComposedIds) destination.AddAsPartOfComposedId(o);
			foreach (var o in source.VersionProperties) destination.AddAsVersionProperty(o);
			foreach (var o in source.NaturalIds) destination.AddAsNaturalId(o);

			foreach (var o in source.Sets) destination.AddAsSet(o);
			foreach (var o in source.Bags) destination.AddAsBag(o);
			foreach (var o in source.IdBags) destination.AddAsIdBag(o);
			foreach (var o in source.Lists) destination.AddAsList(o);
			foreach (var o in source.Arrays) destination.AddAsArray(o);
			foreach (var o in source.Dictionaries) destination.AddAsMap(o);
			foreach (var o in source.Properties) destination.AddAsProperty(o);
			foreach (var o in source.PersistentMembers) destination.AddAsPersistentMember(o);
			foreach (var o in source.SplitDefinitions) destination.AddAsPropertySplit(o);
			foreach (var dynamicComponent in source.DynamicComponents)
			{
				var template = source.GetDynamicComponentTemplate(dynamicComponent);
				destination.AddAsDynamicComponent(dynamicComponent, template);
			}
		}
	}
}
