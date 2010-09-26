using System;
using Iesi.Collections;

namespace NHibernate.Test.Criteria
{
	public class Student
	{
		private long studentNumber;
		private string name;		
		private CityState cityState;
		private Course preferredCourse;		
		private ISet enrolments = new HashedSet();
		
		public virtual long StudentNumber
		{
			get { return studentNumber; }
			set { studentNumber = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		public virtual CityState CityState
		{
			get { return cityState; }
			set { cityState = value; }
		}		

		public virtual Course PreferredCourse
		{
			get { return preferredCourse; }
			set { preferredCourse = value; }
		}

		public virtual ISet Enrolments
		{
			get { return enrolments; }
			set { enrolments = value; }
		}
	}
}