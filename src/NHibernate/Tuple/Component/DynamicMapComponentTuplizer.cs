using System;
using System.Linq;
using System.Collections.Generic;
using NHibernate.Properties;
using System.Collections;
using System.Reflection;

namespace NHibernate.Tuple.Component
{
	/// <summary> 
	/// A <see cref="IComponentTuplizer"/> specific to the dynamic-map entity mode. 
	/// </summary>
	[Serializable]
	public class DynamicMapComponentTuplizer : AbstractComponentTuplizer
	{
		private readonly Mapping.Component component;

		public DynamicMapComponentTuplizer(Mapping.Component component)
			: base(component)
		{
			this.component = component;
			// Fix for NH-3119
			instantiator = BuildInstantiator(component);
		}

		public override System.Type MappedClass
		{
			get
			{
				if ((this.component != null) && (this.component.Owner.MappedClass != null))
				{
					var ownerFullName = this.component.Owner.MappedClass.FullName;
					var roleName = this.component.RoleName;
					var spare = roleName.Substring(ownerFullName.Length + 1);
					var parts = spare.Split('.');
					var prop = null as PropertyInfo;
					var owner = this.component.Owner.MappedClass;

					for (var i = 0; i < parts.Length; ++i)
					{
						prop = owner.GetProperty(parts[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

						owner = prop.PropertyType;
					}

					return typeof(IDictionary<string, object>).IsAssignableFrom(prop.PropertyType) ? typeof(IDictionary<string, object>) : typeof(IDictionary);
				}
				else
				{
					return typeof(IDictionary);
				}
			}
		}

		protected internal override IInstantiator BuildInstantiator(Mapping.Component component)
		{
			return new DynamicMapInstantiator(component);
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
