using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1879
{
	public class Client
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string NameByMethod() => Name;
	}

	public class CorporateClient : Client
	{
		public virtual string CorporateId { get; set; }
	}

	public class Employee
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

		public virtual bool ReviewAsPrimary { get; set; }
		public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
		public virtual ICollection<Issue> WorkIssues { get; set; } = new List<Issue>();
		public virtual ICollection<Issue> ReviewIssues { get; set; } = new List<Issue>();
	}

	public class Project
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual EmailPref EmailPref { get; set; }
		public virtual Client Client { get; set; }
		public virtual Client BillingClient { get; set; }
		public virtual CorporateClient CorporateClient { get; set; }
		public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
	}

	public enum EmailPref
	{
		Primary,
		Billing,
		Corp,
	}

	public class Issue
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Client Client { get; set; }
		public virtual Project Project { get; set; }
	}

	public class Invoice
	{
		public virtual Guid Id { get; set; }
		public virtual int InvoiceNumber { get; set; }
		public virtual Project Project { get; set; }
		public virtual Issue Issue { get; set; }
		public virtual int Amount { get; set; }
		public virtual int? SpecialAmount { get; set; }
		public virtual bool Paid { get; set; }
	}
}
