using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3652;

public class Department : BaseClass
{
	public virtual ISet<Employee> Employees { get; set; } = new HashSet<Employee>();

	public virtual DateTime? DeletedAt { get; set; }
}
