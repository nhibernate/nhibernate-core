using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1882
{
	public class Publisher
	{
		public Publisher()
		{
			Authors = new HashSet<Author>();
		}

		public Publisher(String name)
		{
			Authors = new HashSet<Author>();
			Name = name;
		}

		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual ISet<Author> Authors { get; set; }
	}
}