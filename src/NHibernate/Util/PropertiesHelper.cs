using System;
using System.Collections;
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

			return toParse == null ? defaultValue : bool.Parse(toParse);
		}

		public static bool GetBoolean(string property, IDictionary<string, string> properties)
		{
			string toParse;
			properties.TryGetValue(property, out toParse);
			return toParse == null ? false : bool.Parse(properties[property]);
		}

		public static int GetInt32(string property, IDictionary<string, string> properties, int defaultValue)
		{
			string toParse;
			properties.TryGetValue(property, out toParse);
			return toParse == null ? defaultValue : int.Parse(toParse);
		}

		public static long GetInt64(string property, IDictionary<string, string> properties, long defaultValue)
		{
			string toParse;
			properties.TryGetValue(property, out toParse);
			return toParse == null ? defaultValue : long.Parse(toParse);
		}

		public static string GetString(string property, IDictionary<string, string> properties, string defaultValue)
		{
			string value;
			properties.TryGetValue(property, out value);
			return value ?? defaultValue;
		}

		public static IDictionary<string, string> ToDictionary(string property, string delim, IDictionary<string, string> properties)
		{
			IDictionary<string, string> map = new Dictionary<string, string>();
			string propValue = properties[property];
			if (propValue != null)
			{
				StringTokenizer tokens = new StringTokenizer(propValue, delim, false);
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

		public static string[] ToStringArray(string property, string delim, IDictionary properties)
		{
			return ToStringArray((string) properties[property], delim);
		}

		public static string[] ToStringArray(string propValue, string delim)
		{
			if (propValue != null)
			{
				return StringHelper.Split(delim, propValue);
			}
			else
			{
				return new string[0];
			}
		}
	}
}