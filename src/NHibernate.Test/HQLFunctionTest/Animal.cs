using System;

namespace NHibernate.Test.HQLFunctionTest
{
	public class Animal
	{
		public Animal() { }

		public Animal(string description, float bodyWeight):base()
		{
			_description = description;
			_bodyWeight = bodyWeight;
		}

		private int _id;
		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private string _description;
		public virtual string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		private float _bodyWeight;
		public virtual float BodyWeight
		{
			get { return _bodyWeight; }
			set { _bodyWeight = value; }
		}
	}
}
