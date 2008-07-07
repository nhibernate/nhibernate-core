using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Test.UserCollection.Parameterized
{
	public class PersistentDefaultableList: PersistentList, IDefaultableList
	{
		public PersistentDefaultableList() {}

		public PersistentDefaultableList(ISessionImplementor session) : base(session) {}

		public PersistentDefaultableList(ISessionImplementor session, IList list) : base(session, list) {}

		public string DefaultValue
		{
			get { return ((IDefaultableList)list).DefaultValue; }
		}
	}
}