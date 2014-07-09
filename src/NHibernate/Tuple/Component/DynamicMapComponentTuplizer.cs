using System;
using NHibernate.Properties;

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

		public override System.Type MappedClass
		{
			get { return typeof(System.Collections.IDictionary); }
		}

		protected internal override IInstantiator BuildInstantiator(Mapping.Component component)
		{
			return new DynamicMapInstantiator();
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
			return PropertyAccessorFactory.DynamicMapPropertyAccessor;
		}

	}
}
