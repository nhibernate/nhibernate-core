using System.Collections.Generic;

namespace NHibernate.Test.UserCollection.Parameterized
{
	public interface IDefaultableList : IList<string>
	{
		string DefaultValue { get;}
	}
}