using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3918
{
	public interface IModelObject
	{
		Guid Id { get; set; }
		string Name { get; set; }
	}

	public class Entity : IModelObject
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

		public virtual Owner Owner { get; set; }
	}

	public class Owner : IModelObject
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
