using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3889
{
	public class TimeRecord
	{
		public virtual Guid Id { get; set; }
		public virtual Project Project { get; set; }
		public virtual Job ActualJob { get; set; }
		public virtual decimal Hours { get; set; }

		public virtual TimeSetting Setting { get; set; }
	}

	public class TimeSetting
	{
		public virtual Guid Id { get; set; }
		public virtual TimeInclude Include { get; set; }
	}

	public class TimeInclude
	{
		public virtual Guid Id { get; set; }
		public virtual bool Flag { get; set; }
	}

	public class Project
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Job Job { get; set; }
	}

	public class Job
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
