using System;

namespace NHibernate.Cache 
{
	/// <summary>
	/// Implementors manage transactional access to cached data.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Transactions pass in a timestamp indicating transaction start time.
	/// </para>
	/// <para>
	/// When used to Cache Entities and Collections the <c>key</c> is the <c>id</c> of the
	/// Entity/Collection and the <c>value</c> should be set to the <see cref="Impl.CacheEntry"/> 
	/// for an Entity and the results of <see cref="Collection.PersistentCollection"/>.Disassemble for a Collection.
	/// </para>
	/// </remarks>
	public interface ICacheConcurrencyStrategy 
	{		
		/// <summary>
		/// Attempt to retrieve an object from the Cache
		/// </summary>
		/// <param name="key">The key (id) of the object to get out of the Cache.</param>
		/// <param name="txTimestamp">A timestamp prior to the transaction start time</param>
		/// <returns>The cached object or <c>null</c></returns>
		/// <exception cref="CacheException"></exception>
		object Get(object key, long txTimestamp);

		/// <summary>
		/// Attempt to Cache an object 
		/// </summary>
		/// <param name="key">The key (id) of the object to put in the Cache.</param>
		/// <param name="value">The value</param>
		/// <param name="txTimestamp">A timestamp prior to the transaction start time</param>
		/// <returns><c>true</c> if the object was successfully cached</returns>
		/// <exception cref="CacheException"></exception>
		bool Put(object key, object value, long txTimestamp);

		/// <summary>
		/// We are going to attempt to update/delete the keyed object
		/// </summary>
		/// <param name="key">The key</param>
		/// <exception cref="CacheException"></exception>
		void Lock(object key);

		/// <summary>
		/// We have finished the attempted update/delete (which may or may not have been successful)
		/// </summary>
		/// <param name="key">The key</param>
		/// <exception cref="CacheException"></exception>
		void Release(object key);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <exception cref="CacheException"></exception>
		void Remove(object key);

		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="CacheException"></exception>
		void Clear();

		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="CacheException"></exception>
		void Destroy();

		/// <summary>
		/// Gets or sets the <see cref="ICache"/> for this strategy to use.
		/// </summary>
		/// <value>The <see cref="ICache"/> for this strategy to use.</value>
		ICache Cache { get; set ;}
	}
}
