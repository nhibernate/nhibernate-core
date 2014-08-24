using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1981
{
	public class Article
	{
		public virtual int Id { get; set; }
		public virtual double Longitude { get; set; }
	}
}
