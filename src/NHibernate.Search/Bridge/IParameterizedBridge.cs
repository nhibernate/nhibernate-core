using System.Collections;

namespace NHibernate.Search
{
	public interface IParameterizedBridge
	{
		void SetParameterValues(object [] parameters);
	}
}