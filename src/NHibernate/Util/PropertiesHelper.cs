using System;
using System.Collections.Generic;
using NHibernate.Cfg;

namespace NHibernate.Util
{
	//Much of this code is taken from Maverick.NET
	/// <summary></summary>
	public static class PropertiesHelper
	{
		public static bool GetBoolean(string property, IDictionary<string, string> properties, bool defaultValue)
		{
			if (properties == null)
				throw new ArgumentNullException(nameof(properties));
			properties.TryGetValue(property, out var toParse);
			return bool.TryParse(toParse, out var result) ? result : defaultValue;
		}

		public static bool GetBoolean(string property, IDictionary<string, string> properties)
		{
			return GetBoolean(property, properties, false);
		}

		public static byte? GetByte(string property, IDictionary<string, string> properties, byte? defaultValue)
		{
			if (properties == null)
				throw new ArgumentNullException(nameof(properties));
			properties.TryGetValue(property, out var toParse);
			return byte.TryParse(toParse, out var result) ? result : defaultValue;
		}

		public static int GetInt32(string property, IDictionary<string, string> properties, int defaultValue)
		{
			if (properties == null)
				throw new ArgumentNullException(nameof(properties));
			properties.TryGetValue(property, out var toParse);
			return int.TryParse(toParse, out var result) ? result : defaultValue;
		}

		public static long GetInt64(string property, IDictionary<string, string> properties, long defaultValue)
		{
			if (properties == null)
				throw new ArgumentNullException(nameof(properties));
			properties.TryGetValue(property, out var toParse);
			return long.TryParse(toParse, out var result) ? result : defaultValue;
		}

		public static string GetString(string property, IDictionary<string, string> properties, string defaultValue)
		{
			if (properties == null)
				throw new ArgumentNullException(nameof(properties));
			properties.TryGetValue(property, out var value);
			if (value == string.Empty)
			{
				value = null;
			}
			return value ?? defaultValue;
		}

		public static IDictionary<string, string> ToDictionary(string property, string delim, IDictionary<string, string> properties)
		{
			if (properties == null)
				throw new ArgumentNullException(nameof(properties));
			var map = new Dictionary<string, string>();

			if (properties.TryGetValue(property, out var propValue))
			{
				var tokens = new StringTokenizer(propValue, delim, false);
				using (var en = tokens.GetEnumerator())
				{
					while (en.MoveNext())
					{
						var key = en.Current;
						var value = en.MoveNext() ? en.Current : string.Empty;
						map[key] = value;
					}
				}
			}
			return map;
		}

		/// <summary>
		/// Get an instance of <typeparamref name="TService"/> type by using the <see cref="Cfg.Environment.ServiceProvider"/>.
		/// </summary>
		/// <typeparam name="TService">The type to instantiate.</typeparam>
		/// <param name="property">The configuration property name.</param>
		/// <param name="properties">The configuration properties.</param>
		/// <param name="defaultType">The default type to instantiate.</param>
		/// <returns>The instance of the <typeparamref name="TService"/> type, or <see langword="null" /> if none is
		/// configured and <paramref name="defaultType"/> is <see langword="null" />.</returns>
		public static TService GetInstance<TService>(
			string property, IDictionary<string, string> properties, System.Type defaultType) where TService : class
		{
			var className = GetString(property, properties, null);
			System.Type type = null;
			try
			{
				type = className != null
					? ReflectHelper.ClassForName(className)
					: typeof(TService);

				var instance = (TService) Cfg.Environment.ServiceProvider.GetService(type);
				if (instance != null)
				{
					return instance;
				}

				type = defaultType;
				return defaultType != null ? (TService) Cfg.Environment.ServiceProvider.GetInstance(defaultType) : null;
			}
			catch (Exception e)
			{
				throw new HibernateException(
					$"Could not instantiate {typeof(TService).Name}: {type?.AssemblyQualifiedName ?? className}", e);
			}
		}
	}
}
