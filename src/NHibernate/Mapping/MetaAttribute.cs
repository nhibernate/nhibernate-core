using System;
using System.Collections;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A meta attribute is a named value or values.
	/// </summary>
	public class MetaAttribute
	{
		private string name;
		private IList values;

		/// <summary>
		/// 
		/// </summary>
		public MetaAttribute()
		{
			values = new ArrayList();
		}

		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public IList Values
		{
			get { return values; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Value
		{
			get
			{
				if (values.Count != 1)
				{
					throw new ArgumentException("No unique value");
				}
				return (string) values[0];
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
	}
}