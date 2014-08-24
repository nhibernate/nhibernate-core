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
			SetReflectionOptimizer();

			if (optimizer != null)
			{
				// Fix for NH-3119:
				// Also set the InstantiationOptimizer on the deserialized PocoInstantiator.
				((PocoInstantiator)instantiator).SetOptimizer(optimizer.InstantiationOptimizer);
			}

			ClearOptimizerWhenUsingCustomAccessors();
		}

		public PocoComponentTuplizer(Mapping.Component component)
			: base(component)
		{
			componentClass = component.ComponentClass;

			var parentProperty = component.ParentProperty;
			if (parentProperty == null)
			{
				parentSetter = null;
				parentGetter = null;
			}
			else
			{
				parentSetter = parentProperty.GetSetter(componentClass);
				parentGetter = parentProperty.GetGetter(componentClass);
			}

			SetReflectionOptimizer();

			// Fix for NH-3119
			instantiator = BuildInstantiator(component);

			ClearOptimizerWhenUsingCustomAccessors();
		}

		public override System.Type MappedClass
		{
			get { return componentClass; }
		}

		public override object[] GetPropertyValues(object component)
		{
			// NH Different behavior : for NH-1101
			if (Equals(BackrefPropertyAccessor.Unknown, component) || component == null)
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

		protected void SetReflectionOptimizer()
		{
			if (Cfg.Environment.UseReflectionOptimizer)
			{
				optimizer = Cfg.Environment.BytecodeProvider.GetReflectionOptimizer(componentClass, getters, setters);
			}
		}

		protected void ClearOptimizerWhenUsingCustomAccessors()
		{
			if (hasCustomAccessors)
			{
				optimizer = null;
			}
		}
	}
}
