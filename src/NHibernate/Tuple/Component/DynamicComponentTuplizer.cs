using NHibernate.Properties;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace NHibernate.Tuple.Component
{
    public class DynamicComponentTuplizer : AbstractComponentTuplizer
    {
        public DynamicComponentTuplizer(Mapping.Component component) : base(component) { }

        public override System.Type MappedClass
        {
            get { return typeof(IDynamicMetaObjectProvider); }
        }

        protected internal override IInstantiator BuildInstantiator(Mapping.Component component)
        {
            return new DynamicInstantiator();
        }

        protected internal override IGetter BuildGetter(Mapping.Component component, Mapping.Property prop)
        {
            return BuildPropertyAccessor(prop).GetGetter(null, prop.Name);
        }

        protected internal override ISetter BuildSetter(Mapping.Component component, Mapping.Property prop)
        {
            return BuildPropertyAccessor(prop).GetSetter(null, prop.Name);
        }

        private IPropertyAccessor BuildPropertyAccessor(Mapping.Property property)
        {
            return PropertyAccessorFactory.DynamicPropertyAccessor;
        }

    }
}
