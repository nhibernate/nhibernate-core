using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.LazyGroup
{
	public class Person
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual string NickName { get; set; }

		public virtual Address Address { get; set; }

		public virtual byte[] Image { get; set; }

		public virtual long Age { get; protected set; }
	}
}
