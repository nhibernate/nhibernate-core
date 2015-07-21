using System;
using System.Collections.Generic;

namespace NHibernate.Test.Criteria
{
	public class Student
	{
		private long studentNumber;
		private string name;		
		private CityState cityState;
		private Course preferredCourse;
		private ISet<Enrolment> enrolments = new HashSet<Enrolment>();
		
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

		public virtual ISet<Enrolment> Enrolments
		{
			get { return enrolments; }
			set { enrolments = value; }
		}
	}
}