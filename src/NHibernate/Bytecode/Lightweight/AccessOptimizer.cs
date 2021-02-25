using System;
using System.Linq;
using System.Security;
using NHibernate.Properties;
using NHibernate.Util;

namespace NHibernate.Bytecode.Lightweight
{
	public class AccessOptimizer : IAccessOptimizer
	{
		private readonly GetPropertyValuesInvoker getDelegate;
		private readonly SetPropertyValuesInvoker setDelegate;
		private readonly GetterCallback getterCallback;
		private readonly SetterCallback setterCallback;
		private readonly GetPropertyValueInvoker[] _getters;
		private readonly SetPropertyValueInvoker[] _setters;
		private readonly GetPropertyValueInvoker _specializedGetter;
		private readonly SetPropertyValueInvoker _specializedSetter;

		// Since 5.3
		[Obsolete("This constructor has no usages and will be removed in a future version")]
		public AccessOptimizer(
			GetPropertyValuesInvoker getDelegate,
			SetPropertyValuesInvoker setDelegate,
			IGetter[] getters,
			ISetter[] setters)
			: this(
				getDelegate,
				setDelegate,
				getters.ToArray(o => (GetPropertyValueInvoker) o.Get),
				setters.ToArray(o => (SetPropertyValueInvoker) o.Set),
				null,
				null)
		{
		}

		public AccessOptimizer(GetPropertyValuesInvoker getDelegate,
								SetPropertyValuesInvoker setDelegate,
								GetPropertyValueInvoker[] getters,
								SetPropertyValueInvoker[] setters,
								GetPropertyValueInvoker specializedGetter,
								SetPropertyValueInvoker specializedSetter)
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
			_setters[i](target, value);
		}

		public object GetPropertyValue(object target, int i)
		{
			return _getters[i](target);
		}

		internal void SetSpecializedPropertyValue(object target, object value)
		{
			_specializedSetter(target, value);
		}

		internal object GetSpecializedPropertyValue(object target)
		{
			return _specializedGetter(target);
		}
	}
}
