using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public class ExplicitlyDeclaredModel : IModelInspector, IModelExplicitDeclarationsHolder
	{
		private readonly HashSet<MemberInfo> any = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> arrays = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> bags = new HashSet<MemberInfo>();
		private readonly HashSet<System.Type> components = new HashSet<System.Type>();
		private readonly HashSet<MemberInfo> dictionaries = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> idBags = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> lists = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> manyToManyRelations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> manyToOneRelations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> naturalIds = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> oneToManyRelations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> oneToOneRelations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> persistentProperties = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> poids = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> properties = new HashSet<MemberInfo>();
		private readonly HashSet<System.Type> rootEntities = new HashSet<System.Type>();
		private readonly HashSet<MemberInfo> sets = new HashSet<MemberInfo>();
		private readonly HashSet<System.Type> tablePerClassEntities = new HashSet<System.Type>();
		private readonly HashSet<System.Type> tablePerClassHierarchyEntities = new HashSet<System.Type>();
		private readonly HashSet<System.Type> tablePerClassHierarchyJoinEntities = new HashSet<System.Type>();
		private readonly HashSet<System.Type> tablePerConcreteClassEntities = new HashSet<System.Type>();
		private readonly HashSet<MemberInfo> versionProperties = new HashSet<MemberInfo>();

		#region IModelExplicitDeclarationsHolder Members

		public IEnumerable<System.Type> RootEntities
		{
			get { return rootEntities; }
		}

		public IEnumerable<System.Type> Components
		{
			get { return components; }
		}

		public IEnumerable<System.Type> TablePerClassEntities
		{
			get { return tablePerClassEntities; }
		}

		public IEnumerable<System.Type> TablePerClassHierarchyEntities
		{
			get { return tablePerClassHierarchyEntities; }
		}

		public IEnumerable<System.Type> TablePerClassHierarchyJoinEntities
		{
			get { return tablePerClassHierarchyJoinEntities; }
		}

		public IEnumerable<System.Type> TablePerConcreteClassEntities
		{
			get { return tablePerConcreteClassEntities; }
		}

		public IEnumerable<MemberInfo> OneToOneRelations
		{
			get { return oneToOneRelations; }
		}

		public IEnumerable<MemberInfo> ManyToOneRelations
		{
			get { return manyToOneRelations; }
		}

		public IEnumerable<MemberInfo> ManyToManyRelations
		{
			get { return manyToManyRelations; }
		}

		public IEnumerable<MemberInfo> OneToManyRelations
		{
			get { return oneToManyRelations; }
		}

		public IEnumerable<MemberInfo> Any
		{
			get { return any; }
		}

		public IEnumerable<MemberInfo> Poids
		{
			get { return poids; }
		}

		public IEnumerable<MemberInfo> VersionProperties
		{
			get { return versionProperties; }
		}

		public IEnumerable<MemberInfo> NaturalIds
		{
			get { return naturalIds; }
		}

		public IEnumerable<MemberInfo> Sets
		{
			get { return sets; }
		}

		public IEnumerable<MemberInfo> Bags
		{
			get { return bags; }
		}

		public IEnumerable<MemberInfo> IdBags
		{
			get { return idBags; }
		}

		public IEnumerable<MemberInfo> Lists
		{
			get { return lists; }
		}

		public IEnumerable<MemberInfo> Arrays
		{
			get { return arrays; }
		}

		public IEnumerable<MemberInfo> Dictionaries
		{
			get { return dictionaries; }
		}

		public IEnumerable<MemberInfo> Properties
		{
			get { return properties; }
		}

		public void AddAsRootEntity(System.Type type)
		{
			rootEntities.Add(type);
		}

		public void AddAsComponent(System.Type type)
		{
			components.Add(type);
		}

		public void AddAsTablePerClassEntity(System.Type type)
		{
			var rootEntity = GetRootEntityOrNull(type);
			if(rootEntity != null)
			{
				if(rootEntity.Equals(type))
				{
					throw new MappingException(string.Format("Abiguous mapping of {0}. It was registered as root-entity and as subclass for table-per-class strategy", type.FullName));
				}
				tablePerClassEntities.Add(rootEntity);
			}
		}

		public void AddAsTablePerClassHierarchyEntity(System.Type type)
		{
			var rootEntity = GetRootEntityOrNull(type);
			if (rootEntity != null)
			{
				if (rootEntity.Equals(type))
				{
					throw new MappingException(string.Format("Abiguous mapping of {0}. It was registered as root-entity and as subclass for table-per-class-hierarchy strategy", type.FullName));
				}
				if(IsMappedFor(tablePerClassEntities, type))
				{
					throw new MappingException(string.Format("Abiguous mapping of {0}. It was registered with more than one class-hierarchy strategy", type.FullName));
				}
				tablePerClassHierarchyEntities.Add(rootEntity);
			}
		}

		public void AddAsTablePerClassHierarchyJoinEntity(System.Type type)
		{
			var rootEntity = GetRootEntityOrNull(type);
			if (rootEntity != null)
			{
				if (rootEntity.Equals(type))
				{
					throw new MappingException(string.Format("Abiguous mapping of {0}. It was registered as root-entity and as subclass for table-per-class-hierarchy strategy", type.FullName));
				}
				if (IsMappedFor(tablePerClassEntities, type))
				{
					throw new MappingException(string.Format("Abiguous mapping of {0}. It was registered with more than one class-hierarchy strategy", type.FullName));
				}
				tablePerClassHierarchyEntities.Add(rootEntity);
			}
			tablePerClassHierarchyJoinEntities.Add(type);
		}

		public void AddAsTablePerConcreteClassEntity(System.Type type)
		{
			var rootEntity = GetRootEntityOrNull(type);
			if (rootEntity != null)
			{
				if (rootEntity.Equals(type))
				{
					throw new MappingException(string.Format("Abiguous mapping of {0}. It was registered as root-entity and as subclass for table-per-concrete-class strategy", type.FullName));
				}
				if (IsMappedFor(tablePerClassEntities, type))
				{
					throw new MappingException(string.Format("Abiguous mapping of {0}. It was registered with more than one class-hierarchy strategy", type.FullName));
				}
				tablePerConcreteClassEntities.Add(rootEntity);
			}
		}

		public void AddAsOneToOneRelation(MemberInfo member)
		{
			persistentProperties.Add(member);
			oneToOneRelations.Add(member);
		}

		public void AddAsManyToOneRelation(MemberInfo member)
		{
			persistentProperties.Add(member);
			manyToOneRelations.Add(member);
		}

		public void AddAsManyToManyRelation(MemberInfo member)
		{
			persistentProperties.Add(member);
			manyToManyRelations.Add(member);
		}

		public void AddAsOneToManyRelation(MemberInfo member)
		{
			persistentProperties.Add(member);
			oneToManyRelations.Add(member);
		}

		public void AddAsAny(MemberInfo member)
		{
			persistentProperties.Add(member);
			any.Add(member);
		}

		public void AddAsPoid(MemberInfo member)
		{
			persistentProperties.Add(member);
			poids.Add(member);
		}

		public void AddAsVersionProperty(MemberInfo member)
		{
			persistentProperties.Add(member);
			versionProperties.Add(member);
		}

		public void AddAsNaturalId(MemberInfo member)
		{
			persistentProperties.Add(member);
			naturalIds.Add(member);
		}

		public void AddAsSet(MemberInfo member)
		{
			persistentProperties.Add(member);
			sets.Add(member);
		}

		public void AddAsBag(MemberInfo member)
		{
			persistentProperties.Add(member);
			bags.Add(member);
		}

		public void AddAsIdBag(MemberInfo member)
		{
			persistentProperties.Add(member);
			idBags.Add(member);
		}

		public void AddAsList(MemberInfo member)
		{
			persistentProperties.Add(member);
			lists.Add(member);
		}

		public void AddAsArray(MemberInfo member)
		{
			persistentProperties.Add(member);
			arrays.Add(member);
		}

		public void AddAsMap(MemberInfo member)
		{
			persistentProperties.Add(member);
			dictionaries.Add(member);
		}

		public void AddAsProperty(MemberInfo member)
		{
			persistentProperties.Add(member);
			properties.Add(member);
		}

		#endregion

		#region Implementation of IModelInspector

		public bool IsRootEntity(System.Type type)
		{
			return rootEntities.Contains(type);
		}

		public bool IsComponent(System.Type type)
		{
			return components.Contains(type);
		}

		public bool IsEntity(System.Type type)
		{
			return rootEntities.Contains(type) ||
			       tablePerClassEntities.Contains(type) ||
			       tablePerClassHierarchyEntities.Contains(type) ||
			       tablePerClassHierarchyJoinEntities.Contains(type) ||
			       tablePerConcreteClassEntities.Contains(type);
		}

		public bool IsTablePerClass(System.Type type)
		{
			return IsMappedFor(tablePerClassEntities, type);
		}

		public bool IsTablePerClassHierarchy(System.Type type)
		{
			return tablePerClassHierarchyEntities.Contains(type);
		}

		public bool IsTablePerClassHierarchyJoin(System.Type type)
		{
			return tablePerClassHierarchyJoinEntities.Contains(type);
		}

		public bool IsTablePerConcreteClass(System.Type type)
		{
			return tablePerConcreteClassEntities.Contains(type);
		}

		public bool IsOneToOne(MemberInfo member)
		{
			return oneToOneRelations.Contains(member);
		}

		public bool IsManyToOne(MemberInfo member)
		{
			return manyToOneRelations.Contains(member);
		}

		public bool IsManyToMany(MemberInfo member)
		{
			return manyToManyRelations.Contains(member);
		}

		public bool IsOneToMany(MemberInfo member)
		{
			return oneToManyRelations.Contains(member);
		}

		public bool IsHeterogeneousAssociation(MemberInfo member)
		{
			return any.Contains(member);
		}

		public bool IsPersistentId(MemberInfo member)
		{
			return poids.Contains(member);
		}

		public bool IsVersion(MemberInfo member)
		{
			return versionProperties.Contains(member);
		}

		public bool IsMemberOfNaturalId(MemberInfo member)
		{
			return naturalIds.Contains(member);
		}

		public bool IsPersistentProperty(MemberInfo member)
		{
			return persistentProperties.Contains(member);
		}

		public bool IsSet(MemberInfo role)
		{
			return sets.Contains(role);
		}

		public bool IsBag(MemberInfo role)
		{
			return bags.Contains(role);
		}

		public bool IsIdBag(MemberInfo role)
		{
			return idBags.Contains(role);
		}

		public bool IsList(MemberInfo role)
		{
			return lists.Contains(role);
		}

		public bool IsArray(MemberInfo role)
		{
			return arrays.Contains(role);
		}

		public bool IsDictionary(MemberInfo role)
		{
			return dictionaries.Contains(role);
		}

		public bool IsProperty(MemberInfo member)
		{
			return properties.Contains(member);
		}

		#endregion

		private System.Type GetRootEntityOrNull(System.Type entityType)
		{
			if (entityType == null)
			{
				return null;
			}
			if (IsRootEntity(entityType))
			{
				return entityType;
			}
			return entityType.GetBaseTypes().SingleOrDefault(IsRootEntity);
		}

		protected bool IsMappedFor(ICollection<System.Type> explicitMappedEntities, System.Type type)
		{
			bool isExplicitMapped = explicitMappedEntities.Contains(type);
			bool isDerived = false;

			if (!isExplicitMapped)
			{
				isDerived = type.GetBaseTypes().Any(explicitMappedEntities.Contains);
				if (isDerived)
				{
					explicitMappedEntities.Add(type);
				}
			}
			return isExplicitMapped || isDerived;
		}
	}
}