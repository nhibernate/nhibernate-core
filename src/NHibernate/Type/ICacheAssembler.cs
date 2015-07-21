using NHibernate.Engine;

namespace NHibernate.Type
{
	public interface ICacheAssembler
	{
		/// <summary> Return a cacheable "disassembled" representation of the object.</summary>
		/// <param name="value">the value to cache </param>
		/// <param name="session">the session </param>
		/// <param name="owner">optional parent entity object (needed for collections) </param>
		/// <returns> the disassembled, deep cloned state </returns>		
		object Disassemble(object value, ISessionImplementor session, object owner);

		/// <summary> Reconstruct the object from its cached "disassembled" state.</summary>
		/// <param name="cached">the disassembled state from the cache </param>
		/// <param name="session">the session </param>
		/// <param name="owner">the parent entity object </param>
		/// <returns> the the object </returns>
		object Assemble(object cached, ISessionImplementor session, object owner);

		/// <summary>
		/// Called before assembling a query result set from the query cache, to allow batch fetching
		/// of entities missing from the second-level cache.
		/// </summary>
		void BeforeAssemble(object cached, ISessionImplementor session);
	}
}