using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NHibernate.Mapping.ByCode
{
	public abstract class AbstractExplicitlyDeclaredModel : IModelExplicitDeclarationsHolder
	{
		private readonly HashSet<MemberInfo> any = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> arrays = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> bags = new HashSet<MemberInfo>();
		private readonly HashSet<System.Type> components = new HashSet<System.Type>();
		private readonly HashSet<MemberInfo> dictionaries = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> idBags = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> lists = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> keyManyToManyRelations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> itemManyToManyRelations = new HashSet<MemberInfo>();
		private readonly HashSet<MemberInfo> manyToAnyRelations = new HashSet<MemberInfo>();
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
		private readonly Dictionary<System.Type, HashSet<string>> typeSplitGroups = new Dictionary<System.Type, HashSet<string>>();
		private readonly Dictionary<MemberInfo, string> memberSplitGroup = new Dictionary<MemberInfo, string>();
		private readonly HashSet<SplitDefinition> splitDefinitions = new HashSet<SplitDefinition>();

		#region Delayed registrations
		// To allow a free organization of mappings, especially in presence of a IModelMapper not based exclusivelly on pure declarative mapping, and then
		// provide a descriptive exception when there are some ambiguity, we have to delay the registration as late as we can
		// (in practice to the moment we have to give a certain asnwer about the strategy used to represent a System.Type hierarchy).
		private readonly Queue<System.Action> delayedRootEntityRegistrations = new Queue<System.Action>();
		private readonly Dictionary<System.Type, List<Action<System.Type>>> delayedEntityRegistrations = new Dictionary<System.Type, List<Action<System.Type>>>();

		#endregion

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

		public IEnumerable<MemberInfo> KeyManyToManyRelations
		{
			get { return keyManyToManyRelations; }
		}

		public IEnumerable<MemberInfo> ItemManyToManyRelations
		{
			get { return itemManyToManyRelations; }
		}

		public IEnumerable<MemberInfo> OneToManyRelations
		{
			get { return oneToManyRelations; }
		}

		public IEnumerable<MemberInfo> ManyToAnyRelations
		{
			get { return manyToAnyRelations; }
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
			HashSet<string> splitGroupIds;
			if (typeSplitGroups.TryGetValue(type, out splitGroupIds))
			{
				return splitGroupIds;
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
			rootEntities.Add(type);
			if (IsComponent(type))
			{
				throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered as entity and as component", type.FullName));
			}
		}

		public abstract bool IsComponent(System.Type type);

		public void AddAsComponent(System.Type type)
		{
			components.Add(type);
			var rootEntity = GetSingleRootEntityOrNull(type);
			if (rootEntity != null)
			{
				throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered as entity and as component", type.FullName));
			}
		}

		public void AddAsTablePerClassEntity(System.Type type)
		{
			AddAsTablePerClassEntity(type, false);
		}

		protected virtual void AddAsTablePerClassEntity(System.Type type, bool rootEntityMustExists)
		{
			if(!rootEntityMustExists)
			{
				delayedRootEntityRegistrations.Enqueue(() => System.Array.ForEach(GetRootEntitiesOf(type).ToArray(), root=> tablePerClassEntities.Add(root)));
				EnlistTypeRegistration(type, t => AddAsTablePerClassEntity(t, true));
				return;
			}
			if (IsComponent(type))
			{
				throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered as entity and as component", type.FullName));
			}
			var rootEntity = GetSingleRootEntityOrNull(type);
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
			}
			else
			{
				throw new MappingException(string.Format("The root entity for {0} was never registered", type.FullName));
			}
		}

		public void AddAsTablePerClassHierarchyEntity(System.Type type)
		{
			AddAsTablePerClassHierarchyEntity(type, false);
		}

		protected virtual void AddAsTablePerClassHierarchyEntity(System.Type type, bool rootEntityMustExists)
		{
			if (!rootEntityMustExists)
			{
				delayedRootEntityRegistrations.Enqueue(() => System.Array.ForEach(GetRootEntitiesOf(type).ToArray(), root => tablePerClassHierarchyEntities.Add(root)));
				EnlistTypeRegistration(type, t => AddAsTablePerClassHierarchyEntity(t, true));
				return;
			}

			if (IsComponent(type))
			{
				throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered as entity and as component", type.FullName));
			}
			var rootEntity = GetSingleRootEntityOrNull(type);
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
			}
			else
			{
				throw new MappingException(string.Format("The root entity for {0} was never registered", type.FullName));
			}
		}

		public void AddAsTablePerConcreteClassEntity(System.Type type)
		{
			AddAsTablePerConcreteClassEntity(type, false);
		}

		protected virtual void AddAsTablePerConcreteClassEntity(System.Type type, bool rootEntityMustExists)
		{
			if (!rootEntityMustExists)
			{
				delayedRootEntityRegistrations.Enqueue(() => System.Array.ForEach(GetRootEntitiesOf(type).ToArray(), root => tablePerConcreteClassEntities.Add(root)));
				EnlistTypeRegistration(type, t => AddAsTablePerConcreteClassEntity(t, true));
				return;
			}

			if (IsComponent(type))
			{
				throw new MappingException(string.Format("Ambiguous mapping of {0}. It was registered as entity and as component", type.FullName));
			}
			var rootEntity = GetSingleRootEntityOrNull(type);
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
			}
			else
			{
				throw new MappingException(string.Format("The root entity for {0} was never registered", type.FullName));
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

		public void AddAsManyToManyKeyRelation(MemberInfo member)
		{
			persistentMembers.Add(member);
			keyManyToManyRelations.Add(member);
		}

		public void AddAsManyToManyItemRelation(MemberInfo member)
		{
			persistentMembers.Add(member);
			itemManyToManyRelations.Add(member);
		}

		public void AddAsOneToManyRelation(MemberInfo member)
		{
			persistentMembers.Add(member);
			oneToManyRelations.Add(member);
		}

		public void AddAsManyToAnyRelation(MemberInfo member)
		{
			persistentMembers.Add(member);
			manyToAnyRelations.Add(member);
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

		public virtual System.Type GetDynamicComponentTemplate(MemberInfo member)
		{
			System.Type template;
			dynamicComponentTemplates.TryGetValue(member, out template);
			return template ?? typeof(object);
		}

		protected System.Type GetSingleRootEntityOrNull(System.Type entityType)
		{
			var rootTypes = GetRootEntitiesOf(entityType).ToList();
			if(rootTypes.Count > 1)
			{
				var sb = new StringBuilder(1024);
				sb.AppendLine(string.Format("Ambiguous mapping for {0}. More than one root entities was found:", entityType.FullName));
				foreach (var rootType in rootTypes.AsEnumerable().Reverse())
				{
					sb.AppendLine(rootType.FullName);
				}
				sb.AppendLine("Possible solutions:");
				sb.AppendLine("- Merge the mappings of root entity in the one is representing the real root of the hierarchy.");
				sb.AppendLine("- Inject a IModelInspector with a logic to discover the real root-entity.");
				throw new MappingException(sb.ToString());
			}
			return rootTypes.SingleOrDefault(IsRootEntity);
		}

		protected IEnumerable<System.Type> GetRootEntitiesOf(System.Type entityType)
		{
			if (entityType == null)
			{
				yield break;
			}
			if(IsRootEntity(entityType))
			{
				yield return entityType;
			}
			foreach (var type in entityType.GetBaseTypes().Where(IsRootEntity))
			{
				yield return type;
			}
		}

		public abstract bool IsRootEntity(System.Type entityType);

		protected bool IsMappedForTablePerClassEntities(System.Type type)
		{
			return IsMappedFor(tablePerClassEntities, type);
		}

		protected bool IsMappedForTablePerClassHierarchyEntities(System.Type type)
		{
			return IsMappedFor(tablePerClassHierarchyEntities, type);
		}

		protected bool IsMappedForTablePerConcreteClassEntities(System.Type type)
		{
			return IsMappedFor(tablePerConcreteClassEntities, type);
		}

		private bool IsMappedFor(ICollection<System.Type> explicitMappedEntities, System.Type type)
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
			List<Action<System.Type>> actions;
			if (!delayedEntityRegistrations.TryGetValue(type, out actions))
			{
				actions = new List<Action<System.Type>>();
				delayedEntityRegistrations[type] = actions;
			}
			actions.Add(registration);
		}

		protected void ExecuteDelayedTypeRegistration(System.Type type)
		{
			ExecuteDelayedRootEntitiesRegistrations();
			List<Action<System.Type>> actions;
			if (delayedEntityRegistrations.TryGetValue(type, out actions))
			{
				foreach (var action in actions)
				{
					action(type);
				}
			}
		}

		protected void ExecuteDelayedRootEntitiesRegistrations()
		{
			while (delayedRootEntityRegistrations.Count > 0)
			{
				delayedRootEntityRegistrations.Dequeue().Invoke();
			}
		}

		protected bool HasDelayedEntityRegistration(System.Type type)
		{
			return delayedEntityRegistrations.ContainsKey(type);
		}
	}
}