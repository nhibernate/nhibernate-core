using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Mapping.ByCode.Impl.CustomizersImpl;

namespace NHibernate.Mapping.ByCode
{
	public class ModelMapper
	{
		private readonly ICustomizersHolder customizerHolder;
		private readonly IModelExplicitDeclarationsHolder explicitDeclarationsHolder;
		private readonly ICandidatePersistentMembersProvider membersProvider;
		private readonly IModelInspector modelInspector;
		private readonly List<Import> imports = new List<Import>();

		public ModelMapper() : this(new ExplicitlyDeclaredModel()) { }

		public ModelMapper(IModelInspector modelInspector) : this(modelInspector, modelInspector as IModelExplicitDeclarationsHolder) {}

		public ModelMapper(IModelInspector modelInspector, ICandidatePersistentMembersProvider membersProvider)
			: this(modelInspector, modelInspector as IModelExplicitDeclarationsHolder, new CustomizersHolder(), membersProvider) {}

		public ModelMapper(IModelInspector modelInspector, IModelExplicitDeclarationsHolder explicitDeclarationsHolder)
			: this(modelInspector, explicitDeclarationsHolder, new CustomizersHolder(), new DefaultCandidatePersistentMembersProvider()) {}

		public ModelMapper(IModelInspector modelInspector, IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizerHolder,
		                   ICandidatePersistentMembersProvider membersProvider)
		{
			if (modelInspector == null)
			{
				throw new ArgumentNullException("modelInspector");
			}
			if (customizerHolder == null)
			{
				throw new ArgumentNullException("customizerHolder");
			}
			if (membersProvider == null)
			{
				throw new ArgumentNullException("membersProvider");
			}
			this.modelInspector = modelInspector;
			this.customizerHolder = customizerHolder;
			this.explicitDeclarationsHolder = explicitDeclarationsHolder ?? new FakeModelExplicitDeclarationsHolder();
			this.membersProvider = membersProvider;
		}

		#region Events

		/// <summary>
		/// Occurs before apply pattern-appliers on a root class.
		/// </summary>
		public event RootClassMappingHandler BeforeMapClass;

		/// <summary>
		/// Occurs before apply pattern-appliers on a subclass.
		/// </summary>
		public event SubclassMappingHandler BeforeMapSubclass;

		/// <summary>
		/// Occurs before apply pattern-appliers on a joined-subclass.
		/// </summary>
		public event JoinedSubclassMappingHandler BeforeMapJoinedSubclass;

		/// <summary>
		/// Occurs before apply pattern-appliers on a union-subclass.
		/// </summary>
		public event UnionSubclassMappingHandler BeforeMapUnionSubclass;

		public event PropertyMappingHandler BeforeMapProperty;

		public event ManyToOneMappingHandler BeforeMapManyToOne;

		public event OneToOneMappingHandler BeforeMapOneToOne;

		public event AnyMappingHandler BeforeMapAny;

		public event ComponentMappingHandler BeforeMapComponent;

		public event SetMappingHandler BeforeMapSet;

		public event BagMappingHandler BeforeMapBag;

		public event IdBagMappingHandler BeforeMapIdBag;

		public event ListMappingHandler BeforeMapList;

		public event MapMappingHandler BeforeMapMap;

		public event ManyToManyMappingHandler BeforeMapManyToMany;

		public event ElementMappingHandler BeforeMapElement;

		public event OneToManyMappingHandler BeforeMapOneToMany;

		public event MapKeyManyToManyMappingHandler BeforeMapMapKeyManyToMany;
		public event MapKeyMappingHandler BeforeMapMapKey;

		/// <summary>
		/// Occurs after apply the last customizer on a root class.
		/// </summary>
		public event RootClassMappingHandler AfterMapClass;

		/// <summary>
		/// Occurs after apply the last customizer on a subclass.
		/// </summary>
		public event SubclassMappingHandler AfterMapSubclass;

		/// <summary>
		/// Occurs after apply the last customizer on a joined-subclass..
		/// </summary>
		public event JoinedSubclassMappingHandler AfterMapJoinedSubclass;

		/// <summary>
		/// Occurs after apply the last customizer on a union-subclass..
		/// </summary>
		public event UnionSubclassMappingHandler AfterMapUnionSubclass;

		public event PropertyMappingHandler AfterMapProperty;

		public event ManyToOneMappingHandler AfterMapManyToOne;

		public event OneToOneMappingHandler AfterMapOneToOne;

		public event AnyMappingHandler AfterMapAny;

		public event ComponentMappingHandler AfterMapComponent;

		public event SetMappingHandler AfterMapSet;

		public event BagMappingHandler AfterMapBag;

		public event IdBagMappingHandler AfterMapIdBag;

		public event ListMappingHandler AfterMapList;

		public event MapMappingHandler AfterMapMap;

		public event ManyToManyMappingHandler AfterMapManyToMany;

		public event ElementMappingHandler AfterMapElement;

		public event OneToManyMappingHandler AfterMapOneToMany;

		public event MapKeyManyToManyMappingHandler AfterMapMapKeyManyToMany;

		public event MapKeyMappingHandler AfterMapMapKey;

		private void InvokeBeforeMapUnionSubclass(System.Type type, IUnionSubclassAttributesMapper unionsubclasscustomizer)
		{
			UnionSubclassMappingHandler handler = BeforeMapUnionSubclass;
			if (handler != null)
			{
				handler(ModelInspector, type, unionsubclasscustomizer);
			}
		}

		private void InvokeBeforeMapJoinedSubclass(System.Type type, IJoinedSubclassAttributesMapper joinedsubclasscustomizer)
		{
			JoinedSubclassMappingHandler handler = BeforeMapJoinedSubclass;
			if (handler != null)
			{
				handler(ModelInspector, type, joinedsubclasscustomizer);
			}
		}

		private void InvokeBeforeMapSubclass(System.Type type, ISubclassAttributesMapper subclasscustomizer)
		{
			SubclassMappingHandler handler = BeforeMapSubclass;
			if (handler != null)
			{
				handler(ModelInspector, type, subclasscustomizer);
			}
		}

		private void InvokeBeforeMapClass(System.Type type, IClassAttributesMapper classcustomizer)
		{
			RootClassMappingHandler handler = BeforeMapClass;
			if (handler != null)
			{
				handler(ModelInspector, type, classcustomizer);
			}
		}

