using System;
using System.Collections.Generic;

namespace NHibernate.Mapping.ByCode.Impl
{
	public interface ICustomizersHolder
	{
		void AddCustomizer(System.Type type, Action<IClassMapper> classCustomizer);
		void AddCustomizer(System.Type type, Action<ISubclassMapper> classCustomizer);
		void AddCustomizer(System.Type type, Action<IJoinedSubclassAttributesMapper> classCustomizer);
		void AddCustomizer(System.Type type, Action<IUnionSubclassAttributesMapper> classCustomizer);
		void AddCustomizer(System.Type type, Action<IComponentAttributesMapper> classCustomizer);
		void AddCustomizer(System.Type type, Action<IJoinAttributesMapper> classCustomizer);

		void AddCustomizer(PropertyPath member, Action<IPropertyMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IManyToOneMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IOneToOneMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IAnyMapper> propertyCustomizer);

		void AddCustomizer(PropertyPath member, Action<ISetPropertiesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IBagPropertiesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IListPropertiesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IMapPropertiesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IIdBagPropertiesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<ICollectionPropertiesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IComponentAttributesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IComponentAsIdAttributesMapper> propertyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IDynamicComponentAttributesMapper> propertyCustomizer);

		void InvokeCustomizers(System.Type type, IClassMapper mapper);
		void InvokeCustomizers(System.Type type, ISubclassMapper mapper);
		void InvokeCustomizers(System.Type type, IJoinedSubclassAttributesMapper mapper);
		void InvokeCustomizers(System.Type type, IUnionSubclassAttributesMapper mapper);
		void InvokeCustomizers(System.Type type, IComponentAttributesMapper mapper);
		void InvokeCustomizers(System.Type type, IJoinAttributesMapper mapper);

		void InvokeCustomizers(PropertyPath member, IPropertyMapper mapper);
		void InvokeCustomizers(PropertyPath member, IManyToOneMapper mapper);
		void InvokeCustomizers(PropertyPath member, IOneToOneMapper mapper);
		void InvokeCustomizers(PropertyPath member, IAnyMapper mapper);

		void InvokeCustomizers(PropertyPath member, ISetPropertiesMapper mapper);
		void InvokeCustomizers(PropertyPath member, IBagPropertiesMapper mapper);
		void InvokeCustomizers(PropertyPath member, IListPropertiesMapper mapper);
		void InvokeCustomizers(PropertyPath member, IMapPropertiesMapper mapper);
		void InvokeCustomizers(PropertyPath member, IIdBagPropertiesMapper mapper);
		void InvokeCustomizers(PropertyPath member, IComponentAttributesMapper mapper);
		void InvokeCustomizers(PropertyPath member, IComponentAsIdAttributesMapper mapper);
		void InvokeCustomizers(PropertyPath member, IDynamicComponentAttributesMapper mapper);

		#region Dictionary key relations

		void AddCustomizer(PropertyPath member, Action<IMapKeyManyToManyMapper> mapKeyManyToManyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IMapKeyMapper> mapKeyElementCustomizer);
		void InvokeCustomizers(PropertyPath member, IMapKeyManyToManyMapper mapper);
		void InvokeCustomizers(PropertyPath member, IMapKeyMapper mapper);

		#endregion

		#region Collection Element relations

		void AddCustomizer(PropertyPath member, Action<IManyToManyMapper> collectionRelationManyToManyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IElementMapper> collectionRelationElementCustomizer);
		void AddCustomizer(PropertyPath member, Action<IOneToManyMapper> collectionRelationOneToManyCustomizer);
		void AddCustomizer(PropertyPath member, Action<IManyToAnyMapper> collectionRelationManyToAnyCustomizer);
		void InvokeCustomizers(PropertyPath member, IManyToManyMapper mapper);
		void InvokeCustomizers(PropertyPath member, IElementMapper mapper);
		void InvokeCustomizers(PropertyPath member, IOneToManyMapper mapper);
		void InvokeCustomizers(PropertyPath member, IManyToAnyMapper mapper);

		#endregion

		IEnumerable<System.Type> GetAllCustomizedEntities();
	}
}