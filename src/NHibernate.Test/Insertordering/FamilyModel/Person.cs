using System;

namespace NHibernate.Test.Insertordering.FamilyModel
{
	public class Person
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Woman Mother { get; set; }
		public virtual Man Father { get; set; }
	}

	public class Woman : Person
	{
	}

	public class Man : Person
	{
	}
}
