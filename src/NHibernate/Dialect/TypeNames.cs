using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NHibernate.Util;

namespace NHibernate.Dialect
{
	/// <summary>
	/// This class maps a DbType to names.
	/// </summary>
	/// <remarks>
	/// Associations may be marked with a capacity. Calling the <c>Get()</c>
	/// method with a type and actual size n will return the associated
	/// name with smallest capacity >= n, if available and an unmarked
	/// default type otherwise.
	/// Eg, setting
	/// <code>
	///		Names.Put(DbType,			"TEXT" );
	///		Names.Put(DbType,	255,	"VARCHAR($l)" );
	///		Names.Put(DbType,	65534,	"LONGVARCHAR($l)" );
	/// </code>
	/// will give you back the following:
	/// <code>
	///		Names.Get(DbType)			// --> "TEXT" (default)
	///		Names.Get(DbType,100)		// --> "VARCHAR(100)" (100 is in [0:255])
	///		Names.Get(DbType,1000)	// --> "LONGVARCHAR(1000)" (100 is in [256:65534])
	///		Names.Get(DbType,100000)	// --> "TEXT" (default)
	/// </code>
	/// On the other hand, simply putting
	/// <code>
	///		Names.Put(DbType, "VARCHAR($l)" );
	/// </code>
	/// would result in
	/// <code>
	///		Names.Get(DbType)			// --> "VARCHAR($l)" (will cause trouble)
	///		Names.Get(DbType,100)		// --> "VARCHAR(100)" 
	///		Names.Get(DbType,1000)	// --> "VARCHAR(1000)"
	///		Names.Get(DbType,10000)	// --> "VARCHAR(10000)"
	/// </code>
	/// </remarks>
	public class TypeNames
	{
		public const string LengthPlaceHolder = "$l";
		public const string PrecisionPlaceHolder = "$p";
		public const string ScalePlaceHolder = "$s";

		private readonly Dictionary<DbType, SortedList<int, string>> weighted =
			new Dictionary<DbType, SortedList<int, string>>();

		private readonly Dictionary<DbType, string> defaults = new Dictionary<DbType, string>();

		/// <summary>
		/// Get default type name for specified type
		/// </summary>
		/// <param name="typecode">the type key</param>
		/// <returns>the default type name associated with the specified key</returns>
		public string Get(DbType typecode)
		{
			if (!TryGet(typecode, out var result))
			{
				throw new ArgumentException("Dialect does not support DbType." + typecode, nameof(typecode));
			}
			return result;
		}

		/// <summary>
		/// Get default type name for specified type.
		/// </summary>
		/// <param name="typecode">The type key.</param>
		/// <param name="typeName">The default type name that will be set in case it was found.</param>
		/// <returns>Whether the default type name was found.</returns>
		public bool TryGet(DbType typecode, out string typeName)
		{
			return defaults.TryGetValue(typecode, out typeName);
		}

		/// <summary>
		/// Get the type name specified type and size
		/// </summary>
		/// <param name="typecode">the type key</param>
		/// <param name="size">the SQL length </param>
		/// <param name="scale">the SQL scale </param>
		/// <param name="precision">the SQL precision </param>
		/// <returns>
		/// The associated name with smallest capacity >= size (or precision for decimal, or scale for date time types)
		/// if available, otherwise the default type name.
		/// </returns>
		public string Get(DbType typecode, int size, int precision, int scale)
		{
			if (!TryGet(typecode, size, precision, scale, out var result))
			{
				throw new ArgumentException("Dialect does not support DbType." + typecode, nameof(typecode));
			}

			return result;
		}

