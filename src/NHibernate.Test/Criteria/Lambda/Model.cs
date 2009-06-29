
using System;
using System.Collections.Generic;

namespace NHibernate.Test.Criteria.Lambda
{

	public class Person
	{
		public virtual int					Id			{ get; set; }
		public virtual string				Name		{ get; set; }
		public virtual int					Age			{ get; set; }
		public virtual int					Height		{ get; set; }
		public virtual bool					HasCar		{ get; set; }
		public virtual Person				Father		{ get; set; }

		public virtual IEnumerable<Child>	Children	{ get; set; }
	}

	public class Child
	{
		public virtual int		Id			{ get; set; }
		public virtual string	Nickname	{ get; set; }
	}

}

