using System.Threading;
using System.Threading.Tasks;

namespace NHibernate
{
	public interface IFutureValueAsync<T>
	{
		Task<T> GetValue(CancellationToken cancellationToken = default(CancellationToken));
	}
}
