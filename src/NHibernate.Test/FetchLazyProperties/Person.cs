using System.Collections.Generic;

namespace NHibernate.Test.FetchLazyProperties
{
	public class Person
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual int Formula { get; set; }

		public virtual Address Address { get; set; }

		public virtual byte[] Image { get; set; }

		public virtual Person BestFriend { get; set; }

		// Not mapped property
		public virtual int BirthYear { get; set; }

		public virtual ISet<Animal> Pets { get; set; } = new HashSet<Animal>();

		public virtual ISet<Cat> Cats { get; set; } = new HashSet<Cat>();

		public virtual ISet<Dog> Dogs { get; set; } = new HashSet<Dog>();
	}
}
