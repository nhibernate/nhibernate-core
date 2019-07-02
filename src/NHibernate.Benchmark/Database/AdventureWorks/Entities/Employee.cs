using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class Employee
	{
		public virtual DateTime BirthDate { get; set; }
		public virtual bool CurrentFlag { get; set; }
		public virtual string Gender { get; set; }
		public virtual DateTime HireDate { get; set; }
		public virtual string JobTitle { get; set; }
		public virtual string LoginId { get; set; }
		public virtual string MaritalStatus { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string NationalIdNumber { get; set; }
		public virtual short? OrganizationLevel { get; set; }
		public virtual Guid RowGuid { get; set; }
		public virtual bool SalariedFlag { get; set; }
		public virtual short SickLeaveHours { get; set; }
		public virtual short VacationHours { get; set; }

		public virtual ICollection<EmployeeDepartmentHistory> EmployeeDepartmentHistory { get; set; } = new HashSet<EmployeeDepartmentHistory>();
		public virtual ICollection<EmployeePayHistory> EmployeePayHistory { get; set; } = new HashSet<EmployeePayHistory>();
		public virtual ICollection<JobCandidate> JobCandidate { get; set; } = new HashSet<JobCandidate>();
		public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeader { get; set; } = new HashSet<PurchaseOrderHeader>();
		public virtual SalesPerson SalesPerson { get; set; }
		public virtual Person BusinessEntity { get; set; }
	}
}
