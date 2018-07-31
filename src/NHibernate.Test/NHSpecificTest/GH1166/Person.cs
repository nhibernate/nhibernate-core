using System;

namespace NHibernate.Test.NHSpecificTest.GH1166
{
	public class Person
    {
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
    }
}
