using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.Join
{
	public class TennisPlayer
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Racquet { get; set; }
	}
}
