namespace NHibernate.Bytecode.Lightweight
{
	public delegate void SetterCallback(object obj, int index, object value);

	public delegate object GetterCallback(object obj, int index);

	public delegate object[] GetPropertyValuesInvoker(object obj, GetterCallback callback);

	public delegate object GetPropertyValueInvoker(object obj);

	public delegate void SetPropertyValuesInvoker(object obj, object[] values, SetterCallback callback);

	public delegate void SetPropertyValueInvoker(object obj, object value);

	public delegate object CreateInstanceInvoker();
}
