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
			BeforeMapClass += NoPoidGuid;
			BeforeMapClass += NoSetterPoidToField;
		}

		protected virtual void NoSetterPoidToField(IModelInspector modelInspector, System.Type type, IClassAttributesMapper classCustomizer)
		{
			MemberInfo poidPropertyOrField = MembersProvider.GetEntityMembersForPoid(type).FirstOrDefault(modelInspector.IsPersistentId);
			if(MatchNoSetterProperty(poidPropertyOrField))
			{
				classCustomizer.Id(idm=> idm.Access(Accessor.NoSetter));
			}
		}

		public bool MatchNoSetterProperty(MemberInfo subject)
		{
			var property = subject as PropertyInfo;
			if (property == null || property.CanWrite || !property.CanRead)
			{
				return false;
			}
			var fieldInfo = PropertyToField.GetBackFieldInfo(property);
			if (fieldInfo != null)
			{
				return fieldInfo.FieldType == property.PropertyType;
			}

			return false;
		}

		protected virtual void NoPoidGuid(IModelInspector modelInspector, System.Type type, IClassAttributesMapper classCustomizer)
		{
			MemberInfo poidPropertyOrField = MembersProvider.GetEntityMembersForPoid(type).FirstOrDefault(mi => modelInspector.IsPersistentId(mi));
			if (!ReferenceEquals(null, poidPropertyOrField))
			{
				return;
			}
			classCustomizer.Id(null, idm=> idm.Generator(Generators.Guid));
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