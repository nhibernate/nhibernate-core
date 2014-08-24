namespace NHibernate.Mapping.ByCode
{
	public static class CascadeExtensions
	{
		private const Cascade AnyButOrphans = Cascade.Persist | Cascade.Refresh | Cascade.Merge | Cascade.Remove | Cascade.Detach | Cascade.ReAttach;

		public static bool Has(this Cascade source, Cascade value)
		{
			return (source & value) == value;
		}

		public static Cascade Include(this Cascade source, Cascade value)
		{
			return Cleanup(source | value);
		}

		private static Cascade Cleanup(Cascade cascade)
		{
			bool hasAll = cascade.Has(Cascade.All) || cascade.Has(AnyButOrphans);
			if (hasAll && cascade.Has(Cascade.DeleteOrphans))
			{
				return Cascade.All | Cascade.DeleteOrphans;
			}
			if (hasAll)
			{
				return Cascade.All;
			}
			return cascade;
		}

		public static Cascade Exclude(this Cascade source, Cascade value)
		{
			if (source.Has(Cascade.All) && !value.Has(Cascade.All))
			{
				return Cleanup(((source & ~Cascade.All) | AnyButOrphans) & ~value);
			}
			return Cleanup(source & ~value);
		}
	}
}