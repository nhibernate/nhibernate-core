using System;
using System.Globalization;

namespace NHibernate.Test.TypesTest
{
	public class CultureInfoClass
	{
		public Guid Id { get; set; }
		public CultureInfo BasicCulture { get; set; }
		public CultureInfo ExtendedCulture { get; set; }
	}
}
