namespace NHibernate.Mapping.ByCode.Impl
{
	public delegate void RootClassMappingHandler(IModelInspector modelInspector, System.Type type, IClassAttributesMapper classCustomizer);

	public delegate void SubclassMappingHandler(IModelInspector modelInspector, System.Type type, ISubclassAttributesMapper subclassCustomizer);

	public delegate void JoinedSubclassMappingHandler(IModelInspector modelInspector, System.Type type, IJoinedSubclassAttributesMapper joinedSubclassCustomizer);

	public delegate void UnionSubclassMappingHandler(IModelInspector modelInspector, System.Type type, IUnionSubclassAttributesMapper unionSubclassCustomizer);

	public delegate void PropertyMappingHandler(IModelInspector modelInspector, PropertyPath member, IPropertyMapper propertyCustomizer);

	public delegate void ManyToOneMappingHandler(IModelInspector modelInspector, PropertyPath member, IManyToOneMapper propertyCustomizer);

	public delegate void OneToOneMappingHandler(IModelInspector modelInspector, PropertyPath member, IOneToOneMapper propertyCustomizer);

	public delegate void AnyMappingHandler(IModelInspector modelInspector, PropertyPath member, IAnyMapper propertyCustomizer);

	public delegate void ComponentMappingHandler(IModelInspector modelInspector, PropertyPath member, IComponentAttributesMapper propertyCustomizer);

	public delegate void SetMappingHandler(IModelInspector modelInspector, PropertyPath member, ISetPropertiesMapper propertyCustomizer);

	public delegate void BagMappingHandler(IModelInspector modelInspector, PropertyPath member, IBagPropertiesMapper propertyCustomizer);

	public delegate void IdBagMappingHandler(IModelInspector modelInspector, PropertyPath member, IIdBagPropertiesMapper propertyCustomizer);

	public delegate void ListMappingHandler(IModelInspector modelInspector, PropertyPath member, IListPropertiesMapper propertyCustomizer);

	public delegate void MapMappingHandler(IModelInspector modelInspector, PropertyPath member, IMapPropertiesMapper propertyCustomizer);

	public delegate void ManyToManyMappingHandler(IModelInspector modelInspector, PropertyPath member, IManyToManyMapper collectionRelationManyToManyCustomizer);

	public delegate void ElementMappingHandler(IModelInspector modelInspector, PropertyPath member, IElementMapper collectionRelationElementCustomizer);

	public delegate void OneToManyMappingHandler(IModelInspector modelInspector, PropertyPath member, IOneToManyMapper collectionRelationOneToManyCustomizer);

	public delegate void MapKeyManyToManyMappingHandler(IModelInspector modelInspector, PropertyPath member, IMapKeyManyToManyMapper mapKeyManyToManyCustomizer);

	public delegate void MapKeyMappingHandler(IModelInspector modelInspector, PropertyPath member, IMapKeyMapper mapKeyElementCustomizer);
}