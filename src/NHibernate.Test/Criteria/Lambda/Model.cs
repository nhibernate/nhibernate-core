
using System;
using System.Collections.Generic;

namespace NHibernate.Test.Criteria.Lambda
{

	public enum PersonGender
	{
		Male = 1,
		Female = 2,
	}

	public class Person
	{
		public static string StaticName;

		public virtual int					Id			{ get; set; }
		public virtual string				Name		{ get; set; }
		public virtual int					Age			{ get; set; }
		public virtual PersonGender			Gender		{ get; set; }
		public virtual int					Height		{ get; set; }
		public virtual bool					HasCar		{ get; set; }
		public virtual Person				Father		{ get; set; }
		public virtual bool					IsParent	{ get; set; }

		public virtual IEnumerable<Child>	Children	{ get; set; }
	}

	public class CustomPerson : Person { }

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

