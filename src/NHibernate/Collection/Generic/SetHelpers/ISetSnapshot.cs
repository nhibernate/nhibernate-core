using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Collection.Generic.SetHelpers
{
	internal interface ISetSnapshot<T> : ICollection<T>, ICollection
	{
		bool TryGetValue(T element, out T value);
	}
}