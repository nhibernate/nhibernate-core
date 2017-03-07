using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Util
{
	/// <summary>
	/// Utility class implementing ToString for collections. All <c>ToString</c>
	/// overloads call <c>element.ToString()</c>.
	/// </summary>
	/// <remarks>
	/// To print collections of entities or typed values, use
	/// <see cref="NHibernate.Impl.Printer" />.
	/// </remarks>
	public static class CollectionPrinter
	{
		private static void AppendNullOrValue(StringBuilder builder, object value)
		{
			if (value == null)
			{
				builder.Append("null");
			}
			else
			{
				builder
					.Append("'")
					.Append(value)
					.Append("'");
			}
		}

		public static string ToString(IDictionary dictionary)
		{
			StringBuilder result = new StringBuilder();
			result.Append("{");

			bool first = true;
			foreach (DictionaryEntry de in dictionary)
			{
				if (!first)
				{
					result.Append(", ");
				}
				AppendNullOrValue(result, de.Key);
				result.Append("=");
				AppendNullOrValue(result, de.Value);
				first = false;
			}

			result.Append("}");
			return result.ToString();
		}

		public static string ToString(IDictionary<string, string> dictionary)
		{
			StringBuilder result = new StringBuilder();
			result.Append("{");

			bool first = true;
			foreach (KeyValuePair<string, string> de in dictionary)
			{
				if (!first)
					result.Append(", ");

				AppendNullOrValue(result, de.Key);
				result.Append("=");
				AppendNullOrValue(result, de.Value);
				first = false;
			}

			result.Append("}");
			return result.ToString();
		}

		public static string ToString(IEnumerable elements)
		{
			StringBuilder result = new StringBuilder();
			result.Append("[");

			bool first = true;
			foreach (object item in elements)
			{
				if (!first)
				{
					result.Append(", ");
				}
				AppendNullOrValue(result, item);
				first = false;
			}

			result.Append("]");
			return result.ToString();
		}

	}
}