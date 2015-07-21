using System;
using System.Collections.Generic;

namespace NHibernate.Test.IdTest
{
	public class Parent
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public IList<Child> Children { get; set; }
	}

	public class Child
	{
		public string Id { get; set; }
		public Parent Parent { get; set; }
	}
}
