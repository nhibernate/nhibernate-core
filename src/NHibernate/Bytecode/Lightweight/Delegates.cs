namespace NHibernate.Bytecode.Lightweight
{
	public delegate void SetterCallback(object obj, int index, object value);

	public delegate object GetterCallback(object obj, int index);

	public delegate object[] GetPropertyValuesInvoker(object obj, GetterCallback callback);

	public delegate void SetPropertyValuesInvoker(object obj, object[] values, SetterCallback callback);

	public delegate object CreateInstanceInvoker();
}
