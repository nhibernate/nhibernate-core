#if NET_2_0
using NHibernate.Property;

namespace NHibernate.Bytecode.Lightweight
{
	public class AccessOptimizer : IAccessOptimizer
	{
		private GetPropertyValuesInvoker getDelegate;
		private SetPropertyValuesInvoker setDelegate;
		private IGetter[] getters;
		private ISetter[] setters;
		private GetterCallback getterCallback;
		private SetterCallback setterCallback;

		public AccessOptimizer(GetPropertyValuesInvoker getDelegate, SetPropertyValuesInvoker setDelegate,
		                       IGetter[] getters, ISetter[] setters)
		{
			this.getDelegate = getDelegate;
			this.setDelegate = setDelegate;
			this.getters = getters;
			this.setters = setters;
			this.getterCallback = new GetterCallback(OnGetterCallback);
			this.setterCallback = new SetterCallback(OnSetterCallback);
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

#endif