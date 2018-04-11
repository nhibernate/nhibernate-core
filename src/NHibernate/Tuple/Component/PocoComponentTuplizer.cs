using System;
using NHibernate.Bytecode;
using NHibernate.Intercept;
using NHibernate.Properties;
using NHibernate.Util;

namespace NHibernate.Tuple.Component
{
	using System.Runtime.Serialization;

	/// <summary> 
	/// A <see cref="IComponentTuplizer"/> specific to the POCO entity mode. 
	/// </summary>
	[Serializable]
	public class PocoComponentTuplizer : AbstractComponentTuplizer, IDeserializationCallback
	{
		[NonSerialized]
		private System.Type _componentClass;
		private SerializableSystemType _serializableComponentClass;
		private readonly ISetter _parentSetter;
		private readonly IGetter _parentGetter;

		[NonSerialized]
		private IReflectionOptimizer _optimizer;

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			_serializableComponentClass = SerializableSystemType.Wrap(_componentClass);
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			_componentClass = _serializableComponentClass?.GetSystemType();
			SetReflectionOptimizer();

			if (_optimizer != null)
			{
				// Fix for NH-3119:
				// Also set the InstantiationOptimizer on the deserialized PocoInstantiator.
				((PocoInstantiator)instantiator).SetOptimizer(_optimizer.InstantiationOptimizer);
			}

			ClearOptimizerWhenUsingCustomAccessors();
		}

		public PocoComponentTuplizer(Mapping.Component component)
			: base(component)
		{
			_componentClass = component.ComponentClass;

			var parentProperty = component.ParentProperty;
			if (parentProperty == null)
			{
				_parentSetter = null;
				_parentGetter = null;
			}
			else
			{
				_parentSetter = parentProperty.GetSetter(_componentClass);
				_parentGetter = parentProperty.GetGetter(_componentClass);
			}

			SetReflectionOptimizer();

			// Fix for NH-3119
			instantiator = BuildInstantiator(component);

			ClearOptimizerWhenUsingCustomAccessors();
		}

		public override System.Type MappedClass => _componentClass;

		public override object[] GetPropertyValues(object component)
		{
			// NH Different behavior : for NH-1101
			if (Equals(BackrefPropertyAccessor.Unknown, component) || Equals(LazyPropertyInitializer.UnfetchedProperty, component) || component == null)
			{
				return new object[propertySpan];
			}

			if (_optimizer != null && _optimizer.AccessOptimizer != null)
			{
				return _optimizer.AccessOptimizer.GetPropertyValues(component);
			}
			else
			{
				return base.GetPropertyValues(component);
			}
		}

		public override void SetPropertyValues(object component, object[] values)
		{
			if (_optimizer != null && _optimizer.AccessOptimizer != null)
			{
				_optimizer.AccessOptimizer.SetPropertyValues(component, values);
			}
			else
			{
				base.SetPropertyValues(component, values);
			}
		}

		public override object GetParent(object component)
		{
			return _parentGetter.Get(component);
		}

		public override void SetParent(object component, object parent, Engine.ISessionFactoryImplementor factory)
		{
			_parentSetter.Set(component, parent);
		}

		public override bool HasParentProperty
		{
			get { return _parentGetter != null; }
		}

		protected internal override IInstantiator BuildInstantiator(Mapping.Component component)
		{
			// TODO H3.2 not ported
			//if (component.IsEmbedded && ReflectHelper.IsAbstractClass(component.ComponentClass))
			//{
			//  return new ProxiedInstantiator(component);
			//}

			if (_optimizer == null)
			{
				return new PocoInstantiator(component, null);
			}
			else
			{
				return new PocoInstantiator(component, _optimizer.InstantiationOptimizer);
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
				_optimizer = Cfg.Environment.BytecodeProvider.GetReflectionOptimizer(_componentClass, getters, setters);
			}
		}

		protected void ClearOptimizerWhenUsingCustomAccessors()
		{
			if (hasCustomAccessors)
			{
				_optimizer = null;
			}
		}
	}
}
