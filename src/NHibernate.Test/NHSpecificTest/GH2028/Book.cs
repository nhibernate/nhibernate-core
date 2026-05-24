using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH2028
{
	public class Book
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Word> Words { get; set; }
	}
}
