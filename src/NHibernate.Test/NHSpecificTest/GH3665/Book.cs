using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3665
{
	public class Book
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Word> Words { get; set; }
	}
}
