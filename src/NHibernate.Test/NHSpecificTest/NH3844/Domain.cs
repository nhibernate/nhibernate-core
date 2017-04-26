using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3844
{
	public class Project
	{
		public Project()
		{
			Components = new List<Component>();
		}

		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Component> Components { get; set; }
		public virtual Job Job { get; set; }
	}

	public class Component
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Project Project { get; set; }
	}

	public class TimeRecord
	{
		public TimeRecord()
		{
			Components = new List<Component>();
		}

		public virtual Guid Id { get; set; }
		public virtual double TimeInHours { get; set; }
		public virtual Project Project { get; set; }
		public virtual IList<Component> Components { get; set; }

	}

	public class Job
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual BillingType BillingType { get; set; }
	}

	public enum BillingType
	{
		None,
		Hourly,
		Fixed,
	}
}
