using System.Collections;
using Iesi.Collections;

namespace NHibernate.Test.MappingTest
{
	public class Wicked
	{
		public int Id { get; set; }
		public int VersionProp { get; set; }
		public MonetaryAmount Component { get; set; }
		public ISet SortedEmployee { get; set; }
		public IList AnotherSet { get; set; }
	}

	public class MonetaryAmount
	{
		public string X { get; set; }
	}

	public class Employee
	{
		public int Id { get; set; }
		public string Emp { get; set; }
		public Employee Empinone { get; set; }
	}
}