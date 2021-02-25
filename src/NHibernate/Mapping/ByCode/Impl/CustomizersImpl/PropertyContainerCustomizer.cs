using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class PropertyContainerCustomizer<TEntity>
	{
		private readonly IModelExplicitDeclarationsHolder explicitDeclarationsHolder;

		public PropertyContainerCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder, PropertyPath propertyPath)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			this.explicitDeclarationsHolder = explicitDeclarationsHolder;
			CustomizersHolder = customizersHolder;
			PropertyPath = propertyPath;
		}

		protected internal ICustomizersHolder CustomizersHolder { get; private set; }
		protected internal PropertyPath PropertyPath { get; private set; }

		protected internal IModelExplicitDeclarationsHolder ExplicitDeclarationsHolder
		{
			get { return explicitDeclarationsHolder; }
		}

		public void Property<TProperty>(Expression<Func<TEntity, TProperty>> property)
		{
			Property(property, x => { });
		}

		public void Property<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			RegisterPropertyMapping(property, mapping);
		}

		protected virtual void RegisterPropertyMapping<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegistePropertyMapping(mapping, memberOf);
		}

		public void Property(string notVisiblePropertyOrFieldName, Action<IPropertyMapper> mapping)
		{
			RegisterNoVisiblePropertyMapping(notVisiblePropertyOrFieldName, mapping);
		}

		protected virtual void RegisterNoVisiblePropertyMapping(string notVisiblePropertyOrFieldName, Action<IPropertyMapper> mapping)
		{
			// even seems repetitive, before unify this registration with the registration using Expression take in account that reflection operations
			// done unsing expressions are faster than those done with pure reflection.
			MemberInfo member = GetRequiredPropertyOrFieldByName(notVisiblePropertyOrFieldName);
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegistePropertyMapping(mapping, memberOf);
		}

		protected void RegistePropertyMapping(Action<IPropertyMapper> mapping, params MemberInfo[] members)
		{
			foreach (var member in members)
			{
				CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
				explicitDeclarationsHolder.AddAsProperty(member);
			}
		}

		public void Component<TComponent>(Expression<Func<TEntity, TComponent>> property, Action<IComponentMapper<TComponent>> mapping)
		{
			RegisterComponentMapping(property, mapping);
		}

		public void Component<TComponent>(Expression<Func<TEntity, TComponent>> property)
		{
			RegisterComponentMapping(property, x => { });
		}

		protected virtual void RegisterComponentMapping<TComponent>(Expression<Func<TEntity, TComponent>> property, Action<IComponentMapper<TComponent>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterComponentMapping(mapping, member, memberOf);
		}

		protected void RegisterComponentMapping<TComponent>(Action<IComponentMapper<TComponent>> mapping, params MemberInfo[] members)
		{
			foreach (var member in members)
			{
				mapping(new ComponentCustomizer<TComponent>(explicitDeclarationsHolder, CustomizersHolder, new PropertyPath(PropertyPath, member)));
			}
		}

		/// <summary>
		/// Maps a non-generic dictionary property as a dynamic component.
		/// </summary>
		/// <param name="property">The property to map.</param>
		/// <param name="dynamicComponentTemplate">The template for the component. It should either be a (usually
		/// anonymous) type having the same properties than the component, or an
		/// <c>IDictionary&lt;string, System.Type&gt;</c> of property names with their type.</param>
		/// <param name="mapping">The mapping of the component.</param>
		/// <typeparam name="TComponent">The type of the template.</typeparam>
		public void Component<TComponent>(Expression<Func<TEntity, IDictionary>> property, TComponent dynamicComponentTemplate, Action<IDynamicComponentMapper<TComponent>> mapping)
		{
			if (dynamicComponentTemplate is IEnumerable<KeyValuePair<string, System.Type>> template)
			{
				var componentType = CreateDynamicComponentTypeFromTemplate(template);
				RegisterDynamicComponentMapping(property, componentType, mapping);
			}
			else
			{
				RegisterDynamicComponentMapping(property, mapping);
			}
		}

		/// <summary>
		/// Maps a generic <c>IDictionary&lt;string, object&gt;</c> property as a dynamic component.
		/// </summary>
		/// <param name="property">The property to map.</param>
		/// <param name="dynamicComponentTemplate">The template for the component. It should either be a (usually
		/// anonymous) type having the same properties than the component, or an
		/// <c>IDictionary&lt;string, System.Type&gt;</c> of property names with their type.</param>
		/// <param name="mapping">The mapping of the component.</param>
		/// <typeparam name="TComponent">The type of the template.</typeparam>
		public void Component<TComponent>(Expression<Func<TEntity, IDictionary<string, object>>> property, TComponent dynamicComponentTemplate, Action<IDynamicComponentMapper<TComponent>> mapping) where TComponent : class
		{
			if (dynamicComponentTemplate is IEnumerable<KeyValuePair<string, System.Type>> template)
			{
				var componentType = CreateDynamicComponentTypeFromTemplate(template);
				RegisterDynamicComponentMapping(property, componentType, mapping);
			}
			else
			{
				RegisterDynamicComponentMapping(property, mapping);
			}
		}

		protected virtual void RegisterDynamicComponentMapping<TComponent>(Expression<Func<TEntity, IDictionary>> property, Action<IDynamicComponentMapper<TComponent>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterDynamicComponentMapping(mapping, member, memberOf);
		}
		
		protected virtual void RegisterDynamicComponentMapping<TComponent>(Expression<Func<TEntity, IDictionary>> property, System.Type componentType, Action<IDynamicComponentMapper<TComponent>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterDynamicComponentMapping(componentType, mapping, member, memberOf);
		}

		protected virtual void RegisterDynamicComponentMapping<TComponent>(Expression<Func<TEntity, IDictionary<string, object>>> property, Action<IDynamicComponentMapper<TComponent>> mapping) where TComponent : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterDynamicComponentMapping(mapping, member, memberOf);
		}

		protected virtual void RegisterDynamicComponentMapping<TComponent>(Expression<Func<TEntity, IDictionary<string, object>>> property, System.Type componentType, Action<IDynamicComponentMapper<TComponent>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterDynamicComponentMapping(componentType, mapping, member, memberOf);
		}

		protected void RegisterDynamicComponentMapping<TComponent>(System.Type componentType, Action<IDynamicComponentMapper<TComponent>> mapping, params MemberInfo[] members)
		{
			foreach (var member in members)
			{
				mapping(new DynamicComponentCustomizer<TComponent>(componentType, explicitDeclarationsHolder, CustomizersHolder, new PropertyPath(PropertyPath, member)));
			}
		}

		protected void RegisterDynamicComponentMapping<TComponent>(Action<IDynamicComponentMapper<TComponent>> mapping, params MemberInfo[] members)
		{
			foreach (var member in members)
			{
				mapping(new DynamicComponentCustomizer<TComponent>(explicitDeclarationsHolder, CustomizersHolder, new PropertyPath(PropertyPath, member)));
			}
		}

		private static System.Type CreateDynamicComponentTypeFromTemplate(IEnumerable<KeyValuePair<string, System.Type>> template)
		{
			var assemblyName = new AssemblyName("MyDynamicAssembly");
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
			var typeBuilder = moduleBuilder.DefineType(
				"MyDynamicType",
				TypeAttributes.Public | TypeAttributes.Serializable);

			foreach (var property in template)
			{
				var propertyBuilder = typeBuilder.DefineProperty(
					property.Key,
					PropertyAttributes.HasDefault,
					property.Value,
					null);
				var getMethodBuilder = typeBuilder.DefineMethod(
					"get_" + property.Key,
					MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
					property.Value,
					System.Type.EmptyTypes);
				var setMethodBuilder = typeBuilder.DefineMethod(
					"set_" + property.Key,
					MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
					typeof(void),
					new[] { property.Value });

				var getILGenerator = getMethodBuilder.GetILGenerator();
				getILGenerator.Emit(OpCodes.Ldarg_0);
				getILGenerator.Emit(OpCodes.Nop);
				getILGenerator.Emit(OpCodes.Ret);

				var setILGenerator = setMethodBuilder.GetILGenerator();
				setILGenerator.Emit(OpCodes.Ldarg_0);
				setILGenerator.Emit(OpCodes.Ldarg_1);
				setILGenerator.Emit(OpCodes.Ret);

				propertyBuilder.SetGetMethod(getMethodBuilder);
				propertyBuilder.SetSetMethod(setMethodBuilder);
			}

			return typeBuilder.CreateTypeInfo();
		}

		public void ManyToOne<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IManyToOneMapper> mapping)
			where TProperty : class
		{
			RegisterManyToOneMapping(property, mapping);
		}

		protected virtual void RegisterManyToOneMapping<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IManyToOneMapper> mapping)
			where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterManyToOneMapping<TProperty>(mapping, member, memberOf);
		}

		protected void RegisterManyToOneMapping<TProperty>(Action<IManyToOneMapper> mapping, params MemberInfo[] members)
			where TProperty : class
		{
			foreach (var member in members)
			{
				CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);
				explicitDeclarationsHolder.AddAsManyToOneRelation(member);
			}
		}

		public void ManyToOne<TProperty>(Expression<Func<TEntity, TProperty>> property) where TProperty : class
		{
			ManyToOne(property, x => { });
		}

		public void OneToOne<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IOneToOneMapper<TProperty>> mapping)
			where TProperty : class
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(property);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterOneToOneMapping(mapping, member, memberOf);
		}

		public void OneToOne<TProperty>(string notVisiblePropertyOrFieldName, Action<IOneToOneMapper<TProperty>> mapping) where TProperty : class
		{
			var member = GetRequiredPropertyOrFieldByName(notVisiblePropertyOrFieldName);
			var propertyOrFieldType = member.GetPropertyOrFieldType();
			if (typeof(TProperty) != propertyOrFieldType)
			{
				throw new MappingException(string.Format("Wrong relation type. For the property/field '{0}' of {1} was expected a one-to-one with {2} but was {3}",
														 notVisiblePropertyOrFieldName, typeof(TEntity).FullName, typeof(TProperty).Name, propertyOrFieldType.Name));
			}
			var memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterOneToOneMapping(mapping, member, memberOf);
		}

		protected void RegisterOneToOneMapping<TProperty>(Action<IOneToOneMapper<TProperty>> mapping, params MemberInfo[] members)
			where TProperty : class
		{
			foreach (var member in members)
			{
				CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), (IOneToOneMapper x) => mapping((IOneToOneMapper<TProperty>) x));
				explicitDeclarationsHolder.AddAsOneToOneRelation(member);
			}
		}

		public void Any<TProperty>(Expression<Func<TEntity, TProperty>> property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping)
			where TProperty : class
		{
			RegisterAnyMapping(property, idTypeOfMetaType, mapping);
		}

		protected virtual void RegisterAnyMapping<TProperty>(Expression<Func<TEntity, TProperty>> property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping)
			where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterAnyMapping<TProperty>(mapping, idTypeOfMetaType, member, memberOf);
		}

		protected void RegisterAnyMapping<TProperty>(Action<IAnyMapper> mapping, System.Type idTypeOfMetaType, params MemberInfo[] members)
			where TProperty : class
		{
			foreach (var member in members)
			{
				CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), (IAnyMapper am) => am.IdType(idTypeOfMetaType));
				CustomizersHolder.AddCustomizer(new PropertyPath(PropertyPath, member), mapping);

				explicitDeclarationsHolder.AddAsAny(member);
			}
		}

		public void Set<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
								  Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping,
								  Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterSetMapping(property, collectionMapping, mapping);
		}

		public void Set<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
															Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			Set(property, collectionMapping, x => { });
		}

		protected virtual void RegisterSetMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterSetMapping(collectionMapping, mapping, memberOf);
		}

		protected void RegisterSetMapping<TElement>(Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping, params MemberInfo[] members)
		{
			foreach (var member in members)
			{
				collectionMapping(new SetPropertiesCustomizer<TEntity, TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, member), CustomizersHolder));
				mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, member), CustomizersHolder));
			}
		}

		public void Bag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
								  Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping,
								  Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterBagMapping(property, collectionMapping, mapping);
		}
		public void Bag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
															Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			Bag(property, collectionMapping, x => { });
		}

		protected virtual void RegisterBagMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterBagMapping(collectionMapping, mapping, memberOf);
		}

		protected void RegisterBagMapping<TElement>(Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping, params MemberInfo[] members)
		{
			foreach (var member in members)
			{
				collectionMapping(new BagPropertiesCustomizer<TEntity, TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, member), CustomizersHolder));
				mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, member), CustomizersHolder));
			}
		}

		public void List<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
								   Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping,
								   Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterListMapping(property, collectionMapping, mapping);
		}
		public void List<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
															 Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			List(property, collectionMapping, x => { });
		}

		protected virtual void RegisterListMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterListMapping(collectionMapping, mapping, memberOf);
		}

		protected void RegisterListMapping<TElement>(Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping, params MemberInfo[] members)
		{
			foreach (var member in members)
			{
				collectionMapping(new ListPropertiesCustomizer<TEntity, TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, member), CustomizersHolder));
				mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, member), CustomizersHolder));
			}
		}

		public void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
										Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping,
										Action<IMapKeyRelation<TKey>> keyMapping,
										Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterMapMapping(property, collectionMapping, keyMapping, mapping);
		}
		public void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
																		Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping)
		{
			Map(property, collectionMapping, keyMapping => { }, x => { });
		}

		protected virtual void RegisterMapMapping<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property, Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping, Action<IMapKeyRelation<TKey>> keyMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterMapMapping<TKey, TElement>(collectionMapping, keyMapping, mapping, member, memberOf);
		}

		protected virtual void RegisterMapMapping<TKey, TElement>(Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping, Action<IMapKeyRelation<TKey>> keyMapping, Action<ICollectionElementRelation<TElement>> mapping, params MemberInfo[] members)
		{
			foreach (var member in members)
			{
				var memberPath = new PropertyPath(PropertyPath, member);
				collectionMapping(new MapPropertiesCustomizer<TEntity, TKey, TElement>(explicitDeclarationsHolder, memberPath, CustomizersHolder));
				keyMapping(new MapKeyRelationCustomizer<TKey>(explicitDeclarationsHolder, memberPath, CustomizersHolder));
				mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, memberPath, CustomizersHolder));
			}
		}

		public void Map<TKey, TElement>(Expression<Func<TEntity, IDictionary<TKey, TElement>>> property,
																		Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping,
																		Action<ICollectionElementRelation<TElement>> mapping)
		{
			Map(property, collectionMapping, keyMapping => { }, mapping);
		}

		public void IdBag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
													Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping,
													Action<ICollectionElementRelation<TElement>> mapping)
		{
			RegisterIdBagMapping(property, collectionMapping, mapping);
		}

		public void IdBag<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property,
													Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			RegisterIdBagMapping(property, collectionMapping, x => { });
		}

		protected virtual void RegisterIdBagMapping<TElement>(Expression<Func<TEntity, IEnumerable<TElement>>> property, Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			RegisterIdBagMapping(collectionMapping, mapping, memberOf);
		}

		protected virtual void RegisterIdBagMapping<TElement>(Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping, params MemberInfo[] members)
		{
			foreach (var member in members)
			{
				collectionMapping(new IdBagPropertiesCustomizer<TEntity, TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, member), CustomizersHolder));
				mapping(new CollectionElementRelationCustomizer<TElement>(explicitDeclarationsHolder, new PropertyPath(PropertyPath, member), CustomizersHolder));
			}
		}

		public void Set<TElement>(string notVisiblePropertyOrFieldName, Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = GetRequiredPropertyOrFieldByName(notVisiblePropertyOrFieldName);
			AssertCollectionElementType<TElement>(notVisiblePropertyOrFieldName, member);

			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterSetMapping<TElement>(collectionMapping, mapping, member, memberOf);
		}

		private static void AssertCollectionElementType<TElement>(string propertyOrFieldName, MemberInfo memberInfo)
		{
			System.Type collectionElementType = memberInfo.GetPropertyOrFieldType().DetermineCollectionElementType();

			if (typeof(TElement) != collectionElementType)
			{
				var message = string.Format(
					"Wrong collection element type. For the property/field '{0}' of {1} was expected a generic collection of {2} but was {3}",
					propertyOrFieldName, typeof(TEntity).FullName, typeof(TElement).Name,
					collectionElementType != null ? collectionElementType.Name : "unknown");
				throw new MappingException(message);
			}
		}

		public void Set<TElement>(string notVisiblePropertyOrFieldName, Action<ISetPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			Set(notVisiblePropertyOrFieldName, collectionMapping, x => { });
		}

		public void Bag<TElement>(string notVisiblePropertyOrFieldName, Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = GetRequiredPropertyOrFieldByName(notVisiblePropertyOrFieldName);
			AssertCollectionElementType<TElement>(notVisiblePropertyOrFieldName, member);

			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterBagMapping<TElement>(collectionMapping, mapping, member, memberOf);
		}

		public void Bag<TElement>(string notVisiblePropertyOrFieldName, Action<IBagPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			Bag(notVisiblePropertyOrFieldName, collectionMapping, x => { });
		}

		public void List<TElement>(string notVisiblePropertyOrFieldName, Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = GetRequiredPropertyOrFieldByName(notVisiblePropertyOrFieldName);
			AssertCollectionElementType<TElement>(notVisiblePropertyOrFieldName, member);

			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterListMapping<TElement>(collectionMapping, mapping, member, memberOf);
		}

		public void List<TElement>(string notVisiblePropertyOrFieldName, Action<IListPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			List(notVisiblePropertyOrFieldName, collectionMapping, x => { });
		}

		public void Map<TKey, TElement>(string notVisiblePropertyOrFieldName, Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping, Action<IMapKeyRelation<TKey>> keyMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = GetRequiredPropertyOrFieldByName(notVisiblePropertyOrFieldName);
			var propertyOrFieldType = member.GetPropertyOrFieldType();
			var keyType = propertyOrFieldType.DetermineDictionaryKeyType();
			var collectionElementType = propertyOrFieldType.DetermineDictionaryValueType();
			if (!typeof(TElement).Equals(collectionElementType) || !typeof(TKey).Equals(keyType))
			{
				throw new MappingException(string.Format("Wrong collection element type. For the property/field '{0}' of {1} was expected a dictionary of {2}/{3} but was {4}/{5}",
																								 notVisiblePropertyOrFieldName, typeof(TEntity).FullName, typeof(TKey).Name, keyType.Name, typeof(TElement).Name, collectionElementType.Name));
			}
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterMapMapping<TKey, TElement>(collectionMapping, keyMapping, mapping, member, memberOf);
		}

		public void Map<TKey, TElement>(string notVisiblePropertyOrFieldName, Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			Map(notVisiblePropertyOrFieldName, collectionMapping, x => { }, mapping);
		}

		public void Map<TKey, TElement>(string notVisiblePropertyOrFieldName, Action<IMapPropertiesMapper<TEntity, TKey, TElement>> collectionMapping)
		{
			Map(notVisiblePropertyOrFieldName, collectionMapping, x => { }, y => { });
		}

		public void IdBag<TElement>(string notVisiblePropertyOrFieldName, Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping, Action<ICollectionElementRelation<TElement>> mapping)
		{
			MemberInfo member = GetRequiredPropertyOrFieldByName(notVisiblePropertyOrFieldName);
			AssertCollectionElementType<TElement>(notVisiblePropertyOrFieldName, member);

			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterIdBagMapping<TElement>(collectionMapping, mapping, member, memberOf);
		}

		public void IdBag<TElement>(string notVisiblePropertyOrFieldName, Action<IIdBagPropertiesMapper<TEntity, TElement>> collectionMapping)
		{
			IdBag(notVisiblePropertyOrFieldName, collectionMapping, x => { });
		}

		public void ManyToOne<TProperty>(string notVisiblePropertyOrFieldName, Action<IManyToOneMapper> mapping) where TProperty : class
		{
			MemberInfo member = GetRequiredPropertyOrFieldByName(notVisiblePropertyOrFieldName);
			var propertyOrFieldType = member.GetPropertyOrFieldType();
			if (!typeof(TProperty).Equals(propertyOrFieldType))
			{
				throw new MappingException(string.Format("Wrong relation type. For the property/field '{0}' of {1} was expected a many-to-one with {2} but was {3}",
																								 notVisiblePropertyOrFieldName, typeof(TEntity).FullName, typeof(TProperty).Name, propertyOrFieldType.Name));
			}
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterManyToOneMapping<TProperty>(mapping, member, memberOf);
		}

		public void Component<TComponent>(string notVisiblePropertyOrFieldName, Action<IComponentMapper<TComponent>> mapping)
		{
			MemberInfo member = GetRequiredPropertyOrFieldByName(notVisiblePropertyOrFieldName);
			var propertyOrFieldType = member.GetPropertyOrFieldType();
			if (!typeof(TComponent).Equals(propertyOrFieldType))
			{
				throw new MappingException(string.Format("Wrong relation type. For the property/field '{0}' of {1} was expected a component of {2} but was {3}",
																								 notVisiblePropertyOrFieldName, typeof(TEntity).FullName, typeof(TComponent).Name, propertyOrFieldType.Name));
			}
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterComponentMapping<TComponent>(mapping, member, memberOf);
		}

		public void Component<TComponent>(string notVisiblePropertyOrFieldName)
		{
			Component<TComponent>(notVisiblePropertyOrFieldName, x => { });
		}

		/// <summary>
		/// Maps a property or field as a dynamic component. The property can be a C# <c>dynamic</c> or a dictionary of
		/// property names to their value.
		/// </summary>
		/// <param name="notVisiblePropertyOrFieldName">The property or field name to map.</param>
		/// <param name="dynamicComponentTemplate">The template for the component. It should either be a (usually
		/// anonymous) type having the same properties than the component, or an
		/// <c>IDictionary&lt;string, System.Type&gt;</c> of property names with their type.</param>
		/// <param name="mapping">The mapping of the component.</param>
		/// <typeparam name="TComponent">The type of the template.</typeparam>
		public void Component<TComponent>(string notVisiblePropertyOrFieldName, TComponent dynamicComponentTemplate, Action<IDynamicComponentMapper<TComponent>> mapping)
		{
			MemberInfo member = GetRequiredPropertyOrFieldByName(notVisiblePropertyOrFieldName);
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));

			if (dynamicComponentTemplate is IEnumerable<KeyValuePair<string, System.Type>> template)
			{
				var componentType = CreateDynamicComponentTypeFromTemplate(template);
				RegisterDynamicComponentMapping(componentType, mapping, member, memberOf);
			}
			else
			{
				RegisterDynamicComponentMapping(mapping, member, memberOf);
			}
		}

		public void Any<TProperty>(string notVisiblePropertyOrFieldName, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping) where TProperty : class
		{
			MemberInfo member = GetRequiredPropertyOrFieldByName(notVisiblePropertyOrFieldName);
			var propertyOrFieldType = member.GetPropertyOrFieldType();
			if (!typeof(TProperty).Equals(propertyOrFieldType))
			{
				throw new MappingException(string.Format("Wrong relation type. For the property/field '{0}' of {1} was expected a heterogeneous (any) of type {2} but was {3}",
																								 notVisiblePropertyOrFieldName, typeof(TEntity).FullName, typeof(TProperty).Name, propertyOrFieldType.Name));
			}
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));
			RegisterAnyMapping<TProperty>(mapping, idTypeOfMetaType, member, memberOf);
		}

		protected virtual MemberInfo GetRequiredPropertyOrFieldByName(string memberName)
		{
#pragma warning disable 618
			return GetPropertyOrFieldMatchingNameOrThrow(memberName);
#pragma warning restore 618
		}

		[Obsolete("Please use GetRequiredPropertyOrFieldByName instead.")]
		public static MemberInfo GetPropertyOrFieldMatchingNameOrThrow(string memberName)
		{
			var result = typeof(TEntity).GetPropertyOrFieldMatchingName(memberName);
			if (result == null)
			{
				throw new MappingException(string.Format("Member not found. The member '{0}' does not exists in type {1}", memberName, typeof(TEntity).FullName));
			}
			return result;
		}
	}
}
