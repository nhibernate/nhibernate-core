using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.FetchLazyProperties
{
	public class Continent
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }
	}
}
