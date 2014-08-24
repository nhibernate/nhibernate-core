namespace NHibernate.Event
{
	public static class LoadEventListener
	{
		public static readonly LoadType Reload;
		public static readonly LoadType Get;
		public static readonly LoadType Load;
		public static readonly LoadType ImmediateLoad;
		public static readonly LoadType InternalLoadEager;
		public static readonly LoadType InternalLoadLazy;
		public static readonly LoadType InternalLoadNullable;

		static LoadEventListener()
		{
			Reload = new LoadType("Get")
				.SetAllowNulls(false)
				.SetAllowProxyCreation(false)
				.SetCheckDeleted(true)
				.SetNakedEntityReturned(false)
				.SetExactPersister(true); // NH: Different behavior to pass NH-295

			Get = new LoadType("Get")
				.SetAllowNulls(true)
				.SetAllowProxyCreation(false)
				.SetCheckDeleted(true)
				.SetNakedEntityReturned(false)
				.SetExactPersister(true); // NH: Different behavior to pass NH-295

			Load = new LoadType("Load")
				.SetAllowNulls(false)
				.SetAllowProxyCreation(true)
				.SetCheckDeleted(true)
				.SetNakedEntityReturned(false);

			ImmediateLoad = new LoadType("ImmediateLoad")
				.SetAllowNulls(true)
				.SetAllowProxyCreation(false)
				.SetCheckDeleted(false)
				.SetNakedEntityReturned(true);

			InternalLoadEager = new LoadType("InternalLoadEager")
				.SetAllowNulls(false)
				.SetAllowProxyCreation(false)
				.SetCheckDeleted(false)
				.SetNakedEntityReturned(false);

			InternalLoadLazy = new LoadType("InternalLoadLazy")
				.SetAllowNulls(false)
				.SetAllowProxyCreation(true)
				.SetCheckDeleted(false)
				.SetNakedEntityReturned(false);

			InternalLoadNullable = new LoadType("InternalLoadNullable")
				.SetAllowNulls(true)
				.SetAllowProxyCreation(false)
				.SetCheckDeleted(false)
				.SetNakedEntityReturned(false);
		}
	}
}
