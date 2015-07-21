using NHibernate.Properties;

namespace NHibernate.Bytecode.Lightweight
{
	public class AccessOptimizer : IAccessOptimizer
	{
		private readonly GetPropertyValuesInvoker getDelegate;
		private readonly SetPropertyValuesInvoker setDelegate;
		private readonly IGetter[] getters;
		private readonly ISetter[] setters;
		private readonly GetterCallback getterCallback;
		private readonly SetterCallback setterCallback;

		public AccessOptimizer(GetPropertyValuesInvoker getDelegate, SetPropertyValuesInvoker setDelegate,
		                       IGetter[] getters, ISetter[] setters)
		{
			this.getDelegate = getDelegate;
			this.setDelegate = setDelegate;
			this.getters = getters;
			this.setters = setters;
			getterCallback = OnGetterCallback;
			setterCallback = OnSetterCallback;
		}

		public object[] GetPropertyValues(object target)
		{
			return getDelegate(target, getterCallback);
		}

		public void SetPropertyValues(object target, object[] values)
		{
			setDelegate(target, values, setterCallback);
		}

		private object OnGetterCallback(object target, int i)
		{
			return getters[i].Get(target);
		}

		private void OnSetterCallback(object target, int i, object value)
		{
			setters[i].Set(target, value);
		}
	}
}
