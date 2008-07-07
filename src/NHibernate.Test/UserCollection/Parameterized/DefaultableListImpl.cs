using System.Collections;
namespace NHibernate.Test.UserCollection.Parameterized
{
	public class DefaultableListImpl : ArrayList, IDefaultableList
	{
		private string defaultValue;
		public DefaultableListImpl() {}
		public DefaultableListImpl(int capacity) : base(capacity) {}

		#region IDefaultableList Members

		public string DefaultValue
		{
			get { return defaultValue; }
			set { defaultValue = value; }
		}

		#endregion
	}
}