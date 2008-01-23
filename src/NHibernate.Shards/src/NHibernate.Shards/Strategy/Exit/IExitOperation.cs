using System.Collections;

namespace NHibernate.Shards.Strategy.Exit
{
	public interface IExitOperation
	{
		IList Apply(IList results);
	}
}