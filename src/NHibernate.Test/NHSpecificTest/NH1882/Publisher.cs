using System;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1882
{
	public class Publisher
	{
		public Publisher()
		{
			Authors = new HashedSet<Author>();
		}

		public Publisher(String name)
		{
			Authors = new HashedSet<Author>();
			Name = name;
		}

		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual ISet<Author> Authors { get; set; }
	}
}