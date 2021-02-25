namespace NHibernate.Cfg.MappingSchema
{
	public interface IEntityPropertyMapping: IDecoratable
	{
		string Name { get; }
		string Access { get; }
		bool OptimisticLock { get; }
		bool IsLazyProperty { get; }
	}

	internal static class EntityPropertyMappingExtensiosns
	{
		// 6.0 TODO: Move to IEntityPropertyMapping as a property
		public static string GetLazyGroup(this IEntityPropertyMapping mapping)
		{
			if (mapping is HbmProperty property)
			{
				return property.lazygroup;
			}

			if (mapping is HbmComponent component)
			{
				return component.lazygroup;
			}

			return null;
		}
	}
}
