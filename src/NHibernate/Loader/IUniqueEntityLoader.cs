using System;
using NHibernate.Engine;

namespace NHibernate.Loader 
{
	public interface IUniqueEntityLoader 
	{
		/// <summary>
		/// Load an entity instance. If <c>OptionalObject</c> is supplied, load the entity
		/// state into the given (uninitialized) object
		/// </summary>
		/// <param name="session"></param>
		/// <param name="id"></param>
		/// <param name="optionalObject"></param>
		/// <returns></returns>
		object Load(ISessionImplementor session, object id, object optionalObject);
	}
}
