using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.CachingComplexQuery
{
	public class Person
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int Age { get; set; }

		public virtual ISet<Car> Cars
		{
			get => _cars ?? (_cars = new HashSet<Car>());
			set => _cars = value;
		}
		private ISet<Car> _cars;

		public virtual ISet<Child> Children
		{
			get => _children ?? (_children = new HashSet<Child>());
			set => _children = value;
		}
		private ISet<Child> _children;
	}
}
