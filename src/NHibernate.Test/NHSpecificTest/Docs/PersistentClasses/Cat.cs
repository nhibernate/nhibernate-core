using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.Docs.PersistentClasses
{
	public class Cat
	{
		private long _id; // identifier
		private string _name;
		private DateTime _birthdate;
		private Cat _mate;
		private ISet<Cat> _kittens;
		private Color _color;
		private char _sex;
		private float _weight;

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public DateTime Birthdate
		{
			get { return _birthdate; }
			set { _birthdate = value; }
		}

		public Cat Mate
		{
			get { return _mate; }
			set { _mate = value; }
		}

		public ISet<Cat> Kittens
		{
			get { return _kittens; }
			set { _kittens = value; }
		}

		public Color Color
		{
			get { return _color; }
			set { _color = value; }
		}

		public char Sex
		{
			get { return _sex; }
			set { _sex = value; }
		}

		public float Weight
		{
			get { return _weight; }
			set { _weight = value; }
		}
	}

	public enum Color
	{
		Black,
		White,
		Mix
	}
}