using NHibernate.Properties;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class NoMemberPropertyMapper : IAccessorPropertyMapper
	{
		#region IAccessorPropertyMapper Members

		public void Access(Accessor accessor) {}

		public void Access(System.Type accessorType) {}

		public void Access<T>() where T : IPropertyAccessor, new()
		{}

		#endregion
	}
}