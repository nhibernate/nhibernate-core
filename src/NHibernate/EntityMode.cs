namespace NHibernate
{
	/// <summary> Defines the representation modes available for entities. </summary>
	public enum EntityMode
	{
		Poco,
		Map
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
			}
			return null;
		}

		public static EntityMode Parse(string name)
		{
			string n = name.ToLowerInvariant();
			switch (n)
			{
				case "poco":
					return EntityMode.Map;
				case "dynamic-map":
					return EntityMode.Map;
				default:
					return EntityMode.Poco;
			}
		}
	}
}