using System;
using System.Collections;
using System.Text;

using NHibernate.Mapping;
using NHibernate.Properties;
using NHibernate.Proxy;
using NHibernate.Proxy.Map;

namespace NHibernate.Tuple.Entity
{
	public class DynamicMapEntityTuplizer : AbstractEntityTuplizer
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(PocoEntityTuplizer));

		internal DynamicMapEntityTuplizer(EntityMetamodel entityMetamodel, PersistentClass mappingInfo)
			: base(entityMetamodel, mappingInfo)
		{
			// NH different behavior fo NH-1587
			Instantiator = BuildInstantiator(mappingInfo);
		}

		public override System.Type ConcreteProxyClass
		{
			get { return typeof(IDictionary); }
		}

		public override bool IsInstrumented
		{
			get { return false; }
		}

		public override System.Type MappedClass
		{
			get { return typeof(IDictionary); }
		}

		public override EntityMode EntityMode
		{
			get { return EntityMode.Map; }
		}

		protected override IGetter BuildPropertyGetter(Mapping.Property mappedProperty, PersistentClass mappedEntity)
		{
			return BuildPropertyAccessor(mappedProperty).GetGetter(null, mappedProperty.Name);
		}

		private IPropertyAccessor BuildPropertyAccessor(Mapping.Property property)
		{
			return PropertyAccessorFactory.DynamicMapPropertyAccessor;
		}

		protected override ISetter BuildPropertySetter(Mapping.Property mappedProperty, PersistentClass mappedEntity)
		{
			return BuildPropertyAccessor(mappedProperty).GetSetter(null, mappedProperty.Name);
		}

		protected override IInstantiator BuildInstantiator(PersistentClass mappingInfo)
		{
			return new DynamicMapInstantiator(mappingInfo);
		}

		protected override IProxyFactory BuildProxyFactory(PersistentClass mappingInfo, IGetter idGetter,
		                                                            ISetter idSetter)
		{
			IProxyFactory pf = new MapProxyFactory();
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
