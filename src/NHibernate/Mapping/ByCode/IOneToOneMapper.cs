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
}