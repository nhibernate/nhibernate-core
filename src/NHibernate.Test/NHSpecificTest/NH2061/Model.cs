using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2061
{
	public class Order
	{
		public virtual Guid Id { get; set; }
		public virtual GroupComponent GroupComponent { get; set; }
	}
	
	public class GroupComponent
	{
		public virtual IList<Country> Countries { get; set; }
	}

	public class Country
	{
		public virtual string CountryCode { get; set; }
	} 
}