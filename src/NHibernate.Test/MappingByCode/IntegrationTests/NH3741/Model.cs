using System.Collections.Generic;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3741
{
	public class ParentWithInterfaceChild
	{
		public int Id { get; set; }
		public List<IChild> Children { get; set; }
	}

	public class ParentWithEntityChild
	{
		public int Id { get; set; }
		public List<Child> Children { get; set; }
	}

	public class ParentWithItemChild
	{
		public int Id { get; set; }
		public List<string> Children { get; set; }
	}

	public interface IChild
	{
		int Id { get; set; }
		string Description { get; set; }
	}

	public class Child : IChild
	{
		public int Id { get; set; }
		public string Description { get; set; }
	}
}
