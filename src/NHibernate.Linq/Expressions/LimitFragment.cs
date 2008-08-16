using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Linq.Expressions
{
	public class LimitFragment
	{
		public int? Skip { get; set; }
		public int? Take { get; set; }
	}
}
