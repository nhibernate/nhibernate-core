using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.Join
{
	public class TennisPlayer
	{
		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string RacquetMake { get; set; }
		public virtual string RacquetModel { get; set; }

		public virtual IList<Person> Coaches { get; set; }
	}
}
