using System.Collections.Generic;

namespace NHibernate.Test.UserCollection.Parameterized
{
	public class DefaultableListImpl : List<string>, IDefaultableList
	{
		public DefaultableListImpl() {}
		public DefaultableListImpl(int capacity) : base(capacity) {}

		#region IDefaultableList Members

		public string DefaultValue { get; set; }

		#endregion
	}
}