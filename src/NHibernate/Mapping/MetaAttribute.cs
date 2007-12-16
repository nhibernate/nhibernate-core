using System;
using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A meta attribute is a named value or values.
	/// </summary>
	public class MetaAttribute
	{
		private readonly string name;
		private readonly List<string> values= new List<string>();

		public MetaAttribute(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// 
		/// </summary>
		public IList<string> Values
		{
			get { return values.AsReadOnly(); }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Value
		{
			get
			{
				if (values.Count != 1)
					throw new ArgumentException("No unique value");

				return values[0];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsMultiValued
		{
			get { return values.Count > 1; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void AddValue(string value)
		{
			values.Add(value);
		}

		public override string ToString()
		{
			return string.Format("[{0}={1}]", name, CollectionPrinter.ToString(values));
		}
	}
}