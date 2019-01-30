using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.Extralazy
{
	public class UserPermission
	{
		protected UserPermission() { }

		public UserPermission(string name, User owner)
		{
			Name = name;
			Owner = owner;
		}

		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual User Owner { get; set; }
	}
}
