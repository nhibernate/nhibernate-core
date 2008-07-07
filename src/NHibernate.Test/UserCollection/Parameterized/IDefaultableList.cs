using System.Collections;
namespace NHibernate.Test.UserCollection.Parameterized
{
	public interface IDefaultableList : IList
	{
		string DefaultValue { get;}
	}
}