using System;

namespace NHibernate.Test.NHSpecificTest.GH3291
{
	class Person
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime? DateOfBirth { get; set; }
	}
}
