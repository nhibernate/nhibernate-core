using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate
{
	public interface IFutureEnumerable<T> : IEnumerable<T>
	{
		IAsyncEnumerable<T> AsyncEnumerable { get; }
	}
}
