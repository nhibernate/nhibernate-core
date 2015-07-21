using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1691
{
	public class SubComponent
	{
		public string SubName { get; set; }
		public string SubName1 { get; set; }
		public SubComponent Nested { get; set; }
	}

	public class Component
	{
		public string Name { get; set; }
		public SubComponent SubComponent { get; set; }
	}

	public class DeepComponent
	{
		public string Prop1 { get; set; }
		public string Prop2 { get; set; }
		public string Prop3 { get; set; }
		public string Prop4 { get; set; }
		public Component Component { get; set; }
	}

	public class Nest
	{
		public virtual Component Component { get; set; }
		public virtual int Id { get; set; }
		public string Name { get; set; }
		public virtual IList<Component> Components { get; set; }
		public virtual IList<DeepComponent> ComplexComponents { get; set; }
	}
}