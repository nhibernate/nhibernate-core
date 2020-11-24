using System;
using System.Linq;
using NHibernate.Mapping;
using NHibernate.Type;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public static class MappingLogExtensions
	{
		public static void LogMapped(this Property property, INHibernateLogger log)
		{
			if (log.IsDebugEnabled())
			{
				var msg = "Mapped property: " + property.Name;
				var columns = string.Join(",", property.Value.ColumnIterator.Select(c => c.Text).ToArray());
				if (columns.Length > 0)
					msg += " -> " + columns;
				var propertyTypeName = SafeGetPropertyTypeName(property);
				if (propertyTypeName != null)
					msg += ", type: " + propertyTypeName;
				log.Debug(msg);
			}
		}

		static string SafeGetPropertyTypeName(Property property)
		{
			try
			{
				//property.Type property can trigger a type load
				if (property.Type != null) return property.Type.Name;
			}
			catch (Exception)
			{
				return "<could not load type>";
			}
			return null;
		}
	}
}
