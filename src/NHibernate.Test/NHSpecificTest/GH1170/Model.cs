using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1170
{
	public class Parent
	{
		public Parent()
		{
			ChildComponents = new List<ChildComponent>();
		}

		public virtual ICollection<ChildComponent> ChildComponents { get; set; }
		public virtual Guid Id { get; set; }
	}

	public class ChildComponent
	{
		public virtual bool SomeBool { get; set; }
		public virtual string SomeString { get; set; }
	}
}
