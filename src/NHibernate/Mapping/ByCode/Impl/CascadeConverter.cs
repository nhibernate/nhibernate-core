using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Mapping.ByCode.Impl
{
	public static class CascadeConverter
	{
		public static string ToCascadeString(this Cascade source)
		{
			return source == Cascade.None ? null : string.Join(",", source.CascadeDefinitions().ToArray());
		}

		private static IEnumerable<string> CascadeDefinitions(this Cascade source)
		{
			if (source.Has(Cascade.All))
			{
				yield return "all";
			}
			if (source.Has(Cascade.Persist))
			{
				yield return "save-update, persist";
			}
			if (source.Has(Cascade.Refresh))
			{
				yield return "refresh";
			}
			if (source.Has(Cascade.Merge))
			{
				yield return "merge";
			}
			if (source.Has(Cascade.Remove))
			{
				yield return "delete";
			}
			if (source.Has(Cascade.Detach))
			{
				yield return "evict";
			}
			if (source.Has(Cascade.ReAttach))
			{
				yield return "lock";
			}
			if (source.Has(Cascade.DeleteOrphans))
			{
				yield return "delete-orphan";
			}
		}
	}
}