using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Mapping.ByCode.Impl.CustomizersImpl;

namespace NHibernate.Mapping.ByCode.Conformist
{
	public class UnionSubclassMapping<T> : UnionSubclassCustomizer<T> where T : class
	{
		public UnionSubclassMapping() : base(new ExplicitDeclarationsHolder(), new CustomizersHolder()) { }
	}
}