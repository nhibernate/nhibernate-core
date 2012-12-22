using NHibernate.Mapping;
using NHibernate.Properties;
using NHibernate.Proxy;
using NHibernate.Proxy.Dynamic;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace NHibernate.Tuple.Entity
{
    public class DynamicEntityTuplizer : AbstractEntityTuplizer
    {

        private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(DynamicEntityTuplizer));

        internal DynamicEntityTuplizer(EntityMetamodel entityMetamodel, PersistentClass mappingInfo)
			: base(entityMetamodel, mappingInfo)
		{
			// NH different behavior fo NH-1587
			Instantiator = BuildInstantiator(mappingInfo);
		}

		public override System.Type ConcreteProxyClass
		{
			get { return typeof(IDynamicMetaObjectProvider); }
		}

		public override bool IsInstrumented
		{
			get { return false; }
		}

		public override System.Type MappedClass
		{
            get { return typeof(IDynamicMetaObjectProvider); }
		}

		public override EntityMode EntityMode
		{
			get { return EntityMode.Dynamic; }
		}

		protected override IGetter BuildPropertyGetter(Mapping.Property mappedProperty, PersistentClass mappedEntity)
		{
			return BuildPropertyAccessor(mappedProperty).GetGetter(null, mappedProperty.Name);
		}

		private IPropertyAccessor BuildPropertyAccessor(Mapping.Property property)
		{
			return PropertyAccessorFactory.DynamicPropertyAccessor;
		}

		protected override ISetter BuildPropertySetter(Mapping.Property mappedProperty, PersistentClass mappedEntity)
		{
			return BuildPropertyAccessor(mappedProperty).GetSetter(null, mappedProperty.Name);
		}

		protected override IInstantiator BuildInstantiator(PersistentClass mappingInfo)
		{
			return new DynamicInstantiator(mappingInfo);
		}

		protected override IProxyFactory BuildProxyFactory(PersistentClass mappingInfo, IGetter idGetter,
		                                                            ISetter idSetter)
		{
			IProxyFactory pf = new DynamicProxyFactory();
			try
			{
				//TODO: design new lifecycle for ProxyFactory
				pf.PostInstantiate(EntityName, null, null, null, null, null);
			}
			catch (HibernateException he)
			{
				log.Warn("could not create proxy factory for:" + EntityName, he);
				pf = null;
			}
			return pf;
		}
    }
}
