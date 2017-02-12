using System;
using System.Collections.Generic;

namespace NHibernate.Util
{
	//Much of this code is taken from Maverick.NET
	/// <summary></summary>
	public static class PropertiesHelper
	{
		public static bool GetBoolean(string property, IDictionary<string, string> properties, bool defaultValue)
		{
			string toParse;
			properties.TryGetValue(property, out toParse);
			bool result;
			return bool.TryParse(toParse, out result) ? result : defaultValue;
		}

		public static bool GetBoolean(string property, IDictionary<string, string> properties)
		{
			return GetBoolean(property, properties, false);
		}

		public static byte? GetByte(string property, IDictionary<string, string> properties, byte? defaultValue)
		{
			string toParse;
			properties.TryGetValue(property, out toParse);
			byte result;
			return byte.TryParse(toParse, out result) ? result : defaultValue;
		}

		public static int GetInt32(string property, IDictionary<string, string> properties, int defaultValue)
		{
			string toParse;
			properties.TryGetValue(property, out toParse);
			int result;
			return int.TryParse(toParse, out result) ? result : defaultValue;
		}

		public static long GetInt64(string property, IDictionary<string, string> properties, long defaultValue)
		{
			string toParse;
			properties.TryGetValue(property, out toParse);
			long result;
			return long.TryParse(toParse, out result) ? result : defaultValue;
		}

		public static string GetString(string property, IDictionary<string, string> properties, string defaultValue)
		{
			string value;
			properties.TryGetValue(property, out value);
			if(value == string.Empty)
			{
				value = null;
			}
			return value ?? defaultValue;
		}

		public static IDictionary<string, string> ToDictionary(string property, string delim, IDictionary<string, string> properties)
		{
			IDictionary<string, string> map = new Dictionary<string, string>();

			string propValue;
			if (properties.TryGetValue(property, out propValue))
			{
				var tokens = new StringTokenizer(propValue, delim, false);
				IEnumerator<string> en = tokens.GetEnumerator();
				while (en.MoveNext())
				{
					string key = en.Current;

					string value = en.MoveNext() ? en.Current : String.Empty;
					map[key] = value;
				}
			}
			return map;
		}
	}
}
