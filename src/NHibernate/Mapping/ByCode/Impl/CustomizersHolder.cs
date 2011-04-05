using System;
using System.Collections.Generic;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class CustomizersHolder : ICustomizersHolder
	{
		private readonly Dictionary<PropertyPath, List<Action<IAnyMapper>>> anyCustomizers =
			new Dictionary<PropertyPath, List<Action<IAnyMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IBagPropertiesMapper>>> bagCustomizers =
			new Dictionary<PropertyPath, List<Action<IBagPropertiesMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<ICollectionPropertiesMapper>>> collectionCustomizers =
			new Dictionary<PropertyPath, List<Action<ICollectionPropertiesMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IElementMapper>>> collectionRelationElementCustomizers =
			new Dictionary<PropertyPath, List<Action<IElementMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IManyToManyMapper>>> collectionRelationManyToManyCustomizers =
			new Dictionary<PropertyPath, List<Action<IManyToManyMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IOneToManyMapper>>> collectionRelationOneToManyCustomizers =
			new Dictionary<PropertyPath, List<Action<IOneToManyMapper>>>();

		private readonly Dictionary<System.Type, List<Action<IComponentAttributesMapper>>> componentClassCustomizers =
			new Dictionary<System.Type, List<Action<IComponentAttributesMapper>>>();

		private readonly Dictionary<PropertyPath, List<Action<IComponentAttributesMapper>>> componentPropertyCustomizers =
			new Dictionary<PropertyPath, List<Action<IComponentAttributesMapper>>>();

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

		private readonly Dictionary<System.Type, List<Action<ISubclassAttributesMapper>>> subclassCustomizers =
			new Dictionary<System.Type, List<Action<ISubclassAttributesMapper>>>();

		private readonly Dictionary<System.Type, List<Action<IUnionSubclassAttributesMapper>>> unionClassCustomizers =
			new Dictionary<System.Type, List<Action<IUnionSubclassAttributesMapper>>>();

		private readonly Dictionary<System.Type, List<Action<IJoinAttributesMapper>>> joinCustomizers = 
			new Dictionary<System.Type, List<Action<IJoinAttributesMapper>>>();

		#region ICustomizersHolder Members

		public void AddCustomizer(System.Type type, Action<IClassMapper> classCustomizer)
		{
			AddCustomizer(rootClassCustomizers, type, classCustomizer);
		}

		public void AddCustomizer(System.Type type, Action<ISubclassAttributesMapper> classCustomizer)
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

		public void AddCustomizer(PropertyPath member, Action<ICollectionPropertiesMapper> propertyCustomizer)
		{
			AddCustomizer(collectionCustomizers, member, propertyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IComponentAttributesMapper> propertyCustomizer)
		{
			AddCustomizer(componentPropertyCustomizers, member, propertyCustomizer);
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

		public void AddCustomizer(PropertyPath member, Action<IMapKeyManyToManyMapper> mapKeyManyToManyCustomizer)
		{
			AddCustomizer(mapKeyManyToManyCustomizers, member, mapKeyManyToManyCustomizer);
		}

		public void AddCustomizer(PropertyPath member, Action<IMapKeyMapper> mapKeyElementCustomizer)
		{
			AddCustomizer(mapKeyElementCustomizers, member, mapKeyElementCustomizer);
		}

		public void InvokeCustomizers(System.Type type, IClassMapper mapper)
		{
			InvokeCustomizers(rootClassCustomizers, type, mapper);
		}

		public void InvokeCustomizers(System.Type type, ISubclassAttributesMapper mapper)
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

		public void InvokeCustomizers(PropertyPath member, IComponentAttributesMapper mapper)
		{
			InvokeCustomizers(componentPropertyCustomizers, member, mapper);
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

		#endregion

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