using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Mapping.ByCode.Impl.CustomizersImpl;

namespace NHibernate.Mapping.ByCode.Conformist
{
	public class SubclassMapping<T> : SubclassCustomizer<T> where T : class
	{
		public SubclassMapping() : base(new ExplicitDeclarationsHolder(), new CustomizersHolder()) { }
	}
}