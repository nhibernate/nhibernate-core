using System;

namespace NHibernate.Examples.ForumQuestions.T1078029
{
	public class Member
	{
		private string id;
		private string name;
		private DateTime admission;

		public Member()
		{
		}

		public string Id 
		{
			get { return id; }
			set { id = value; }
		}
		
		public string Name 
		{
			get { return name; }
			set { name = value; }
		}

		public DateTime Admission
		{
			get { return admission; }
			set { admission = value; }
		}
	}
}
