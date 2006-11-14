using System;
using Iesi.Collections;

namespace NHibernate.Test.Criteria
{
	public class Student
	{
		private long studentNumber;
		public virtual long StudentNumber
		{
			get { return studentNumber; }
			set { studentNumber = value; }
		}

		private string name;
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		private Course preferredCourse;
		public virtual Course PreferredCourse
		{
			get { return preferredCourse; }
			set { preferredCourse = value; }
		}

		private ISet enrolments = new HashedSet();
		public virtual ISet Enrolments
		{
			get { return enrolments; }
			set { enrolments = value; }
		}
	}
}
