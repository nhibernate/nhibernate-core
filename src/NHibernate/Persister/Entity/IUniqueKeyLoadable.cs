using NHibernate.Engine;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// Describes a class that may be loaded via a unique key.
	/// </summary>
	public partial interface IUniqueKeyLoadable
	{
		/// <summary>
		/// Load an instance of the persistent class, by a unique key other than the primary key.
		/// </summary>
		object LoadByUniqueKey(string propertyName, object uniqueKey, ISessionImplementor session);

		/// <summary>
		/// Get the property number of the unique key property
		/// </summary>
		int GetPropertyIndex(string propertyName);
	}

	public static partial class UniqueKeyLoadableExtensions
	{
		private static readonly INHibernateLogger Logger = NHibernateLogger.For(typeof(UniqueKeyLoadableExtensions));

		// 6.0 TODO: merge in IUniqueKeyLoadable
		public static void CacheByUniqueKeys(
			this IUniqueKeyLoadable ukLoadable,
			object entity,
			ISessionImplementor session)
		{
			if (ukLoadable is AbstractEntityPersister persister)
			{
				persister.CacheByUniqueKeys(entity, session);
				return;
			}

			// Use reflection for supporting custom persisters.
			var ukLoadableType = ukLoadable.GetType();
			var cacheByUniqueKeysMethod = ukLoadableType.GetMethod(
				nameof(AbstractEntityPersister.CacheByUniqueKeys),
				new[] { typeof(object), typeof(ISessionImplementor) });
			if (cacheByUniqueKeysMethod != null)
			{
				cacheByUniqueKeysMethod.Invoke(ukLoadable, new[] { entity, session });
				return;
			}

			Logger.Warn(
				"{0} does not implement 'void CacheByUniqueKeys(object, ISessionImplementor)', " +
				"some caching may be lacking",
				ukLoadableType);
		}
	}
}
