namespace NHibernate
{
	/// <summary> Defines the representation modes available for entities. </summary>
	public enum EntityMode
	{
		Poco,
		Map,
		Xml
	}

	public static class EntityModeHelper
	{
		public static string ToString(EntityMode entityMode)
		{
			switch (entityMode)
			{
				case EntityMode.Poco:
					return "poco";
				case EntityMode.Map:
					return "dynamic-map";
				case EntityMode.Xml:
					return "xml";
			}
			return null;
		}

		public static EntityMode Parse(string name)
		{
			string n = name.ToLowerInvariant();
			switch (n)
			{
				case "poco":
					return EntityMode.Poco;
				case "dynamic-map":
					return EntityMode.Map;
				case "xml":
					return EntityMode.Xml;
				default:
					return EntityMode.Poco;
			}
		}
	}
}