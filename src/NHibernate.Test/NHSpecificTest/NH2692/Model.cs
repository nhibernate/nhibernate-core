namespace NHibernate.Test.NHSpecificTest.NH2692
{
	using System;
	using System.Collections.Generic;

	public class Parent
	{
		public Parent()
		{
			ChildComponents = new List<ChildComponent>();
		}

		public virtual Guid Id { get; set; }
		public virtual ICollection<ChildComponent> ChildComponents { get; set; }
	}

	public class ChildComponent
	{
		public virtual Parent Parent { get; set; }
		public virtual bool SomeBool { get; set; }
		public virtual string SomeString { get; set; }
	}
}
