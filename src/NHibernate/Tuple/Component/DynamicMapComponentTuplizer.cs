using System;
using NHibernate.Properties;
using NHibernate.Util;

namespace NHibernate.Tuple.Component
{
	/// <summary> 
	/// A <see cref="IComponentTuplizer"/> specific to the dynamic-map entity mode. 
	/// </summary>
	[Serializable]
	public class DynamicMapComponentTuplizer : AbstractComponentTuplizer
	{
		public DynamicMapComponentTuplizer(Mapping.Component component)
			: base(component)
		{
			// Fix for NH-3119
			instantiator = BuildInstantiator(component);
		}

		public override System.Type MappedClass =>
			typeof(DynamicComponent);

		protected internal override IInstantiator BuildInstantiator(Mapping.Component component) =>
			new DynamicComponentInstantiator();

		protected internal override IGetter BuildGetter(Mapping.Component component, Mapping.Property prop) =>
			PropertyAccessorFactory.DynamicMapPropertyAccessor.GetGetter(null, prop.Name);

		protected internal override ISetter BuildSetter(Mapping.Component component, Mapping.Property prop) =>
			PropertyAccessorFactory.DynamicMapPropertyAccessor.GetSetter(null, prop.Name);
	}
}
