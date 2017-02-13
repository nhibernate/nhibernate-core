using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	/// <summary>
	/// A <see cref="IModelInspector"/> which allows customization of conditions with explicitly declared members.
	/// </summary>
	public class SimpleModelInspector : IModelInspector, IModelExplicitDeclarationsHolder
	{
		private class MixinDeclaredModel : AbstractExplicitlyDeclaredModel
		{
			private readonly IModelInspector inspector;

			public MixinDeclaredModel(IModelInspector inspector)
			{
				this.inspector = inspector;
			}

			public override bool IsComponent(System.Type type)
			{
				return Components.Contains(type);
			}

			public override bool IsRootEntity(System.Type entityType)
			{
				return inspector.IsRootEntity(entityType);
			}

			public bool IsEntity(System.Type type)
			{
				return RootEntities.Contains(type) || type.GetBaseTypes().Any(t => RootEntities.Contains(t)) || HasDelayedEntityRegistration(type);
			}

			public bool IsTablePerClass(System.Type type)
			{
				ExecuteDelayedTypeRegistration(type);
				return IsMappedForTablePerClassEntities(type);
			}

			public bool IsTablePerClassSplit(System.Type type, object splitGroupId, MemberInfo member)
			{
				return Equals(splitGroupId, GetSplitGroupFor(member));
			}

			public bool IsTablePerClassHierarchy(System.Type type)
			{
				ExecuteDelayedTypeRegistration(type);
				return IsMappedForTablePerClassHierarchyEntities(type);
			}

			public bool IsTablePerConcreteClass(System.Type type)
			{
				ExecuteDelayedTypeRegistration(type);
				return IsMappedForTablePerConcreteClassEntities(type);
			}

			public bool IsOneToOne(MemberInfo member)
			{
				return OneToOneRelations.Contains(member);
			}

			public bool IsManyToOne(MemberInfo member)
			{
				return ManyToOneRelations.Contains(member);
			}

			public bool IsManyToManyItem(MemberInfo member)
			{
				return ItemManyToManyRelations.Contains(member);
			}

			public bool IsManyToManyKey(MemberInfo member)
			{
				return KeyManyToManyRelations.Contains(member);
			}

			public bool IsOneToMany(MemberInfo member)
			{
				return OneToManyRelations.Contains(member);
			}

			public bool IsManyToAny(MemberInfo member)
			{
				return ManyToAnyRelations.Contains(member);
			}

			public bool IsAny(MemberInfo member)
			{
				return Any.Contains(member);
			}

			public bool IsPersistentId(MemberInfo member)
			{
				return Poids.Contains(member);
			}

			public bool IsMemberOfComposedId(MemberInfo member)
			{
				return ComposedIds.Contains(member);
			}

			public bool IsVersion(MemberInfo member)
			{
				return VersionProperties.Contains(member);
			}

			public bool IsMemberOfNaturalId(MemberInfo member)
			{
				return NaturalIds.Contains(member);
			}

			public bool IsPersistentProperty(MemberInfo member)
			{
				return PersistentMembers.Contains(member);
			}

			public bool IsSet(MemberInfo role)
			{
				return Sets.Contains(role);
			}

			public bool IsBag(MemberInfo role)
			{
				return Bags.Contains(role);
			}

			public bool IsIdBag(MemberInfo role)
			{
				return IdBags.Contains(role);
			}

			public bool IsList(MemberInfo role)
			{
				return Lists.Contains(role);
			}

			public bool IsArray(MemberInfo role)
			{
				return Arrays.Contains(role);
			}

			public bool IsDictionary(MemberInfo role)
			{
				return Dictionaries.Contains(role);
			}

			public bool IsProperty(MemberInfo member)
			{
				return Properties.Contains(member);
			}

			public bool IsDynamicComponent(MemberInfo member)
			{
				return DynamicComponents.Contains(member);
			}

			public IEnumerable<string> GetPropertiesSplits(System.Type type)
			{
				return GetSplitGroupsFor(type);
			}
		}

		private readonly MixinDeclaredModel declaredModel;

		private Func<System.Type, bool, bool> isEntity = (t, declared) => declared;
		private Func<System.Type, bool, bool> isRootEntity;
		private Func<System.Type, bool, bool> isTablePerClass;
		private Func<SplitDefinition, bool, bool> isTablePerClassSplit = (sd, declared) => declared;
		private Func<System.Type, bool, bool> isTablePerClassHierarchy = (t, declared) => declared;
		private Func<System.Type, bool, bool> isTablePerConcreteClass = (t, declared) => declared;
		private Func<System.Type, IEnumerable<string>, IEnumerable<string>> splitsForType = (t, declared) => declared;
		private Func<System.Type, bool, bool> isComponent;

		private Func<MemberInfo, bool, bool> isPersistentId;
		private Func<MemberInfo, bool, bool> isPersistentProperty;
		private Func<MemberInfo, bool, bool> isVersion = (m, declared) => declared;

		private Func<MemberInfo, bool, bool> isProperty = (m, declared) => declared;
		private Func<MemberInfo, bool, bool> isDynamicComponent = (m, declared) => declared;
		private Func<MemberInfo, bool, bool> isAny = (m, declared) => declared;
		private Func<MemberInfo, bool, bool> isManyToMany = (m, declared) => declared;
		private Func<MemberInfo, bool, bool> isManyToAny = (m, declared) => declared;
		private Func<MemberInfo, bool, bool> isManyToOne;
		private Func<MemberInfo, bool, bool> isMemberOfNaturalId = (m, declared) => declared;
		private Func<MemberInfo, bool, bool> isOneToMany;
		private Func<MemberInfo, bool, bool> isOneToOne = (m, declared) => declared;

		private Func<MemberInfo, bool, bool> isSet;
		private Func<MemberInfo, bool, bool> isArray;
		private Func<MemberInfo, bool, bool> isBag;
		private Func<MemberInfo, bool, bool> isDictionary;
		private Func<MemberInfo, bool, bool> isIdBag = (m, declared) => declared;
		private Func<MemberInfo, bool, bool> isList = (m, declared) => declared;

		public SimpleModelInspector()
		{
			isEntity = (t, declared) => declared || MatchEntity(t);
			isRootEntity = (t, declared) => declared || MatchRootEntity(t);
			isTablePerClass = (t, declared) => declared || MatchTablePerClass(t);
			isPersistentId = (m, declared) => declared || MatchPoIdPattern(m);
			isComponent = (t, declared) => declared || MatchComponentPattern(t);
			isPersistentProperty = (m, declared) => declared || ((m is PropertyInfo) && MatchNoReadOnlyPropertyPattern(m));
			isSet = (m, declared) => declared || MatchCollection(m, MatchSetMember);
			isArray = (m, declared) => declared || MatchCollection(m, MatchArrayMember);
			isBag = (m, declared) => declared || MatchCollection(m, MatchBagMember);
			isDictionary = (m, declared) => declared || MatchCollection(m, MatchDictionaryMember);
			isManyToOne = (m, declared) => declared || MatchManyToOne(m);
			isOneToMany = (m, declared) => declared || MatchOneToMany(m);
			declaredModel = new MixinDeclaredModel(this);
		}

		private bool MatchRootEntity(System.Type type)
		{
			return type.IsClass && typeof(object).Equals(type.BaseType) && ((IModelInspector)this).IsEntity(type);
		}

		private bool MatchTablePerClass(System.Type type)
		{
			return !declaredModel.IsTablePerClassHierarchy(type) && !declaredModel.IsTablePerConcreteClass(type);
		}

		private bool MatchOneToMany(MemberInfo memberInfo)
		{
			var modelInspector = (IModelInspector) this;
			System.Type from = memberInfo.ReflectedType;
			System.Type to = memberInfo.GetPropertyOrFieldType().DetermineCollectionElementOrDictionaryValueType();
			if(to == null)
			{
				// no generic collection or simple property
				return false;
			}
			bool areEntities = modelInspector.IsEntity(from) && modelInspector.IsEntity(to);
			bool isFromComponentToEntity = modelInspector.IsComponent(from) && modelInspector.IsEntity(to);
			return !declaredModel.IsManyToManyItem(memberInfo) && (areEntities || isFromComponentToEntity);
		}

		private bool MatchManyToOne(MemberInfo memberInfo)
		{
			var modelInspector = (IModelInspector)this;
			System.Type from = memberInfo.ReflectedType;
			System.Type to = memberInfo.GetPropertyOrFieldType();

			bool areEntities = modelInspector.IsEntity(from) && modelInspector.IsEntity(to);
			bool isFromComponentToEntity = modelInspector.IsComponent(from) && modelInspector.IsEntity(to);
			return isFromComponentToEntity || (areEntities && !modelInspector.IsOneToOne(memberInfo));
		}

		protected bool MatchArrayMember(MemberInfo subject)
		{
			System.Type memberType = subject.GetPropertyOrFieldType();
			return memberType.IsArray && memberType.GetElementType() != typeof(byte);
		}

		protected bool MatchDictionaryMember(MemberInfo subject)
		{
			System.Type memberType = subject.GetPropertyOrFieldType();
			if (typeof(System.Collections.IDictionary).IsAssignableFrom(memberType))
			{
				return true;
			}
			if (memberType.IsGenericType)
			{
				return memberType.GetGenericInterfaceTypeDefinitions().Contains(typeof(IDictionary<,>));
			}
			return false;
		}

		protected bool MatchBagMember(MemberInfo subject)
		{
			System.Type memberType = subject.GetPropertyOrFieldType();
			return typeof(System.Collections.IEnumerable).IsAssignableFrom(memberType) && !(memberType == typeof(string) || memberType == typeof(byte[]));
		}

		protected bool MatchCollection(MemberInfo subject, Predicate<MemberInfo> specificCollectionPredicate)
		{
			const BindingFlags defaultBinding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

			if (specificCollectionPredicate(subject)) return true;
			var pi = subject as PropertyInfo;
			if (pi != null)
			{
				var fieldInfo = (from ps in PropertyToField.DefaultStrategies.Values
												 let fi = subject.DeclaringType.GetField(ps.GetFieldName(pi.Name), defaultBinding)
												 where fi != null
												 select fi).FirstOrDefault();

				if (fieldInfo != null)
				{
					return specificCollectionPredicate(fieldInfo);
				}
			}
			return false;
		}

		protected bool MatchSetMember(MemberInfo subject)
		{
			var memberType = subject.GetPropertyOrFieldType();

			if (memberType.IsGenericType)
			{
				return memberType.GetGenericInterfaceTypeDefinitions().Contains(typeof(ISet<>));
			}
			return false;
		}

		protected bool MatchNoReadOnlyPropertyPattern(MemberInfo subject)
		{
			var isReadOnlyProperty = IsReadOnlyProperty(subject);
			return !isReadOnlyProperty;
		}

		protected bool IsReadOnlyProperty(MemberInfo subject)
		{
			const BindingFlags defaultBinding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

			var property = subject as PropertyInfo;
			if (property == null)
			{
				return false;
			}
			if (CanReadCantWriteInsideType(property) || CanReadCantWriteInBaseType(property))
			{
				return !PropertyToField.DefaultStrategies.Values.Any(s => subject.DeclaringType.GetField(s.GetFieldName(property.Name), defaultBinding) != null) || IsAutoproperty(property);
			}
			return false;
		}

		protected bool IsAutoproperty(PropertyInfo property)
		{
			return property.ReflectedType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
																					 | BindingFlags.DeclaredOnly).Any(pi => pi.Name == string.Concat("<", property.Name, ">k__BackingField"));
		}

		protected bool CanReadCantWriteInsideType(PropertyInfo property)
		{
			return !property.CanWrite && property.CanRead && property.DeclaringType == property.ReflectedType;
		}

		protected bool CanReadCantWriteInBaseType(PropertyInfo property)
		{
			if (property.DeclaringType == property.ReflectedType)
			{
				return false;
			}
			var rfprop = property.DeclaringType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
																					 | BindingFlags.DeclaredOnly).SingleOrDefault(pi => pi.Name == property.Name);
			return rfprop != null && !rfprop.CanWrite && rfprop.CanRead;
		}

		protected bool MatchPoIdPattern(MemberInfo subject)
		{
			var name = subject.Name;
			return name.Equals("id", StringComparison.InvariantCultureIgnoreCase)
						 || name.Equals("poid", StringComparison.InvariantCultureIgnoreCase)
						 || name.Equals("oid", StringComparison.InvariantCultureIgnoreCase)
						 || (name.StartsWith(subject.DeclaringType.Name) && name.Equals(subject.DeclaringType.Name + "id", StringComparison.InvariantCultureIgnoreCase));
		}

		protected bool MatchComponentPattern(System.Type subject)
		{
			const BindingFlags flattenHierarchyMembers =
				BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			if (declaredModel.IsEntity(subject))
			{
				return false;
			}
			var modelInspector = (IModelInspector) this;
			return !subject.IsEnum && (subject.Namespace == null || !subject.Namespace.StartsWith("System")) /* hack */
							&& !modelInspector.IsEntity(subject)
							&& !subject.GetProperties(flattenHierarchyMembers).Cast<MemberInfo>().Concat(
			       		subject.GetFields(flattenHierarchyMembers)).Any(m => modelInspector.IsPersistentId(m));
		}

		protected bool MatchEntity(System.Type subject)
		{
			const BindingFlags flattenHierarchyMembers =
				BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			if(declaredModel.Components.Contains(subject))
			{
				return false;
			}
			var modelInspector = (IModelInspector) this;
			return subject.IsClass &&
			       subject.GetProperties(flattenHierarchyMembers).Cast<MemberInfo>().Concat(subject.GetFields(flattenHierarchyMembers)).Any(m => modelInspector.IsPersistentId(m));
		}

		#region IModelExplicitDeclarationsHolder Members

		IEnumerable<System.Type> IModelExplicitDeclarationsHolder.RootEntities
		{
			get { return declaredModel.RootEntities; }
		}

		IEnumerable<System.Type> IModelExplicitDeclarationsHolder.Components
		{
			get { return declaredModel.Components; }
		}

		IEnumerable<System.Type> IModelExplicitDeclarationsHolder.TablePerClassEntities
		{
			get { return declaredModel.TablePerClassEntities; }
		}

		IEnumerable<System.Type> IModelExplicitDeclarationsHolder.TablePerClassHierarchyEntities
		{
			get { return declaredModel.TablePerClassHierarchyEntities; }
		}

		IEnumerable<System.Type> IModelExplicitDeclarationsHolder.TablePerConcreteClassEntities
		{
			get { return declaredModel.TablePerConcreteClassEntities; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.OneToOneRelations
		{
			get { return declaredModel.OneToOneRelations; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.ManyToOneRelations
		{
			get { return declaredModel.ManyToOneRelations; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.KeyManyToManyRelations
		{
			get { return declaredModel.KeyManyToManyRelations; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.ItemManyToManyRelations
		{
			get { return declaredModel.ItemManyToManyRelations; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.OneToManyRelations
		{
			get { return declaredModel.OneToManyRelations; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.ManyToAnyRelations
		{
			get { return declaredModel.ManyToAnyRelations; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.Any
		{
			get { return declaredModel.Any; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.Poids
		{
			get { return declaredModel.Poids; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.VersionProperties
		{
			get { return declaredModel.VersionProperties; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.NaturalIds
		{
			get { return declaredModel.NaturalIds; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.Sets
		{
			get { return declaredModel.Sets; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.Bags
		{
			get { return declaredModel.Bags; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.IdBags
		{
			get { return declaredModel.IdBags; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.Lists
		{
			get { return declaredModel.Lists; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.Arrays
		{
			get { return declaredModel.Arrays; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.Dictionaries
		{
			get { return declaredModel.Dictionaries; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.Properties
		{
			get { return declaredModel.Properties; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.PersistentMembers
		{
			get { return declaredModel.PersistentMembers; }
		}

		IEnumerable<SplitDefinition> IModelExplicitDeclarationsHolder.SplitDefinitions
		{
			get { return declaredModel.SplitDefinitions; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.DynamicComponents
		{
			get { return declaredModel.DynamicComponents; }
		}

		IEnumerable<string> IModelExplicitDeclarationsHolder.GetSplitGroupsFor(System.Type type)
		{
			return declaredModel.GetSplitGroupsFor(type);
		}

		string IModelExplicitDeclarationsHolder.GetSplitGroupFor(MemberInfo member)
		{
			return declaredModel.GetSplitGroupFor(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsRootEntity(System.Type type)
		{
			declaredModel.AddAsRootEntity(type);
		}

		void IModelExplicitDeclarationsHolder.AddAsComponent(System.Type type)
		{
			declaredModel.AddAsComponent(type);
		}

		void IModelExplicitDeclarationsHolder.AddAsTablePerClassEntity(System.Type type)
		{
			declaredModel.AddAsTablePerClassEntity(type);
		}

		void IModelExplicitDeclarationsHolder.AddAsTablePerClassHierarchyEntity(System.Type type)
		{
			declaredModel.AddAsTablePerClassHierarchyEntity(type);
		}

		void IModelExplicitDeclarationsHolder.AddAsTablePerConcreteClassEntity(System.Type type)
		{
			declaredModel.AddAsTablePerConcreteClassEntity(type);
		}

		void IModelExplicitDeclarationsHolder.AddAsOneToOneRelation(MemberInfo member)
		{
			declaredModel.AddAsOneToOneRelation(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsManyToOneRelation(MemberInfo member)
		{
			declaredModel.AddAsManyToOneRelation(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsManyToManyKeyRelation(MemberInfo member)
		{
			declaredModel.AddAsManyToManyKeyRelation(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsManyToManyItemRelation(MemberInfo member)
		{
			declaredModel.AddAsManyToManyItemRelation(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsOneToManyRelation(MemberInfo member)
		{
			declaredModel.AddAsOneToManyRelation(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsManyToAnyRelation(MemberInfo member)
		{
			declaredModel.AddAsManyToAnyRelation(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsAny(MemberInfo member)
		{
			declaredModel.AddAsAny(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsPoid(MemberInfo member)
		{
			declaredModel.AddAsPoid(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsVersionProperty(MemberInfo member)
		{
			declaredModel.AddAsVersionProperty(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsNaturalId(MemberInfo member)
		{
			declaredModel.AddAsNaturalId(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsSet(MemberInfo member)
		{
			declaredModel.AddAsSet(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsBag(MemberInfo member)
		{
			declaredModel.AddAsBag(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsIdBag(MemberInfo member)
		{
			declaredModel.AddAsIdBag(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsList(MemberInfo member)
		{
			declaredModel.AddAsList(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsArray(MemberInfo member)
		{
			declaredModel.AddAsArray(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsMap(MemberInfo member)
		{
			declaredModel.AddAsMap(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsProperty(MemberInfo member)
		{
			declaredModel.AddAsProperty(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsPersistentMember(MemberInfo member)
		{
			declaredModel.AddAsPersistentMember(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsPropertySplit(SplitDefinition definition)
		{
			declaredModel.AddAsPropertySplit(definition);
		}

		void IModelExplicitDeclarationsHolder.AddAsDynamicComponent(MemberInfo member, System.Type componentTemplate)
		{
			declaredModel.AddAsDynamicComponent(member, componentTemplate);
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.ComposedIds
		{
			get { return declaredModel.ComposedIds; }
		}

		void IModelExplicitDeclarationsHolder.AddAsPartOfComposedId(MemberInfo member)
		{
			declaredModel.AddAsPartOfComposedId(member);
		}

		#endregion

		#region Implementation of IModelInspector

		bool IModelInspector.IsRootEntity(System.Type type)
		{
			bool declaredResult = declaredModel.RootEntities.Contains(type);
			return isRootEntity(type, declaredResult);
		}

		bool IModelInspector.IsComponent(System.Type type)
		{
			bool declaredResult = declaredModel.Components.Contains(type);
			return isComponent(type, declaredResult);
		}

		bool IModelInspector.IsEntity(System.Type type)
		{
			bool declaredResult = declaredModel.IsEntity(type);
			return isEntity(type, declaredResult);
		}

		bool IModelInspector.IsTablePerClass(System.Type type)
		{
			bool declaredResult = declaredModel.IsTablePerClass(type);
			return isTablePerClass(type, declaredResult);
		}

		bool IModelInspector.IsTablePerClassSplit(System.Type type, object splitGroupId, MemberInfo member)
		{
			bool declaredResult = declaredModel.IsTablePerClassSplit(type, splitGroupId, member);
			return isTablePerClassSplit(new SplitDefinition(type, splitGroupId.ToString(), member), declaredResult);
		}

		bool IModelInspector.IsTablePerClassHierarchy(System.Type type)
		{
			bool declaredResult = declaredModel.IsTablePerClassHierarchy(type);
			return isTablePerClassHierarchy(type, declaredResult);
		}

		bool IModelInspector.IsTablePerConcreteClass(System.Type type)
		{
			bool declaredResult = declaredModel.IsTablePerConcreteClass(type);
			return isTablePerConcreteClass(type, declaredResult);
		}

		bool IModelInspector.IsOneToOne(MemberInfo member)
		{
			bool declaredResult = DeclaredPolymorphicMatch(member, m => declaredModel.IsOneToOne(m));
			return isOneToOne(member, declaredResult);
		}

		bool IModelInspector.IsManyToOne(MemberInfo member)
		{
			bool declaredResult = DeclaredPolymorphicMatch(member, m => declaredModel.IsManyToOne(m));
			return isManyToOne(member, declaredResult);
		}

		bool IModelInspector.IsManyToManyItem(MemberInfo member)
		{
			bool declaredResult = DeclaredPolymorphicMatch(member, m => declaredModel.IsManyToManyItem(m));
			return isManyToMany(member, declaredResult);
		}

		bool IModelInspector.IsManyToManyKey(MemberInfo member)
		{
			bool declaredResult = DeclaredPolymorphicMatch(member, m => declaredModel.IsManyToManyKey(m));
			return isManyToMany(member, declaredResult);
		}

		bool IModelInspector.IsOneToMany(MemberInfo member)
		{
			bool declaredResult = DeclaredPolymorphicMatch(member, m => declaredModel.IsOneToMany(m));
			return isOneToMany(member, declaredResult);
		}

		bool IModelInspector.IsManyToAny(MemberInfo member)
		{
			bool declaredResult = DeclaredPolymorphicMatch(member, m => declaredModel.IsManyToAny(m));
			return isManyToAny(member, declaredResult);
		}

		bool IModelInspector.IsAny(MemberInfo member)
		{
			bool declaredResult = DeclaredPolymorphicMatch(member, m => declaredModel.IsAny(m));
			return isAny(member, declaredResult);
		}

		bool IModelInspector.IsPersistentId(MemberInfo member)
		{
			bool declaredResult = DeclaredPolymorphicMatch(member, m => declaredModel.IsPersistentId(m));
			return isPersistentId(member, declaredResult);
		}

		bool IModelInspector.IsMemberOfComposedId(MemberInfo member)
		{
			return declaredModel.IsMemberOfComposedId(member);
		}

		bool IModelInspector.IsVersion(MemberInfo member)
		{
			bool declaredResult = DeclaredPolymorphicMatch(member, m => declaredModel.IsVersion(m));
			return isVersion(member, declaredResult);
		}

		bool IModelInspector.IsMemberOfNaturalId(MemberInfo member)
		{
			bool declaredResult = DeclaredPolymorphicMatch(member, m => declaredModel.IsMemberOfNaturalId(m));
			return isMemberOfNaturalId(member, declaredResult);
		}

		bool IModelInspector.IsPersistentProperty(MemberInfo member)
		{
			bool declaredResult = DeclaredPolymorphicMatch(member, m => declaredModel.IsPersistentProperty(m));
			return isPersistentProperty(member, declaredResult);
		}

		bool IModelInspector.IsSet(MemberInfo role)
		{
			bool declaredResult = DeclaredPolymorphicMatch(role, m => declaredModel.IsSet(m));
			return isSet(role, declaredResult);
		}

		bool IModelInspector.IsBag(MemberInfo role)
		{
			bool declaredResult = DeclaredPolymorphicMatch(role, m => declaredModel.IsBag(m));
			return isBag(role, declaredResult);
		}

		bool IModelInspector.IsIdBag(MemberInfo role)
		{
			bool declaredResult = DeclaredPolymorphicMatch(role, m => declaredModel.IsIdBag(m));
			return isIdBag(role, declaredResult);
		}

		bool IModelInspector.IsList(MemberInfo role)
		{
			bool declaredResult = DeclaredPolymorphicMatch(role, m => declaredModel.IsList(m));
			return isList(role, declaredResult);
		}

		bool IModelInspector.IsArray(MemberInfo role)
		{
			bool declaredResult = DeclaredPolymorphicMatch(role, m => declaredModel.IsArray(m));
			return isArray(role, declaredResult);
		}

		bool IModelInspector.IsDictionary(MemberInfo role)
		{
			bool declaredResult = DeclaredPolymorphicMatch(role, m => declaredModel.IsDictionary(m));
			return isDictionary(role, declaredResult);
		}

		bool IModelInspector.IsProperty(MemberInfo member)
		{
			bool declaredResult = DeclaredPolymorphicMatch(member, m => declaredModel.IsProperty(m));
			return isProperty(member, declaredResult);
		}

		bool IModelInspector.IsDynamicComponent(MemberInfo member)
		{
			bool declaredResult = DeclaredPolymorphicMatch(member, m => declaredModel.IsDynamicComponent(m));
			return isDynamicComponent(member, declaredResult);
		}

		System.Type IModelInspector.GetDynamicComponentTemplate(MemberInfo member)
		{
			return declaredModel.GetDynamicComponentTemplate(member);
		}
		System.Type IModelExplicitDeclarationsHolder.GetDynamicComponentTemplate(MemberInfo member)
		{
			return declaredModel.GetDynamicComponentTemplate(member);
		}

		IEnumerable<string> IModelInspector.GetPropertiesSplits(System.Type type)
		{
			IEnumerable<string> declaredResult = declaredModel.GetPropertiesSplits(type);
			return splitsForType(type, declaredResult);
		}

		#endregion

		protected virtual bool DeclaredPolymorphicMatch(MemberInfo member, Func<MemberInfo, bool> declaredMatch)
		{
			return declaredMatch(member)
						 || member.GetMemberFromDeclaringClasses().Any(declaredMatch)
						 || member.GetPropertyFromInterfaces().Any(declaredMatch);
		}

		public void IsRootEntity(Func<System.Type, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isRootEntity = match;
		}

		public void IsComponent(Func<System.Type, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isComponent = match;
		}

		public void IsEntity(Func<System.Type, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isEntity = match;
		}

		public void IsTablePerClass(Func<System.Type, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isTablePerClass = match;
		}

		public void IsTablePerClassHierarchy(Func<System.Type, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isTablePerClassHierarchy = match;
		}

		public void IsTablePerConcreteClass(Func<System.Type, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isTablePerConcreteClass = match;
		}

		public void IsOneToOne(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isOneToOne = match;
		}

		public void IsManyToOne(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isManyToOne = match;
		}

		public void IsManyToMany(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isManyToMany = match;
		}

		public void IsOneToMany(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isOneToMany = match;
		}

		public void IsManyToAny(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isManyToAny = match;
		}

		public void IsAny(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isAny = match;
		}

		public void IsPersistentId(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isPersistentId = match;
		}

		public void IsVersion(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isVersion = match;
		}

		public void IsMemberOfNaturalId(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isMemberOfNaturalId = match;
		}

		public void IsPersistentProperty(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isPersistentProperty = match;
		}

		public void IsSet(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isSet = match;
		}

		public void IsBag(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isBag = match;
		}

		public void IsIdBag(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isIdBag = match;
		}

		public void IsList(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isList = match;
		}

		public void IsArray(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isArray = match;
		}

		public void IsDictionary(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isDictionary = match;
		}

		public void IsProperty(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isProperty = match;
		}

		public void SplitsFor(Func<System.Type, IEnumerable<string>, IEnumerable<string>> getPropertiesSplitsId)
		{
			if (getPropertiesSplitsId == null)
			{
				return;
			}
			splitsForType = getPropertiesSplitsId;
		}

		public void IsTablePerClassSplit(Func<SplitDefinition, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isTablePerClassSplit = match;
		}

		public void IsDynamicComponent(Func<MemberInfo, bool, bool> match)
		{
			if (match == null)
			{
				return;
			}
			isDynamicComponent = match;
		}
	}
}