using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH2009
{
	public class BlogPost
	{
		public virtual int ID { get; set; }
		public virtual string Title { get; set; }
		public virtual User Poster { get; set; }
	}

	public class User
	{
		public virtual int ID { get; set; }
		public virtual string FullName { get; set; }
		public virtual string UserName { get; set; }
	}
}