		/// <summary>
		/// Get the type name specified type and size.
		/// </summary>
		/// <param name="typecode">The type key.</param>
		/// <param name="size">The SQL length.</param>
		/// <param name="scale">The SQL scale.</param>
		/// <param name="precision">The SQL precision.</param>
		/// <param name="typeName">
		/// The associated name with smallest capacity >= size (or precision for decimal, or scale for date time types)
		/// if available, otherwise the default type name.
		/// </param>
		/// <returns>Whether the type name was found.</returns>
		public bool TryGet(DbType typecode, int size, int precision, int scale, out string typeName)
		{
			weighted.TryGetValue(typecode, out var map);
			if (map != null && map.Count > 0)
			{
				var isPrecisionType = IsPrecisionType(typecode);
				var requiredCapacity = isPrecisionType
					? precision
					: IsScaleType(typecode) ? scale : size;
				foreach (var entry in map)
				{
					if (requiredCapacity <= entry.Key)
					{
						typeName = Replace(entry.Value, size, precision, scale);
						return true;
					}
				}
				if (isPrecisionType && precision != 0)
				{
					// The default is usually not the max for precision type, fallback to last entry instead.
					var maxEntry = map.Last();
					var adjustedPrecision = maxEntry.Key;
					// Reduce the scale (most databases restrict scale to be less or equal to precision)
					// For a proportionnal reduction, we could use
					// Math.Min((int) Math.Round(scale * adjustedPrecision / (double) precision), adjustedPrecision);
					// But if the type is used for storing amounts, this may cause losing the ability to store cents...
					// So better just reduce as few as possible.
					var adjustedScale = Math.Min(scale, adjustedPrecision);
					typeName = Replace(maxEntry.Value, size, adjustedPrecision, adjustedScale);
					return true;
				}
			}
			//Could not find a specific type for the capacity, using the default
			return TryGet(typecode, out typeName);
		}

		/// <summary>
		/// For types with a simple length (or precision for decimal, or scale for date time types), this method
		/// returns the definition for the longest registered type.
		/// </summary>
		/// <param name="typecode"></param>
		/// <returns></returns>
		public string GetLongest(DbType typecode)
		{
			weighted.TryGetValue(typecode, out var map);
			if (map != null && map.Count > 0)
			{
				var isPrecisionType = IsPrecisionType(typecode);
				var isScaleType = IsScaleType(typecode);
				var isSizeType = !isPrecisionType && !isScaleType;
				var capacity = map.Keys[map.Count - 1];
				return Replace(
					map.Values[map.Count - 1],
					isSizeType ? capacity : 0,
					isPrecisionType ? capacity : 0,
					isScaleType ? capacity : 0);
			}

			return Get(typecode);
		}

		private static bool IsPrecisionType(DbType typecode)
		{
			switch (typecode)
			{
				case DbType.Decimal:
				// Oracle dialect defines precision and scale for double, because it uses number instead of binary_double.
				case DbType.Double:
					return true;
			}
			return false;
		}

		private static bool IsScaleType(DbType typecode)
		{
			switch (typecode)
			{
				case DbType.DateTime:
				case DbType.DateTime2:
				case DbType.DateTimeOffset:
				case DbType.Time:
					return true;
			}
			return false;
		}

		private static string Replace(string type, int size, int precision, int scale)
		{
			type = StringHelper.ReplaceOnce(type, LengthPlaceHolder, size.ToString());
			type = StringHelper.ReplaceOnce(type, ScalePlaceHolder, scale.ToString());
			return StringHelper.ReplaceOnce(type, PrecisionPlaceHolder, precision.ToString());
		}

		/// <summary>
		/// Set a type name for specified type key and capacity
		/// </summary>
		/// <param name="typecode">the type key</param>
		/// <param name="capacity">the (maximum) type size/length, precision or scale</param>
		/// <param name="value">The associated name</param>
		public void Put(DbType typecode, int capacity, string value)
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			SortedList<int, string> map;
			if (!weighted.TryGetValue(typecode, out map))
			{
				// add new ordered map
				weighted[typecode] = map = new SortedList<int, string>();
			}
			map[capacity] = value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="typecode"></param>
		/// <param name="value"></param>
		public void Put(DbType typecode, string value)
		{
			defaults[typecode] = value ?? throw new ArgumentNullException(nameof(value));
		}
	}
}
