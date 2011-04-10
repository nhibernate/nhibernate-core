using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Iesi.Collections;

namespace NHibernate.Mapping.ByCode
{
	/// <summary>
	/// A <see cref="IModelInspector"/> which allows customization of conditions with explicitly declared members.
	/// </summary>
	public class SimpleModelInspector : IModelInspector, IModelExplicitDeclarationsHolder
	{
		private readonly ExplicitlyDeclaredModel declaredModel = new ExplicitlyDeclaredModel();

		private Func<System.Type, bool, bool> isEntity = (t, declared) => declared;
		private Func<System.Type, bool, bool> isRootEntity = (t, declared) => declared;
		private Func<System.Type, bool, bool> isTablePerClass = (t, declared) => declared;
		private Func<SplitDefinition, bool, bool> isTablePerClassSplit = (sd, declared) => declared;
		private Func<System.Type, bool, bool> isTablePerClassHierarchy = (t, declared) => declared;
		private Func<System.Type, bool, bool> isTablePerConcreteClass = (t, declared) => declared;
		private Func<System.Type, IEnumerable<string>, IEnumerable<string>> splitsForType = (t, declared) => declared;
		private Func<System.Type, bool, bool> isComponent;

		private Func<MemberInfo, bool, bool> isPersistentId;
		private Func<MemberInfo, bool, bool> isPersistentProperty;
		private Func<MemberInfo, bool, bool> isVersion = (m, declared) => declared;

		private Func<MemberInfo, bool, bool> isProperty = (m, declared) => declared;
		private Func<MemberInfo, bool, bool> isAny = (m, declared) => declared;
		private Func<MemberInfo, bool, bool> isManyToMany = (m, declared) => declared;
		private Func<MemberInfo, bool, bool> isManyToOne = (m, declared) => declared;
		private Func<MemberInfo, bool, bool> isMemberOfNaturalId = (m, declared) => declared;
		private Func<MemberInfo, bool, bool> isOneToMany = (m, declared) => declared;
		private Func<MemberInfo, bool, bool> isOneToOne = (m, declared) => declared;

		private Func<MemberInfo, bool, bool> isSet;
		private Func<MemberInfo, bool, bool> isArray;
		private Func<MemberInfo, bool, bool> isBag;
		private Func<MemberInfo, bool, bool> isDictionary;
		private Func<MemberInfo, bool, bool> isIdBag = (m, declared) => declared;
		private Func<MemberInfo, bool, bool> isList;

