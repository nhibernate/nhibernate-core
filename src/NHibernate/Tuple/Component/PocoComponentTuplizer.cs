using System;
using NHibernate.Bytecode;
using NHibernate.Properties;

namespace NHibernate.Tuple.Component
{
	using System.Runtime.Serialization;

	/// <summary> 
	/// A <see cref="IComponentTuplizer"/> specific to the POCO entity mode. 
	/// </summary>
	[Serializable]
	public class PocoComponentTuplizer : AbstractComponentTuplizer
	{
		private readonly System.Type componentClass;
		private readonly ISetter parentSetter;
		private readonly IGetter parentGetter;
		[NonSerialized]
		private IReflectionOptimizer optimizer;


		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			this.optimizer = Cfg.Environment.BytecodeProvider.GetReflectionOptimizer(componentClass, getters, setters);
		}
		public PocoComponentTuplizer(Mapping.Component component) : base(component)
		{
			componentClass = component.ComponentClass;

			string parentPropertyName = component.ParentProperty;
			if (parentPropertyName == null)
			{
				parentSetter = null;
				parentGetter = null;
			}
			else
			{
				IPropertyAccessor pa = PropertyAccessorFactory.GetPropertyAccessor(null);
				parentSetter = pa.GetSetter(componentClass, parentPropertyName);
				parentGetter = pa.GetGetter(componentClass, parentPropertyName);
			}

			if (hasCustomAccessors || !Cfg.Environment.UseReflectionOptimizer)
			{
				optimizer = null;
			}
			else
			{
				optimizer = Cfg.Environment.BytecodeProvider.GetReflectionOptimizer(componentClass, getters, setters);
			}
		}

		public override System.Type MappedClass
		{
			get { return componentClass; }
		}

		public override object[] GetPropertyValues(object component)
		{
			// NH Different behavior : for NH-1101
			if (component == BackrefPropertyAccessor.Unknown || component == null)
			{
				return new object[propertySpan];
			}

			if (optimizer != null && optimizer.AccessOptimizer != null)
			{
				return optimizer.AccessOptimizer.GetPropertyValues(component);
			}
			else
			{
				return base.GetPropertyValues(component);
			}
		}

		public override void SetPropertyValues(object component, object[] values)
		{
			if (optimizer != null && optimizer.AccessOptimizer != null)
			{
				optimizer.AccessOptimizer.SetPropertyValues(component, values);
			}
			else
			{
				base.SetPropertyValues(component, values);
			}
		}

		public override object GetParent(object component)
		{
			return parentGetter.Get(component);
		}

		public override void SetParent(object component, object parent, Engine.ISessionFactoryImplementor factory)
		{
			parentSetter.Set(component, parent);
		}

		public override bool HasParentProperty
		{
			get { return parentGetter != null; }
		}

		protected internal override IInstantiator BuildInstantiator(Mapping.Component component)
		{
			// TODO H3.2 not ported
			//if (component.IsEmbedded && ReflectHelper.IsAbstractClass(component.ComponentClass))
			//{
			//  return new ProxiedInstantiator(component);
			//}

			if (optimizer == null)
			{
				return new PocoInstantiator(component, null);
			}
			else
			{
				return new PocoInstantiator(component, optimizer.InstantiationOptimizer);
			}
		}

		protected internal override IGetter BuildGetter(Mapping.Component component, Mapping.Property prop)
		{
			return prop.GetGetter(component.ComponentClass);
		}

		protected internal override ISetter BuildSetter(Mapping.Component component, Mapping.Property prop)
		{
			return prop.GetSetter(component.ComponentClass);
		}
	}
}
