using System;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class CustomizersHolder : ICustomizersHolder
	{
		private readonly Dictionary<PropertyPath, List<Action<IAnyMapper>>> anyCustomizers =
			new Dictionary<PropertyPath, List<Action<IAnyMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IBagPropertiesMapper>>> bagCustomizers =
			new Dictionary<PropertyPath, List<Action<IBagPropertiesMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IIdBagPropertiesMapper>>> idBagCustomizers =
			new Dictionary<PropertyPath, List<Action<IIdBagPropertiesMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<ICollectionPropertiesMapper>>> collectionCustomizers =
			new Dictionary<PropertyPath, List<Action<ICollectionPropertiesMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IElementMapper>>> collectionRelationElementCustomizers =
			new Dictionary<PropertyPath, List<Action<IElementMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IManyToManyMapper>>> collectionRelationManyToManyCustomizers =
			new Dictionary<PropertyPath, List<Action<IManyToManyMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IManyToAnyMapper>>> collectionRelationManyToAnyCustomizers =
			new Dictionary<PropertyPath, List<Action<IManyToAnyMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IOneToManyMapper>>> collectionRelationOneToManyCustomizers =
			new Dictionary<PropertyPath, List<Action<IOneToManyMapper>>>();

		private readonly Dictionary<System.Type, List<Action<IComponentAttributesMapper>>> componentClassCustomizers =
			new Dictionary<System.Type, List<Action<IComponentAttributesMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IComponentAttributesMapper>>> componentPropertyCustomizers =
			new Dictionary<PropertyPath, List<Action<IComponentAttributesMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IComponentAsIdAttributesMapper>>> componentAsIdPropertyCustomizers =
			new Dictionary<PropertyPath, List<Action<IComponentAsIdAttributesMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IDynamicComponentAttributesMapper>>> dynamicComponentCustomizers =
			new Dictionary<PropertyPath, List<Action<IDynamicComponentAttributesMapper>>>();

		private readonly Dictionary<System.Type, List<Action<IJoinedSubclassAttributesMapper>>> joinedClassCustomizers =
			new Dictionary<System.Type, List<Action<IJoinedSubclassAttributesMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IListPropertiesMapper>>> listCustomizers =
			new Dictionary<PropertyPath, List<Action<IListPropertiesMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IManyToOneMapper>>> manyToOneCustomizers =
			new Dictionary<PropertyPath, List<Action<IManyToOneMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IMapPropertiesMapper>>> mapCustomizers =
			new Dictionary<PropertyPath, List<Action<IMapPropertiesMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IMapKeyMapper>>> mapKeyElementCustomizers =
			new Dictionary<PropertyPath, List<Action<IMapKeyMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IMapKeyManyToManyMapper>>> mapKeyManyToManyCustomizers =
			new Dictionary<PropertyPath, List<Action<IMapKeyManyToManyMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IOneToOneMapper>>> oneToOneCustomizers =
			new Dictionary<PropertyPath, List<Action<IOneToOneMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IPropertyMapper>>> propertyCustomizers =
			new Dictionary<PropertyPath, List<Action<IPropertyMapper>>>();

		private readonly Dictionary<System.Type, List<Action<IClassMapper>>> rootClassCustomizers =
			new Dictionary<System.Type, List<Action<IClassMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<ISetPropertiesMapper>>> setCustomizers =
			new Dictionary<PropertyPath, List<Action<ISetPropertiesMapper>>>();

		private readonly Dictionary<System.Type, List<Action<ISubclassMapper>>> subclassCustomizers =
			new Dictionary<System.Type, List<Action<ISubclassMapper>>>();

		private readonly Dictionary<System.Type, List<Action<IUnionSubclassAttributesMapper>>> unionClassCustomizers =
			new Dictionary<System.Type, List<Action<IUnionSubclassAttributesMapper>>>();

		private readonly Dictionary<System.Type, List<Action<IJoinAttributesMapper>>> joinCustomizers = 
			new Dictionary<System.Type, List<Action<IJoinAttributesMapper>>>();

		#region ICustomizersHolder Members

		public void AddCustomizer(System.Type type, Action<IClassMapper> classCustomizer)
		{
			AddCustomizer(rootClassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(System.Type type, Action<ISubclassMapper> classCustomizer)
		{
			AddCustomizer(subclassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(System.Type type, Action<IJoinedSubclassAttributesMapper> classCustomizer)
		{
			AddCustomizer(joinedClassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(System.Type type, Action<IUnionSubclassAttributesMapper> classCustomizer)
		{
			AddCustomizer(unionClassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(System.Type type, Action<IComponentAttributesMapper> classCustomizer)
		{
			AddCustomizer(componentClassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(System.Type type, Action<IJoinAttributesMapper> joinCustomizer)
		{
			AddCustomizer(joinCustomizers, type, joinCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IPropertyMapper> propertyCustomizer)
		{
			AddCustomizer(propertyCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IManyToOneMapper> propertyCustomizer)
		{
			AddCustomizer(manyToOneCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IOneToOneMapper> propertyCustomizer)
		{
			AddCustomizer(oneToOneCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IAnyMapper> propertyCustomizer)
		{
			AddCustomizer(anyCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<ISetPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(setCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IBagPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(bagCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IListPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(listCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IMapPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(mapCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IIdBagPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(idBagCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<ICollectionPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(collectionCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IComponentAttributesMapper> propertyCustomizer)
		{
			AddCustomizer(componentPropertyCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IComponentAsIdAttributesMapper> propertyCustomizer)
		{
			AddCustomizer(componentAsIdPropertyCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IDynamicComponentAttributesMapper> propertyCustomizer)
		{
			AddCustomizer(dynamicComponentCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IManyToManyMapper> collectionRelationManyToManyCustomizer)
		{
			AddCustomizer(collectionRelationManyToManyCustomizers, member, collectionRelationManyToManyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IElementMapper> collectionRelationElementCustomizer)
		{
			AddCustomizer(collectionRelationElementCustomizers, member, collectionRelationElementCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IOneToManyMapper> collectionRelationOneToManyCustomizer)
		{
			AddCustomizer(collectionRelationOneToManyCustomizers, member, collectionRelationOneToManyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IManyToAnyMapper> collectionRelationManyToAnyCustomizer)
		{
			AddCustomizer(collectionRelationManyToAnyCustomizers, member, collectionRelationManyToAnyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IMapKeyManyToManyMapper> mapKeyManyToManyCustomizer)
		{
			AddCustomizer(mapKeyManyToManyCustomizers, member, mapKeyManyToManyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IMapKeyMapper> mapKeyElementCustomizer)
		{
			AddCustomizer(mapKeyElementCustomizers, member, mapKeyElementCustomizer);
		}

		public void InvokeCustomizers(PropertyPath member, IManyToAnyMapper mapper)
		{
			InvokeCustomizers(collectionRelationManyToAnyCustomizers, member, mapper);
		}

		public IEnumerable<System.Type> GetAllCustomizedEntities()
		{
			return rootClassCustomizers.Keys.Concat(subclassCustomizers.Keys).Concat(joinedClassCustomizers.Keys).Concat(unionClassCustomizers.Keys);
		}

		public void InvokeCustomizers(System.Type type, IClassMapper mapper)
		{
			InvokeCustomizers(rootClassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(System.Type type, ISubclassMapper mapper)
		{
			InvokeCustomizers(subclassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(System.Type type, IJoinedSubclassAttributesMapper mapper)
		{
			InvokeCustomizers(joinedClassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(System.Type type, IUnionSubclassAttributesMapper mapper)
		{
			InvokeCustomizers(unionClassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(System.Type type, IComponentAttributesMapper mapper)
		{
			InvokeCustomizers(componentClassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(System.Type type, IJoinAttributesMapper mapper)
		{
			InvokeCustomizers(joinCustomizers, type, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IPropertyMapper mapper)
		{
			InvokeCustomizers(propertyCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IManyToOneMapper mapper)
		{
			InvokeCustomizers(manyToOneCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IOneToOneMapper mapper)
		{
			InvokeCustomizers(oneToOneCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IAnyMapper mapper)
		{
			InvokeCustomizers(anyCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, ISetPropertiesMapper mapper)
		{
			InvokeCustomizers(collectionCustomizers, member, mapper);
			InvokeCustomizers(setCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IBagPropertiesMapper mapper)
		{
			InvokeCustomizers(collectionCustomizers, member, mapper);
			InvokeCustomizers(bagCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IListPropertiesMapper mapper)
		{
			InvokeCustomizers(collectionCustomizers, member, mapper);
			InvokeCustomizers(listCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IMapPropertiesMapper mapper)
		{
			InvokeCustomizers(collectionCustomizers, member, mapper);
			InvokeCustomizers(mapCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IIdBagPropertiesMapper mapper)
		{
			InvokeCustomizers(collectionCustomizers, member, mapper);
			InvokeCustomizers(idBagCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IComponentAttributesMapper mapper)
		{
			InvokeCustomizers(componentPropertyCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IComponentAsIdAttributesMapper mapper)
		{
			InvokeCustomizers(componentAsIdPropertyCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IDynamicComponentAttributesMapper mapper)
		{
			InvokeCustomizers(dynamicComponentCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IManyToManyMapper mapper)
		{
			InvokeCustomizers(collectionRelationManyToManyCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IElementMapper mapper)
		{
			InvokeCustomizers(collectionRelationElementCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IOneToManyMapper mapper)
		{
			InvokeCustomizers(collectionRelationOneToManyCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IMapKeyManyToManyMapper mapper)
		{
			InvokeCustomizers(mapKeyManyToManyCustomizers, member, mapper);
		}

		public void InvokeCustomizers(PropertyPath member, IMapKeyMapper mapper)
		{
			InvokeCustomizers(mapKeyElementCustomizers, member, mapper);
		}

		public void Merge(CustomizersHolder source)
		{
			if (source == null)
			{
				return;
			}
			MergeDictionary(rootClassCustomizers, source.rootClassCustomizers);
			MergeDictionary(subclassCustomizers, source.subclassCustomizers);
			MergeDictionary(joinedClassCustomizers, source.joinedClassCustomizers);
			MergeDictionary(unionClassCustomizers, source.unionClassCustomizers);
			MergeDictionary(componentClassCustomizers, source.componentClassCustomizers);
			MergeDictionary(joinCustomizers, source.joinCustomizers);
			MergeDictionary(propertyCustomizers, source.propertyCustomizers);
			MergeDictionary(manyToOneCustomizers, source.manyToOneCustomizers);
			MergeDictionary(oneToOneCustomizers, source.oneToOneCustomizers);
			MergeDictionary(anyCustomizers, source.anyCustomizers);
			MergeDictionary(setCustomizers, source.setCustomizers);
			MergeDictionary(bagCustomizers, source.bagCustomizers);
			MergeDictionary(listCustomizers, source.listCustomizers);
			MergeDictionary(mapCustomizers, source.mapCustomizers);
			MergeDictionary(idBagCustomizers, source.idBagCustomizers);
			MergeDictionary(collectionCustomizers, source.collectionCustomizers);
			MergeDictionary(componentPropertyCustomizers, source.componentPropertyCustomizers);
			MergeDictionary(collectionRelationManyToManyCustomizers, source.collectionRelationManyToManyCustomizers);
			MergeDictionary(collectionRelationElementCustomizers, source.collectionRelationElementCustomizers);
			MergeDictionary(collectionRelationOneToManyCustomizers, source.collectionRelationOneToManyCustomizers);
			MergeDictionary(collectionRelationManyToAnyCustomizers, source.collectionRelationManyToAnyCustomizers);
			MergeDictionary(mapKeyManyToManyCustomizers, source.mapKeyManyToManyCustomizers);
			MergeDictionary(mapKeyElementCustomizers, source.mapKeyElementCustomizers);
			MergeDictionary(dynamicComponentCustomizers, source.dynamicComponentCustomizers);
			MergeDictionary(componentAsIdPropertyCustomizers, source.componentAsIdPropertyCustomizers);
		}

		#endregion

		private void MergeDictionary<TSubject, TCustomizable>(Dictionary<TSubject, List<Action<TCustomizable>>> destination,Dictionary<TSubject, List<Action<TCustomizable>>> source)
		{
			foreach (var element in source)
			{
				List<Action<TCustomizable>> actions;
				if (!destination.TryGetValue(element.Key, out actions))
				{
					actions = new List<Action<TCustomizable>>();
					destination[element.Key] = actions;
				}
				actions.AddRange(element.Value);
			}
		}

		private void AddCustomizer<TSubject, TCustomizable>(IDictionary<TSubject, List<Action<TCustomizable>>> customizers,
															TSubject member, Action<TCustomizable> customizer)
		{
			List<Action<TCustomizable>> actions;
			if (!customizers.TryGetValue(member, out actions))
			{
				actions = new List<Action<TCustomizable>>();
				customizers[member] = actions;
			}
			actions.Add(customizer);
		}

		private void InvokeCustomizers<TSubject, TCustomizable>(
			IDictionary<TSubject, List<Action<TCustomizable>>> customizers, TSubject member, TCustomizable customizable)
		{
			List<Action<TCustomizable>> actions;
			if (customizers.TryGetValue(member, out actions))
			{
				foreach (var action in actions)
				{
					action(customizable);
				}
			}
		}
	}
}