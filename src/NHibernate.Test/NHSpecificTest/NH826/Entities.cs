using System;
using System.Collections.Generic;

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
		private ISet<Activity> _activities = new HashSet<Activity>();

		public ISet<Activity> Activities
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
		private ISet<Question> _questions;

		public ISet<Question> Questions
		{
			get { return _questions; }
			set { _questions = value; }
		}
	}

	public class Question : Entity
	{
	}
}