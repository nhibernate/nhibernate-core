using System;
using System.Linq;
using System.Security;
using NHibernate.Properties;

namespace NHibernate.Bytecode.Lightweight
{
	public class AccessOptimizer : IAccessOptimizer
	{
		private readonly GetPropertyValuesInvoker getDelegate;
		private readonly SetPropertyValuesInvoker setDelegate;
		private readonly GetterCallback getterCallback;
		private readonly SetterCallback setterCallback;
		private readonly Getter[] _getters;
		private readonly Setter[] _setters;
		private readonly Getter _specializedGetter;
		private readonly Setter _specializedSetter;

		// Since 5.3
		[Obsolete("This constructor has no usages and will be removed in a future version")]
		public AccessOptimizer(GetPropertyValuesInvoker getDelegate, SetPropertyValuesInvoker setDelegate,
		                       IGetter[] getters, ISetter[] setters)
			: this(
				getDelegate,
				setDelegate, 
				getters.Select(o => new Getter(o)).ToArray(),
				setters.Select(o => new Setter(o)).ToArray(),
				null,
				null)
		{
		}

		public AccessOptimizer(GetPropertyValuesInvoker getDelegate,
								SetPropertyValuesInvoker setDelegate,
								Getter[] getters,
								Setter[] setters,
								Getter specializedGetter,
								Setter specializedSetter)
		{
			this.getDelegate = getDelegate;
			this.setDelegate = setDelegate;
			_getters = getters;
			_setters = setters;
			_specializedGetter = specializedGetter;
			_specializedSetter = specializedSetter;
			getterCallback = GetPropertyValue;
			setterCallback = SetPropertyValue;
		}

		public object[] GetPropertyValues(object target)
		{
			return getDelegate(target, getterCallback);
		}

		public void SetPropertyValues(object target, object[] values)
		{
			setDelegate(target, values, setterCallback);
		}

		public void SetPropertyValue(object target, int i, object value)
		{
			SetPropertyValue(target, value, _setters[i]);
		}

		public object GetPropertyValue(object target, int i)
		{
			return GetPropertyValue(target, _getters[i]);
		}

		internal void SetSpecializedPropertyValue(object target, object value)
		{
			SetPropertyValue(target, value, _specializedSetter);
		}

		internal object GetSpecializedPropertyValue(object target)
		{
			return GetPropertyValue(target, _specializedGetter);
		}

		private static object GetPropertyValue(object target, Getter getter)
		{
			if (getter.Optimized == null)
			{
				return getter.Default.Get(target);
			}

			return getter.Optimized(target);
		}

		private static void SetPropertyValue(object target, object value, Setter setter)
		{
			if (setter.Optimized == null)
			{
				setter.Default.Set(target, value);
			}
			else
			{
				setter.Optimized(target, value);
			}
		}
	}
}
