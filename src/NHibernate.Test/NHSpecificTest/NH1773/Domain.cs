using System;

namespace NHibernate.Test.NHSpecificTest.NH1773
{
	public class Person
	{
		private string _name;
		private int _age;
		private int _id;
		private Country _country;

		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public virtual int Age
		{
			get { return _age; }
			set { _age = value; }
		}

		public virtual Country Country
		{
			get { return _country; }
			set { _country = value; }
		}
	}

	public class Country
	{
		private int _id;
		private string _name;

		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}

	public class PersonResult
	{
		private Person _person;
		private DateTime _time;

		public PersonResult(Person x)
		{
		}

		public PersonResult(Person person, DateTime time)
		{
			_person = person;
			_time = time;
		}

		public Person Person
		{
			get { return _person; }
		}

		public DateTime Time
		{
			get { return _time; }
		}
	}
}
