using System;

namespace NHibernate.Cfg.MappingSchema
{
	public static class MappingExtensions
	{
		public static EntityMode ToEntityMode(this HbmTuplizerEntitymode source)
		{
			switch (source)
			{
				case HbmTuplizerEntitymode.Poco:
					return EntityMode.Poco;
				case HbmTuplizerEntitymode.Xml:
					return EntityMode.Xml;
				case HbmTuplizerEntitymode.DynamicMap:
					return EntityMode.Map;
				default:
					throw new ArgumentOutOfRangeException("source");
			}
		}
	}
}