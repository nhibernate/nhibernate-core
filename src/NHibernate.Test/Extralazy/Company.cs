using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.Extralazy
{
	public class Company
	{
		protected Company() { }

		public Company(string name, int index, User owner)
		{
			Name = name;
			Owner = owner;
			OriginalIndex = ListIndex = index;
		}

		public virtual int Id { get; set; }

		public virtual int ListIndex { get; set; }

		public virtual int OriginalIndex { get; set; }

		public virtual string Name { get; set; }

		public virtual User Owner { get; set; }
	}
}
