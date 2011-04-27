using System;
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
		private readonly HashSet<MemberInfo> composedIds = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> oneToManyRelations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> oneToOneRelations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> poids = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> properties = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> dynamicComponents = new HashSet<MemberInfo>();
		private readonly Dictionary<MemberInfo, System.Type> dynamicComponentTemplates = new Dictionary<MemberInfo, System.Type>();
		private readonly HashSet<MemberInfo> persistentMembers = new HashSet<MemberInfo>();
		private readonly HashSet<System.Type> rootEntities = new HashSet<System.Type>();
		private readonly HashSet<MemberInfo> sets = new HashSet<MemberInfo>();
		private readonly HashSet<System.Type> tablePerClassEntities = new HashSet<System.Type>();
		private readonly HashSet<System.Type> tablePerClassHierarchyEntities = new HashSet<System.Type>();
		private readonly HashSet<System.Type> tablePerConcreteClassEntities = new HashSet<System.Type>();
		private readonly HashSet<MemberInfo> versionProperties = new HashSet<MemberInfo>();
		private readonly Dictionary<System.Type, Action<System.Type>> delayedEntityRegistrations = new Dictionary<System.Type, Action<System.Type>>();
		private readonly Dictionary<System.Type, HashSet<string>> typeSplitGroups = new Dictionary<System.Type, HashSet<string>>();
		private readonly Dictionary<MemberInfo, string> memberSplitGroup = new Dictionary<MemberInfo, string>();
		private readonly HashSet<SplitDefinition> splitDefinitions = new HashSet<SplitDefinition>();

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

		public IEnumerable<MemberInfo> ComposedIds
		{
			get { return composedIds; }
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

		public IEnumerable<MemberInfo> DynamicComponents
		{
			get { return dynamicComponents; }
		}

		public IEnumerable<MemberInfo> PersistentMembers
		{
			get { return persistentMembers; }
		}

		public IEnumerable<SplitDefinition> SplitDefinitions
		{
			get { return splitDefinitions; }
		}

		public IEnumerable<string> GetSplitGroupsFor(System.Type type)
		{
			HashSet<string> splitsGroupsIds;
			if (typeSplitGroups.TryGetValue(type, out splitsGroupsIds))
			{
				return splitsGroupsIds;
			}
			return Enumerable.Empty<string>();
		}

		public string GetSplitGroupFor(MemberInfo member)
		{
			var memberKey = member.GetMemberFromDeclaringType();
			string splitGroup;
			if (memberSplitGroup.TryGetValue(memberKey, out splitGroup))
			{
				return splitGroup;
			}
			return null;
		}

		public void AddAsRootEntity(System.Type type)
		{
			if (IsComponent(type))
			{
				throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered as entity and as component", type.FullName));
			}
			rootEntities.Add(type);
		}

		public void AddAsComponent(System.Type type)
		{
			var rootEntity = GetRootEntityOrNull(type);
			if (rootEntity != null)
			{
				throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered as entity and as component", type.FullName));
			}
			components.Add(type);
		}

		public void AddAsTablePerClassEntity(System.Type type)
		{
			AddAsTablePerClassEntity(type, false);
		}

		public void AddAsTablePerClassEntity(System.Type type, bool rootEntityMustExists)
		{
			if (IsComponent(type))
			{
				throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered as entity and as component", type.FullName));
			}
			var rootEntity = GetRootEntityOrNull(type);
			if (rootEntity != null)
			{
				if (rootEntity.Equals(type))
				{
					throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered as root-entity and as subclass for table-per-class strategy", type.FullName));
				}
				if (IsMappedFor(tablePerClassHierarchyEntities, type) || IsMappedFor(tablePerConcreteClassEntities, type))
				{
					throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered with more than one class-hierarchy strategy", type.FullName));
				}
				tablePerClassEntities.Add(rootEntity);
			}
			else
			{
				if(rootEntityMustExists)
				{
					throw new MappingException(string.Format("The root entity for {0} was never registered", type.FullName));
				}
				EnlistTypeRegistration(type, t => AddAsTablePerClassEntity(t, true));
			}
		}

		public void AddAsTablePerClassHierarchyEntity(System.Type type)
		{
			AddAsTablePerClassHierarchyEntity(type, false);
		}

		public void AddAsTablePerClassHierarchyEntity(System.Type type, bool rootEntityMustExists)
		{
			if (IsComponent(type))
			{
				throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered as entity and as component", type.FullName));
			}
			var rootEntity = GetRootEntityOrNull(type);
			if (rootEntity != null)
			{
				if (rootEntity.Equals(type))
				{
					throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered as root-entity and as subclass for table-per-class-hierarchy strategy", type.FullName));
				}
				if (IsMappedFor(tablePerClassEntities, type) || IsMappedFor(tablePerConcreteClassEntities, type))
				{
					throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered with more than one class-hierarchy strategy", type.FullName));
				}
				tablePerClassHierarchyEntities.Add(rootEntity);
			}
			else
			{
				if (rootEntityMustExists)
				{
					throw new MappingException(string.Format("The root entity for {0} was never registered", type.FullName));
				}
				EnlistTypeRegistration(type, t => AddAsTablePerClassHierarchyEntity(t, true));
			}
		}

		public void AddAsTablePerConcreteClassEntity(System.Type type)
		{
			AddAsTablePerConcreteClassEntity(type, false);
		}

		public void AddAsTablePerConcreteClassEntity(System.Type type, bool rootEntityMustExists)
		{
			if (IsComponent(type))
			{
				throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered as entity and as component", type.FullName));
			}
			var rootEntity = GetRootEntityOrNull(type);
			if (rootEntity != null)
			{
				if (rootEntity.Equals(type))
				{
					throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered as root-entity and as subclass for table-per-concrete-class strategy", type.FullName));
				}
				if (IsMappedFor(tablePerClassEntities, type) || IsMappedFor(tablePerClassHierarchyEntities, type))
				{
					throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered with more than one class-hierarchy strategy", type.FullName));
				}
				tablePerConcreteClassEntities.Add(rootEntity);
			}
			else
			{
				if (rootEntityMustExists)
				{
					throw new MappingException(string.Format("The root entity for {0} was never registered", type.FullName));
				}
				EnlistTypeRegistration(type, t => AddAsTablePerConcreteClassEntity(t, true));
			}
		}

		public void AddAsOneToOneRelation(MemberInfo member)
		{
			persistentMembers.Add(member);
			oneToOneRelations.Add(member);
		}

		public void AddAsManyToOneRelation(MemberInfo member)
		{
			persistentMembers.Add(member);
			manyToOneRelations.Add(member);
		}

		public void AddAsManyToManyRelation(MemberInfo member)
		{
			persistentMembers.Add(member);
			manyToManyRelations.Add(member);
		}

		public void AddAsOneToManyRelation(MemberInfo member)
		{
			persistentMembers.Add(member);
			oneToManyRelations.Add(member);
		}

		public void AddAsAny(MemberInfo member)
		{
			persistentMembers.Add(member);
			any.Add(member);
		}

		public void AddAsPoid(MemberInfo member)
		{
			persistentMembers.Add(member);
			poids.Add(member);
		}

		public void AddAsPartOfComposedId(MemberInfo member)
		{
			persistentMembers.Add(member);
			composedIds.Add(member);
		}

		public void AddAsVersionProperty(MemberInfo member)
		{
			persistentMembers.Add(member);
			versionProperties.Add(member);
		}

		public void AddAsNaturalId(MemberInfo member)
		{
			persistentMembers.Add(member);
			naturalIds.Add(member);
		}

		public void AddAsSet(MemberInfo member)
		{
			persistentMembers.Add(member);
			sets.Add(member);
		}

		public void AddAsBag(MemberInfo member)
		{
			persistentMembers.Add(member);
			bags.Add(member);
		}

		public void AddAsIdBag(MemberInfo member)
		{
			persistentMembers.Add(member);
			idBags.Add(member);
		}

		public void AddAsList(MemberInfo member)
		{
			persistentMembers.Add(member);
			lists.Add(member);
		}

		public void AddAsArray(MemberInfo member)
		{
			persistentMembers.Add(member);
			arrays.Add(member);
		}

		public void AddAsMap(MemberInfo member)
		{
			persistentMembers.Add(member);
			dictionaries.Add(member);
		}

		public void AddAsProperty(MemberInfo member)
		{
			persistentMembers.Add(member);
			properties.Add(member);
		}

		public void AddAsPersistentMember(MemberInfo member)
		{
			persistentMembers.Add(member);
		}

		public void AddAsPropertySplit(SplitDefinition definition)
		{
			if (definition == null)
			{
				return;
			}
			/* Note: if the user "jump/exclude" a class and then map the property in two subclasses the usage of GetMemberFromDeclaringType() may cause a problem
			   for a legal usage... we will see when the case happen */
			System.Type propertyContainer = definition.On;
			string splitGroupId = definition.GroupId;
			MemberInfo member = definition.Member;
			var memberKey = member.GetMemberFromDeclaringType();
			string splitGroup;
			if (!memberSplitGroup.TryGetValue(memberKey, out splitGroup))
			{
				AddTypeSplits(propertyContainer, splitGroupId);
				memberSplitGroup[memberKey] = splitGroupId;
			}

			splitDefinitions.Add(definition);
		}

		public void AddAsDynamicComponent(MemberInfo member, System.Type componentTemplate)
		{
			persistentMembers.Add(member);
			dynamicComponents.Add(member);
			dynamicComponentTemplates[member] = componentTemplate;
		}

		private void AddTypeSplits(System.Type propertyContainer, string splitGroupId)
		{
			HashSet<string> splitsGroupsIds;
			typeSplitGroups.TryGetValue(propertyContainer, out splitsGroupsIds);
			if(splitsGroupsIds == null)
			{
				splitsGroupsIds = new HashSet<string>();
				typeSplitGroups[propertyContainer] = splitsGroupsIds;
			}
			splitsGroupsIds.Add(splitGroupId);
		}

		#endregion

		#region Implementation of IModelInspector

		public virtual bool IsRootEntity(System.Type type)
		{
			return rootEntities.Contains(type);
		}

		public virtual bool IsComponent(System.Type type)
		{
			return components.Contains(type);
		}

		public virtual bool IsEntity(System.Type type)
		{
			return rootEntities.Contains(type) || type.GetBaseTypes().Any(t => rootEntities.Contains(t)) || HasDelayedEntityRegistration(type);
		}

		public virtual bool IsTablePerClass(System.Type type)
		{
			ExecuteDelayedTypeRegistration(type);
			return IsMappedFor(tablePerClassEntities, type);
		}

		public virtual bool IsTablePerClassSplit(System.Type type, object splitGroupId, MemberInfo member)
		{
			return Equals(splitGroupId, GetSplitGroupFor(member));
		}

		public virtual bool IsTablePerClassHierarchy(System.Type type)
		{
			ExecuteDelayedTypeRegistration(type);
			return IsMappedFor(tablePerClassHierarchyEntities, type);
		}

		public virtual bool IsTablePerConcreteClass(System.Type type)
		{
			ExecuteDelayedTypeRegistration(type);
			return IsMappedFor(tablePerConcreteClassEntities, type);
		}

		public virtual bool IsOneToOne(MemberInfo member)
		{
			return oneToOneRelations.Contains(member);
		}

		public virtual bool IsManyToOne(MemberInfo member)
		{
			return manyToOneRelations.Contains(member);
		}

		public virtual bool IsManyToMany(MemberInfo member)
		{
			return manyToManyRelations.Contains(member);
		}

		public virtual bool IsOneToMany(MemberInfo member)
		{
			return oneToManyRelations.Contains(member);
		}

		public virtual bool IsAny(MemberInfo member)
		{
			return any.Contains(member);
		}

		public virtual bool IsPersistentId(MemberInfo member)
		{
			return poids.Contains(member);
		}

		public bool IsMemberOfComposedId(MemberInfo member)
		{
			return composedIds.Contains(member);
		}

		public virtual bool IsVersion(MemberInfo member)
		{
			return versionProperties.Contains(member);
		}

		public virtual bool IsMemberOfNaturalId(MemberInfo member)
		{
			return naturalIds.Contains(member);
		}

		public virtual bool IsPersistentProperty(MemberInfo member)
		{
			return persistentMembers.Contains(member);
		}

		public virtual bool IsSet(MemberInfo role)
		{
			return sets.Contains(role);
		}

		public virtual bool IsBag(MemberInfo role)
		{
			return bags.Contains(role);
		}

		public virtual bool IsIdBag(MemberInfo role)
		{
			return idBags.Contains(role);
		}

		public virtual bool IsList(MemberInfo role)
		{
			return lists.Contains(role);
		}

		public virtual bool IsArray(MemberInfo role)
		{
			return arrays.Contains(role);
		}

		public virtual bool IsDictionary(MemberInfo role)
		{
			return dictionaries.Contains(role);
		}

		public virtual bool IsProperty(MemberInfo member)
		{
			return properties.Contains(member);
		}

		public bool IsDynamicComponent(MemberInfo member)
		{
			return dynamicComponents.Contains(member);
		}

		public System.Type GetDynamicComponentTemplate(MemberInfo member)
		{
			System.Type template;
			dynamicComponentTemplates.TryGetValue(member, out template);
			return template ?? typeof(object);
		}

		public IEnumerable<string> GetPropertiesSplits(System.Type type)
		{
			return GetSplitGroupsFor(type);
		}

		#endregion

		protected System.Type GetRootEntityOrNull(System.Type entityType)
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

		protected void EnlistTypeRegistration(System.Type type, Action<System.Type> registration)
		{
			delayedEntityRegistrations.Add(type, registration);
		}

		protected void ExecuteDelayedTypeRegistration(System.Type type)
		{
			Action<System.Type> registration;
			if(delayedEntityRegistrations.TryGetValue(type, out registration))
			{
				delayedEntityRegistrations.Remove(type);
				registration(type);
			}
		}

		protected bool HasDelayedEntityRegistration(System.Type type)
		{
			return delayedEntityRegistrations.ContainsKey(type);
		}
	}
}