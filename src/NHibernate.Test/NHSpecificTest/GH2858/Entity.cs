using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH2858
{
	public interface IDepartment
	{
		Guid Id { get; set; }
		string Name { get; set; }
	}

	public class Department : IDepartment
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	public interface IProject
	{
		Guid Id { get; set; }
		string Name { get; set; }
		Department Department { get; set; }
	}

	public class Project : IProject
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Department Department { get; set; }
	}

	public interface IIssue
	{
		Guid Id { get; set; }
		string Name { get; set; }
		Project Project { get; set; }
		IList<Department> Departments { get; set; }
	}

	public class Issue : IIssue
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Department> Departments { get; set; } = new List<Department>();
		public virtual Project Project { get; set; }
	}

	public interface ITimeChunk
	{
		Guid Id { get; set; }
		Issue Issue { get; set; }
		int Seconds { get; set; }
	}

	public class TimeChunk : ITimeChunk
	{
		public virtual Guid Id { get; set; }
		public virtual Issue Issue { get; set; }
		public virtual int Seconds { get; set; }
	}
}
