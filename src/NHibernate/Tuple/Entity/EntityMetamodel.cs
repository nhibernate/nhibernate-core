using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;
using Array = NHibernate.Mapping.Array;

namespace NHibernate.Tuple.Entity
{
	[Serializable]
	public class EntityMetamodel
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(EntityMetamodel));

		private const int NoVersionIndex = -66;

		private readonly ISessionFactoryImplementor sessionFactory;

		private readonly string name;
		private readonly string rootName;
		private readonly System.Type type;
		private readonly System.Type rootType;
		private readonly string rootTypeAssemblyQualifiedName;

		private readonly EntityType entityType;

		private readonly IdentifierProperty identifierProperty;
		private readonly bool versioned;

		private readonly int propertySpan;
		private readonly int versionPropertyIndex;

		private readonly StandardProperty[] properties;

		private readonly string[] propertyNames;
		private readonly IType[] propertyTypes;
		private readonly bool[] propertyLaziness;
		private readonly bool[] propertyUpdateability;
		private readonly bool[] nonlazyPropertyUpdateability;
		private readonly bool[] propertyCheckability;
		private readonly bool[] propertyInsertability;
		private readonly ValueInclusion[] insertInclusions;
		private readonly ValueInclusion[] updateInclusions;
		private readonly bool[] propertyNullability;
		private readonly bool[] propertyVersionability;
		private readonly CascadeStyle[] cascadeStyles;

		private readonly IDictionary<string, int?> propertyIndexes = new Dictionary<string, int?>();
		private readonly bool hasCollections;
		private readonly bool hasMutableProperties;
		private readonly bool hasLazyProperties;


		private readonly int[] naturalIdPropertyNumbers;

		private bool lazy;
		private readonly bool hasCascades;
		private readonly bool hasNonIdentifierPropertyNamedId;
		private readonly bool mutable;
		private readonly bool isAbstract;
		private readonly bool selectBeforeUpdate;
		private readonly bool dynamicUpdate;
		private readonly bool dynamicInsert;
		private readonly Versioning.OptimisticLock optimisticLockMode;

		private readonly bool polymorphic;
		private readonly string superclass;
		private readonly System.Type superclassType;

		private readonly bool explicitPolymorphism;
		private readonly bool inherited;
		private readonly bool hasSubclasses;

		private readonly HashSet<string> subclassEntityNames = new HashSet<string>();
		private readonly bool hasInsertGeneratedValues;
		private readonly bool hasUpdateGeneratedValues;

		public EntityMetamodel(PersistentClass persistentClass, ISessionFactoryImplementor sessionFactory)
		{
			this.sessionFactory = sessionFactory;


			name = persistentClass.EntityName;
			rootName = persistentClass.RootClazz.EntityName;
			entityType = TypeFactory.ManyToOne(name);
			type = persistentClass.MappedClass;
			rootType = persistentClass.RootClazz.MappedClass;
			rootTypeAssemblyQualifiedName = rootType == null ? null : rootType.AssemblyQualifiedName;

			identifierProperty = PropertyFactory.BuildIdentifierProperty(persistentClass,
			                                                             sessionFactory.GetIdentifierGenerator(rootName));

			versioned = persistentClass.IsVersioned;

			bool lazyAvailable = persistentClass.HasPocoRepresentation
			                     && FieldInterceptionHelper.IsInstrumented(persistentClass.MappedClass);
			bool hasLazy = false;

			propertySpan = persistentClass.PropertyClosureSpan;
			properties = new StandardProperty[propertySpan];
			List<int> naturalIdNumbers = new List<int>();

			propertyNames = new string[propertySpan];
			propertyTypes = new IType[propertySpan];
			propertyUpdateability = new bool[propertySpan];
			propertyInsertability = new bool[propertySpan];
			insertInclusions = new ValueInclusion[propertySpan];
			updateInclusions = new ValueInclusion[propertySpan];
			nonlazyPropertyUpdateability = new bool[propertySpan];
			propertyCheckability = new bool[propertySpan];
			propertyNullability = new bool[propertySpan];
			propertyVersionability = new bool[propertySpan];
			propertyLaziness = new bool[propertySpan];
			cascadeStyles = new CascadeStyle[propertySpan];

			int i = 0;
			int tempVersionProperty = NoVersionIndex;
			bool foundCascade = false;
			bool foundCollection = false;
			bool foundMutable = false;
			bool foundInsertGeneratedValue = false;
			bool foundUpdateGeneratedValue = false;
			bool foundNonIdentifierPropertyNamedId = false;
			HasPocoRepresentation = persistentClass.HasPocoRepresentation;
			
			// NH: WARNING if we have to disable lazy/unproxy properties we have to do it in the whole process.
			lazy = persistentClass.IsLazy && (!persistentClass.HasPocoRepresentation || !ReflectHelper.IsFinalClass(persistentClass.ProxyInterface));
			lazyAvailable &= lazy; // <== Disable lazy properties if the class is marked with lazy=false

			bool hadLazyProperties = false;
			bool hadNoProxyRelations = false;
			foreach (Mapping.Property prop in persistentClass.PropertyClosureIterator)
			{
				if (prop.IsLazy)
				{
					hadLazyProperties = true;
				}
				if(prop.UnwrapProxy)
				{
					hadNoProxyRelations = true;
				}

				// NH: A lazy property is a simple property marked with lazy=true
				bool islazyProperty = prop.IsLazy && lazyAvailable && (!prop.IsEntityRelation || prop.UnwrapProxy);
				// NH: A Relation (in this case many-to-one or one-to-one) marked as "no-proxy"
				var isUnwrapProxy = prop.UnwrapProxy && lazyAvailable;

				if (islazyProperty || isUnwrapProxy)
				{
					// NH: verify property proxiability
					var getter = prop.GetGetter(persistentClass.MappedClass);
					if (getter.Method == null || getter.Method.IsDefined(typeof(CompilerGeneratedAttribute), false) == false)
					{
						log.ErrorFormat("Lazy or no-proxy property {0}.{1} is not an auto property, which may result in uninitialized property access", persistentClass.EntityName, prop.Name);
					}
				}

				if (prop == persistentClass.Version)
				{
					tempVersionProperty = i;
					properties[i] = PropertyFactory.BuildVersionProperty(prop, islazyProperty);
				}
				else
				{
					properties[i] = PropertyFactory.BuildStandardProperty(prop, islazyProperty);
				}

				if (prop.IsNaturalIdentifier)
				{
					naturalIdNumbers.Add(i);
				}

				if ("id".Equals(prop.Name))
				{
					foundNonIdentifierPropertyNamedId = true;
				}

				if (islazyProperty)
				{
					hasLazy = true;
				}
				if (isUnwrapProxy)
				{
					hasUnwrapProxyForProperties = true;
				}

				propertyLaziness[i] = islazyProperty;

				propertyNames[i] = properties[i].Name;
				propertyTypes[i] = properties[i].Type;
				propertyNullability[i] = properties[i].IsNullable;
				propertyUpdateability[i] = properties[i].IsUpdateable;
				propertyInsertability[i] = properties[i].IsInsertable;
				insertInclusions[i] = DetermineInsertValueGenerationType(prop, properties[i]);
				updateInclusions[i] = DetermineUpdateValueGenerationType(prop, properties[i]);
				propertyVersionability[i] = properties[i].IsVersionable;
				nonlazyPropertyUpdateability[i] = properties[i].IsUpdateable && !islazyProperty;
				propertyCheckability[i] = propertyUpdateability[i]
				                          ||
				                          (propertyTypes[i].IsAssociationType
				                           && ((IAssociationType) propertyTypes[i]).IsAlwaysDirtyChecked);

				cascadeStyles[i] = properties[i].CascadeStyle;

				if (properties[i].IsLazy)
				{
					hasLazy = true;
				}

				if (properties[i].CascadeStyle != CascadeStyle.None)
				{
					foundCascade = true;
				}

				if (IndicatesCollection(properties[i].Type))
				{
					foundCollection = true;
				}

				if (propertyTypes[i].IsMutable && propertyCheckability[i])
				{
					foundMutable = true;
				}

				if (insertInclusions[i] != ValueInclusion.None)
				{
					foundInsertGeneratedValue = true;
				}

				if (updateInclusions[i] != ValueInclusion.None)
				{
					foundUpdateGeneratedValue = true;
				}

				MapPropertyToIndex(prop, i);
				i++;
			}

			if (naturalIdNumbers.Count == 0)
				naturalIdPropertyNumbers = null;
			else
				naturalIdPropertyNumbers = naturalIdNumbers.ToArray();

			hasCascades = foundCascade;
			hasInsertGeneratedValues = foundInsertGeneratedValue;
			hasUpdateGeneratedValues = foundUpdateGeneratedValue;
			hasNonIdentifierPropertyNamedId = foundNonIdentifierPropertyNamedId;

			versionPropertyIndex = tempVersionProperty;
			hasLazyProperties = hasLazy;

			if(hadLazyProperties && !hasLazy)
			{
				log.WarnFormat("Disabled lazy property fetching for {0} because it does not support lazy at the entity level", name);
			}
			if (hasLazy)
			{
				log.Info("lazy property fetching available for: " + name);
			}

			if(hadNoProxyRelations && !hasUnwrapProxyForProperties)
			{
				log.WarnFormat("Disabled ghost property fetching for {0} because it does not support lazy at the entity level", name);
			}
			if (hasUnwrapProxyForProperties)
			{
				log.Info("no-proxy property fetching available for: " + name);
			}

			mutable = persistentClass.IsMutable;

			if (!persistentClass.IsAbstract.HasValue)
			{
				// legacy behavior (with no abstract attribute specified)
				isAbstract = persistentClass.HasPocoRepresentation && ReflectHelper.IsAbstractClass(persistentClass.MappedClass);
			}
			else
			{
				isAbstract = persistentClass.IsAbstract.Value;
				if (!isAbstract && persistentClass.HasPocoRepresentation
				    && ReflectHelper.IsAbstractClass(persistentClass.MappedClass))
				{
					log.Warn("entity [" + type.FullName
					         + "] is abstract-class/interface explicitly mapped as non-abstract; be sure to supply entity-names");
				}
			}
			selectBeforeUpdate = persistentClass.SelectBeforeUpdate;
			dynamicUpdate = persistentClass.DynamicUpdate;
			dynamicInsert = persistentClass.DynamicInsert;

			polymorphic = persistentClass.IsPolymorphic;
			explicitPolymorphism = persistentClass.IsExplicitPolymorphism;
			inherited = persistentClass.IsInherited;
			superclass = inherited ? persistentClass.Superclass.EntityName : null;
			superclassType = inherited ? persistentClass.Superclass.MappedClass : null;
			hasSubclasses = persistentClass.HasSubclasses;

			optimisticLockMode = persistentClass.OptimisticLockMode;
			if (optimisticLockMode > Versioning.OptimisticLock.Version && !dynamicUpdate)
			{
				throw new MappingException("optimistic-lock setting requires dynamic-update=\"true\": " + type.FullName);
			}

			hasCollections = foundCollection;
			hasMutableProperties = foundMutable;

			foreach (Subclass obj in persistentClass.SubclassIterator)
			{
				subclassEntityNames.Add(obj.EntityName);
			}
			subclassEntityNames.Add(name);

			tuplizerMapping = new EntityEntityModeToTuplizerMapping(persistentClass, this);
		}

		public bool HasPocoRepresentation { get; private set; }

		private ValueInclusion DetermineInsertValueGenerationType(Mapping.Property mappingProperty, StandardProperty runtimeProperty)
		{
			if (runtimeProperty.IsInsertGenerated)
			{
				return ValueInclusion.Full;
			}
			else if (mappingProperty.Value is Mapping.Component)
			{
				if (HasPartialInsertComponentGeneration((Mapping.Component)mappingProperty.Value))
				{
					return ValueInclusion.Partial;
				}
			}
			return ValueInclusion.None;
		}

		private bool HasPartialInsertComponentGeneration(Mapping.Component component)
		{
			foreach (Mapping.Property prop in component.PropertyIterator)
			{
				if (prop.Generation == PropertyGeneration.Always || prop.Generation == PropertyGeneration.Insert)
				{
					return true;
				}
				else if (prop.Value is Mapping.Component)
				{
					if (HasPartialInsertComponentGeneration((Mapping.Component)prop.Value))
					{
						return true;
					}
				}
			}
			return false;
		}

		private ValueInclusion DetermineUpdateValueGenerationType(Mapping.Property mappingProperty, StandardProperty runtimeProperty)
		{
			if (runtimeProperty.IsUpdateGenerated)
			{
				return ValueInclusion.Full;
			}
			else if (mappingProperty.Value is Mapping.Component)
			{
				if (HasPartialUpdateComponentGeneration((Mapping.Component)mappingProperty.Value))
				{
					return ValueInclusion.Partial;
				}
			}
			return ValueInclusion.None;
		}

		private bool HasPartialUpdateComponentGeneration(Mapping.Component component)
		{
			foreach (Mapping.Property prop in component.PropertyIterator)
			{
				if (prop.Generation == PropertyGeneration.Always)
				{
					return true;
				}
				else if (prop.Value is Mapping.Component)
				{
					if (HasPartialUpdateComponentGeneration((Mapping.Component)prop.Value))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void MapPropertyToIndex(Mapping.Property prop, int i)
		{
			propertyIndexes[prop.Name] = i;
			Mapping.Component comp = prop.Value as Mapping.Component;
			if (comp != null)
			{
				foreach (Mapping.Property subprop in comp.PropertyIterator)
				{
					propertyIndexes[prop.Name + '.' + subprop.Name] = i;
				}
			}
		}

		public ISet<string> SubclassEntityNames
		{
			get { return subclassEntityNames; }
		}

		private bool IndicatesCollection(IType type)
		{
			if (type.IsCollectionType)
			{
				return true;
			}
			else if (type.IsComponentType)
			{
				IType[] subtypes = ((IAbstractComponentType) type).Subtypes;
				for (int i = 0; i < subtypes.Length; i++)
				{
					if (IndicatesCollection(subtypes[i]))
					{
						return true;
					}
				}
			}
			return false;
		}

		public ISessionFactoryImplementor SessionFactory
		{
			get { return sessionFactory; }
		}

		public System.Type Type
		{
			get { return type; }
		}

		public System.Type RootType
		{
			get { return rootType; }
		}

		public string RootTypeAssemblyQualifiedName
		{
			get { return rootTypeAssemblyQualifiedName; }
		}

		public string Name
		{
			get { return name; }
		}

		public string RootName
		{
			get { return rootName; }
		}

		public EntityType EntityType
		{
			get { return entityType; }
		}

		public IdentifierProperty IdentifierProperty
		{
			get { return identifierProperty; }
		}

		public int PropertySpan
		{
			get { return propertySpan; }
		}

		public int VersionPropertyIndex
		{
			get { return versionPropertyIndex; }
		}

		public VersionProperty VersionProperty
		{
			get
			{
				if (NoVersionIndex == versionPropertyIndex)
				{
					return null;
				}
				else
				{
					return (VersionProperty) properties[versionPropertyIndex];
				}
			}
		}

		public StandardProperty[] Properties
		{
			get { return properties; }
		}

		public int GetPropertyIndex(string propertyName)
		{
			int? index = GetPropertyIndexOrNull(propertyName);
			if (!index.HasValue)
			{
				throw new HibernateException("Unable to resolve property: " + propertyName);
			}
			return index.Value;
		}

		public int? GetPropertyIndexOrNull(string propertyName)
		{
			int? result;
			if (propertyIndexes.TryGetValue(propertyName, out result))
				return result;
			else
				return null;
		}

		public bool HasCollections
		{
			get { return hasCollections; }
		}

		public bool HasMutableProperties
		{
			get { return hasMutableProperties; }
		}

		public bool HasLazyProperties
		{
			get { return hasLazyProperties; }
		}

		public bool HasCascades
		{
			get { return hasCascades; }
		}

		public bool IsMutable
		{
			get { return mutable; }
		}

		public bool IsSelectBeforeUpdate
		{
			get { return selectBeforeUpdate; }
		}

		public bool IsDynamicUpdate
		{
			get { return dynamicUpdate; }
		}

		public bool IsDynamicInsert
		{
			get { return dynamicInsert; }
		}

		public Versioning.OptimisticLock OptimisticLockMode
		{
			get { return optimisticLockMode; }
		}

		public bool IsPolymorphic
		{
			get { return polymorphic; }
		}

		public string Superclass
		{
			get { return superclass; }
		}

		public System.Type SuperclassType
		{
			get { return superclassType; }
		}

		public bool IsExplicitPolymorphism
		{
			get { return explicitPolymorphism; }
		}

		public bool IsInherited
		{
			get { return inherited; }
		}

		public bool HasSubclasses
		{
			get { return hasSubclasses; }
		}

		public bool IsLazy
		{
			get { return lazy; }
			set { lazy = value; }
		}

		public bool IsVersioned
		{
			get { return versioned; }
		}

		public bool IsAbstract
		{
			get { return isAbstract; }
		}

		public override string ToString()
		{
			return "EntityMetamodel(" + type.FullName + ':' + ArrayHelper.ToString(properties) + ')';
		}

		#region temporary
		public string[] PropertyNames
		{
			get { return propertyNames; }
		}

		public IType[] PropertyTypes
		{
			get { return propertyTypes; }
		}

		public bool[] PropertyLaziness
		{
			get { return propertyLaziness; }
		}

		public bool[] PropertyUpdateability
		{
			get { return propertyUpdateability; }
		}

		public bool[] PropertyCheckability
		{
			get { return propertyCheckability; }
		}

		public bool[] NonlazyPropertyUpdateability
		{
			get { return nonlazyPropertyUpdateability; }
		}

		public bool[] PropertyInsertability
		{
			get { return propertyInsertability; }
		}

		public bool[] PropertyNullability
		{
			get { return propertyNullability; }
		}

		public bool[] PropertyVersionability
		{
			get { return propertyVersionability; }
		}

		public CascadeStyle[] CascadeStyles
		{
			get { return cascadeStyles; }
		}

		public ValueInclusion[] PropertyInsertGenerationInclusions
		{
			get { return insertInclusions; }
		}

		public ValueInclusion[] PropertyUpdateGenerationInclusions
		{
			get { return updateInclusions; }
		}

		public bool HasInsertGeneratedValues
		{
			get { return hasInsertGeneratedValues; }
		}

		public bool HasUpdateGeneratedValues
		{
			get { return hasUpdateGeneratedValues; }
		}

		#endregion

		#region Tuplizer
		private readonly EntityEntityModeToTuplizerMapping tuplizerMapping;
		private bool hasUnwrapProxyForProperties;

		public IEntityTuplizer GetTuplizer(EntityMode entityMode)
		{
			return (IEntityTuplizer)tuplizerMapping.GetTuplizer(entityMode);
		}

		public IEntityTuplizer GetTuplizerOrNull(EntityMode entityMode)
		{
			return (IEntityTuplizer)tuplizerMapping.GetTuplizerOrNull(entityMode);
		}

		public EntityMode? GuessEntityMode(object obj)
		{
			return tuplizerMapping.GuessEntityMode(obj);
		}
		#endregion

		public bool HasNaturalIdentifier
		{
			get { return naturalIdPropertyNumbers != null; }
		}

		public bool HasUnwrapProxyForProperties
		{
			get { return hasUnwrapProxyForProperties; }
		}

		public bool HasNonIdentifierPropertyNamedId
		{
			get { return hasNonIdentifierPropertyNamedId; }
		}

		public int[] NaturalIdentifierProperties
		{
			get { return naturalIdPropertyNumbers; }
		}
	}
}
