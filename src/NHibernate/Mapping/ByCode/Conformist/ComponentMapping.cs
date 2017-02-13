using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Mapping.ByCode.Impl.CustomizersImpl;

namespace NHibernate.Mapping.ByCode.Conformist
{
	public class ComponentMapping<T> : ComponentCustomizer<T>
	{
		public ComponentMapping() : base(new ExplicitDeclarationsHolder(), new CustomizersHolder()) { }
	}
}