using System.Linq;


namespace NHibernate.Cfg.XmlHbmBinding
{
	public static class MappingLogExtensions
	{
		public static void LogMapped(this Mapping.Property property, IInternalLogger log)
		{
			if (log.IsDebugEnabled)
			{
				string msg = "Mapped property: " + property.Name;
				string columns = string.Join(",", property.Value.ColumnIterator.Select(c => c.Text).ToArray());
				if (columns.Length > 0)
					msg += " -> " + columns;
				if (property.Type != null)
					msg += ", type: " + property.Type.Name;
				log.Debug(msg);
			}
		}
	}
}