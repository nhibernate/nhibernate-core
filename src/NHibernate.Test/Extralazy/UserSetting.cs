using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.Extralazy
{
	public class UserSetting
	{
		protected UserSetting() { }
		public UserSetting(string name, string data, User owner)
		{
			Name = name;
			Data = data;
			Owner = owner;
		}

		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual string Data { get; set; }

		public virtual User Owner { get; set; }
	}
}
