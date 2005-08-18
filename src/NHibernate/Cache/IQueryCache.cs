using System;
using System.Collections;
using Iesi.Collections;

using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Cache
{
	/// <summary>
	/// Defines the contract for caches capable of storing query results.  These
	/// caches should only concern themselves with storing the matching result ids.
	/// The transactional semantics are necessarily less strict than the semantics
	/// of an item cache.
	/// </summary>
	public interface IQueryCache
	{
		void Clear();
		void Put( QueryKey key, IType[ ] returnTypes, IList result, ISessionImplementor session );
		IList Get( QueryKey key, IType[ ] returnTypes, ISet spaces, ISessionImplementor session );
		void Destroy();
	}
}
