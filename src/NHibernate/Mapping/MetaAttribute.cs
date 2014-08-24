using System;
using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A meta attribute is a named value or values.
	/// </summary>
	[Serializable]
	public class MetaAttribute
	{
		private readonly string name;
		private readonly List<string> values= new List<string>();

		public MetaAttribute(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		public IList<string> Values
		{
			get { return values.AsReadOnly(); }
		}

		public string Value
		{
			get
			{
				if (values.Count != 1)
					throw new ArgumentException("No unique value");

				return values[0];
			}
		}

		public bool IsMultiValued
		{
			get { return values.Count > 1; }
		}

		public void AddValue(string value)
		{
			values.Add(value);
		}

		public void AddValues(IEnumerable<string> range)
		{
			values.AddRange(range);
		}

		public override string ToString()
		{
			return string.Format("[{0}={1}]", name, CollectionPrinter.ToString(values));
		}
	}
}