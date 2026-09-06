using System;

namespace NHibernate.Test.NHSpecificTest.GH3363
{
	class Mother
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
	
	class Child1 : Mother
	{
		public virtual Thing1 Thing {  get; set; }
	}
	class Child2 : Mother
	{
		public virtual Thing2 Thing { get; set; }
	}
	class Thing1
	{
		public virtual string Id { get; set;}
		public virtual string Name { get; set; }
	}
	class Thing2
	{
		public virtual string Id { get; set;}
		public virtual string Name { get; set; }
	}
}
