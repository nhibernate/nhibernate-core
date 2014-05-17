using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public interface IOneToOneMapper : IEntityPropertyMapper
	{
		void Cascade(Cascade cascadeStyle);
		void Lazy(LazyRelation lazyRelation);
		void Constrained(bool value);
		void PropertyReference(MemberInfo propertyInTheOtherSide);
		void Formula(string formula);
		void ForeignKey(string foreignKeyName);
	}

	public interface IOneToOneMapper<T> : IOneToOneMapper
	{
		void PropertyReference<TProperty>(Expression<Func<T, TProperty>> reference);
	}
}