		public SimpleModelInspector()
		{
			isPersistentId = (m, declared) => declared || MatchPoIdPattern(m);
			isComponent = (t, declared) => declared || MatchComponentPattern(t);
			isPersistentProperty = (m, declared) => declared || MatchNoReadOnlyPropertyPattern(m);
			isSet = (m, declared) => declared || MatchCollection(m, MatchSetMember);
			isArray = (m, declared) => declared;
			isBag = (m, declared) => declared || MatchCollection(m, MatchBagMember);
			isDictionary = (m, declared) => declared || MatchCollection(m, MatchDictionaryMember);
			isList = (m, declared) => declared;
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
				return memberType.GetGenericIntercafesTypeDefinitions().Contains(typeof(IDictionary<,>));
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
			if (typeof(ISet).IsAssignableFrom(memberType))
			{
				return true;
			}
			if (memberType.IsGenericType)
			{
				return memberType.GetGenericIntercafesTypeDefinitions().Contains(typeof(Iesi.Collections.Generic.ISet<>));
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

			var modelInspector = (IModelInspector) this;
			return !subject.IsEnum && !subject.Namespace.StartsWith("System") /* hack */&& !modelInspector.IsEntity(subject)
			       &&
			       !subject.GetProperties(flattenHierarchyMembers).Cast<MemberInfo>().Concat(
			       	subject.GetFields(flattenHierarchyMembers)).Any(m => modelInspector.IsPersistentId(m));
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
			get { return declaredModel.ManyToManyRelations; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.ManyToManyRelations
		{
			get { return declaredModel.ManyToManyRelations; }
		}

		IEnumerable<MemberInfo> IModelExplicitDeclarationsHolder.OneToManyRelations
		{
			get { return declaredModel.OneToManyRelations; }
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

		void IModelExplicitDeclarationsHolder.AddAsManyToManyRelation(MemberInfo member)
		{
			declaredModel.AddAsManyToManyRelation(member);
		}

		void IModelExplicitDeclarationsHolder.AddAsOneToManyRelation(MemberInfo member)
		{
			declaredModel.AddAsOneToManyRelation(member);
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

		#endregion

		#region Implementation of IModelInspector

		bool IModelInspector.IsRootEntity(System.Type type)
		{
			bool declaredResult = declaredModel.IsRootEntity(type);
			return isRootEntity(type, declaredResult);
		}

		bool IModelInspector.IsComponent(System.Type type)
		{
			bool declaredResult = declaredModel.IsComponent(type);
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
			bool declaredResult = declaredModel.IsOneToOne(member);
			return isOneToOne(member, declaredResult);
		}

		bool IModelInspector.IsManyToOne(MemberInfo member)
		{
			bool declaredResult = declaredModel.IsManyToOne(member);
			return isManyToOne(member, declaredResult);
		}

		bool IModelInspector.IsManyToMany(MemberInfo member)
		{
			bool declaredResult = declaredModel.IsManyToMany(member);
			return isManyToMany(member, declaredResult);
		}

		bool IModelInspector.IsOneToMany(MemberInfo member)
		{
			bool declaredResult = declaredModel.IsOneToMany(member);
			return isOneToMany(member, declaredResult);
		}

		bool IModelInspector.IsAny(MemberInfo member)
		{
			bool declaredResult = declaredModel.IsAny(member);
			return isAny(member, declaredResult);
		}

		bool IModelInspector.IsPersistentId(MemberInfo member)
		{
			bool declaredResult = declaredModel.IsPersistentId(member);
			return isPersistentId(member, declaredResult);
		}

		bool IModelInspector.IsVersion(MemberInfo member)
		{
			bool declaredResult = declaredModel.IsVersion(member);
			return isVersion(member, declaredResult);
		}

		bool IModelInspector.IsMemberOfNaturalId(MemberInfo member)
		{
			bool declaredResult = declaredModel.IsMemberOfNaturalId(member);
			return isMemberOfNaturalId(member, declaredResult);
		}

		bool IModelInspector.IsPersistentProperty(MemberInfo member)
		{
			bool declaredResult = declaredModel.IsPersistentProperty(member);
			return isPersistentProperty(member, declaredResult);
		}

		bool IModelInspector.IsSet(MemberInfo role)
		{
			bool declaredResult = declaredModel.IsSet(role);
			return isSet(role, declaredResult);
		}

		bool IModelInspector.IsBag(MemberInfo role)
		{
			bool declaredResult = declaredModel.IsBag(role);
			return isBag(role, declaredResult);
		}

		bool IModelInspector.IsIdBag(MemberInfo role)
		{
			bool declaredResult = declaredModel.IsIdBag(role);
			return isIdBag(role, declaredResult);
		}

		bool IModelInspector.IsList(MemberInfo role)
		{
			bool declaredResult = declaredModel.IsList(role);
			return isList(role, declaredResult);
		}

		bool IModelInspector.IsArray(MemberInfo role)
		{
			bool declaredResult = declaredModel.IsArray(role);
			return isArray(role, declaredResult);
		}

		bool IModelInspector.IsDictionary(MemberInfo role)
		{
			bool declaredResult = declaredModel.IsDictionary(role);
			return isDictionary(role, declaredResult);
		}

		bool IModelInspector.IsProperty(MemberInfo member)
		{
			bool declaredResult = declaredModel.IsProperty(member);
			return isProperty(member, declaredResult);
		}

		IEnumerable<string> IModelInspector.GetPropertiesSplits(System.Type type)
		{
			IEnumerable<string> declaredResult = declaredModel.GetPropertiesSplits(type);
			return splitsForType(type, declaredResult);
		}

		#endregion

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
	}
}