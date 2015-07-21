using System;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Mapping.ByCode
{
	[Flags]
	public enum SchemaAction
	{
		None = 0,
		Drop = 1,
		Update = 2,
		Export = 4,
		Validate = 8,
		All = Drop | Update | Export | Validate
	}

	public static class SchemaActionConverter
	{
		public static string ToSchemaActionString(this SchemaAction source)
		{
			return source == SchemaAction.All ? null : string.Join(",", source.SchemaActionDefinitions().ToArray());
		}

		public static bool Has(this SchemaAction source, SchemaAction value)
		{
			return (source & value) == value;
		}

		private static IEnumerable<string> SchemaActionDefinitions(this SchemaAction source)
		{
			if (SchemaAction.None.Equals(source))
			{
				yield return "none";
			}
			else
			{
				if (source.Has(SchemaAction.Drop))
				{
					yield return "drop";
				}
				if (source.Has(SchemaAction.Update))
				{
					yield return "update";
				}
				if (source.Has(SchemaAction.Export))
				{
					yield return "export";
				}
				if (source.Has(SchemaAction.Validate))
				{
					yield return "validate";
				}
			}
		}
	}
}