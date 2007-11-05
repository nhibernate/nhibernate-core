using System;
using System.Collections;
using Iesi.Collections;
using log4net;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Tuple
{
	[Serializable]
	public class EntityMetamodel
	{
		private static ILog log = LogManager.GetLogger(typeof(EntityMetamodel));

		private static int NoVersionIndex = -66;

		private ISessionFactoryImplementor sessionFactory;

		// DONE H3: ->these are stored as System.Types for now<-
		// store name and rootName
		private readonly string name;
		private readonly string rootName;
		private System.Type type;
		private System.Type rootType;
		private string rootTypeAssemblyQualifiedName;

		private EntityType entityType;

		private IdentifierProperty identifierProperty;
		private bool versioned;

		private int propertySpan;
		private int versionPropertyIndex;

		private StandardProperty[] properties;
		// temporary ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		private string[] propertyNames;
		private IType[] propertyTypes;
		private bool[] propertyLaziness;
		private bool[] propertyUpdateability;
		private bool[] nonlazyPropertyUpdateability;
		private bool[] propertyCheckability;
		private bool[] propertyInsertability;
		private ValueInclusion[] insertInclusions;
		private ValueInclusion[] updateInclusions;
		private bool[] propertyNullability;
		private bool[] propertyVersionability;
		private Cascades.CascadeStyle[] cascadeStyles;
		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		private IDictionary propertyIndexes = new Hashtable();
		private bool hasCollections;
		private bool hasMutableProperties;
		private bool hasLazyProperties;

		// TODO H3:
		//private int[] naturalIdPropertyNumbers;

		private bool lazy;
		private bool hasCascades;
		private bool mutable;
		private bool isAbstract;
		private bool selectBeforeUpdate;
		private bool dynamicUpdate;
		private bool dynamicInsert;
		private OptimisticLockMode optimisticLockMode;

		private bool polymorphic;
		// TODO H3: This is stored as System.Type currently
		//private string superclass;  // superclass entity-name
		private System.Type superclassType;

		private bool explicitPolymorphism;
		private bool inherited;
		private bool hasSubclasses;

		// TODO H3: These are stored as System.Types currently
		//private ISet subclassEntityNames = new HashedSet();
		private ISet subclassTypes = new HashedSet();
		private bool hasInsertGeneratedValues;
		private bool hasUpdateGeneratedValues;

		public EntityMetamodel(PersistentClass persistentClass, ISessionFactoryImplementor sessionFactory)
		{
			this.sessionFactory = sessionFactory;


			name = persistentClass.EntityName;
			rootName = persistentClass.RootClazz.EntityName;
			// TODO H3:
			//entityType = TypeFactory.manyToOne( name );
			type = persistentClass.MappedClass;
			rootType = persistentClass.RootClazz.MappedClass;
			rootTypeAssemblyQualifiedName = rootType.AssemblyQualifiedName;
			entityType = TypeFactory.ManyToOne(type);

			identifierProperty = PropertyFactory.BuildIdentifierProperty(
				persistentClass,
				sessionFactory.GetIdentifierGenerator(rootType)
				);

			versioned = persistentClass.IsVersioned;

			bool lazyAvailable = false;
			// TODO H3:
			//bool lazyAvailable = persistentClass.HasPojoRepresentation &&
			//	typeof(InterceptFieldEnabled).isAssignableFrom( persistentClass.getMappedClass() );
			bool hasLazy = false;

			propertySpan = persistentClass.PropertyClosureSpan;
			properties = new StandardProperty[propertySpan];
			//IList naturalIdNumbers = new ArrayList();

			// temporary ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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
			cascadeStyles = new Cascades.CascadeStyle[propertySpan];
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


			int i = 0;
			int tempVersionProperty = NoVersionIndex;
			bool foundCascade = false;
			bool foundCollection = false;
			bool foundMutable = false;
			bool foundInsertGeneratedValue = false;
			bool foundUpdateGeneratedValue = false;

			foreach (Mapping.Property prop in persistentClass.PropertyClosureCollection)
			{
				if (prop == persistentClass.Version)
				{
					tempVersionProperty = i;
					properties[i] = PropertyFactory.BuildVersionProperty(prop, lazyAvailable);
				}
				else
				{
					properties[i] = PropertyFactory.BuildStandardProperty(prop, lazyAvailable);
				}

//				if ( prop.IsNaturalIdentifier ) 
//				{
//					naturalIdNumbers.Add( i );
//				}

				// temporary ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				// TODO H3:
				//bool lazy = prop.IsLazy && lazyAvailable;
				bool lazyProperty = false;
				if (lazyProperty) hasLazy = true;
				propertyLaziness[i] = lazyProperty;

				propertyNames[i] = properties[i].Name;
				propertyTypes[i] = properties[i].Type;
				propertyNullability[i] = properties[i].IsNullable;
				propertyUpdateability[i] = properties[i].IsUpdateable;
				propertyInsertability[i] = properties[i].IsInsertable;
				insertInclusions[i] = DetermineInsertValueGenerationType(prop, properties[i]);
				updateInclusions[i] = DetermineUpdateValueGenerationType(prop, properties[i]);
				propertyVersionability[i] = properties[i].IsVersionable;
				nonlazyPropertyUpdateability[i] = properties[i].IsUpdateable && !lazyProperty;
				propertyCheckability[i] = propertyUpdateability[i] ||
				                          (propertyTypes[i].IsAssociationType &&
				                           ((IAssociationType) propertyTypes[i]).IsAlwaysDirtyChecked);

				cascadeStyles[i] = properties[i].CascadeStyle;
				// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

				if (properties[i].IsLazy)
				{
					hasLazy = true;
				}

				if (properties[i].CascadeStyle != Cascades.CascadeStyle.StyleNone)
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

			// TODO H3:
//			if( naturalIdNumbers.Count == 0 )
//			{
//				naturalIdPropertyNumbers = null;
//			}
//			else 
//			{
//				naturalIdPropertyNumbers = ArrayHelper.ToIntArray( naturalIdNumbers );
//			}

			hasCascades = foundCascade;
			hasInsertGeneratedValues = foundInsertGeneratedValue;
			hasUpdateGeneratedValues = foundUpdateGeneratedValue;
			versionPropertyIndex = tempVersionProperty;
			hasLazyProperties = hasLazy;
			if (hasLazyProperties) log.Info("lazy property fetching available for: " + type.FullName);

			lazy = persistentClass.IsLazy;
			// TODO H3:
//			lazy = persistentClass.IsLazy && (
//				// TODO: this disables laziness even in non-pojo entity modes:
//				!persistentClass.HasPojoRepresentation ||
//				!ReflectHelper.IsFinalClass( persistentClass.ProxyInterface )
//				);
			mutable = persistentClass.IsMutable;

			if (!persistentClass.IsAbstract.HasValue)
			{
				// legacy behavior (with no abstract attribute specified)
				isAbstract = persistentClass.HasPojoRepresentation &&
				             ReflectHelper.IsAbstractClass(persistentClass.MappedClass);
			}
			else
			{
				isAbstract = persistentClass.IsAbstract.Value;
				if (!isAbstract && persistentClass.HasPojoRepresentation &&
				    ReflectHelper.IsAbstractClass(persistentClass.MappedClass))
				{
					log.Warn("entity [" + type.FullName +
					         "] is abstract-class/interface explicitly mapped as non-abstract; be sure to supply entity-names");
				}
			}
			selectBeforeUpdate = persistentClass.SelectBeforeUpdate;
			dynamicUpdate = persistentClass.DynamicUpdate;
			dynamicInsert = persistentClass.DynamicInsert;

			polymorphic = persistentClass.IsPolymorphic;
			explicitPolymorphism = persistentClass.IsExplicitPolymorphism;
			inherited = persistentClass.IsInherited;
			superclassType = inherited ?
			                 persistentClass.Superclass.MappedClass :
			                 null;
			hasSubclasses = persistentClass.HasSubclasses;

			optimisticLockMode = persistentClass.OptimisticLockMode;
			if (optimisticLockMode > OptimisticLockMode.Version && !dynamicUpdate)
			{
				throw new MappingException("optimistic-lock setting requires dynamic-update=\"true\": " + type.FullName);
			}

			hasCollections = foundCollection;
			hasMutableProperties = foundMutable;

			// TODO H3: tuplizers = TuplizerLookup.create(persistentClass, this);

			foreach (PersistentClass obj in persistentClass.SubclassCollection)
			{
				// TODO H3: subclassEntityNames.Add( obj.EntityName );
				subclassTypes.Add(obj.MappedClass);
			}
			// TODO H3: subclassEntityNames.Add( name );
			subclassTypes.Add(type);
		}

		private ValueInclusion DetermineInsertValueGenerationType(Mapping.Property mappingProperty, StandardProperty runtimeProperty)
		{
			if (runtimeProperty.IsInsertGenerated)
			{
				return ValueInclusion.Full;
			}
			else if (mappingProperty.Value is Component)
			{
				if (HasPartialInsertComponentGeneration((Component) mappingProperty.Value))
				{
					return ValueInclusion.Partial;
				}
			}
			return ValueInclusion.None;
		}

		private bool HasPartialInsertComponentGeneration(Component component)
		{
			foreach (Mapping.Property prop in component.PropertyCollection)
			{
				if (prop.Generation == PropertyGeneration.Always || prop.Generation == PropertyGeneration.Insert)
				{
					return true;
				}
				else if (prop.Value is Component)
				{
					if (HasPartialInsertComponentGeneration((Component) prop.Value))
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
			else if (mappingProperty.Value is Component)
			{
				if (HasPartialUpdateComponentGeneration((Component) mappingProperty.Value))
				{
					return ValueInclusion.Partial;
				}
			}
			return ValueInclusion.None;
		}

		private bool HasPartialUpdateComponentGeneration(Component component)
		{
			foreach (Mapping.Property prop in component.PropertyCollection)
			{
				if (prop.Generation == PropertyGeneration.Always)
				{
					return true;
				}
				else if (prop.Value is Component)
				{
					if (HasPartialUpdateComponentGeneration((Component) prop.Value))
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
			if (prop.Value is Component)
			{
				foreach (Mapping.Property subprop in ((Component) prop.Value).PropertyCollection)
				{
					propertyIndexes[prop.Name + '.' + subprop.Name] = i;
				}
			}
		}

		// TODO H3:
//		public int[] NaturalIdentifierProperties 
//		{
//			get { return naturalIdPropertyNumbers; }
//		}
//	
//		public bool HasNaturalIdentifier
//		{
//			get { return naturalIdPropertyNumbers != null; }
//		}
//	
//		public ISet SubclassEntityNames
//		{
//			get { return subclassEntityNames; }
//		}

		public ISet SubclassTypes
		{
			get { return subclassTypes; }
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

		/*
		public string getName() 
		{
			return name;
		}

		public string getRootName() 
		{
			return rootName;
		}
		*/

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
			object index = GetPropertyIndexOrNull(propertyName);
			if (index == null)
			{
				throw new HibernateException("Unable to resolve property: " + propertyName);
			}
			return (int) index;
		}

		public object GetPropertyIndexOrNull(string propertyName)
		{
			return propertyIndexes[propertyName];
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

		public OptimisticLockMode OptimisticLockMode
		{
			get { return optimisticLockMode; }
		}

		public bool IsPolymorphic
		{
			get { return polymorphic; }
		}

		// TODO H3:
//		public string Superclass
//		{
//			get { return superclass; }
//		}

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

		// temporary ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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

		public Cascades.CascadeStyle[] CascadeStyles
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

		// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	}
}
