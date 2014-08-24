using NHibernate.Mapping;
using NHibernate.Tuple.Entity;

namespace NHibernate.Test.DynamicEntity.Tuplizer
{
	public class MyEntityTuplizer : PocoEntityTuplizer
	{
		public MyEntityTuplizer(EntityMetamodel entityMetamodel, PersistentClass mappedEntity) : base(entityMetamodel, mappedEntity) {}

		protected override Tuple.IInstantiator BuildInstantiator(PersistentClass persistentClass)
		{
			return new MyEntityInstantiator(persistentClass.MappedClass);
		}

		protected override Proxy.IProxyFactory BuildProxyFactory(PersistentClass persistentClass, NHibernate.Properties.IGetter idGetter, NHibernate.Properties.ISetter idSetter)
		{
			// allows defining a custom proxy factory, which is responsible for
			// generating lazy proxies for a given entity.
			//
			// Here we simply use the default...
			return base.BuildProxyFactory(persistentClass, idGetter, idSetter);
		}
	}
}