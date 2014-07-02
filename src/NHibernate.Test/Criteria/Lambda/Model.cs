
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

		public Person()
		{
			Children = new List<Child>();
			Blood = 'O';
			BirthDate = new DateTime(1950, 01, 01);
			BirthDateAsDateTimeOffset = new DateTimeOffset(BirthDate);
		}

		public static string StaticName;

		public virtual int					Id			{ get; set; }
		public virtual string				Name		{ get; set; }
		public virtual int					Age			{ get; set; }
		public virtual PersonGender			Gender		{ get; set; }
		public virtual int					Height		{ get; set; }
		public virtual bool					HasCar		{ get; set; }
		public virtual Person				Father		{ get; set; }
		public virtual bool					IsParent	{ get; set; }
		public virtual char					Blood		{ get; set; }
		public virtual DateTime				BirthDate	{ get; set; }
		public virtual DateTimeOffset		BirthDateAsDateTimeOffset	{ get; set; }
		public virtual PersonDetail			Detail		{ get; set; }

		public virtual int?					NullableAge			{ get; set; }
		public virtual PersonGender?		NullableGender		{ get; set; }
		public virtual bool?				NullableIsParent	{ get; set; }

		public virtual IEnumerable<Child>	Children	{ get; set; }
		public virtual IList<Person>		PersonList	{ get; set; }

		public virtual Person AddChild(Child child)
		{
			child.Parent = this;
			(Children as IList<Child>).Add(child);
			return this;
		}

	}

	public class PersonDetail
	{
		public string	MaidenName	{ get; set; }
		public DateTime Anniversary { get; set; }
	}

	public class CustomPerson : Person
	{
		public virtual string MiddleName { get; set; }
	}

	public class Child
	{
		public virtual int		Id			{ get; set; }
		public virtual string	Nickname	{ get; set; }
		public virtual int		Age			{ get; set; }

		public virtual Person	Parent		{ get; set; }
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

	public class PersonSummary
	{
		public string Name { get; set; }
		public int Count { get; set; }
	}

	public class Parent
	{
		public Parent()
		{
			Children = new List<JoinedChild>();
		}

		public virtual int						Id			{ get; set; }
		public virtual IEnumerable<JoinedChild>	Children	{ get; set; }

		public virtual Parent AddChild(JoinedChild child)
		{
			child.Parent = this;
			(Children as IList<JoinedChild>).Add(child);
			return this;
		}
	}

	public class JoinedChild
	{
		public virtual int		Id			{ get; set; }
		public virtual Parent	Parent		{ get; set; }
	}
}

