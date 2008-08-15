using System;
using System.Collections;
using Iesi.Collections;

namespace NHibernate.Test.NHSpecificTest.NH826
{
	public class Entity
	{
		private int _id;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public override bool Equals(object obj)
		{
			Entity that = obj as Entity;
			if(that == null)
				return false;
			return _id == that.Id;
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}
	}

	public class ActivitySet : Entity
	{
		private ISet _activities = new HashedSet();

		public ISet Activities
		{
			get { return _activities; }
			set { _activities = value; }
		}
	}

	public class Activity : Entity
	{
	}

	public class EvaluationActivity : Activity
	{
		private ISet _questions;

		public ISet Questions
		{
			get { return _questions; }
			set { _questions = value; }
		}
	}

	public class Question : Entity
	{
	}
}