		private void InvokeBeforeMapProperty(PropertyPath member, IPropertyMapper propertycustomizer)
		{
			PropertyMappingHandler handler = BeforeMapProperty;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapManyToOne(PropertyPath member, IManyToOneMapper propertycustomizer)
		{
			ManyToOneMappingHandler handler = BeforeMapManyToOne;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapOneToOne(PropertyPath member, IOneToOneMapper propertycustomizer)
		{
			OneToOneMappingHandler handler = BeforeMapOneToOne;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapAny(PropertyPath member, IAnyMapper propertycustomizer)
		{
			AnyMappingHandler handler = BeforeMapAny;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapComponent(PropertyPath member, IComponentAttributesMapper propertycustomizer)
		{
			ComponentMappingHandler handler = BeforeMapComponent;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapSet(PropertyPath member, ISetPropertiesMapper propertycustomizer)
		{
			SetMappingHandler handler = BeforeMapSet;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapBag(PropertyPath member, IBagPropertiesMapper propertycustomizer)
		{
			BagMappingHandler handler = BeforeMapBag;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapIdBag(PropertyPath member, IIdBagPropertiesMapper propertycustomizer)
		{
			IdBagMappingHandler handler = BeforeMapIdBag;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapList(PropertyPath member, IListPropertiesMapper propertycustomizer)
		{
			ListMappingHandler handler = BeforeMapList;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapMap(PropertyPath member, IMapPropertiesMapper propertycustomizer)
		{
			MapMappingHandler handler = BeforeMapMap;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeBeforeMapManyToMany(PropertyPath member, IManyToManyMapper collectionrelationmanytomanycustomizer)
		{
			ManyToManyMappingHandler handler = BeforeMapManyToMany;
			if (handler != null)
			{
				handler(ModelInspector, member, collectionrelationmanytomanycustomizer);
			}
		}

		private void InvokeBeforeMapElement(PropertyPath member, IElementMapper collectionrelationelementcustomizer)
		{
			ElementMappingHandler handler = BeforeMapElement;
			if (handler != null)
			{
				handler(ModelInspector, member, collectionrelationelementcustomizer);
			}
		}

		private void InvokeBeforeMapOneToMany(PropertyPath member, IOneToManyMapper collectionrelationonetomanycustomizer)
		{
			OneToManyMappingHandler handler = BeforeMapOneToMany;
			if (handler != null)
			{
				handler(ModelInspector, member, collectionrelationonetomanycustomizer);
			}
		}

		private void InvokeBeforeMapMapKeyManyToMany(PropertyPath member, IMapKeyManyToManyMapper mapkeymanytomanycustomizer)
		{
			MapKeyManyToManyMappingHandler handler = BeforeMapMapKeyManyToMany;
			if (handler != null)
			{
				handler(ModelInspector, member, mapkeymanytomanycustomizer);
			}
		}

		private void InvokeBeforeMapMapKey(PropertyPath member, IMapKeyMapper mapkeyelementcustomizer)
		{
			MapKeyMappingHandler handler = BeforeMapMapKey;
			if (handler != null)
			{
				handler(ModelInspector, member, mapkeyelementcustomizer);
			}
		}

		private void InvokeAfterMapUnionSubclass(System.Type type, IUnionSubclassAttributesMapper unionsubclasscustomizer)
		{
			UnionSubclassMappingHandler handler = AfterMapUnionSubclass;
			if (handler != null)
			{
				handler(ModelInspector, type, unionsubclasscustomizer);
			}
		}

		private void InvokeAfterMapJoinedSubclass(System.Type type, IJoinedSubclassAttributesMapper joinedsubclasscustomizer)
		{
			JoinedSubclassMappingHandler handler = AfterMapJoinedSubclass;
			if (handler != null)
			{
				handler(ModelInspector, type, joinedsubclasscustomizer);
			}
		}

		private void InvokeAfterMapSubclass(System.Type type, ISubclassAttributesMapper subclasscustomizer)
		{
			SubclassMappingHandler handler = AfterMapSubclass;
			if (handler != null)
			{
				handler(ModelInspector, type, subclasscustomizer);
			}
		}

		private void InvokeAfterMapClass(System.Type type, IClassAttributesMapper classcustomizer)
		{
			RootClassMappingHandler handler = AfterMapClass;
			if (handler != null)
			{
				handler(ModelInspector, type, classcustomizer);
			}
		}

		private void InvokeAfterMapProperty(PropertyPath member, IPropertyMapper propertycustomizer)
		{
			PropertyMappingHandler handler = AfterMapProperty;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapManyToOne(PropertyPath member, IManyToOneMapper propertycustomizer)
		{
			ManyToOneMappingHandler handler = AfterMapManyToOne;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapOneToOne(PropertyPath member, IOneToOneMapper propertycustomizer)
		{
			OneToOneMappingHandler handler = AfterMapOneToOne;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapAny(PropertyPath member, IAnyMapper propertycustomizer)
		{
			AnyMappingHandler handler = AfterMapAny;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapComponent(PropertyPath member, IComponentAttributesMapper propertycustomizer)
		{
			ComponentMappingHandler handler = AfterMapComponent;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapSet(PropertyPath member, ISetPropertiesMapper propertycustomizer)
		{
			SetMappingHandler handler = AfterMapSet;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapBag(PropertyPath member, IBagPropertiesMapper propertycustomizer)
		{
			BagMappingHandler handler = AfterMapBag;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapIdBag(PropertyPath member, IIdBagPropertiesMapper propertycustomizer)
		{
			IdBagMappingHandler handler = AfterMapIdBag;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapList(PropertyPath member, IListPropertiesMapper propertycustomizer)
		{
			ListMappingHandler handler = AfterMapList;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapMap(PropertyPath member, IMapPropertiesMapper propertycustomizer)
		{
			MapMappingHandler handler = AfterMapMap;
			if (handler != null)
			{
				handler(ModelInspector, member, propertycustomizer);
			}
		}

		private void InvokeAfterMapManyToMany(PropertyPath member, IManyToManyMapper collectionrelationmanytomanycustomizer)
		{
			ManyToManyMappingHandler handler = AfterMapManyToMany;
			if (handler != null)
			{
				handler(ModelInspector, member, collectionrelationmanytomanycustomizer);
			}
		}

		private void InvokeAfterMapElement(PropertyPath member, IElementMapper collectionrelationelementcustomizer)
		{
			ElementMappingHandler handler = AfterMapElement;
			if (handler != null)
			{
				handler(ModelInspector, member, collectionrelationelementcustomizer);
			}
		}

		private void InvokeAfterMapOneToMany(PropertyPath member, IOneToManyMapper collectionrelationonetomanycustomizer)
		{
			OneToManyMappingHandler handler = AfterMapOneToMany;
			if (handler != null)
			{
				handler(ModelInspector, member, collectionrelationonetomanycustomizer);
			}
		}

		private void InvokeAfterMapMapKeyManyToMany(PropertyPath member, IMapKeyManyToManyMapper mapkeymanytomanycustomizer)
		{
			MapKeyManyToManyMappingHandler handler = AfterMapMapKeyManyToMany;
			if (handler != null)
			{
				handler(ModelInspector, member, mapkeymanytomanycustomizer);
			}
		}

		private void InvokeAfterMapMapKey(PropertyPath member, IMapKeyMapper mapkeyelementcustomizer)
		{
			MapKeyMappingHandler handler = AfterMapMapKey;
			if (handler != null)
			{
				handler(ModelInspector, member, mapkeyelementcustomizer);
			}
		}

		#endregion

		public IModelInspector ModelInspector
		{
			get { return modelInspector; }
		}

		protected ICandidatePersistentMembersProvider MembersProvider
		{
			get { return membersProvider; }
		}

		public void Class<TRootEntity>(Action<IClassMapper<TRootEntity>> customizeAction) where TRootEntity : class
		{
			var customizer = new ClassCustomizer<TRootEntity>(explicitDeclarationsHolder, customizerHolder);
			customizeAction(customizer);
		}

		public void Subclass<TEntity>(Action<ISubclassMapper<TEntity>> customizeAction) where TEntity : class
		{
			var customizer = new SubclassCustomizer<TEntity>(explicitDeclarationsHolder, customizerHolder);
			customizeAction(customizer);
		}

		public void JoinedSubclass<TEntity>(Action<IJoinedSubclassMapper<TEntity>> customizeAction) where TEntity : class
		{
			var customizer = new JoinedSubclassCustomizer<TEntity>(explicitDeclarationsHolder, customizerHolder);
			customizeAction(customizer);
		}

		public void UnionSubclass<TEntity>(Action<IUnionSubclassMapper<TEntity>> customizeAction) where TEntity : class
		{
			var customizer = new UnionSubclassCustomizer<TEntity>(explicitDeclarationsHolder, customizerHolder);
			customizeAction(customizer);
		}

		public void Component<TComponent>(Action<IComponentMapper<TComponent>> customizeAction) where TComponent : class
		{
			var customizer = new ComponentCustomizer<TComponent>(explicitDeclarationsHolder, customizerHolder);
			customizeAction(customizer);
		}

		public void Import<TImportClass>()
		{
			Import<TImportClass>(typeof(TImportClass).Name);
		}

		public void Import<TImportClass>(string rename)
		{
			imports.Add(new Import(typeof(TImportClass), rename));
		}

		public HbmMapping CompileMappingFor(IEnumerable<System.Type> types)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			var typeToMap = new HashSet<System.Type>(types);

			string defaultAssemblyName = null;
			string defaultNamespace = null;
			System.Type firstType = typeToMap.FirstOrDefault();
			if (firstType != null && typeToMap.All(t => t.Assembly.Equals(firstType.Assembly)))
			{
				//NH-2831: always use the full name of the assembly because it may come from GAC
				defaultAssemblyName = firstType.Assembly.GetName().FullName;
			}
			if (firstType != null && typeToMap.All(t => t.Namespace == firstType.Namespace))
			{
				defaultNamespace = firstType.Namespace;
			}
			var mapping = NewHbmMapping(defaultAssemblyName, defaultNamespace);
			foreach (System.Type type in RootClasses(typeToMap))
			{
				MapRootClass(type, mapping);
			}
			foreach (System.Type type in Subclasses(typeToMap))
			{
				AddSubclassMapping(mapping, type);
			}
			return mapping;
		}

		public IEnumerable<HbmMapping> CompileMappingForEach(IEnumerable<System.Type> types)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			var typeToMap = new HashSet<System.Type>(types);

			//NH-2831: always use the full name of the assembly because it may come from GAC
			foreach (System.Type type in RootClasses(typeToMap))
			{
				var mapping = NewHbmMapping(type.Assembly.GetName().FullName, type.Namespace);
				MapRootClass(type, mapping);
				yield return mapping;
			}
			foreach (System.Type type in Subclasses(typeToMap))
			{
				var mapping = NewHbmMapping(type.Assembly.GetName().FullName, type.Namespace);
				AddSubclassMapping(mapping, type);
				yield return mapping;
			}
		}

		private HbmMapping NewHbmMapping(string defaultAssemblyName, string defaultNamespace)
		{
			var hbmMapping = new HbmMapping {assembly = defaultAssemblyName, @namespace = defaultNamespace};

			imports.ForEach(i => i.AddToMapping(hbmMapping));
			imports.Clear();

			return hbmMapping;
		}

		private IEnumerable<System.Type> Subclasses(IEnumerable<System.Type> types)
		{
			return types.Where(type => modelInspector.IsEntity(type) && !modelInspector.IsRootEntity(type));
		}

		private IEnumerable<System.Type> RootClasses(IEnumerable<System.Type> types)
		{
			return types.Where(type => modelInspector.IsEntity(type) && modelInspector.IsRootEntity(type));
		}

		private void AddSubclassMapping(HbmMapping mapping, System.Type type)
		{
			if (modelInspector.IsTablePerClassHierarchy(type))
			{
				MapSubclass(type, mapping);
			}
			else if (modelInspector.IsTablePerClass(type))
			{
				MapJoinedSubclass(type, mapping);
			}
			else if (modelInspector.IsTablePerConcreteClass(type))
			{
				MapUnionSubclass(type, mapping);
			}
		}

		private void MapUnionSubclass(System.Type type, HbmMapping mapping)
		{
			var classMapper = new UnionSubclassMapper(type, mapping);

			IEnumerable<MemberInfo> candidateProperties = null;
			if (!modelInspector.IsEntity(type.BaseType))
			{
				System.Type baseType = GetEntityBaseType(type);
				if (baseType != null)
				{
					classMapper.Extends(baseType);
					candidateProperties = membersProvider.GetSubEntityMembers(type, baseType);
				}
			}
			candidateProperties = candidateProperties ?? membersProvider.GetSubEntityMembers(type, type.BaseType);
			IEnumerable<MemberInfo> propertiesToMap =
				candidateProperties.Where(p => modelInspector.IsPersistentProperty(p) && !modelInspector.IsPersistentId(p));

			InvokeBeforeMapUnionSubclass(type, classMapper);
			customizerHolder.InvokeCustomizers(type, classMapper);
			InvokeAfterMapUnionSubclass(type, classMapper);

			MapProperties(type, propertiesToMap, classMapper);
		}

		private void MapSubclass(System.Type type, HbmMapping mapping)
		{
			var classMapper = new SubclassMapper(type, mapping);
			IEnumerable<MemberInfo> candidateProperties = null;
			if (!modelInspector.IsEntity(type.BaseType))
			{
				System.Type baseType = GetEntityBaseType(type);
				if (baseType != null)
				{
					classMapper.Extends(baseType);
					candidateProperties = membersProvider.GetSubEntityMembers(type, baseType);
				}
			}
			candidateProperties = candidateProperties ?? membersProvider.GetSubEntityMembers(type, type.BaseType);

			InvokeBeforeMapSubclass(type, classMapper);
			customizerHolder.InvokeCustomizers(type, classMapper);
			foreach (var joinMapper in classMapper.JoinMappers.Values)
			{
				customizerHolder.InvokeCustomizers(type, joinMapper);
			}

			var splitGroups = modelInspector.GetPropertiesSplits(type);
			var propertiesToMap = candidateProperties.Where(p => modelInspector.IsPersistentProperty(p) && !modelInspector.IsPersistentId(p));
			var propertiesInSplits = new HashSet<MemberInfo>();
			foreach (var splitGroup in splitGroups)
			{
				var groupId = splitGroup;
				var propertiesOfTheGroup = propertiesToMap.Where(p => modelInspector.IsTablePerClassSplit(type, groupId, p)).ToList();
				IJoinMapper joinMapper;
				if (propertiesOfTheGroup.Count > 0 && classMapper.JoinMappers.TryGetValue(groupId, out joinMapper))
				{
					MapSplitProperties(type, propertiesOfTheGroup, joinMapper);
					propertiesInSplits.UnionWith(propertiesOfTheGroup);
				}
			}

			MapProperties(type, propertiesToMap.Except(propertiesInSplits), classMapper);

			InvokeAfterMapSubclass(type, classMapper);
		}

		private void MapJoinedSubclass(System.Type type, HbmMapping mapping)
		{
			var classMapper = new JoinedSubclassMapper(type, mapping);
			IEnumerable<MemberInfo> candidateProperties = null;
			if (!modelInspector.IsEntity(type.BaseType))
			{
				System.Type baseType = GetEntityBaseType(type);
				if (baseType != null)
				{
					classMapper.Extends(baseType);
					classMapper.Key(km => km.Column(baseType.Name.ToLowerInvariant() + "_key"));
					candidateProperties = membersProvider.GetSubEntityMembers(type, baseType);
				}
			}
			candidateProperties = candidateProperties ?? membersProvider.GetSubEntityMembers(type, type.BaseType);
			IEnumerable<MemberInfo> propertiesToMap =
				candidateProperties.Where(p => modelInspector.IsPersistentProperty(p) && !modelInspector.IsPersistentId(p));

			InvokeBeforeMapJoinedSubclass(type, classMapper);
			customizerHolder.InvokeCustomizers(type, classMapper);
			InvokeAfterMapJoinedSubclass(type, classMapper);

			MapProperties(type, propertiesToMap, classMapper);
		}

		private System.Type GetEntityBaseType(System.Type type)
		{
			System.Type analyzingType = type;
			while (analyzingType != null && analyzingType != typeof (object))
			{
				analyzingType = analyzingType.BaseType;
				if (modelInspector.IsEntity(analyzingType))
				{
					return analyzingType;
				}
			}
			return type.GetInterfaces().FirstOrDefault(i => modelInspector.IsEntity(i));
		}

		private void MapRootClass(System.Type type, HbmMapping mapping)
		{
			MemberInfo poidPropertyOrField = membersProvider.GetEntityMembersForPoid(type).FirstOrDefault(mi => modelInspector.IsPersistentId(mi));
			var classMapper = new ClassMapper(type, mapping, poidPropertyOrField);
			if (modelInspector.IsTablePerClassHierarchy(type))
			{
				classMapper.Discriminator(x => { });
			}
			MemberInfo[] persistentProperties =
				membersProvider.GetRootEntityMembers(type).Where(
					p => modelInspector.IsPersistentProperty(p) && !modelInspector.IsPersistentId(p)).ToArray();

			InvokeBeforeMapClass(type, classMapper);
			InvokeClassCustomizers(type, classMapper);

			if (poidPropertyOrField != null && modelInspector.IsComponent(poidPropertyOrField.GetPropertyOrFieldType()))
			{
				classMapper.ComponentAsId(poidPropertyOrField, compoAsId =>
				                                               {
				                                               	var memberPath = new PropertyPath(null, poidPropertyOrField);
				                                               	var componentMapper = new ComponentAsIdLikeComponentAttributesMapper(compoAsId);
				                                               	InvokeBeforeMapComponent(memberPath, componentMapper);

				                                               	System.Type componentType = poidPropertyOrField.GetPropertyOrFieldType();
				                                               	IEnumerable<MemberInfo> componentPersistentProperties =
				                                               		membersProvider.GetComponentMembers(componentType).Where(p => modelInspector.IsPersistentProperty(p));

				                                               	customizerHolder.InvokeCustomizers(componentType, componentMapper);
				                                               	ForEachMemberPath(poidPropertyOrField, memberPath, pp => customizerHolder.InvokeCustomizers(pp, compoAsId));
				                                               	InvokeAfterMapComponent(memberPath, componentMapper);

				                                               	foreach (MemberInfo property in componentPersistentProperties)
				                                               	{
				                                               		MapComposedIdProperties(compoAsId, new PropertyPath(memberPath, property));
				                                               	}
				                                               });
			}

			MemberInfo[] composedIdPropeties = persistentProperties.Where(mi => modelInspector.IsMemberOfComposedId(mi)).ToArray();
			if (composedIdPropeties.Length > 0)
			{
				classMapper.ComposedId(composedIdMapper =>
				                       {
				                       	foreach (MemberInfo property in composedIdPropeties)
				                       	{
				                       		MapComposedIdProperties(composedIdMapper, new PropertyPath(null, property));
				                       	}
				                       });
			}

			MemberInfo[] naturalIdPropeties = persistentProperties.Except(composedIdPropeties).Where(mi => modelInspector.IsMemberOfNaturalId(mi)).ToArray();
			if (naturalIdPropeties.Length > 0)
			{
				classMapper.NaturalId(naturalIdMapper =>
				                      {
				                      	foreach (var property in naturalIdPropeties)
				                      	{
				                      		MapNaturalIdProperties(type, naturalIdMapper, property);
				                      	}
				                      });
			}
			var splitGroups = modelInspector.GetPropertiesSplits(type);
			var propertiesToMap = persistentProperties.Except(naturalIdPropeties).Except(composedIdPropeties).Where(mi => !modelInspector.IsVersion(mi) && !modelInspector.IsVersion(mi.GetMemberFromDeclaringType())).ToList();
			var propertiesInSplits = new HashSet<MemberInfo>();
			foreach (var splitGroup in splitGroups)
			{
				var groupId= splitGroup;
				var propertiesOfTheGroup = propertiesToMap.Where(p => modelInspector.IsTablePerClassSplit(type, groupId, p)).ToList();
				IJoinMapper joinMapper;
				if (propertiesOfTheGroup.Count > 0 && classMapper.JoinMappers.TryGetValue(groupId, out joinMapper))
				{
					MapSplitProperties(type, propertiesOfTheGroup, joinMapper);
					propertiesInSplits.UnionWith(propertiesOfTheGroup);
				}
			}

			MapProperties(type, propertiesToMap.Except(propertiesInSplits), classMapper);
			InvokeAfterMapClass(type, classMapper);
		}

		private void MapSplitProperties(System.Type propertiesContainerType, IEnumerable<MemberInfo> propertiesToMap, IJoinMapper propertiesContainer)
		{
			foreach (var property in propertiesToMap)
			{
				MemberInfo member = property;
				System.Type propertyType = property.GetPropertyOrFieldType();
				var memberPath = new PropertyPath(null, member);
				if (modelInspector.IsProperty(member))
				{
					MapProperty(member, memberPath, propertiesContainer);
				}
				else if (modelInspector.IsAny(member))
				{
					MapAny(member, memberPath, propertiesContainer);
				}
				else if (modelInspector.IsManyToOne(property))
				{
					MapManyToOne(member, memberPath, propertiesContainer);
				}
				else if (modelInspector.IsSet(property))
				{
					MapSet(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (modelInspector.IsDictionary(property))
				{
					MapDictionary(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (modelInspector.IsArray(property))
				{
					throw new NotSupportedException();
				}
				else if (modelInspector.IsList(property))
				{
					MapList(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (modelInspector.IsIdBag(property))
				{
					MapIdBag(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (modelInspector.IsBag(property))
				{
					MapBag(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (modelInspector.IsComponent(propertyType))
				{
					MapComponent(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else
				{
					MapProperty(member, memberPath, propertiesContainer);
				}
			}
		}

		private void InvokeClassCustomizers(System.Type type, ClassMapper classMapper)
		{
			InvokeAncestorsCustomizers(type.GetInterfaces(), classMapper);
			InvokeAncestorsCustomizers(type.GetHierarchyFromBase(), classMapper);
			customizerHolder.InvokeCustomizers(type, classMapper);
			foreach (var joinMapper in classMapper.JoinMappers.Values)
			{
				customizerHolder.InvokeCustomizers(type, joinMapper);
			}
		}

		private void InvokeAncestorsCustomizers(IEnumerable<System.Type> typeAncestors, IClassMapper classMapper)
		{
			// only apply the polymorphic mapping for no entities:
			// this is to avoid a possible caos in entity-subclassing:
			// an example of caos is when a base class has a specific TableName and a subclass does not have a specific name (use the default class name).
			// I can remove this "limitation", where required, and delegate to the user the responsibility of his caos.
			foreach (System.Type entityType in typeAncestors.Where(t => !modelInspector.IsEntity(t)))
			{
				customizerHolder.InvokeCustomizers(entityType, classMapper);
			}
		}

		private void MapComposedIdProperties(IMinimalPlainPropertyContainerMapper composedIdMapper, PropertyPath propertyPath)
		{
			MemberInfo member = propertyPath.LocalMember;
			System.Type propertyType = member.GetPropertyOrFieldType();
			var memberPath = propertyPath;
			if (modelInspector.IsProperty(member))
			{
				MapProperty(member, memberPath, composedIdMapper);
			}
			else if (modelInspector.IsManyToOne(member))
			{
				MapManyToOne(member, memberPath, composedIdMapper);
			}
			else if (modelInspector.IsAny(member) || modelInspector.IsComponent(propertyType) ||
							 modelInspector.IsOneToOne(member) || modelInspector.IsSet(member)
							 || modelInspector.IsDictionary(member) || modelInspector.IsArray(member)
							 || modelInspector.IsList(member) || modelInspector.IsBag(member))
			{
				throw new ArgumentOutOfRangeException("propertyPath",
				                                      string.Format("The property {0} of {1} can't be part of composite-id.",
																														member.Name, member.DeclaringType));
			}
			else
			{
				MapProperty(member, memberPath, composedIdMapper);
			}
		}

		private void MapNaturalIdProperties(System.Type rootEntityType, INaturalIdMapper naturalIdMapper, MemberInfo property)
		{
			MemberInfo member = property;
			System.Type propertyType = property.GetPropertyOrFieldType();
			var memberPath = new PropertyPath(null, member);
			if (modelInspector.IsProperty(member))
			{
				MapProperty(member, memberPath, naturalIdMapper);
			}
			else if (modelInspector.IsAny(member))
			{
				MapAny(member, memberPath, naturalIdMapper);
			}
			else if (modelInspector.IsManyToOne(member))
			{
				MapManyToOne(member, memberPath, naturalIdMapper);
			}
			else if (modelInspector.IsComponent(propertyType))
			{
				MapComponent(member, memberPath, propertyType, naturalIdMapper, rootEntityType);
			}
			else if (modelInspector.IsOneToOne(member) || modelInspector.IsSet(property)
			         || modelInspector.IsDictionary(property) || modelInspector.IsArray(property)
			         || modelInspector.IsList(property) || modelInspector.IsBag(property))
			{
				throw new ArgumentOutOfRangeException("property",
				                                      string.Format("The property {0} of {1} can't be part of natural-id.",
				                                                    property.Name, property.DeclaringType));
			}
			else
			{
				MapProperty(member, memberPath, naturalIdMapper);
			}
		}

		private void MapProperties(System.Type propertiesContainerType, IEnumerable<MemberInfo> propertiesToMap,
		                           IPropertyContainerMapper propertiesContainer)
		{
			MapProperties(propertiesContainerType, propertiesToMap, propertiesContainer, null);
		}

		private void MapProperties(System.Type propertiesContainerType, IEnumerable<MemberInfo> propertiesToMap,
		                           IPropertyContainerMapper propertiesContainer, PropertyPath path)
		{
			foreach (var property in propertiesToMap)
			{
				MemberInfo member = property;
				System.Type propertyType = property.GetPropertyOrFieldType();
				var memberPath = new PropertyPath(path, member);
				if (modelInspector.IsProperty(member))
				{
					MapProperty(member, memberPath, propertiesContainer);
				}
				else if (modelInspector.IsAny(member))
				{
					MapAny(member, memberPath, propertiesContainer);
				}
				else if (modelInspector.IsManyToOne(property))
				{
					MapManyToOne(member, memberPath, propertiesContainer);
				}
				else if (modelInspector.IsOneToOne(property))
				{
					MapOneToOne(member, memberPath, propertiesContainer);
				}
				else if (modelInspector.IsDynamicComponent(property))
				{
					MapDynamicComponent(member, memberPath, propertyType, propertiesContainer);
				}
				else if (modelInspector.IsSet(property))
				{
					MapSet(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (modelInspector.IsDictionary(property))
				{
					MapDictionary(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (modelInspector.IsArray(property))
				{
					throw new NotSupportedException();
				}
				else if (modelInspector.IsList(property))
				{
					MapList(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (modelInspector.IsIdBag(property))
				{
					MapIdBag(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (modelInspector.IsBag(property))
				{
					MapBag(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (modelInspector.IsComponent(propertyType))
				{
					MapComponent(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else
				{
					MapProperty(member, memberPath, propertiesContainer);
				}
			}
		}

		private void MapDynamicComponent(MemberInfo member, PropertyPath memberPath, System.Type propertyType, IPropertyContainerMapper propertiesContainer)
		{
			propertiesContainer.Component(member, (IDynamicComponentMapper componentMapper) =>
			{
				System.Type componentType = modelInspector.GetDynamicComponentTemplate(member);
				IEnumerable<MemberInfo> persistentProperties = membersProvider.GetComponentMembers(componentType);

				ForEachMemberPath(member, memberPath, pp => customizerHolder.InvokeCustomizers(pp, componentMapper));

				MapProperties(propertyType, persistentProperties, componentMapper, memberPath);
			});
		}

		private void MapAny(MemberInfo member, PropertyPath memberPath, IBasePlainPropertyContainerMapper propertiesContainer)
		{
			propertiesContainer.Any(member, typeof (int), anyMapper =>
			                                              {
			                                              	InvokeBeforeMapAny(memberPath, anyMapper);
			                                              	MemberInfo poidPropertyOrField =
			                                              		membersProvider.GetEntityMembersForPoid(memberPath.GetRootMember().DeclaringType).FirstOrDefault(
			                                              			mi => modelInspector.IsPersistentId(mi));

			                                              	if (poidPropertyOrField != null)
			                                              	{
			                                              		anyMapper.IdType(poidPropertyOrField.GetPropertyOrFieldType());
			                                              	}
			                                              	ForEachMemberPath(member, memberPath, pp => customizerHolder.InvokeCustomizers(pp, anyMapper));
			                                              	InvokeAfterMapAny(memberPath, anyMapper);
			                                              });
		}

		private void MapProperty(MemberInfo member, PropertyPath propertyPath, IMinimalPlainPropertyContainerMapper propertiesContainer)
		{
			propertiesContainer.Property(member, propertyMapper =>
			                                     {
			                                     	InvokeBeforeMapProperty(propertyPath, propertyMapper);
			                                     	ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, propertyMapper));
			                                     	InvokeAfterMapProperty(propertyPath, propertyMapper);
			                                     });
		}

		protected void ForEachMemberPath(MemberInfo member, PropertyPath progressivePath, Action<PropertyPath> invoke)
		{
			// This method will "fail" if the IModelInspector is based only on IModelExplicitDeclarationsHolder and will work 
			// when IModelInspector has the capability to distinguish entities form explicit mappings 
			// (you can specify the mapping using an interface but the interface is not an entity).

			// To ensure that a customizer is called just once I can't use a set because all customizers
			// needs to be called in a certain sequence starting from the most general (interfaces) to the
			// most specific (on progressivePath).

			// There are three levels of direct-customization of a property of the class under-mapping:
			// 1) at interface level
			// 2) at class implementation level
			// 3) at inherited class implementation level
			// After these three levels we have a special behavior for componets mapping.
			// As example :
			// We three components classes named C1, C2, C3. The C1 has a property of C2 and C2 has a property of C3.
			// Given a class X with a collection or a property of type C1 we can customize C3 starting from:
			// a) C3 itself (the cases 1-2-3 above)
			// b) from C2
			// c) from C1 then from C2
			// d) from the collection in X then C1 then from C2
			// The full-progressive-path of the property is : X.Collection->C1.C2.C3.MyProp
			// I have to execute the customization at each possible level (a,b,c,d).

			var invokedPaths = new HashSet<PropertyPath>();

			// paths on interfaces (note: when a property is the implementation of more then one interface a specific order can't be applied...AFAIK)
			IEnumerable<MemberInfo> propertiesOnInterfaces = member.GetPropertyFromInterfaces();
			foreach (MemberInfo propertyOnInterface in propertiesOnInterfaces)
			{
				var propertyPathInterfaceLevel = new PropertyPath(null, propertyOnInterface);
				invoke(propertyPathInterfaceLevel);
			}

			// path on declaring type
			var propertyPathLevel0 = new PropertyPath(null, member.GetMemberFromDeclaringType());
			// path on reflected type
			var propertyPathLevel1 = new PropertyPath(null, member);

			invoke(propertyPathLevel0);
			invokedPaths.Add(propertyPathLevel0);

			if (!propertyPathLevel0.Equals(propertyPathLevel1))
			{
				invoke(propertyPathLevel1);
				invokedPaths.Add(propertyPathLevel1);
			}

			foreach (PropertyPath propertyPath in progressivePath.InverseProgressivePath())
			{
				if (!invokedPaths.Contains(propertyPath))
				{
					invoke(propertyPath);
					invokedPaths.Add(propertyPath);
				}
			}
		}

		private void MapComponent(MemberInfo member, PropertyPath memberPath, System.Type propertyType, IBasePlainPropertyContainerMapper propertiesContainer,
		                          System.Type propertiesContainerType)
		{
			propertiesContainer.Component(member, componentMapper =>
			                                      {
			                                      	InvokeBeforeMapComponent(memberPath, componentMapper); // <<== perhaps is better after find the parent
			                                      	System.Type componentType = propertyType;
			                                      	IEnumerable<MemberInfo> persistentProperties =
			                                      		membersProvider.GetComponentMembers(componentType).Where(p => modelInspector.IsPersistentProperty(p));

			                                      	MemberInfo parentReferenceProperty = GetComponentParentReferenceProperty(persistentProperties, propertiesContainerType);
			                                      	if (parentReferenceProperty != null)
			                                      	{
			                                      		componentMapper.Parent(parentReferenceProperty);
			                                      	}
																							customizerHolder.InvokeCustomizers(componentType, componentMapper);
			                                      	ForEachMemberPath(member, memberPath, pp => customizerHolder.InvokeCustomizers(pp, componentMapper));
			                                      	InvokeAfterMapComponent(memberPath, componentMapper);

			                                      	MapProperties(propertyType, persistentProperties.Where(pi => pi != parentReferenceProperty), componentMapper, memberPath);
			                                      });
		}

		protected MemberInfo GetComponentParentReferenceProperty(IEnumerable<MemberInfo> persistentProperties, System.Type propertiesContainerType)
		{
			// if container is component, then all properties referencing container are assumed parent reference
			if (modelInspector.IsComponent(propertiesContainerType))
				return persistentProperties.FirstOrDefault(pp => pp.GetPropertyOrFieldType() == propertiesContainerType);

			// return the first non-many-to-one property
			return persistentProperties.Where(pp => !modelInspector.IsManyToOne(pp)).FirstOrDefault(pp => pp.GetPropertyOrFieldType() == propertiesContainerType);
		}

		private void MapBag(MemberInfo member, PropertyPath propertyPath, System.Type propertyType, ICollectionPropertiesContainerMapper propertiesContainer,
		                    System.Type propertiesContainerType)
		{
			System.Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, member, propertyType);
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, propertyPath, collectionElementType);
			propertiesContainer.Bag(member, collectionPropertiesMapper =>
			                                {
			                                	cert.MapCollectionProperties(collectionPropertiesMapper);
																				InvokeBeforeMapBag(propertyPath, collectionPropertiesMapper);
																				ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, collectionPropertiesMapper));
			                                	InvokeAfterMapBag(propertyPath, collectionPropertiesMapper);
			                                }, cert.Map);
		}

		private void MapList(MemberInfo member, PropertyPath propertyPath, System.Type propertyType, ICollectionPropertiesContainerMapper propertiesContainer,
		                     System.Type propertiesContainerType)
		{
			System.Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, member, propertyType);
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, propertyPath, collectionElementType);
			propertiesContainer.List(member, collectionPropertiesMapper =>
			                                 {
			                                 	cert.MapCollectionProperties(collectionPropertiesMapper);
																				InvokeBeforeMapList(propertyPath, collectionPropertiesMapper);
																				ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, collectionPropertiesMapper));
			                                 	InvokeAfterMapList(propertyPath, collectionPropertiesMapper);
			                                 }, cert.Map);
		}

		private void MapDictionary(MemberInfo member, PropertyPath propertyPath, System.Type propertyType, ICollectionPropertiesContainerMapper propertiesContainer,
		                           System.Type propertiesContainerType)
		{
			System.Type dictionaryKeyType = propertyType.DetermineDictionaryKeyType();
			if (dictionaryKeyType == null)
			{
				throw new NotSupportedException(string.Format("Can't determine collection element relation (property {0} in {1})",
				                                              member.Name, propertiesContainerType));
			}
			IMapKeyRelationMapper mkrm = DetermineMapKeyRelationType(member, propertyPath, dictionaryKeyType);

			System.Type dictionaryValueType = propertyType.DetermineDictionaryValueType();
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, propertyPath, dictionaryValueType);

			propertiesContainer.Map(member, collectionPropertiesMapper =>
			                                {
			                                	cert.MapCollectionProperties(collectionPropertiesMapper);
																				InvokeBeforeMapMap(propertyPath, collectionPropertiesMapper);
																				ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, collectionPropertiesMapper));
			                                	InvokeAfterMapMap(propertyPath, collectionPropertiesMapper);
			                                }, mkrm.Map, cert.Map);
		}

		private void MapSet(MemberInfo member, PropertyPath propertyPath, System.Type propertyType, ICollectionPropertiesContainerMapper propertiesContainer,
		                    System.Type propertiesContainerType)
		{
			System.Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, member, propertyType);
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, propertyPath, collectionElementType);
			propertiesContainer.Set(member, collectionPropertiesMapper =>
			                                {
			                                	cert.MapCollectionProperties(collectionPropertiesMapper);
																				InvokeBeforeMapSet(propertyPath, collectionPropertiesMapper);
																				ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, collectionPropertiesMapper));
			                                	InvokeAfterMapSet(propertyPath, collectionPropertiesMapper);
			                                }, cert.Map);
		}

		private void MapIdBag(MemberInfo member, PropertyPath propertyPath, System.Type propertyType, ICollectionPropertiesContainerMapper propertiesContainer,
												System.Type propertiesContainerType)
		{
			System.Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, member, propertyType);
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, propertyPath, collectionElementType);
			if(cert is OneToManyRelationMapper)
			{
				throw new NotSupportedException("id-bag does not suppot one-to-many relation");
			}
			propertiesContainer.IdBag(member, collectionPropertiesMapper =>
			{
				cert.MapCollectionProperties(collectionPropertiesMapper);
				InvokeBeforeMapIdBag(propertyPath, collectionPropertiesMapper);
				ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, collectionPropertiesMapper));
				InvokeAfterMapIdBag(propertyPath, collectionPropertiesMapper);
			}, cert.Map);
		}

		private void MapOneToOne(MemberInfo member, PropertyPath propertyPath, IPlainPropertyContainerMapper propertiesContainer)
		{
			propertiesContainer.OneToOne(member, oneToOneMapper =>
			                                     {
			                                     	InvokeBeforeMapOneToOne(propertyPath, oneToOneMapper);
			                                     	ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, oneToOneMapper));
			                                     	InvokeAfterMapOneToOne(propertyPath, oneToOneMapper);
			                                     });
		}

		private void MapManyToOne(MemberInfo member, PropertyPath propertyPath, IMinimalPlainPropertyContainerMapper propertiesContainer)
		{
			propertiesContainer.ManyToOne(member, manyToOneMapper =>
			                                      {
			                                      	InvokeBeforeMapManyToOne(propertyPath, manyToOneMapper);
			                                      	ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, manyToOneMapper));
			                                      	InvokeAfterMapManyToOne(propertyPath, manyToOneMapper);
			                                      });
		}

		private System.Type GetCollectionElementTypeOrThrow(System.Type type, MemberInfo property, System.Type propertyType)
		{
			System.Type collectionElementType = propertyType.DetermineCollectionElementType();
			if (collectionElementType == null)
			{
				throw new NotSupportedException(string.Format("Can't determine collection element relation (property {0} in {1})",
				                                              property.Name, type));
			}
			return collectionElementType;
		}

		protected virtual ICollectionElementRelationMapper DetermineCollectionElementRelationType(MemberInfo property, PropertyPath propertyPath, System.Type collectionElementType)
		{
			System.Type ownerType = property.ReflectedType;
			if (modelInspector.IsOneToMany(property))
			{
				return new OneToManyRelationMapper(propertyPath, ownerType, collectionElementType, modelInspector, customizerHolder, this);
			}
			//NH-3667 & NH-3102
			//check if property is really a many-to-many: as detected by modelInspector.IsManyToMany and also the collection type is an entity
			if (modelInspector.IsManyToMany(property) == true)
			{
				if (property.GetPropertyOrFieldType().IsGenericCollection() == true)
				{
					var args = property.GetPropertyOrFieldType().GetGenericArguments();

					if (modelInspector.IsEntity(args.Last()) == true)
					{
						return new ManyToManyRelationMapper(propertyPath, customizerHolder, this);
					}
				}
			}
			if (modelInspector.IsComponent(collectionElementType))
			{
				return new ComponentRelationMapper(property, ownerType, collectionElementType, membersProvider, modelInspector, customizerHolder, this);
			}
			if (modelInspector.IsManyToAny(property))
			{
				return new ManyToAnyRelationMapper(propertyPath, customizerHolder, this);
			}
			return new ElementRelationMapper(propertyPath, customizerHolder, this);
		}

		private IMapKeyRelationMapper DetermineMapKeyRelationType(MemberInfo member, PropertyPath propertyPath, System.Type dictionaryKeyType)
		{
			// Perhaps we have to change IModelInspector with IsDictionaryKeyManyToMany(member), IsDictionaryKeyComponent(member) and so on

			//if (modelInspector.IsManyToMany(member) || modelInspector.IsOneToMany(member))
			if (modelInspector.IsEntity(dictionaryKeyType))
			{
				// OneToMany is not possible as map-key so we map it as many-to-many instead ignore the case
				return new KeyManyToManyRelationMapper(propertyPath, customizerHolder, this);
			}
			if (modelInspector.IsComponent(dictionaryKeyType))
			{
				return new KeyComponentRelationMapper(dictionaryKeyType, propertyPath, membersProvider, modelInspector, customizerHolder, this);
			}
			return new KeyElementRelationMapper(propertyPath, customizerHolder, this);
		}

		#region Nested type: ComponentRelationMapper

		private class ComponentRelationMapper : ICollectionElementRelationMapper
		{
			private readonly MemberInfo collectionMember;
			private readonly System.Type componentType;
			private readonly ICustomizersHolder customizersHolder;
			private readonly IModelInspector domainInspector;
			private readonly ICandidatePersistentMembersProvider membersProvider;
			private readonly ModelMapper modelMapper;
			private readonly System.Type ownerType;

			public ComponentRelationMapper(MemberInfo collectionMember, System.Type ownerType, System.Type componentType, ICandidatePersistentMembersProvider membersProvider,
			                               IModelInspector domainInspector, ICustomizersHolder customizersHolder, ModelMapper modelMapper)
			{
				this.collectionMember = collectionMember;
				this.ownerType = ownerType;
				this.componentType = componentType;
				this.membersProvider = membersProvider;
				this.domainInspector = domainInspector;
				this.customizersHolder = customizersHolder;
				this.modelMapper = modelMapper;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.Component(x =>
													 {
														 IEnumerable<MemberInfo> persistentProperties = GetPersistentProperties(componentType);

														 MemberInfo parentReferenceProperty = modelMapper.GetComponentParentReferenceProperty(persistentProperties, ownerType);
														 if (parentReferenceProperty != null)
														 {
															 x.Parent(parentReferenceProperty);
														 }
														 customizersHolder.InvokeCustomizers(componentType, x);

														 var propertyPath = new PropertyPath(null, collectionMember);
														 MapProperties(componentType, propertyPath, x, persistentProperties.Where(pi => pi != parentReferenceProperty));
													 });
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped) {}

			#endregion

			private IEnumerable<MemberInfo> GetPersistentProperties(System.Type type)
			{
				IEnumerable<MemberInfo> properties = membersProvider.GetComponentMembers(type);
				return properties.Where(p => domainInspector.IsPersistentProperty(p));
			}

			private void MapProperties(System.Type type, PropertyPath memberPath, IComponentElementMapper propertiesContainer, IEnumerable<MemberInfo> persistentProperties)
			{
				// TODO check PropertyPath behaviour when the component is in a collection
				foreach (MemberInfo property in persistentProperties)
				{
					MemberInfo member = property;
					System.Type propertyType = property.GetPropertyOrFieldType();
					var propertyPath = new PropertyPath(memberPath, member);

					if (domainInspector.IsManyToOne(member))
					{
						propertiesContainer.ManyToOne(member, manyToOneMapper =>
						                                      {
						                                      	modelMapper.InvokeBeforeMapManyToOne(propertyPath, manyToOneMapper);
						                                      	modelMapper.ForEachMemberPath(member, propertyPath, pp => customizersHolder.InvokeCustomizers(pp, manyToOneMapper));
						                                      	modelMapper.InvokeAfterMapManyToOne(propertyPath, manyToOneMapper);
						                                      });
					}
					else if (domainInspector.IsComponent(propertyType))
					{
						propertiesContainer.Component(member, x =>
						                                      {
						                                      	modelMapper.InvokeBeforeMapComponent(propertyPath, x);
						                                      	// Note: for nested-components the Parent discovering is mandatory (recursive nested-component); 
						                                      	// for the same reason you can't have more than one property of the type of the Parent component
						                                      	System.Type componentOwnerType = type;
						                                      	System.Type componentPropertyType = propertyType;

						                                      	IEnumerable<MemberInfo> componentProperties = GetPersistentProperties(componentPropertyType);

																										MemberInfo parentReferenceProperty = modelMapper.GetComponentParentReferenceProperty(componentProperties, componentOwnerType);
						                                      	if (parentReferenceProperty != null)
						                                      	{
						                                      		x.Parent(parentReferenceProperty);
						                                      	}
						                                      	customizersHolder.InvokeCustomizers(componentPropertyType, x);
						                                      	modelMapper.ForEachMemberPath(member, propertyPath, pp => customizersHolder.InvokeCustomizers(pp, x));
						                                      	modelMapper.InvokeAfterMapComponent(propertyPath, x);

						                                      	MapProperties(componentPropertyType, propertyPath, x, componentProperties.Where(pi => pi != parentReferenceProperty));
						                                      });
					}
					else
					{
						propertiesContainer.Property(member, propertyMapper =>
						                                     {
						                                     	modelMapper.InvokeBeforeMapProperty(propertyPath, propertyMapper);
						                                     	modelMapper.ForEachMemberPath(member, propertyPath, pp => customizersHolder.InvokeCustomizers(pp, propertyMapper));
						                                     	modelMapper.InvokeAfterMapProperty(propertyPath, propertyMapper);
						                                     });
					}
				}
			}
		}

		#endregion

		#region Nested type: ElementRelationMapper

		private class ElementRelationMapper : ICollectionElementRelationMapper
		{
			private readonly ICustomizersHolder customizersHolder;
			private readonly ModelMapper modelMapper;
			private readonly PropertyPath propertyPath;

			public ElementRelationMapper(PropertyPath propertyPath, ICustomizersHolder customizersHolder, ModelMapper modelMapper)
			{
				this.propertyPath = propertyPath;
				this.customizersHolder = customizersHolder;
				this.modelMapper = modelMapper;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.Element(x =>
				                 {
				                 	modelMapper.InvokeBeforeMapElement(propertyPath, x);
				                 	customizersHolder.InvokeCustomizers(propertyPath, x);
				                 	modelMapper.InvokeAfterMapElement(propertyPath, x);
				                 });
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped) {}

			#endregion
		}

		#endregion

		#region Nested type: ICollectionElementRelationMapper

		protected interface ICollectionElementRelationMapper
		{
			void Map(ICollectionElementRelation relation);
			void MapCollectionProperties(ICollectionPropertiesMapper mapped);
		}

		#endregion

		#region Nested type: IMapKeyRelationMapper

		protected interface IMapKeyRelationMapper
		{
			void Map(IMapKeyRelation relation);
		}

		#endregion

		#region Nested type: KeyComponentRelationMapper

		private class KeyComponentRelationMapper : IMapKeyRelationMapper
		{
			private readonly ICustomizersHolder customizersHolder;
			private readonly System.Type dictionaryKeyType;
			private readonly IModelInspector domainInspector;
			private readonly ICandidatePersistentMembersProvider membersProvider;
			private readonly ModelMapper modelMapper;
			private readonly PropertyPath propertyPath;

			public KeyComponentRelationMapper(System.Type dictionaryKeyType, PropertyPath propertyPath, ICandidatePersistentMembersProvider membersProvider, IModelInspector domainInspector,
			                                  ICustomizersHolder customizersHolder, ModelMapper modelMapper)
			{
				this.dictionaryKeyType = dictionaryKeyType;
				this.propertyPath = propertyPath;
				this.membersProvider = membersProvider;
				this.domainInspector = domainInspector;
				this.customizersHolder = customizersHolder;
				this.modelMapper = modelMapper;
			}

			#region IMapKeyRelationMapper Members

			public void Map(IMapKeyRelation relation)
			{
				relation.Component(x =>
				                   {
				                   	IEnumerable<MemberInfo> persistentProperties = GetPersistentProperties(dictionaryKeyType);

				                   	MapProperties(x, persistentProperties);
				                   });
			}

			#endregion

			private IEnumerable<MemberInfo> GetPersistentProperties(System.Type type)
			{
				IEnumerable<MemberInfo> properties = membersProvider.GetComponentMembers(type);
				return properties.Where(p => domainInspector.IsPersistentProperty(p));
			}

			private void MapProperties(IComponentMapKeyMapper propertiesContainer, IEnumerable<MemberInfo> persistentProperties)
			{
				foreach (MemberInfo property in persistentProperties)
				{
					MemberInfo member = property;
					if (domainInspector.IsManyToOne(member))
					{
						propertiesContainer.ManyToOne(member, manyToOneMapper =>
						                                      {
						                                      	var progressivePath = new PropertyPath(propertyPath, member);
						                                      	modelMapper.InvokeBeforeMapManyToOne(progressivePath, manyToOneMapper);
						                                      	modelMapper.ForEachMemberPath(member, progressivePath, pp => customizersHolder.InvokeCustomizers(pp, manyToOneMapper));
						                                      	modelMapper.InvokeAfterMapManyToOne(progressivePath, manyToOneMapper);
						                                      });
					}
					else
					{
						propertiesContainer.Property(member, propertyMapper =>
						                                     {
						                                     	var progressivePath = new PropertyPath(propertyPath, member);
						                                     	modelMapper.InvokeBeforeMapProperty(progressivePath, propertyMapper);
						                                     	modelMapper.ForEachMemberPath(member, progressivePath, pp => customizersHolder.InvokeCustomizers(pp, propertyMapper));
						                                     	modelMapper.InvokeAfterMapProperty(progressivePath, propertyMapper);
						                                     });
					}
				}
			}
		}

		#endregion

		#region Nested type: KeyElementRelationMapper

		private class KeyElementRelationMapper : IMapKeyRelationMapper
		{
			private readonly ICustomizersHolder customizersHolder;
			private readonly ModelMapper modelMapper;
			private readonly PropertyPath propertyPath;

			public KeyElementRelationMapper(PropertyPath propertyPath, ICustomizersHolder customizersHolder, ModelMapper modelMapper)
			{
				this.propertyPath = propertyPath;
				this.customizersHolder = customizersHolder;
				this.modelMapper = modelMapper;
			}

			#region IMapKeyRelationMapper Members

			public void Map(IMapKeyRelation relation)
			{
				relation.Element(x =>
				                 {
				                 	modelMapper.InvokeBeforeMapMapKey(propertyPath, x);
				                 	customizersHolder.InvokeCustomizers(propertyPath, x);
				                 	modelMapper.InvokeAfterMapMapKey(propertyPath, x);
				                 });
			}

			#endregion
		}

		#endregion

		#region Nested type: KeyManyToManyRelationMapper

		private class KeyManyToManyRelationMapper : IMapKeyRelationMapper
		{
			private readonly ICustomizersHolder customizersHolder;
			private readonly ModelMapper modelMapper;
			private readonly PropertyPath propertyPath;

			public KeyManyToManyRelationMapper(PropertyPath propertyPath, ICustomizersHolder customizersHolder, ModelMapper modelMapper)
			{
				this.propertyPath = propertyPath;
				this.customizersHolder = customizersHolder;
				this.modelMapper = modelMapper;
			}

			#region IMapKeyRelationMapper Members

			public void Map(IMapKeyRelation relation)
			{
				relation.ManyToMany(x =>
				                    {
				                    	modelMapper.InvokeBeforeMapMapKeyManyToMany(propertyPath, x);
				                    	customizersHolder.InvokeCustomizers(propertyPath, x);
				                    	modelMapper.InvokeAfterMapMapKeyManyToMany(propertyPath, x);
				                    });
			}

			#endregion
		}

		#endregion

		#region Nested type: ManyToManyRelationMapper

		private class ManyToManyRelationMapper : ICollectionElementRelationMapper
		{
			private readonly ICustomizersHolder customizersHolder;
			private readonly ModelMapper modelMapper;
			private readonly PropertyPath propertyPath;

			public ManyToManyRelationMapper(PropertyPath propertyPath, ICustomizersHolder customizersHolder, ModelMapper modelMapper)
			{
				this.propertyPath = propertyPath;
				this.customizersHolder = customizersHolder;
				this.modelMapper = modelMapper;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.ManyToMany(x =>
				                    {
				                    	modelMapper.InvokeBeforeMapManyToMany(propertyPath, x);
				                    	customizersHolder.InvokeCustomizers(propertyPath, x);
				                    	modelMapper.InvokeAfterMapManyToMany(propertyPath, x);
				                    });
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped) {}

			#endregion
		}

		#endregion

		#region Nested type: ManyToAnyRelationMapper

		private class ManyToAnyRelationMapper : ICollectionElementRelationMapper
		{
			private readonly ICustomizersHolder customizersHolder;
			private readonly ModelMapper modelMapper;
			private readonly PropertyPath propertyPath;

			public ManyToAnyRelationMapper(PropertyPath propertyPath, ICustomizersHolder customizersHolder, ModelMapper modelMapper)
			{
				this.propertyPath = propertyPath;
				this.customizersHolder = customizersHolder;
				this.modelMapper = modelMapper;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.ManyToAny(typeof(int), x => customizersHolder.InvokeCustomizers(propertyPath, x));
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped) { }

			#endregion
		}

		#endregion

		#region Nested type: OneToManyRelationMapper

		private class OneToManyRelationMapper : ICollectionElementRelationMapper
		{
			private const BindingFlags FlattenHierarchyBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
			private readonly System.Type collectionElementType;
			private readonly ICustomizersHolder customizersHolder;
			private readonly IModelInspector domainInspector;
			private readonly ModelMapper modelMapper;
			private readonly System.Type ownerType;
			private readonly PropertyPath propertyPath;

			public OneToManyRelationMapper(PropertyPath propertyPath, System.Type ownerType, System.Type collectionElementType, IModelInspector domainInspector,
			                               ICustomizersHolder customizersHolder, ModelMapper modelMapper)
			{
				this.propertyPath = propertyPath;
				this.ownerType = ownerType;
				this.collectionElementType = collectionElementType;
				this.domainInspector = domainInspector;
				this.customizersHolder = customizersHolder;
				this.modelMapper = modelMapper;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.OneToMany(x =>
				                   {
				                   	modelMapper.InvokeBeforeMapOneToMany(propertyPath, x);
				                   	customizersHolder.InvokeCustomizers(propertyPath, x);
				                   	modelMapper.InvokeAfterMapOneToMany(propertyPath, x);
				                   });
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped)
			{
				string parentColumnNameInChild = GetParentColumnNameInChild();
				if (parentColumnNameInChild != null)
				{
					mapped.Key(k => k.Column(parentColumnNameInChild));
				}
			}

			private string GetParentColumnNameInChild()
			{
				MemberInfo propertyInfo =
					collectionElementType.GetProperties(FlattenHierarchyBindingFlags).FirstOrDefault(p => p.PropertyType.IsAssignableFrom(ownerType) && domainInspector.IsPersistentProperty(p));

				if (propertyInfo != null)
				{
					return propertyInfo.Name;
				}
				return null;
			}

			#endregion
		}

		#endregion

		public void AddMapping<T>() where T: IConformistHoldersProvider, new()
		{
			AddMapping(new T());
		}

		public void AddMapping(IConformistHoldersProvider mapping)
		{
			var thisCustomizerHolder = customizerHolder as CustomizersHolder;
			if (thisCustomizerHolder == null)
			{
				throw new NotSupportedException("To merge 'conformist' mappings, the instance of ICustomizersHolder, provided in the ModelMapper constructor, have to be a CustomizersHolder instance.");
			}
			var otherCustomizerHolder = mapping.CustomizersHolder as CustomizersHolder;
			if (otherCustomizerHolder == null)
			{
				throw new NotSupportedException("The mapping class have to provide a CustomizersHolder instance.");
			}
			thisCustomizerHolder.Merge(otherCustomizerHolder);
			explicitDeclarationsHolder.Merge(mapping.ExplicitDeclarationsHolder);
		}

		public void AddMapping(System.Type type)
		{
			object mappingInstance;
			try
			{
				mappingInstance = Activator.CreateInstance(type);
			}
			catch (Exception e)
			{
				throw new MappingException("Unable to instantiate mapping class (see InnerException): " + type, e);
			}

			var mapping = mappingInstance as IConformistHoldersProvider;
			if(mapping == null)
			{
				throw new ArgumentOutOfRangeException("type", "The mapping class must be an implementation of IConformistHoldersProvider.");
			}
			AddMapping(mapping);
		}

		public void AddMappings(IEnumerable<System.Type> types)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			foreach (var type in types.Where(x=> typeof(IConformistHoldersProvider).IsAssignableFrom(x) && !x.IsGenericTypeDefinition))
			{
				AddMapping(type);
			}
		}

		public HbmMapping CompileMappingForAllExplicitlyAddedEntities()
		{
			return CompileMappingFor(customizerHolder.GetAllCustomizedEntities());
		}

		public IEnumerable<HbmMapping> CompileMappingForEachExplicitlyAddedEntity()
		{
			return CompileMappingForEach(customizerHolder.GetAllCustomizedEntities());
		}
	}
}