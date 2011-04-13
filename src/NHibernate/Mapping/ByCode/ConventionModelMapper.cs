using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public class ConventionModelMapper : ModelMapper
	{
		public ConventionModelMapper()
			: base(new SimpleModelInspector())
		{
			AppendDefaultEvents();
		}

		protected virtual void AppendDefaultEvents()
		{
			BeforeMapJoinedSubclass += JoinedSubclassKeyAsRootIdColumn;
			BeforeMapProperty += PropertyColumnName;
			BeforeMapList += ManyToManyInCollectionTable;
			BeforeMapBag += ManyToManyInCollectionTable;
			BeforeMapIdBag += ManyToManyInCollectionTable;
			BeforeMapSet += ManyToManyInCollectionTable;
			BeforeMapMap += ManyToManyInCollectionTable;

			BeforeMapList += ManyToManyKeyIdColumn;
			BeforeMapBag += ManyToManyKeyIdColumn;
			BeforeMapIdBag += ManyToManyKeyIdColumn;
			BeforeMapSet += ManyToManyKeyIdColumn;
			BeforeMapMap += ManyToManyKeyIdColumn;
		}

		protected void ManyToManyKeyIdColumn(IModelInspector modelInspector, PropertyPath member, ICollectionPropertiesMapper collectionCustomizer)
		{
			System.Type propertyType = member.LocalMember.GetPropertyOrFieldType();

			System.Type fromMany = member.GetContainerEntity(modelInspector);
			System.Type toMany = propertyType.DetermineCollectionElementOrDictionaryValueType();
			if (!modelInspector.IsEntity(toMany))
			{
				// does not apply when the relation is on the key of the dictionary
				return;
			}

			collectionCustomizer.Key(km => km.Column(fromMany.Name + "Id"));
		}

		protected void ManyToManyInCollectionTable(IModelInspector modelInspector, PropertyPath member, ICollectionPropertiesMapper collectionCustomizer)
		{
			System.Type propertyType = member.LocalMember.GetPropertyOrFieldType();

			System.Type fromMany = member.GetContainerEntity(modelInspector);
			System.Type toMany = propertyType.DetermineCollectionElementOrDictionaryValueType();
			if(!modelInspector.IsEntity(toMany))
			{
				// does not apply when the relation is on the key of the dictionary
				// Note: a dictionary may have relation with 3 entities; in this handler we are covering only relations on values
				return;
			}
			var relation = new[] { fromMany, toMany };
			var twoEntitiesNames = (from relationOn in relation orderby relationOn.Name select relationOn.Name).ToArray();

			collectionCustomizer.Table(string.Format("{0}To{1}", twoEntitiesNames[0], twoEntitiesNames[1]));
		}

		protected void PropertyColumnName(IModelInspector modelInspector, PropertyPath member, IPropertyMapper propertyCustomizer)
		{
			if (member.PreviousPath == null || member.LocalMember == null)
			{
				return;
			}
			if (member.PreviousPath.LocalMember.GetPropertyOrFieldType().IsGenericCollection())
			{
				return;
			}

			var pathToMap = member.DepureFirstLevelIfCollection();
			propertyCustomizer.Column(pathToMap.ToColumnName());
		}

		protected void JoinedSubclassKeyAsRootIdColumn(IModelInspector modelInspector, System.Type type, IJoinedSubclassAttributesMapper joinedSubclassCustomizer)
		{
			var idMember = type.GetProperties().Cast<MemberInfo>().Concat(type.GetFields()).FirstOrDefault(mi => modelInspector.IsPersistentId(mi.GetMemberFromDeclaringType()));
			if (idMember != null)
			{
				joinedSubclassCustomizer.Key(km => km.Column(idMember.Name));
			}
		}

		protected SimpleModelInspector SimpleModelInspector
		{
			get { return (SimpleModelInspector) base.ModelInspector; }
		}

		public void IsRootEntity(Func<System.Type, bool, bool> match)
		{
			SimpleModelInspector.IsRootEntity(match);
		}

		public void IsComponent(Func<System.Type, bool, bool> match)
		{
			SimpleModelInspector.IsComponent(match);
		}

		public void IsEntity(Func<System.Type, bool, bool> match)
		{
			SimpleModelInspector.IsEntity(match);
		}

		public void IsTablePerClass(Func<System.Type, bool, bool> match)
		{
			SimpleModelInspector.IsTablePerClass(match);
		}

		public void IsTablePerClassHierarchy(Func<System.Type, bool, bool> match)
		{
			SimpleModelInspector.IsTablePerClassHierarchy(match);
		}

		public void IsTablePerConcreteClass(Func<System.Type, bool, bool> match)
		{
			SimpleModelInspector.IsTablePerConcreteClass(match);
		}

		public void IsOneToOne(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsOneToOne(match);
		}

		public void IsManyToOne(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsManyToOne(match);
		}

		public void IsManyToMany(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsManyToMany(match);
		}

		public void IsOneToMany(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsOneToMany(match);
		}

		public void IsAny(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsAny(match);
		}

		public void IsPersistentId(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsPersistentId(match);
		}

		public void IsVersion(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsVersion(match);
		}

		public void IsMemberOfNaturalId(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsMemberOfNaturalId(match);
		}

		public void IsPersistentProperty(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsPersistentProperty(match);
		}

		public void IsSet(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsSet(match);
		}

		public void IsBag(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsBag(match);
		}

		public void IsIdBag(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsIdBag(match);
		}

		public void IsList(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsList(match);
		}

		public void IsArray(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsArray(match);
		}

		public void IsDictionary(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsDictionary(match);
		}

		public void IsProperty(Func<MemberInfo, bool, bool> match)
		{
			SimpleModelInspector.IsProperty(match);
		}

		public void SplitsFor(Func<System.Type, IEnumerable<string>, IEnumerable<string>> getPropertiesSplitsId)
		{
			SimpleModelInspector.SplitsFor(getPropertiesSplitsId);
		}

		public void IsTablePerClassSplit(Func<SplitDefinition, bool, bool> match)
		{
			SimpleModelInspector.IsTablePerClassSplit(match);
		}
	}
}