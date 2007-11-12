using System;
using System.Collections;
using System.Text;
using log4net;
using NHibernate.Mapping;
using NHibernate.Property;
using NHibernate.Proxy;
using NHibernate.Proxy.Map;

namespace NHibernate.Tuple.Entity
{
	public class DynamicMapEntityTuplizer : AbstractEntityTuplizer
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(PocoEntityTuplizer));

		internal DynamicMapEntityTuplizer(EntityMetamodel entityMetamodel, PersistentClass mappingInfo)
			: base(entityMetamodel, mappingInfo) {}

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

		protected internal override IGetter BuildPropertyGetter(Mapping.Property mappedProperty, PersistentClass mappedEntity)
		{
			return BuildPropertyAccessor(mappedProperty).GetGetter(null, mappedProperty.Name);
		}

		private IPropertyAccessor BuildPropertyAccessor(Mapping.Property property)
		{
			return PropertyAccessorFactory.DynamicMapPropertyAccessor;
		}

		protected internal override ISetter BuildPropertySetter(Mapping.Property mappedProperty, PersistentClass mappedEntity)
		{
			return BuildPropertyAccessor(mappedProperty).GetSetter(null, mappedProperty.Name);
		}

		protected internal override IInstantiator BuildInstantiator(PersistentClass mappingInfo)
		{
			return new DynamicMapInstantiator(mappingInfo);
		}

		protected internal override IProxyFactory BuildProxyFactory(PersistentClass mappingInfo, IGetter idGetter,
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
