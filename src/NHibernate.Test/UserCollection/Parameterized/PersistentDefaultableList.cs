using System.Collections.Generic;
using NHibernate.Collection.Generic;
using NHibernate.Engine;

namespace NHibernate.Test.UserCollection.Parameterized
{
	public class PersistentDefaultableList: PersistentGenericList<string>, IDefaultableList
	{
		public PersistentDefaultableList() {}

		public PersistentDefaultableList(ISessionImplementor session) : base(session) {}

		public PersistentDefaultableList(ISessionImplementor session, IList<string> list) : base(session, list) {}

		public string DefaultValue
		{
			get { return ((IDefaultableList)WrappedList).DefaultValue; }
		}
	}
}