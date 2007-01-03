using System;
using System.Collections;

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
	}

	public class ActivitySet : Entity
	{
		IList _activities = new ArrayList();
		public IList Activities
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
		IList _questions;
		public IList Questions
		{
			get { return _questions; }
			set { _questions = value; }
		}
	}

	public class Question : Entity
	{
	}
}
