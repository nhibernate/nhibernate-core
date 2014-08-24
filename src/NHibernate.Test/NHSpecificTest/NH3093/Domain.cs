using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3093
{
	public class Product
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Family Family { get; set; }
	}
	
	public class Family
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ICollection<Product> Products { get; set; }
		public virtual ICollection<Cultivation> Cultivations { get; set; }
		public virtual Segment Segment { get; set; }
	}
	
	public class Cultivation 
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Family Family { get; set; }
	}
	
	public class Segment  
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ICollection<Family> Families { get; set; } 
	}
}