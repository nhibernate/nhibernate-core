using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Mapping.ByCode.Impl.CustomizersImpl;

namespace NHibernate.Mapping.ByCode.Conformist
{
	public class ClassMapping<T> : ClassCustomizer<T> where T : class
	{
		public ClassMapping() : base(new ExplicitDeclarationsHolder(), new CustomizersHolder()) { }
	}
}