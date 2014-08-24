using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1948
{
	public class Person
	{
		public virtual int Id { get; set; }
		public virtual decimal Age { get; set; }
		public virtual decimal ShoeSize { get; set; }
		public virtual IList<decimal> FavouriteNumbers { get; set; }
	}
}
