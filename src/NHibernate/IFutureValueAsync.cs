#if ASYNC
using System.Threading.Tasks;

namespace NHibernate
{
	public interface IFutureValueAsync<T>
	{
		Task<T> GetValue();
	}
}
#endif