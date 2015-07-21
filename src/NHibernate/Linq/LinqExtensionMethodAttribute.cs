using System;

namespace NHibernate.Linq
{
	public class LinqExtensionMethodAttribute: Attribute
	{
		public LinqExtensionMethodAttribute()
		{
		}

		public LinqExtensionMethodAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}