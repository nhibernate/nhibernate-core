using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Id
{
	/// <summary>
	/// An <c>IIdentifierGenerator</c> that returns the current identifier
	/// assigned to an instance
	/// </summary>
	public class Assigned : IIdentifierGenerator
	{
		/// <summary></summary>
		public static readonly Assigned Instance = new Assigned();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object Generate( ISessionImplementor session, object obj )
		{
			if( obj is PersistentCollection )
			{
				throw new IdentifierGenerationException(
					"Illegal use of assigned id generation fro a toplevel collection"
					);
			}

			object id = session.GetPersister( obj ).GetIdentifier( obj );
			if( id == null )
			{
				throw new IdentifierGenerationException(
					"ids for this class must be manually assigned before calling save(): " + obj.GetType().FullName
					);
			}
			return id;
		}
	}
}