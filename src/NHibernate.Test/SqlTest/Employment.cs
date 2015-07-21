using System;

namespace NHibernate.Test.SqlTest
{
	public class Employment
	{
		private long employmentId;
		private Person employee;
		private Organization employer;
		private DateTime startDate;
		private DateTime endDate;
		private String regionCode;
		private MonetaryAmount salary;

		public Employment()
		{
		}

		public Employment(Person employee, Organization employer, String regionCode)
		{
			this.employee = employee;
			this.employer = employer;
			this.startDate = DateTime.Today;
			this.regionCode = regionCode;
			employer.Employments.Add(this);
		}

		public virtual Person Employee
		{
			get { return employee; }
			set { employee = value; }
		}

		public virtual Organization Employer
		{
			get { return employer; }
			set { employer = value; }
		}

		public virtual DateTime StartDate
		{
			get { return startDate; }
			set { startDate = value; }
		}

		public virtual DateTime EndDate
		{
			get { return endDate; }
			set { endDate = value; }
		}

		public virtual string RegionCode
		{
			get { return regionCode; }
			set { regionCode = value; }
		}

		public virtual MonetaryAmount Salary
		{
			get { return salary; }
			set { salary = value; }
		}

		public virtual long EmploymentId
		{
			get { return employmentId; }
			set { employmentId = value; }
		}
	}
}