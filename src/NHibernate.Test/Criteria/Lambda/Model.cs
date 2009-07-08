
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

	public class Relation
	{

		public virtual Relation Related1	{ get; set; }
		public virtual Relation Related2	{ get; set; }
		public virtual Relation Related3	{ get; set; }
		public virtual Relation Related4	{ get; set; }

		public virtual IEnumerable<Relation>	Collection1	{ get; set; }
		public virtual IEnumerable<Relation>	Collection2	{ get; set; }
		public virtual IEnumerable<Relation>	Collection3	{ get; set; }
		public virtual IEnumerable<Relation>	Collection4	{ get; set; }

		public virtual IEnumerable<Person>		People		{ get; set; }

	}

}

